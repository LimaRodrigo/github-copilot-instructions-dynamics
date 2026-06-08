using System;

namespace Account.Extend.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um CNPJ inválido é fornecido.
    /// </summary>
    public class InvalidCNPJException : Exception
    {
        public InvalidCNPJException(string message) : base(message)
        {
        }

        public InvalidCNPJException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
