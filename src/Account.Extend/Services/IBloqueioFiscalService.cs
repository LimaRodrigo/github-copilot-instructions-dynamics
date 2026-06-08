using Microsoft.Xrm.Sdk;

namespace Account.Extend.Services
{
    /// <summary>
    /// Interface do serviço de Bloqueio Fiscal.
    /// Responsável pela orquestração da lógica de validação de CNPJ bloqueado.
    /// </summary>
    public interface IBloqueioFiscalService
    {
        /// <summary>
        /// Valida se um CNPJ está bloqueado no Bloqueio Fiscal.
        /// Lança BloqueioFiscalException se o CNPJ estiver bloqueado.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser validado</param>
        void ValidateBloqueioFiscal(string cnpj);
    }
}
