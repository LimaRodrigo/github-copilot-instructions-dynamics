using System;
using System.Collections.Generic;

namespace Account.Extend.Repositories
{
    /// <summary>
    /// Interface do repositório para consultar dados de Bloqueio Fiscal.
    /// Responsável pela abstração de acesso aos dados.
    /// </summary>
    public interface IBloqueioFiscalRepository
    {
        /// <summary>
        /// Verifica se um CNPJ está bloqueado no Bloqueio Fiscal.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser verificado</param>
        /// <returns>True se o CNPJ está bloqueado, False caso contrário</returns>
        bool IsCNPJBlocked(string cnpj);

        /// <summary>
        /// Obtém os detalhes do bloqueio fiscal para um CNPJ.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser consultado</param>
        /// <returns>Dicionário com os dados do bloqueio fiscal, ou null se não bloqueado</returns>
        Dictionary<string, object>? GetBlockedCNPJDetails(string cnpj);
    }
}
