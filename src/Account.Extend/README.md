# Account.Extend - Plugin de Validação de CNPJ

## Descrição

Plugin para Microsoft Dynamics 365 / Power Platform (Dataverse) que valida o CNPJ na criação de registros de contas, impedindo a criação caso o CNPJ seja inválido.

## Regras de Negócio

O CNPJ do cliente (atributo `rcl_cnpj`) deve ser validado na criação do registro, impedindo a criação caso seja inválido.

### Validação de CNPJ

A validação inclui:
- Verificação de formato (14 dígitos numéricos)
- Verificação de dígitos verificadores (algoritmo mod 11)
- Rejeição de CNPJs com todos os dígitos iguais
- Suporte a CNPJs com ou sem formatação (XX.XXX.XXX/XXXX-XX)

## Configuração no Dynamics 365

**Nome do Plugin:** `Account.Extend.Plugins.ValidarCNPJ`

**Configuração no registro de Plugin:**
- **Message:** Create
- **Entity:** account
- **Stage:** Pre-operation (Validação)
- **Execution Mode:** Synchronous
- **Atributo Monitorado:** rcl_cnpj

## Estrutura do Projeto

```
src/Account.Extend/
├── Domain/
│   ├── ValueObjects/
│   │   └── CNPJ.cs                    # Value Object para encapsular lógica de CNPJ
│   └── Exceptions/
│       └── InvalidCNPJException.cs    # Exceção de CNPJ inválido
├── Services/
│   └── CNPJValidationService.cs       # Serviço com regras de negócio
├── Infrastructure/
│   └── LoggerAdapter.cs               # Adaptador de logging para ILogger
└── Plugins/
    └── ValidarCNPJ.cs                 # Plugin (thin adapter)

tests/Account.Extend.Tests/
├── Domain/
│   └── ValueObjects/
│       └── CNPJTests.cs               # Testes do Value Object CNPJ
├── Services/
│   └── CNPJValidationServiceTests.cs  # Testes do serviço de validação
└── Plugins/
    └── ValidarCNPJTests.cs            # Testes de integração do plugin
```

## Stack Técnico

- **.NET:** 4.7.1
- **Dynamics SDK:** Microsoft.CrmSdk.CoreAssemblies 9.0.0.5
- **Logging:** Microsoft.Extensions.Logging
- **Testes:** xUnit 2.4.1 + FakeXrmEasy 3.0
- **Arquitetura:** Domain-Driven Design (DDD) + SOLID

## Fluxo de Execução do Plugin

1. **Validação de Contexto** - Verifica se o contexto do plugin é válido
2. **Validação de Profundidade** - Ignora execuções com profundidade > 1 (evita loops)
3. **Criação do Service** - Instancia o serviço de validação
4. **Processamento** - Extrai o CNPJ do registro e valida

## Como Usar

### Desenvolvimento

```csharp
// Importar o namespace
using Account.Extend.Domain.ValueObjects;

// Criar e validar um CNPJ
try
{
    var cnpj = CNPJ.Create("11.222.333/0001-81");
    Console.WriteLine($"CNPJ válido: {cnpj.Value}");
}
catch (InvalidCNPJException ex)
{
    Console.WriteLine($"CNPJ inválido: {ex.Message}");
}
```

### Testes

Execute os testes unitários com xUnit:

```bash
dotnet test tests/Account.Extend.Tests/Account.Extend.Tests.csproj
```

## Exemplos de CNPJs para Testes

**CNPJ Válido:**
- `11.222.333/0001-81` (formatado)
- `11222333000181` (sem formatação)

**CNPJs Inválidos:**
- `12345678901234` (dígito verificador inválido)
- `11111111111111` (todos os dígitos iguais)
- `123` (comprimento inválido)

## Logging

O plugin registra as seguintes informações:

- **INFO:** Início/fim da validação, CNPJ validado com sucesso
- **WARNING:** CNPJ vazio, validação falhou
- **ERROR:** Erros inesperados durante a validação

Os logs são capturados pelo serviço de tracing do Dynamics 365.

## Padrões SOLID e DDD

- **Single Responsibility:** Cada classe tem uma responsabilidade clara
- **Open/Closed:** Fácil estender sem modificar código existente
- **Dependency Injection:** Serviços injetáveis via construtor
- **Value Object:** CNPJ encapsula validação e comportamento
- **Domain Exception:** Erros de domínio com InvalidCNPJException
- **Thin Adapter:** Plugin apenas orquestra, lógica no Service

## Autores

Criado em 2026.

## Licença

Veja LICENSE no repositório raiz.
