using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Extensions.Logging;

namespace Account.Extend.Repositories
{
    /// <summary>
    /// Implementação do repositório para consultar dados de Bloqueio Fiscal.
    /// Acessa a entidade customizada 'rcl_bloqueiofiscal' via IOrganizationService.
    /// </summary>
    public class BloqueioFiscalRepository : IBloqueioFiscalRepository
    {
        private const string BloqueioFiscalEntityName = "rcl_bloqueiofiscal";
        private const string CNPJAttributeName = "rcl_cnpj";
        private const string BloquedoAttributeName = "rcl_bloqueado";

        private readonly IOrganizationService _organizationService;
        private readonly ILogger _logger;

        public BloqueioFiscalRepository(IOrganizationService organizationService, ILogger logger)
        {
            _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsCNPJBlocked(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            try
            {
                var details = GetBlockedCNPJDetails(cnpj);
                
                if (details == null)
                {
                    _logger.LogInformation("CNPJ não encontrado em Bloqueio Fiscal: {CNPJ}", MaskCNPJ(cnpj));
                    return false;
                }

                var isBlocked = details.ContainsKey(BloquedoAttributeName) 
                    && (bool)details[BloquedoAttributeName];

                if (isBlocked)
                {
                    _logger.LogWarning("CNPJ encontrado bloqueado em Bloqueio Fiscal: {CNPJ}", MaskCNPJ(cnpj));
                }

                return isBlocked;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se CNPJ está bloqueado: {CNPJ}", MaskCNPJ(cnpj));
                throw;
            }
        }

        public Dictionary<string, object>? GetBlockedCNPJDetails(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return null;

            try
            {
                var query = new QueryExpression(BloqueioFiscalEntityName)
                {
                    ColumnSet = new ColumnSet(CNPJAttributeName, BloquedoAttributeName, "rcl_bloqueiofiscalid", "createdon", "createdby"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(CNPJAttributeName, ConditionOperator.Equal, cnpj),
                            new ConditionExpression(BloquedoAttributeName, ConditionOperator.Equal, true)
                        }
                    },
                    PageInfo = new PagingInfo { Count = 1, PageNumber = 1 }
                };

                var result = _organizationService.RetrieveMultiple(query);

                if (result.Entities.Count == 0)
                {
                    return null;
                }

                var entity = result.Entities[0];
                var details = new Dictionary<string, object>();

                foreach (var attr in entity.Attributes)
                {
                    details[attr.Key] = attr.Value;
                }

                return details;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar detalhes de Bloqueio Fiscal para CNPJ: {CNPJ}", MaskCNPJ(cnpj));
                throw;
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
