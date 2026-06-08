using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Extensions.Logging;
using Account.Extend.Services;
using Account.Extend.Infrastructure;

namespace Account.Extend.Plugins
{
    /// <summary>
    /// Plugin para validar CNPJ na criação de registros de Account.
    /// 
    /// Configuração:
    /// - Message: Create
    /// - Entity: account
    /// - Stage: Pre-operation
    /// - Execution mode: Synchronous
    /// 
    /// Este plugin segue rigorosamente o padrão mínimo de execução:
    /// 1. Validar o contexto
    /// 2. Validar profundidade
    /// 3. Criar o Service
    /// 4. Executar processamento
    /// </summary>
    public class ValidarCNPJ : IPlugin
    {
        private const int MaxExecutionDepth = 1;
        private const string CNPJAttributeName = "rcl_cnpj";

        public void Execute(IServiceProvider serviceProvider)
        {
            // 1. Validar o contexto
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            if (context == null)
                throw new InvalidPluginExecutionException("Plugin execution context is null");

            var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
            if (tracingService == null)
                throw new InvalidPluginExecutionException("Tracing service is null");

            var logger = new LoggerAdapter(tracingService, nameof(ValidarCNPJ));

            try
            {
                logger.LogInformation("Plugin ValidarCNPJ iniciado. Message: {Message}, Entity: {Entity}, Stage: {Stage}",
                    context.MessageName, context.PrimaryEntityName, context.Stage);

                // 2. Validar profundidade
                if (context.Depth > MaxExecutionDepth)
                {
                    logger.LogInformation("Plugin ignorado - Profundidade excedida: {Depth}", context.Depth);
                    return;
                }

                // Validar que é a mensagem correta
                if (context.MessageName != "Create")
                {
                    logger.LogInformation("Plugin ignorado - Mensagem incorreta: {Message}", context.MessageName);
                    return;
                }

                // Validar que é a entidade correta
                if (context.PrimaryEntityName != "account")
                {
                    logger.LogInformation("Plugin ignorado - Entidade incorreta: {Entity}", context.PrimaryEntityName);
                    return;
                }

                // 3. Criar o Service
                var validationService = new CNPJValidationService(logger);

                // 4. Executar processamento
                ExecuteValidation(context, validationService, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao executar plugin ValidarCNPJ");
                throw new InvalidPluginExecutionException("Erro ao validar CNPJ da conta", ex);
            }
        }

        private void ExecuteValidation(IPluginExecutionContext context, CNPJValidationService validationService, ILogger logger)
        {
            logger.LogInformation("Executando validação de CNPJ");

            // Obter a entidade do contexto
            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity target))
            {
                logger.LogWarning("Parametro 'Target' não encontrado no contexto");
                return;
            }

            // Verificar se o atributo CNPJ está presente
            if (!target.Attributes.Contains(CNPJAttributeName))
            {
                logger.LogInformation("Atributo CNPJ não está presente no registro");
                return;
            }

            // Obter o valor do CNPJ
            object cnpjValue = target[CNPJAttributeName];
            if (cnpjValue == null)
            {
                logger.LogInformation("CNPJ está nulo");
                return;
            }

            // Validar o CNPJ usando o serviço
            validationService.ValidateCNPJ(cnpjValue.ToString());

            logger.LogInformation("Validação concluída com sucesso");
        }
    }
}
