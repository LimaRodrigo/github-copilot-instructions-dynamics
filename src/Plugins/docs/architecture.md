# Dynamics Plugins Architecture

## Arquitetura

O projeto segue arquitetura em camadas:

Plugin
    -> Service
        -> Repository
            -> Dataverse

---

# Camadas

## Plugins

Responsáveis por:
- entrada do pipeline
- validação do contexto
- tracing
- orquestração

---

## Services

Responsáveis por:
- regras de negócio
- validações
- fluxos de processamento

---

## Repositories

Responsáveis por:
- acesso ao Dataverse
- queries
- retrieve/retrievemultiple

---

## Domain

Contém:
- DTOs
- Models
- Enums
- Constants

---

## Infrastructure

Contém:
- factories
- context helpers
- organization service helpers

---

# Pipeline do Dynamics

Validar sempre:
- Message
- Stage
- Mode
- Depth

---

# Stages suportados

## PreValidation

Validações antes da transação.

## PreOperation

Manipulação antes da persistência.

## PostOperation

Ações após persistência.

---

# Observabilidade

Utilizamos:
- ITracingService
- logs estruturados
- tracing padronizado

---

# Estratégia de Performance

- minimizar chamadas ao Dataverse
- evitar retrieves desnecessários
- utilizar ColumnSet específicos
- evitar processamento pesado no plugin

---