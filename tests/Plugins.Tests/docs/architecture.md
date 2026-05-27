# Dynamics Plugins Tests Architecture

## Objetivo

Garantir qualidade e previsibilidade dos plugins Dynamics 365 através de testes automatizados.

---

# Estrutura

/tests/DynamicsPlugins.Tests
    /Plugins
    /Services
    /Repositories
    /Builders
    /Fixtures
    /Mocks

---

# Estratégia

A estratégia de testes segue:

- Unit Tests
- Isolamento
- Arrange / Act / Assert
- Mocking controlado

---

# Plugins

Os testes de plugins devem validar:
- execução do pipeline
- contexto correto
- depth
- exceptions
- chamadas esperadas

---

# Services

Services devem ser testados isoladamente.

Objetivo:
- validar regras de negócio
- validar cenários positivos e negativos

---

# Repositories

Repositories devem validar:
- queries
- filtros
- dados retornados

---

# Builders

Builders devem:
- simplificar setup
- evitar duplicação
- gerar entidades consistentes

---

# FakeXrmEasy

Utilizamos FakeXrmEasy para:
- mock do Dataverse
- fake organization service
- fake execution context

---

# Cobertura

Priorizar:
- regras críticas
- validações
- fluxos principais

Evitar testes redundantes.