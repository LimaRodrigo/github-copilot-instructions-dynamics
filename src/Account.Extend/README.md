# Account.Extend

## Descrição

Projeto de extensões para a entidade **Account** no Microsoft Dynamics 365 / Power Platform (Dataverse).

O assembly concentra plugins e regras de negócio relacionadas ao cadastro de contas, mantendo os plugins como adapters finos e delegando a lógica para serviços, value objects, repositórios e exceções de domínio.

## Plugins Disponíveis

### ValidarCNPJ

Plugin que valida o CNPJ informado na criação de registros de contas, impedindo a criação caso o CNPJ seja inválido.

**Nome do Plugin:** `Account.Extend.Plugins.ValidarCNPJ`

**Configuração no registro de Plugin:**
- **Message:** Create
- **Entity:** account
- **Stage:** Pre-operation
- **Execution Mode:** Synchronous
- **Atributo Monitorado:** `rcl_cnpj`

#### Regra de Negócio

O CNPJ do cliente, armazenado no atributo `rcl_cnpj`, deve ser validado na criação do registro de conta.

A validação inclui:
- Verificação de formato com 14 dígitos numéricos
- Verificação de dígitos verificadores pelo algoritmo módulo 11
- Rejeição de CNPJs com todos os dígitos iguais
- Suporte a CNPJs com ou sem formatação (`XX.XXX.XXX/XXXX-XX`)

### ValidarBloqueioFiscal

Plugin que consulta a base de Bloqueio Fiscal antes da criação da conta e impede o cadastro quando o CNPJ informado estiver marcado como bloqueado.

**Nome do Plugin:** `Account.Extend.Plugins.ValidarBloqueioFiscal`

**Configuração no registro de Plugin:**
- **Message:** Create
- **Entity:** account
- **Stage:** Pre-operation
- **Execution Mode:** Synchronous
- **Atributo Monitorado:** `rcl_cnpj`

#### Regra de Negócio

O CNPJ informado na conta deve ser consultado na entidade customizada `rcl_bloqueiofiscal`.

A validação de bloqueio fiscal:
- Ignora a validação quando o CNPJ não estiver preenchido
- Consulta registros de `rcl_bloqueiofiscal` pelo atributo `rcl_cnpj`
- Considera o CNPJ bloqueado quando o atributo `rcl_bloqueado` estiver marcado como verdadeiro
- Lança `BloqueioFiscalException` quando o CNPJ estiver bloqueado
- Mascara o CNPJ nos logs para reduzir exposição de dados sensíveis

## Fluxo Padrão de Execução

Os plugins seguem o mesmo padrão mínimo de execução:

1. Validar o contexto recebido pelo Dataverse
2. Validar profundidade para evitar reprocessamento indevido
3. Confirmar mensagem, entidade e parâmetros de entrada esperados
4. Criar o serviço de aplicação correspondente
5. Executar a regra de negócio

## Estrutura do Projeto

```text
src/Account.Extend/
|-- Domain/
|   |-- Exceptions/
|   |   |-- BloqueioFiscalException.cs
|   |   `-- InvalidCNPJException.cs
|   `-- ValueObjects/
|       `-- CNPJ.cs
|-- Infrastructure/
|   `-- LoggerAdapter.cs
|-- Plugins/
|   |-- ValidarBloqueioFiscal.cs
|   `-- ValidarCNPJ.cs
|-- Repositories/
|   |-- BloqueioFiscalRepository.cs
|   `-- IBloqueioFiscalRepository.cs
`-- Services/
    |-- BloqueioFiscalService.cs
    |-- CNPJValidationService.cs
    `-- IBloqueioFiscalService.cs

tests/Account.Extend.Tests/
|-- Domain/
|   `-- ValueObjects/
|       `-- CNPJTests.cs
|-- Plugins/
|   |-- ValidarBloqueioFiscalTests.cs
|   `-- ValidarCNPJTests.cs
`-- Services/
    `-- CNPJValidationServiceTests.cs
```

## Stack Técnico

- **.NET:** 4.7.1
- **Dynamics SDK:** Microsoft.CrmSdk.CoreAssemblies 9.0.0.5
- **Logging:** Microsoft.Extensions.Logging
- **Testes:** xUnit 2.4.1 + FakeXrmEasy 3.0
- **Arquitetura:** DDD + SOLID

## Como Usar

### Desenvolvimento

```csharp
using Account.Extend.Domain.ValueObjects;
using Account.Extend.Domain.Exceptions;

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

**CNPJ válido:**
- `11.222.333/0001-81` (formatado)
- `11222333000181` (sem formatação)

**CNPJs inválidos:**
- `12345678901234` (dígito verificador inválido)
- `11111111111111` (todos os dígitos iguais)
- `123` (comprimento inválido)

## Logging

Os plugins registram informações de execução no tracing nativo do Dynamics 365 por meio do `LoggerAdapter`.

Eventos comuns:
- **INFO:** Início e fim de execução, validações concluídas com sucesso e cenários não aplicáveis
- **WARNING:** Falhas de validação, CNPJ inválido, CNPJ bloqueado ou parâmetros ausentes
- **ERROR:** Erros inesperados durante a execução

## Padrões SOLID e DDD

- **Single Responsibility:** Cada classe possui uma responsabilidade clara
- **Open/Closed:** Novas regras podem ser adicionadas por novos plugins e serviços
- **Dependency Injection:** Serviços e repositórios recebem dependências por construtor
- **Value Object:** `CNPJ` encapsula validação e comportamento de CNPJ
- **Domain Exceptions:** Erros de domínio são representados por exceções específicas
- **Thin Adapter:** Plugins orquestram a execução e mantêm a lógica nos serviços

## Autores

Criado em 2026.

## Licença

Veja LICENSE no repositório raiz.
