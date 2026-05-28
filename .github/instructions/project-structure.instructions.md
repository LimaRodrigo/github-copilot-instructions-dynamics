<!-- ---
applyTo: "src/**/*.cs, tests/**/*.cs"
---

# Estrutura de Pastas do Projeto

Todo plugin deve seguir esta estrutura dentro de `src/**/`:

- **Domain/** → Entidades, Value Objects, Domain Events, Exceptions
- **Services/** → Regras de negócio (Application Services)
- **Infrastructure/** → Implementações concretas (ex: logging, external services)
- **Repositories/** → Repositórios com padrão Query
- **Plugins/** → Classes que implementam IPlugin (apenas o ponto de entrada)

Sempre que criar um novo plugin, crie também o projeto de testes em `tests/**.tests`. -->