---
applyTo: "src/**/*.cs"
---

# Regras de Desenvolvimento de Plugins

Todo plugin deve **executar apenas** estes 4 passos na ordem exata:

1. Validar o contexto (`IPluginExecutionContext`)
2. Validar a profundidade (`context.Depth`)
3. Criar o Service necessário (via DI ou factory)
4. Executar o processamento no Service

**Nunca** coloque lógica de negócio diretamente dentro da classe do plugin.  
O plugin é apenas um thin adapter.

Exemplo mínimo de estrutura:
```csharp
public class NomePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        // 1. Validar contexto
        // 2. Validar depth
        // 3. Criar service
        // 4. Executar
    }
}
Mantenha a classe do plugin o mais limpa possível.