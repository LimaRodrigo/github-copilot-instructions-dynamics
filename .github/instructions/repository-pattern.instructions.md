---
applyTo: "src/**/Repositories/**/*.cs"
---

# Padrão de Repositories

Os repositórios devem seguir o padrão Query (nunca comandos misturados com queries quando possível).

**Regras obrigatórias:**
- Sempre especifique explicitamente as colunas:
  ```csharp
  new ColumnSet("name", "statuscode", "new_campo1")
  ```
- Nunca use new ColumnSet(true) (traz todas as colunas — proibido)
- Use métodos claros e específicos: GetById, GetActiveAccounts, FindByEmail, etc.
- Retorne Entity ou modelo de domínio mapeado (preferência por modelo rico quando possível)
- Devem ser instanciados apenas dentro de serviços nunca dentro de plugins ou controllers diretamente