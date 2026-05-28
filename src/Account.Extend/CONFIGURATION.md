# Guia de Configuração do Plugin ValidarCNPJ

## Pré-requisitos

- Dynamics 365 / Power Platform (Dataverse)
- Plugin Registration Tool
- Conta com permissões administrativas

## Passos para Registrar o Plugin

### 1. Compilar o Projeto

```bash
dotnet build src/Account.Extend/Account.Extend.csproj -c Release
```

### 2. Registrar o Assembly

Usando o **Plugin Registration Tool**:

1. Conecte-se à sua organização do Dynamics 365
2. Clique em **Register** → **Register New Assembly**
3. Navegue até `Account.Extend.dll` (na pasta `bin/Release`)
4. Selecione:
   - **Isolated** (recomendado para plugins em produção)
   - **Database** como local de armazenamento
5. Clique em **Register**

### 3. Registrar o Passo do Plugin

1. No Plugin Registration Tool, expanda **Plugins** e localize `Account.Extend.ValidarCNPJ`
2. Clique com botão direito e selecione **Register New Step**
3. Preencha os seguintes campos:

| Campo | Valor |
|-------|-------|
| **Message** | Create |
| **Primary Entity** | account |
| **Event Pipeline Stage of Execution** | Pre-operation |
| **Execution Mode** | Synchronous |
| **Run in User's Context** | Calling User |

4. Clique em **Register New Step**

### 4. Configurar Atributo Monitorado (Opcional)

1. Clique com botão direito no passo criado e selecione **Update**
2. No campo **Filtering Attributes**, adicione: `rcl_cnpj`
3. Salve

Isso garante que o plugin só execute quando o atributo `rcl_cnpj` for modificado.

## Testando o Plugin

### Teste Manual

1. No Dynamics 365, navegue para **Contas**
2. Crie uma nova conta
3. Preencha o campo **CNPJ** com um valor inválido, ex: `12345678901234`
4. Tente salvar
5. Deve aparecer a mensagem de erro: "Erro ao validar CNPJ da conta"

### Teste com CNPJ Válido

1. Crie uma nova conta
2. Preencha o campo **CNPJ** com um valor válido, ex: `11.222.333/0001-81`
3. Salve com sucesso

## Verificar Logs

Para visualizar os logs do plugin:

1. No Plugin Registration Tool, clique em **Tools** → **Plug-in Trace Log**
2. Filtre por:
   - **Type Name**: `Account.Extend.Plugins.ValidarCNPJ`
   - **User**: seu usuário
   - **Date**: últimas 24 horas

## Desabilitar o Plugin

Se necessário desabilitar o plugin temporariamente:

1. No Plugin Registration Tool, clique com botão direito no step
2. Selecione **Disable**

Para reabilitar:

1. Clique com botão direito no step desabilitado
2. Selecione **Enable**

## Desinstalar o Plugin

Para remover o plugin completamente:

1. No Plugin Registration Tool, clique com botão direito no step
2. Selecione **Delete**
3. Clique com botão direito no assembly
4. Selecione **Unregister**

## Troubleshooting

### Plugin não executa

- Verifique se o stage está configurado como **Pre-operation**
- Confirme se a mensagem é **Create** na entidade **account**
- Verifique os logs de trace para erros

### Erro: "Assembly não encontrado"

- Recompile o projeto
- Certifique-se de que o caminho do arquivo está correto
- Tente unregister e registrar novamente o assembly

### CNPJ válido é rejeitado

- Verifique se o dígito verificador está correto
- Tente formatar o CNPJ: `XX.XXX.XXX/XXXX-XX`

## Performance

- O plugin é **síncrono** na stage de **pré-operação**, portanto executa antes da validação do Dynamics
- Tempo típico de execução: < 100ms
- Não afeta significativamente a performance de criação de contas

## Segurança

- O CNPJ é validado **antes** de ser salvo no banco de dados
- O valor do CNPJ é mascarado nos logs (últimos 4 dígitos visíveis)
- O plugin usa apenas APIs nativas do Dynamics 365
