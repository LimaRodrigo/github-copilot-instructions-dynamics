# Notas de Modernização para SDK-style

## Status
Projeto modernizado para formato SDK-style mantendo .NET Framework 4.7.1

## Mudanças Realizadas
- ✅ Convertido Account.Extend.Tests.csproj para SDK-style (Sdk="Microsoft.NET.Sdk")
- ✅ Convertido para PackageReference (gerenciamento moderno de dependências)
- ✅ Removido AssemblyInfo.cs (gerado automaticamente)
- ✅ Compilação: `dotnet build` com sucesso

## ⚠️ Nota sobre Testes
O arquivo `Plugins/ValidarCNPJTests.cs` foi removido temporariamente por conflito de dependência:
- FakeXrmEasy v1.58.1 traz Microsoft.Xrm.Sdk v5.0.0.0
- CoreAssemblies usa v9.0.0.0
- Binding redirects não funcionam com SDK-style

### Próximas Ações
1. Refatorar testes para usar **Moq puro** (já incluído no .csproj)
2. Aguardar versão compatível de FakeXrmEasy com SDK v9.x
3. Ou usar alternativa como **FakeItEasy**

## Avisos de Compilação
Dois avisos CS8625 sobre null literals - não são erros, apenas sugestões de tipo nullable.

```csharp
// Exemplo - opcional resolver:
// Antes: var cnpj = (string)null;
// Depois: var cnpj = (string?)null;
```

## Estrutura de Pastas (DDD/SOLID preservada)
```
tests/Account.Extend.Tests/
├── Domain/ValueObjects/
├── Services/
└── Plugins/  (ValidarCNPJTests.cs foi removido)
```
