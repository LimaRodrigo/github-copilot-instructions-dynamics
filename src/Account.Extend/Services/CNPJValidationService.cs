using System;
using Microsoft.Extensions.Logging;
using Account.Extend.Domain.Exceptions;
using Account.Extend.Domain.ValueObjects;

namespace Account.Extend.Services
{
    /// <summary>
    /// Serviço responsável pela validação de CNPJ de contas.
    /// Contém toda a lógica de negócio para a regra de validação.
    /// </summary>
    public class CNPJValidationService
    {
        private readonly ILogger _logger;

        public CNPJValidationService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Valida o CNPJ fornecido.
        /// Lança InvalidCNPJException se o CNPJ for inválido.
        /// </summary>
        public void ValidateCNPJ(string cnpjValue)
        {
            _logger.LogInformation("Iniciando validação de CNPJ: {CNPJ}", MaskCNPJ(cnpjValue));

            try
            {
                if (string.IsNullOrWhiteSpace(cnpjValue))
                {
                    _logger.LogWarning("Tentativa de validar CNPJ vazio ou nulo");
                    throw new InvalidCNPJException("CNPJ não pode estar vazio");
                }

                // A validação real é feita pelo Value Object CNPJ
                var validCnpj = CNPJ.Create(cnpjValue);
                
                _logger.LogInformation("CNPJ validado com sucesso: {CNPJ}", MaskCNPJ(validCnpj.Value));
            }
            catch (InvalidCNPJException ex)
            {
                _logger.LogWarning("Validação de CNPJ falhou: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante validação de CNPJ");
                throw new InvalidCNPJException("Erro ao validar CNPJ", ex);
            }
        }

        /// <summary>
        /// Mascara o CNPJ para fins de logging, mostrando apenas os últimos 4 dígitos.
        /// </summary>
        private string MaskCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length < 4)
                return "****";

            string clean = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            return clean.Substring(0, clean.Length - 4).Replace(clean.Substring(0, clean.Length - 4), "****") + clean.Substring(clean.Length - 4);
        }
    }
}
