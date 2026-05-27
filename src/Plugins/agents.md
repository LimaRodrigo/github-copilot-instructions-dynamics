# Dynamics 365 Plugins - AI Guidelines

## Objetivo

Este módulo contém plugins Microsoft Dynamics 365 / Dataverse desenvolvidos em C#.

Toda implementação deve seguir:
- SOLID
- Clean Architecture
- Baixo acoplamento
- Alta testabilidade
- Performance
- Reutilização

---

# Estrutura Obrigatória

/src/DynamicsPlugins
    /Plugins
    /Services
    /Repositories
    /Domain
    /Infrastructure
    /Common

---

# Responsabilidades

## Plugins

Responsáveis apenas por:
- validar contexto
- validar stage
- validar depth
- orquestrar chamadas
- executar tracing

Plugins NÃO devem conter:
- regra de negócio
- queries complexas
- lógica de integração

---

## Services

Responsáveis por:
- regras de negócio
- validações
- processamento principal

---

## Repositories

Responsáveis por:
- Retrieve
- RetrieveMultiple
- queries Dataverse
- centralização de acesso a dados

---

# Regras Técnicas

## Context Validation

Sempre validar:
- MessageName
- Stage
- PrimaryEntityName
- Depth

Exemplo:

if (context.Depth > 1)
    return;

---

## Performance

Evitar:
- ColumnSet(true)
- retrieves desnecessários
- loops com chamadas ao Dataverse
- LINQ pesado

Preferir:

new ColumnSet("name", "emailaddress1")

---

## Logging

Sempre utilizar tracing:

tracingService.Trace("Iniciando processamento");

Registrar:
- início
- fim
- parâmetros importantes
- exceptions

---

## Exceptions

Sempre utilizar:

throw new InvalidPluginExecutionException("Mensagem amigável");

Nunca expor exception técnica ao usuário.

---

# Qualidade

Toda implementação deve possuir testes unitários.

Os testes devem ser criados no projeto:

/tests/Plugins.Tests

---

# Segurança

Nunca utilizar:
- GUID hardcoded
- URLs hardcoded
- credenciais no código

Utilizar:
- Environment Variables
- Secure Config
- Unsecure Config

---

# Objetivo da Arquitetura

A arquitetura deve priorizar:
- manutenção
- escalabilidade
- testabilidade
- legibilidade
- desacoplamento