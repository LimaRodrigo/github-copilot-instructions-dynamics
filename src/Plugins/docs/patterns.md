# Dynamics Plugin Patterns

## Plugin Pattern

Todo plugin deve:

1. Validar contexto
2. Validar depth
3. Criar service
4. Executar processamento

Exemplo:

public class AccountPreUpdatePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var context = PluginContextFactory.Create(serviceProvider);

        if (context.Depth > 1)
            return;

        var service = new AccountService(context);

        service.ValidateCreditLimit();
    }
}

---

# Service Pattern

Services:
- centralizam regras
- não acessam UI
- não conhecem pipeline diretamente

Exemplo:

public class AccountService
{
    public void ValidateCreditLimit()
    {
    }
}

---

# Repository Pattern

Repositories centralizam queries.

Exemplo:

public class AccountRepository
{
    public Entity GetById(Guid id)
    {
    }
}

---

# Logging Pattern

Sempre utilizar tracing:

tracingService.Trace("Mensagem");

---

# Exception Pattern

Sempre lançar:

InvalidPluginExecutionException

---

# Validation Pattern

Validações devem ficar em services.

Evitar validações complexas diretamente no plugin.

---

# Query Pattern

Sempre especificar colunas:

new ColumnSet("name", "statuscode")

Evitar:

new ColumnSet(true)

---