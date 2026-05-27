# Dynamics Plugins Tests - AI Guidelines

## Objetivo

Este módulo contém testes unitários e de integração para plugins Dynamics 365 / Dataverse.

Toda implementação de teste deve priorizar:
- legibilidade
- isolamento
- previsibilidade
- baixo acoplamento
- manutenção simples

---

# Frameworks

Utilizar:
- xUnit
- FakeXrmEasy

---

# Estrutura Esperada

/tests/DynamicsPlugins.Tests
    /Plugins
    /Services
    /Repositories
    /Builders
    /Fixtures
    /Mocks
    /Helpers

---

# Organização dos Testes

## Plugins

Testar:
- validação de contexto
- depth
- execução da regra
- chamadas de services
- exceptions

---

## Services

Testar:
- regras de negócio
- validações
- cenários positivos
- cenários negativos

---

## Repositories

Testar:
- queries
- filtros
- retorno esperado

---

# Naming Convention

Utilizar padrão:

Should_[ResultadoEsperado]_When_[Cenario]

Exemplo:

Should_Create_Task_When_Account_Is_VIP

---

# Padrão AAA

Todos os testes devem seguir:

- Arrange
- Act
- Assert

---

# Boas práticas

Priorizar:
- testes pequenos
- um cenário por teste
- asserts objetivos
- builders reutilizáveis

Evitar:
- mocks excessivos
- múltiplos cenários no mesmo teste
- lógica complexa no teste
- asserts genéricos

---

# FakeXrmEasy

Utilizar:
- XrmFakedContext
- entidades fake
- organization service fake

Evitar dependência real do Dataverse.

---

# Builders

Utilizar builders para criação de entidades complexas.

Exemplo:

AccountBuilder
ContactBuilder

---

# Objetivo dos Testes

Os testes devem garantir:
- estabilidade
- previsibilidade
- segurança para refatoração
- validação de regras críticas