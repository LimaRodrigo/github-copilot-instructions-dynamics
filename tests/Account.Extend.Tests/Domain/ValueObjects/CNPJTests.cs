using Xunit;
using Account.Extend.Domain.Exceptions;
using Account.Extend.Domain.ValueObjects;

namespace Account.Extend.Tests.Domain.ValueObjects
{
    /// <summary>
    /// Testes unitários para o Value Object CNPJ.
    /// Valida a criação e validação de CNPJs com diferentes formatos e valores.
    /// </summary>
    public class CNPJTests
    {
        [Fact]
        public void Given_ValidCNPJWithoutFormatting_When_Create_Then_ShouldSucceed()
        {
            // Arrange
            string validCnpj = "11222333000181"; // CNPJ válido: 11.222.333/0001-81

            // Act
            var cnpj = CNPJ.Create(validCnpj);

            // Assert
            Assert.NotNull(cnpj);
            Assert.Equal("11222333000181", cnpj.Value);
        }

        [Fact]
        public void Given_ValidCNPJWithFormatting_When_Create_Then_ShouldSucceed()
        {
            // Arrange
            string validCnpj = "11.222.333/0001-81"; // CNPJ válido formatado

            // Act
            var cnpj = CNPJ.Create(validCnpj);

            // Assert
            Assert.NotNull(cnpj);
            Assert.Equal("11222333000181", cnpj.Value);
        }

        [Fact]
        public void Given_InvalidCNPJLength_When_Create_Then_ShouldThrowException()
        {
            // Arrange
            string invalidCnpj = "123"; // Tamanho inválido

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => CNPJ.Create(invalidCnpj));
        }

        [Fact]
        public void Given_InvalidCNPJNonNumeric_When_Create_Then_ShouldThrowException()
        {
            // Arrange
            string invalidCnpj = "1122233300018A"; // Contém letra

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => CNPJ.Create(invalidCnpj));
        }

        [Fact]
        public void Given_InvalidCNPJAllDigitsEqual_When_Create_Then_ShouldThrowException()
        {
            // Arrange
            string invalidCnpj = "11111111111111"; // Todos os dígitos iguais

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => CNPJ.Create(invalidCnpj));
        }

        [Fact]
        public void Given_InvalidCNPJWrongCheckDigit_When_Create_Then_ShouldThrowException()
        {
            // Arrange
            string invalidCnpj = "11222333000182"; // Dígito verificador incorreto

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => CNPJ.Create(invalidCnpj));
        }

        [Fact]
        public void Given_NullCNPJ_When_Create_Then_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => CNPJ.Create(null));
        }

        [Fact]
        public void Given_EmptyCNPJ_When_Create_Then_ShouldThrowException()
        {
            // Arrange
            string emptyCnpj = "";

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => CNPJ.Create(emptyCnpj));
        }

        [Fact]
        public void Given_TwoCNPJsWithSameValue_When_Equals_Then_ShouldReturnTrue()
        {
            // Arrange
            var cnpj1 = CNPJ.Create("11.222.333/0001-81");
            var cnpj2 = CNPJ.Create("11222333000181");

            // Act & Assert
            Assert.Equal(cnpj1, cnpj2);
        }

        [Fact]
        public void Given_ValidCNPJ_When_ToString_Then_ShouldReturnFormattedValue()
        {
            // Arrange
            var cnpj = CNPJ.Create("11.222.333/0001-81");

            // Act
            string result = cnpj.ToString();

            // Assert
            Assert.Equal("11222333000181", result);
        }
    }
}
