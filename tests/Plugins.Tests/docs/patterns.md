# Dynamics Plugins Test Patterns

## Test Pattern

Todos os testes devem seguir AAA:

Arrange
Act
Assert

---

# Naming Pattern

Padrão obrigatório:

Should_[ResultadoEsperado]_When_[Cenario]

Exemplo:

Should_Throw_Exception_When_Credit_Limit_Is_Invalid

---

# Plugin Test Pattern

Exemplo:

[Fact]
public void Should_Execute_Plugin_When_Context_Is_Valid()
{
    // Arrange

    // Act

    // Assert
}

---

# Builder Pattern

Utilizar builders para criação de entidades.

Exemplo:

var account = new AccountBuilder()
    .WithName("Empresa XPTO")
    .WithCreditLimit(50000)
    .Build();

---

# Fake Context Pattern

Utilizar:

XrmFakedContext

Exemplo:

var context = new XrmFakedContext();

---

# Assertion Pattern

Assertions devem ser:
- objetivas
- específicas
- legíveis

Evitar asserts genéricos.

---

# Mock Pattern

Mocks devem:
- ser mínimos
- focados no cenário
- evitar acoplamento

---

# Exception Pattern

Validar exceptions utilizando:

Assert.Throws<InvalidPluginExecutionException>()

---

# Isolation Pattern

Cada teste deve ser independente.

Nunca compartilhar estado entre testes.