# Coffee Barista Game — Progresso

> Este arquivo existe pra qualquer sessão futura (sua ou de uma IA) saber onde paramos, sem depender do histórico do chat. Atualize conforme o projeto avança.

## O jogo

Jogo de cafeteria em primeira pessoa pra Steam. O jogador só vê as mãos (POV), recebe pedidos de café, monta a bebida caminhando entre balcões (WASD), e entrega. Feito em **Unity 6 (6000.5.2f1)**, C#.

## Decisões importantes já tomadas

- **Sem IA/API na validação.** A ideia original era usar a Claude API pra validar os pedidos, mas isso foi trocado por **validação 100% local em C#** (`OrderValidator.cs`) — porque um jogo vendido na Steam pagaria API por cada jogador/toda hora que alguém preparasse um café, e comparar ingredientes é simples o bastante pra não precisar de IA. Sem custo por jogador, sem depender de internet, mais rápido.
- **Tipo de bebida não é escolhido por botão.** Testamos um "botão de tipo" (Cappuccino/Latte/etc.) e descartamos — no mundo real o barista não escolhe o "tipo", ele emerge da combinação de ingredientes (leite, espuma, água, gelo...). Por isso `DrinkType` hoje é só decorativo; a validação compara ingredientes reais.
- **Movimentação livre (WASD), não fixa.** A cena tem 4 balcões formando um quadrado, e o jogador anda de verdade carregando a xícara de um lado pro outro (não é "estação de preparo fixa").
- **Mãos/braços: ainda sem solução visual boa.** Tentamos gerar via IA (Figma AI), depois via script Python no Blender (baixo-poli/geométrico) — nenhuma ficou com cara de mão de verdade. Decidimos tentar **decalcar uma foto de referência** no Figma (traçar o contorno por cima de uma foto real, não desenhar do zero nem gerar por IA/código). **Isso ainda não foi feito.**

## Estrutura do código (`Assets/Scripts/`)

| Script | Função |
|---|---|
| `CoffeeOrderData.cs` | Os 30 pedidos hardcoded, cada um com tamanho/base/shots/leite/quantidade de leite/espuma/água/sorvete/sabor/gelo |
| `DrinkBuilder.cs` | Acumula o que o jogador monta (singleton), sabe se tem xícara na mão |
| `OrderValidator.cs` | Compara pedido vs entregue, campo a campo, gera score/feedback — sem IA |
| `GameManager.cs` | Fluxo geral: pedido atual, score, estado do jogo |
| `UIManager.cs` | Painéis de pedido/receita/feedback/score na tela |
| `RecipeFormatter.cs` | Transforma o pedido numa lista de passos ("pegue xícara", "puxe shot"...) |
| `PlayerInteraction.cs` | Raycast central da câmera, pega/usa objetos, define `IInteractable`/`IUsable` |
| `PlayerMovement.cs` | WASD com CharacterController |
| `SimpleMouseLook.cs` | Olhar em 1ª pessoa (corpo gira no Y, câmera no X) |
| `PickupProp.cs` | Base pra objetos que voltam sozinhos ao lugar de origem |
| `Interactables/` | `CupPickup`, `EspressoMachineButton`, `MilkDispenserButton`, `FlavorPumpButton`, `IceBinButton`, `WaterTapButton`, `IceCreamButton`, `SteamWandButton`, `BaseBrewButton` |

`Assets/Editor/SceneSetup.cs` — script de editor que monta a cena de teste inteira do zero (menu **Tools > Coffee Barista > Montar Cena de Teste**). Roda quantas vezes quiser, é idempotente (não duplica). **Só funciona fora do modo Play.**

## Estado atual — o que funciona

- Cena de teste salva em `Assets/scenes/TestScene.unity`, com 4 balcões (Norte: xícaras+máquina; Leste: leites; Sul: xaropes+gelo; Oeste: água/sorvete/vaporizador/bases de chá-matcha-earl grey), tudo em cubos coloridos (placeholder).
- Loop completo testado e funcionando: pegar xícara → caminhar → usar equipamentos → soltar em espaço vazio pra entregar → feedback → clicar de novo pra próximo pedido.
- Git conectado ao GitHub (`brunavarela/coffee-barista-steam`), já com 2 commits.

## O que falta / próximos passos

1. **Arte das mãos/braços** — decalcar foto de referência no Figma (ver seção acima). Parado aqui.
2. Depois de ter o traçado: importar no Unity, substituir os cubos placeholder da mecânica de mãos, ligar a animação (girar dedos por código, já que não vai ter esqueleto/Animator tradicional).
3. Trocar os cubos placeholder dos itens (xícara, máquina, leites, etc.) por sprites/modelos de verdade.
4. Cenário (balcões, parede, chão) — hoje é só cubo cinza, precisa de arte.
5. Considerar assets prontos da Asset Store como atalho pro cenário/props (ver opções já pesquisadas na conversa: "Coffee shop - interior and props", "CoffeeShop Starter Pack").

## Como retomar de onde paramos

1. Abrir o projeto no Unity Hub (pasta `coffee-barista-steam`).
2. Abrir a cena `Assets/scenes/TestScene.unity`.
3. Se mudou algo na estrutura da cena via código: **Tools > Coffee Barista > Montar Cena de Teste** (fora do Play), depois Ctrl+S.
4. `git status` / `git log` pra ver o que já foi commitado.
