using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Extensions.Logging;
using Account.Extend.Services;
using Account.Extend.Infrastructure;

namespace Account.Extend.Plugins
{
    /// <summary>
    /// Plugin para validar Bloqueio Fiscal na criação de registros de Account.
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
    public class ValidarBloqueioFiscal : IPlugin
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

            var logger = new LoggerAdapter(tracingService, nameof(ValidarBloqueioFiscal));

            try
            {
                logger.LogInformation("Plugin ValidarBloqueioFiscal iniciado. Message: {Message}, Entity: {Entity}, Stage: {Stage}",
                    context.MessageName, context.PrimaryEntityName, context.Stage);

                // 2. Validar profundidade
                if (context.Depth > MaxExecutionDepth)
                {
                    logger.LogInformation("Plugin desativado para depth {Depth} (máximo: {MaxDepth})", 
                        context.Depth, MaxExecutionDepth);
                    return;
                }

                // Validar se é a operação esperada
                if (context.MessageName != "Create" || context.PrimaryEntityName != "account")
                {
                    logger.LogInformation("Plugin não aplicável para Message: {Message}, Entity: {Entity}",
                        context.MessageName, context.PrimaryEntityName);
                    return;
                }

                // Obter a entidade de entrada
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                {
                    logger.LogWarning("Parâmetro Target não encontrado ou não é uma entidade válida");
                    return;
                }

                var targetEntity = (Entity)context.InputParameters["Target"];

                // Verificar se possui o atributo CNPJ
                if (!targetEntity.Contains(CNPJAttributeName))
                {
                    logger.LogInformation("Entidade não possui o atributo CNPJ, nenhuma validação realizada");
                    return;
                }

                var cnpjValue = targetEntity[CNPJAttributeName]?.ToString();
                
                if (string.IsNullOrWhiteSpace(cnpjValue))
                {
                    logger.LogInformation("Atributo CNPJ está vazio, nenhuma validação realizada");
                    return;
                }

                // 3. Criar o Service
                var organizationService = serviceProvider.GetService(typeof(IOrganizationService)) as IOrganizationService;
                if (organizationService == null)
                    throw new InvalidPluginExecutionException("Organization service is null");

                // O serviço cria internamente o repositório (seguindo padrão de repository)
                var service = new BloqueioFiscalService(organizationService, logger);

                // 4. Executar processamento
                logger.LogInformation("Iniciando validação de Bloqueio Fiscal para CNPJ");
                service.ValidateBloqueioFiscal(cnpjValue ?? string.Empty);

                logger.LogInformation("Plugin ValidarBloqueioFiscal executado com sucesso");
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro durante execução do plugin ValidarBloqueioFiscal");
                throw new InvalidPluginExecutionException($"Erro ao validar Bloqueio Fiscal: {ex.Message}", ex);
            }
        }
    }
}
