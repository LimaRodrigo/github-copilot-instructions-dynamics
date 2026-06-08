namespace Account.Extend.Domain.ValueObjects
{
    /// <summary>
    /// Representa um valor de CNPJ validado.
    /// Encapsula a lógica de validação e manipulação de CNPJ.
    /// </summary>
    public class CNPJ
    {
        public string Value { get; private set; }

        private CNPJ(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Cria uma instância de CNPJ após validação.
        /// Lança InvalidCNPJException caso o CNPJ seja inválido.
        /// </summary>
        public static CNPJ Create(string? cnpj)
        {
            if (!IsValid(cnpj))
            {
                throw new Domain.Exceptions.InvalidCNPJException($"CNPJ inválido: {cnpj}");
            }

            return new CNPJ(RemoveFormatting(cnpj));
        }

        /// <summary>
        /// Valida um número de CNPJ usando o algoritmo de dígito verificador.
        /// </summary>
        private static bool IsValid(string? cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = RemoveFormatting(cnpj);

            if (cnpj.Length != 14)
                return false;

            if (!IsNumeric(cnpj))
                return false;

            // Rejeita CNPJs conhecidos como inválidos (todos os dígitos iguais)
            if (IsAllDigitsEqual(cnpj))
                return false;

            return ValidateCheckDigits(cnpj);
        }

        private static string RemoveFormatting(string? cnpj)
        {
            return cnpj?.Replace(".", "").Replace("/", "").Replace("-", "").Trim() ?? string.Empty;
        }

        private static bool IsNumeric(string cnpj)
        {
            foreach (char c in cnpj)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private static bool IsAllDigitsEqual(string cnpj)
        {
            char first = cnpj[0];
            foreach (char c in cnpj)
            {
                if (c != first)
                    return false;
            }
            return true;
        }

        private static bool ValidateCheckDigits(string cnpj)
        {
            // Validar primeiro dígito verificador
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;

            for (int i = 0; i < 12; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * multiplier1[i];
            }

            int remainder = sum % 11;
            int firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cnpj[12].ToString()) != firstCheckDigit)
                return false;

            // Validar segundo dígito verificador
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;

            for (int i = 0; i < 13; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * multiplier2[i];
            }

            remainder = sum % 11;
            int secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cnpj[13].ToString()) == secondCheckDigit;
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (!(obj is CNPJ other))
                return false;

            return Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
