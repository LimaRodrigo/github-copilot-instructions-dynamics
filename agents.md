# Estrutura Organizacional do Monorepo

Todo novo projeto deve ser criado na stack correspondente.

---

# Estrutura Oficial

/src
    /Plugins
    /PCFControls
    /Stripts

/tests
    /Plugins.Tests
    /PCFControls.Tests
    /Stripts.Tests

---

# Regras de Organização

## Dynamics 365 Plugins

Todo plugin Dynamics 365 deve ser criado em:

`/src/Plugins`


Testes devem ser criados em:

`/tests/Plugins.Tests`

---

## PCF Controls

Todo componente PCF React/TypeScript deve ser criado em:

`/src/PCFControls`

Testes:

`/tests/PCFControls.Tests`

---
## Scritps Js (Recurso Web)

Todo componente Script javascript deve ser criado em:

`/src/Scripts`

Testes:

`/tests/Scripts.Tests`

---

# Regra Geral

Ao criar novos arquivos ou projetos:
- identificar corretamente a stack
- utilizar a pasta correspondente
- respeitar a arquitetura da stack
- seguir os agents.md locais
- seguir os docs locais