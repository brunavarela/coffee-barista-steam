using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValidationResult
{
    public bool correct;
    public string feedback;
    public int score;
}

// O que o jogador efetivamente montou, capturado pelas interações (xícara,
// base, leite, água, sorvete, espuma, sabor, gelo...). Mesmo formato de
// campos do CoffeeOrder pedido, pra dar pra comparar direto.
[System.Serializable]
public class PreparedDrink
{
    public CupSize Size;
    public BaseIngredient Base;
    public int Shots;
    public MilkType Milk;
    public MilkAmount MilkAmount;
    public bool Foamed;
    public bool WaterAdded;
    public bool IceCreamAdded;
    public bool Iced;
    public string Flavor;
    public int Quantity = 1;
}

// Validação 100% local, sem API/IA: compara o pedido solicitado com o que foi
// entregue, campo a campo, e gera pontuação + feedback em português. Não
// existe campo "tipo de bebida" pra comparar — o tipo emerge da combinação
// de ingredientes, igual na vida real.
public static class OrderValidator
{
    private const int PenaltyPerMistake = 15;

    public static ValidationResult Validate(CoffeeOrder requested, PreparedDrink delivered)
    {
        List<string> mistakes = new List<string>();

        if (requested.Size != delivered.Size)
        {
            mistakes.Add($"Tamanho errado: pedido pedia {requested.Size}, você usou {delivered.Size}.");
        }

        if (requested.Base != delivered.Base)
        {
            mistakes.Add($"Base errada: pedido pedia {CoffeeOrderData.BaseName(requested.Base)}, você usou {CoffeeOrderData.BaseName(delivered.Base)}.");
        }

        if (requested.Shots != delivered.Shots)
        {
            mistakes.Add($"Quantidade de shots errada: pedido pedia {requested.Shots}, você fez {delivered.Shots}.");
        }

        if (requested.Milk != delivered.Milk)
        {
            mistakes.Add($"Leite errado: pedido pedia {requested.Milk}, você usou {delivered.Milk}.");
        }

        if (requested.MilkAmount != delivered.MilkAmount)
        {
            mistakes.Add($"Quantidade de leite errada: pedido pedia {CoffeeOrderData.MilkAmountName(requested.MilkAmount)}, você usou {CoffeeOrderData.MilkAmountName(delivered.MilkAmount)}.");
        }

        if (requested.Foamed != delivered.Foamed)
        {
            mistakes.Add(requested.Foamed
                ? "Faltou vaporizar/espumar o leite."
                : "Não devia vaporizar o leite.");
        }

        if (requested.WaterAdded != delivered.WaterAdded)
        {
            mistakes.Add(requested.WaterAdded
                ? "Faltou adicionar água quente."
                : "Não devia adicionar água.");
        }

        if (requested.IceCreamAdded != delivered.IceCreamAdded)
        {
            mistakes.Add(requested.IceCreamAdded
                ? "Faltou adicionar sorvete."
                : "Não devia adicionar sorvete.");
        }

        if (requested.Iced != delivered.Iced)
        {
            mistakes.Add(requested.Iced
                ? "Pedido era gelado e você entregou quente."
                : "Pedido era quente e você entregou gelado.");
        }

        if (!string.IsNullOrEmpty(requested.Flavor) && requested.Flavor != delivered.Flavor)
        {
            string deliveredFlavor = string.IsNullOrEmpty(delivered.Flavor) ? "nenhum" : delivered.Flavor;
            mistakes.Add($"Sabor errado: pedido pedia {requested.Flavor}, você usou {deliveredFlavor}.");
        }

        if (requested.Quantity != delivered.Quantity)
        {
            mistakes.Add($"Quantidade errada: pedido pedia {requested.Quantity}x, você entregou {delivered.Quantity}x.");
        }

        bool correct = mistakes.Count == 0;
        int score = correct ? 100 : Mathf.Max(0, 100 - mistakes.Count * PenaltyPerMistake);
        string feedback = correct
            ? "Perfeito! Pedido preparado corretamente."
            : string.Join(" ", mistakes);

        return new ValidationResult
        {
            correct = correct,
            feedback = feedback,
            score = score
        };
    }
}
