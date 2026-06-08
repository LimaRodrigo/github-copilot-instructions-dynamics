using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Extensions.Logging;
using Account.Extend.Domain.Exceptions;
using Account.Extend.Repositories;

namespace Account.Extend.Services
{
    /// <summary>
    /// Implementação do serviço de Bloqueio Fiscal.
    /// Orquestra a validação de CNPJ bloqueado consultando o repositório.
    /// Contém toda a lógica de negócio para a regra de bloqueio fiscal.
    /// </summary>
    public class BloqueioFiscalService : IBloqueioFiscalService
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger _logger;

        public BloqueioFiscalService(IOrganizationService organizationService, ILogger logger)
        {
            _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ValidateBloqueioFiscal(string cnpj)
        {
            _logger.LogInformation("Iniciando validação de Bloqueio Fiscal para CNPJ: {CNPJ}", MaskCNPJ(cnpj));

            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    _logger.LogWarning("Tentativa de validar CNPJ vazio ou nulo para Bloqueio Fiscal");
                    // CNPJ vazio não é considerado bloqueado, apenas não valida
                    return;
                }

                // Criar o repositório internamente (não expor ao plugin)
                var repository = new BloqueioFiscalRepository(_organizationService, _logger);

                // Verifica se o CNPJ está bloqueado
                if (repository.IsCNPJBlocked(cnpj))
                {
                    _logger.LogWarning("CNPJ bloqueado encontrado em Bloqueio Fiscal: {CNPJ}", MaskCNPJ(cnpj));
                    throw new BloqueioFiscalException(
                        $"CNPJ {MaskCNPJ(cnpj)} está bloqueado no registro de Bloqueio Fiscal e não pode ser utilizado.",
                        cnpj);
                }

                _logger.LogInformation("CNPJ validado com sucesso em Bloqueio Fiscal: {CNPJ}", MaskCNPJ(cnpj));
            }
            catch (BloqueioFiscalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante validação de Bloqueio Fiscal para CNPJ: {CNPJ}", MaskCNPJ(cnpj));
                throw new BloqueioFiscalException("Erro ao validar Bloqueio Fiscal", cnpj, ex);
            }
        }

        private static string MaskCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length < 8)
                return "***";

            return $"{cnpj.Substring(0, 2)}.***.***/****-{cnpj.Substring(cnpj.Length - 2)}";
        }
    }
}
