using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Account.Extend.Plugins;
using Account.Extend.Domain.Exceptions;
using Account.Extend.Services;

namespace Account.Extend.Tests.Plugins
{
    /// <summary>
    /// Testes unitários para o plugin ValidarBloqueioFiscal.
    /// Valida o comportamento do plugin usando Moq para mock dos serviços.
    /// </summary>
    public class ValidarBloqueioFiscalTests
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
            mockContext.Setup(c => c.PrimaryEntityName).Returns(entityName);
            mockContext.Setup(c => c.PrimaryEntityId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.IsExecutingOffline).Returns(false);
            mockContext.Setup(c => c.IsInTransaction).Returns(false);
            mockContext.Setup(c => c.BusinessUnitId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.OrganizationId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.OrganizationName).Returns("TestOrganization");
            mockContext.Setup(c => c.UserId).Returns(Guid.NewGuid());
            mockContext.Setup(c => c.InitiatingUserId).Returns(Guid.NewGuid());

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
            
            // Mock da query para Bloqueio Fiscal - CNPJ não bloqueado
            mockOrgService
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection()); // Sem registros = não bloqueado

            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

            // Act & Assert - Should not throw exception
            plugin.Execute(mockServiceProvider.Object);
        }

        [Fact]
        public void Given_AccountWithBlockedCNPJ_When_PluginExecutes_Then_ShouldThrowException()
        {
            // Arrange
            var blockedCnpj = "12.345.678/0001-90";
            var account = new Entity("account")
            {
                ["name"] = "Blocked Account",
                ["rcl_cnpj"] = blockedCnpj
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            var mockOrgService = new Mock<IOrganizationService>();
            
            // Mock da query para Bloqueio Fiscal - CNPJ bloqueado
            var blockedEntity = new Entity("rcl_bloqueiofiscal")
            {
                ["rcl_cnpj"] = blockedCnpj,
                ["rcl_bloqueado"] = true
            };
            mockOrgService
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new[] { blockedEntity }));

            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

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
            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

            // Act & Assert - Should not throw exception
            plugin.Execute(mockServiceProvider.Object);
        }

        [Fact]
        public void Given_AccountWithEmptyCNPJ_When_PluginExecutes_Then_ShouldSucceed()
        {
            // Arrange
            var account = new Entity("account")
            {
                ["name"] = "Test Account",
                ["rcl_cnpj"] = string.Empty
            };

            var mockContext = GetMockPluginContext("Create", "account", account);
            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

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
            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

            // Act & Assert - Should return early without processing
            plugin.Execute(mockServiceProvider.Object);

            // Verify RetrieveMultiple was never called (because depth check prevented it)
            mockOrgService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Never);
        }

        [Fact]
        public void Given_UpdateMessage_When_PluginExecutes_Then_ShouldNotProcess()
        {
            // Arrange
            var account = new Entity("account")
            {
                ["name"] = "Test Account",
                ["rcl_cnpj"] = "11.222.333/0001-81"
            };

            var mockContext = GetMockPluginContext("Update", "account", account); // Update instead of Create
            var mockOrgService = new Mock<IOrganizationService>();
            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

            // Act & Assert
            plugin.Execute(mockServiceProvider.Object);

            // Verify RetrieveMultiple was never called (because message is not Create)
            mockOrgService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Never);
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
            
            mockOrgService
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection()); // Sem registros = não bloqueado

            var mockServiceProvider = GetMockServiceProvider(mockContext.Object, mockOrgService.Object);
            var plugin = new ValidarBloqueioFiscal();

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
