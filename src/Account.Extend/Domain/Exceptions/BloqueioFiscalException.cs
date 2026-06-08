using System;

namespace Account.Extend.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um CNPJ está bloqueado no Bloqueio Fiscal.
    /// </summary>
    public class BloqueioFiscalException : Exception
    {
        public string? CNPJ { get; }

        public BloqueioFiscalException(string message) 
            : base(message)
        {
        }

        public BloqueioFiscalException(string message, string cnpj) 
            : base(message)
        {
            CNPJ = cnpj;
        }

        public BloqueioFiscalException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        public BloqueioFiscalException(string message, string cnpj, Exception innerException) 
            : base(message, innerException)
        {
            CNPJ = cnpj;
        }
    }
}
