# Instruções Gerais do Repositório - Plugins Dynamics CRM

Este é um projeto 100% C# de plugins para Microsoft Dynamics 365 / Power Platform (Dataverse).

**Padrões obrigatórios do projeto:**
- Arquitetura baseada em DDD (Domain-Driven Design) e SOLID
- Camadas principais: Domain, Services (Application), Infrastructure, Repositories
- Observabilidade com logs estruturados (Microsoft.Extensions.Logging)
- Todo plugin deve seguir rigorosamente o fluxo mínimo de execução
- Sempre que um novo plugin for criado, deve ser gerado automaticamente o projeto de testes correspondente
- Todo projeto *.csproj deve ser addicionado ao arquivo de solução `*.sln` na raiz do repositório
- Todo projeto deve conter um arquivo `README.md` com descrição e instruções de uso
- Todo projeto *.csproj deve ser gerado com o SDK-style e usar PackageReference para dependências

**Nomeação de projetos:**
- Projeto principal: `src/NomeDoPlugin`
- Projeto de testes: `tests/NomeDoPlugin.tests`

**Stack técnica:**
- .NET 4.7.1
- Microsoft.CrmSdk.XrmTooling.CoreAssembly / Microsoft.PowerPlatform.Dataverse.Client
- xUnit + FakeXrmEasy para testes
- ILogger com logs estruturados e tracingService para observabilidade

# Estrutura de Pastas do Projeto
Todo plugin deve seguir esta estrutura dentro de `src/**/`:
- **Domain/** → Entidades, Value Objects, Domain Events, Exceptions
- **Services/** → Regras de negócio (Application Services)
- **Infrastructure/** → Implementações concretas (ex: logging, external services)
- **Repositories/** → Repositórios com padrão Query
- **Plugins/** → Classes que implementam IPlugin (apenas o ponto de entrada)

Copilot deve sempre respeitar SOLID, DDD e as regras específicas definidas nos arquivos modulares em `.github/instructions/`.