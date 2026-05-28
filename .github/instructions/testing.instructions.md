---
applyTo: "tests/**/*.cs"
---
# Padrões de Testes (xUnit + FakeXrmEasy)

**Framework:** xUnit + FakeXrmEasy

**Princípios obrigatórios:**
- Um cenário por teste (método pequeno e focado)
- Asserts claros e objetivos
- Use **Builders** para criar entidades complexas (AccountBuilder, ContactBuilder, OpportunityBuilder, etc.)
- Prefira testes pequenos e legíveis
- Evite mocks excessivos
- Evite múltiplos cenários no mesmo teste
- Evite lógica complexa dentro do teste

Exemplo de nomenclatura:
`Given_Condicao_When_Acao_Then_ResultadoEsperado`