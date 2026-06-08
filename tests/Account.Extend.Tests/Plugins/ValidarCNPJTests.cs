using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Xrm.Sdk;
using Account.Extend.Plugins;
using Account.Extend.Domain.Exceptions;

namespace Account.Extend.Tests.Plugins
{
    /// <summary>
    /// Testes unitários para o plugin ValidarCNPJ.
    /// Valida o comportamento do plugin usando Moq para mock dos serviços.
    /// </summary>
    public class ValidarCNPJTests
    {
        private Mock<IPluginExecutionContext> GetMockPluginContext(
            string messageName,
            string entityName,
            Entity targetEntity)
        {
            var mockContext = new Mock<IPluginExecutionContext>();
            mockContext.Setup(c => c.MessageName).Returns(messageName);
            mockContext.Setup(c => c.Depth).Returns(1);
            mockContext.Setup(c => c.InputParameters).Returns(
                new ParameterCollection 
                { 
                    { "Target", targetEntity } 
                });
            mockContext.Setup(c => c.CorrelationId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.RequestId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.IsExecutingOffline).Returns(false);
            mockContext.Setup(c => c.IsInTransaction).Returns(false);
            mockContext.Setup(c => c.BusinessUnitId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.OrganizationId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.OrganizationName).Returns("TestOrganization");
            mockContext.Setup(c => c.UserId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.InitiatingUserId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.PrimaryEntityId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.PrimaryEntityName).Returns(entityName);

            return mockContext;
        }

        private Mock<IServiceProvider> GetMockServiceProvider(
            IPluginExecutionContext pluginContext,
            IOrganizationService organizationService)
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(s => s.GetService(typeof(IPluginExecutionContext)))
                .Returns(pluginContext);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(IOrganizationService)))
                .Returns(organizationService);
            mockServiceProvider
                .Setup(s => s.GetService(typeof(ITracingService)))
                .Returns(new MockTracingService());

            return mockServiceProvider;
        }

        [Fact]
        public void Given_AccountWithValidCNPJ_When_PluginExecutes_Then_ShouldSucceed()
        {
            // Arrange
            var validCnpj = "11.222.333/0001-81";
            var account = new Entity("account")
            {
                ["name"] = "Test Account",
                ["rcl_cnpj"] = validCnpj
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(
                mockContext.Object, 
                mockOrgService.Object);

            var plugin = new ValidarCNPJ();

            // Act & Assert - Should not throw exception
            plugin.Execute(mockServiceProvider.Object);
        }

        [Fact]
        public void Given_AccountWithInvalidCNPJ_When_PluginExecutes_Then_ShouldThrowException()
        {
            // Arrange
            var invalidCnpj = "12345678901234"; // CNPJ inválido
            var account = new Entity("account")
            {
                ["name"] = "Test Account",
                ["rcl_cnpj"] = invalidCnpj
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(
                mockContext.Object,
                mockOrgService.Object);

            var plugin = new ValidarCNPJ();

            // Act & Assert
            Assert.Throws<InvalidPluginExecutionException>(() =>
                plugin.Execute(mockServiceProvider.Object));
        }

        [Fact]
        public void Given_AccountWithoutCNPJField_When_PluginExecutes_Then_ShouldSucceed()
        {
            // Arrange
            var account = new Entity("account")
            {
                ["name"] = "Test Account"
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(
                mockContext.Object,
                mockOrgService.Object);

            var plugin = new ValidarCNPJ();

            // Act & Assert - Should not throw exception
            plugin.Execute(mockServiceProvider.Object);
        }

        [Fact]
        public void Given_DepthGreaterThanOne_When_PluginExecutes_Then_ShouldReturn()
        {
            // Arrange
            var validCnpj = "11.222.333/0001-81";
            var account = new Entity("account")
            {
                ["name"] = "Test Account",
                ["rcl_cnpj"] = validCnpj
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            mockContext.Setup(c => c.Depth).Returns(2); // Depth > 1

            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(
                mockContext.Object,
                mockOrgService.Object);

            var plugin = new ValidarCNPJ();

            // Act & Assert - Should return early without processing
            plugin.Execute(mockServiceProvider.Object);
        }

        [Theory]
        [InlineData("11.222.333/0001-81")]
        [InlineData("34.028.316/0001-03")]
        public void Given_MultipleValidCNPJs_When_PluginExecutes_Then_ShouldSucceed(string cnpj)
        {
            // Arrange
            var account = new Entity("account")
            {
                ["name"] = "Test Account",
                ["rcl_cnpj"] = cnpj
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(
                mockContext.Object,
                mockOrgService.Object);

            var plugin = new ValidarCNPJ();

            // Act & Assert
            plugin.Execute(mockServiceProvider.Object);
        }

        /// <summary>
        /// Mock implementation of ITracingService for testing purposes.
        /// </summary>
        private class MockTracingService : ITracingService
        {
            public void Trace(string message)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }

            public void Trace(string format, params object[] args)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(format, args));
            }
        }
    }
}
