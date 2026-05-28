using Xunit;
using System;
using Microsoft.Extensions.Logging;
using Moq;
using Account.Extend.Domain.Exceptions;
using Account.Extend.Services;

namespace Account.Extend.Tests.Services
{
    /// <summary>
    /// Testes unitários para o serviço de validação de CNPJ.
    /// Valida a execução das regras de negócio de validação.
    /// </summary>
    public class CNPJValidationServiceTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly CNPJValidationService _service;

        public CNPJValidationServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _service = new CNPJValidationService(_loggerMock.Object);
        }

        [Fact]
        public void Given_ValidCNPJ_When_ValidateCNPJ_Then_ShouldNotThrow()
        {
            // Arrange
            string validCnpj = "11.222.333/0001-81";

            // Act & Assert
            _service.ValidateCNPJ(validCnpj);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("validado com sucesso")),
                    It.IsAny<Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Given_InvalidCNPJ_When_ValidateCNPJ_Then_ShouldThrowException()
        {
            // Arrange
            string invalidCnpj = "11222333000182";

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => _service.ValidateCNPJ(invalidCnpj));
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("falhou")),
                    It.IsAny<Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Given_NullCNPJ_When_ValidateCNPJ_Then_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => _service.ValidateCNPJ(null));
        }

        [Fact]
        public void Given_EmptyCNPJ_When_ValidateCNPJ_Then_ShouldThrowException()
        {
            // Arrange
            string emptyCnpj = "";

            // Act & Assert
            Assert.Throws<InvalidCNPJException>(() => _service.ValidateCNPJ(emptyCnpj));
        }

        [Fact]
        public void Given_ValidCNPJ_When_ValidateCNPJ_Then_ShouldLogInformation()
        {
            // Arrange
            string validCnpj = "11.222.333/0001-81";

            // Act
            _service.ValidateCNPJ(validCnpj);

            // Assert
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Iniciando")),
                    It.IsAny<Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
