# github-copilot-instructions-dynamics

Repositório de referência para desenvolvimento de plugins C# para Microsoft Dynamics 365 / Power Platform (Dataverse), orientado por instruções do GitHub Copilot.

O objetivo deste projeto é padronizar como novos plugins, serviços, repositórios, testes e documentações devem ser criados, mantendo arquitetura DDD, princípios SOLID, observabilidade e testes automatizados.

## Como Funcionam as Instructions

As instruções ficam na pasta `.github` e em suas subpastas. Elas orientam o Copilot sobre padrões obrigatórios do repositório e sobre regras específicas para cada tipo de arquivo.

## Pontos Mais Relevantes

De acordo com a documentação do GitHub Copilot, as instruções personalizadas do repositório são adicionadas automaticamente às solicitações feitas ao Copilot quando a conversa ou tarefa está no contexto deste repositório.

Isso significa que não é necessário copiar manualmente o conteúdo das instructions em cada prompt. O Copilot deve considerar automaticamente os arquivos de instrução relevantes, desde que o chat, a edição ou a tarefa esteja associada ao repositório.

Pontos importantes:

- **Contexto do repositório:** as instructions são aplicadas quando o Copilot está trabalhando com este repositório como contexto.
- **Instruction geral:** `.github/copilot-instructions.md` define as regras globais do projeto.
- **Instructions por escopo:** arquivos em `.github/instructions/*.instructions.md` usam `applyTo` para indicar em quais caminhos devem ser aplicados.
- **Aplicação seletiva:** uma instruction modular só entra quando o arquivo ou tarefa corresponde ao padrão definido em `applyTo`.
- **Verificação no GitHub Copilot Chat:** quando estiver usando o Copilot no GitHub, é possível conferir as referências da resposta para verificar se `.github/copilot-instructions.md` foi usado.
- **Boa prática:** mesmo com aplicação automática, prompts claros ajudam o Copilot a escolher melhor os arquivos, camadas e padrões envolvidos.

### Instrução Geral do Repositório

Arquivo: `.github/copilot-instructions.md`

Define as regras globais para qualquer geração ou alteração de código neste repositório:

- O projeto é 100% C# para plugins Dynamics 365 / Dataverse
- A arquitetura deve seguir DDD e SOLID
- As camadas principais são `Domain`, `Services`, `Infrastructure`, `Repositories` e `Plugins`
- Plugins devem ser thin adapters
- Logs devem usar `Microsoft.Extensions.Logging` com tracing do Dynamics
- Novos plugins devem vir acompanhados do respectivo projeto de testes
- Projetos `.csproj` devem ser adicionados à solução `.sln` da raiz
- Cada projeto deve ter um `README.md`
- Projetos devem usar SDK-style e `PackageReference`

Essa instruction é a base do comportamento esperado para todo o repositório.

### Instructions Modulares

As regras específicas ficam em `.github/instructions/`. Cada arquivo possui um `applyTo`, que indica em quais arquivos a regra deve ser aplicada.

#### Desenvolvimento de Plugins

Arquivo: `.github/instructions/plugin-development.instructions.md`

Aplicação: `src/**/*.cs`

Define que toda classe de plugin deve executar apenas quatro passos, nesta ordem:

1. Validar o contexto (`IPluginExecutionContext`)
2. Validar a profundidade (`context.Depth`)
3. Criar o service necessário
4. Executar o processamento no service

A lógica de negócio não deve ficar dentro do plugin. Ela deve ficar nos serviços, value objects, entidades de domínio ou repositórios apropriados.

#### Padrão de Repositories

Arquivo: `.github/instructions/repository-pattern.instructions.md`

Aplicação: `src/**/Repositories/**/*.cs`

Define o padrão para consultas ao Dataverse:

- Repositórios devem seguir padrão Query
- Sempre informar explicitamente as colunas em `ColumnSet`
- Nunca usar `new ColumnSet(true)`
- Usar métodos claros e específicos, como `GetById`, `FindByEmail` ou `GetActiveAccounts`
- Retornar `Entity` ou modelo de domínio mapeado
- Repositórios devem ser instanciados por serviços, não diretamente por plugins

#### Padrões de Testes

Arquivo: `.github/instructions/testing.instructions.md`

Aplicação: `tests/**/*.cs`

Define o padrão para testes com xUnit e FakeXrmEasy:

- Um cenário por teste
- Asserts claros e objetivos
- Uso de builders para entidades complexas
- Testes pequenos, focados e legíveis
- Evitar mocks excessivos
- Evitar múltiplos cenários no mesmo teste
- Nomenclatura recomendada: `Given_Condicao_When_Acao_Then_ResultadoEsperado`

## Como Usar no Dia a Dia

Ao pedir para o Copilot criar ou alterar um plugin, mencione a regra de negócio, a entidade do Dataverse, a mensagem do plugin, o estágio de execução e os atributos envolvidos.

O Copilot deve usar as instructions como contrato de implementação:

- `.github/copilot-instructions.md` para padrões gerais do repositório
- `.github/instructions/plugin-development.instructions.md` para estrutura do plugin
- `.github/instructions/repository-pattern.instructions.md` quando houver consulta ao Dataverse
- `.github/instructions/testing.instructions.md` para criar ou ajustar testes

## Exemplo de Prompt

```text
Crie um novo projeto de plugin Dynamics chamado Contact.Extend seguindo as instructions do repositório.

Regra de negócio:
Ao criar um contato, validar se o campo emailaddress1 foi informado e se possui formato válido.

Configuração do plugin:
- Message: Create
- Entity: contact
- Stage: Pre-operation
- Execution Mode: Synchronous
- Atributo monitorado: emailaddress1

Use as referências:
- .github/copilot-instructions.md
- .github/instructions/plugin-development.instructions.md
- .github/instructions/testing.instructions.md

Crie também o projeto de testes correspondente, adicione os projetos à solution da raiz e gere um README.md para o novo projeto.
```
## Prompts Utilizados
```
Tarefa:
Crie um novo projeto de plugin para o dynamics crm 365, com o nome 'Account.Extend', e o plugin de nome 'ValidarCNPJ'
Regras de negócios:
O cnpj("rcl_cnpj") do cliente ("account") deve ser validado na criação do registro, impedindo a criação caso seja inválido.
Observações:
Consulte as instruções
```
```
Tarefa:
Crie um novo plugin de nome 'ValidarBloqueioFiscal' para o dynamics crm 365, no projeto existente 'Account.Extend'
Regras de negócios:
De acordo com o cnpj("rcl_cnpj") do cliente ("account") deve ser validado na criação do registro, impedindo a criação caso seja inválido, consultando od dados de Bloqueio Fiscal(rcl_bloqueiofiscal).
Observações:
Consulte as instruções
```