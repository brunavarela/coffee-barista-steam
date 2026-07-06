using System.Collections.Generic;

// Converte um CoffeeOrder numa lista de passos legíveis, mostrada na tela
// pra guiar o jogador do "pegar xícara" até a entrega. Não menciona o "tipo"
// da bebida — só os ingredientes/ações reais, igual um barista de verdade
// segue uma receita por passos, não por "apertar o botão Cappuccino".
public static class RecipeFormatter
{
    public static List<string> BuildSteps(CoffeeOrder order)
    {
        List<string> steps = new List<string>();
        int step = 1;

        steps.Add($"{step++}. Pegue uma xicara {SizeName(order.Size)}");

        if (order.Base == BaseIngredient.Espresso)
        {
            for (int i = 0; i < order.Shots; i++)
            {
                steps.Add($"{step}. Puxe um shot de espresso ({i + 1}/{order.Shots})");
            }

            if (order.Shots > 0)
            {
                step++;
            }
        }
        else if (order.Base != BaseIngredient.None)
        {
            steps.Add($"{step++}. Prepare a base: {CoffeeOrderData.BaseName(order.Base)}");
        }

        if (order.WaterAdded)
        {
            steps.Add($"{step++}. Adicione agua quente");
        }

        if (order.IceCreamAdded)
        {
            steps.Add($"{step++}. Adicione sorvete");
        }

        if (order.MilkAmount != MilkAmount.None)
        {
            steps.Add($"{step++}. Adicione leite {MilkName(order.Milk)} ({CoffeeOrderData.MilkAmountName(order.MilkAmount)})");
        }

        if (order.Foamed)
        {
            steps.Add($"{step++}. Vaporize/espume o leite");
        }

        if (!string.IsNullOrEmpty(order.Flavor))
        {
            steps.Add($"{step++}. Adicione o sabor {order.Flavor}");
        }

        if (order.Iced)
        {
            steps.Add($"{step++}. Adicione gelo");
        }

        if (order.Quantity > 1)
        {
            steps.Add($"{step++}. Repita tudo isso {order.Quantity}x");
        }

        steps.Add($"{step}. Entregue (solte a xicara em espaco vazio)");

        return steps;
    }

    private static string SizeName(CupSize size)
    {
        switch (size)
        {
            case CupSize.Small: return "Pequena";
            case CupSize.Medium: return "Media";
            case CupSize.Large: return "Grande";
            default: return "";
        }
    }

    private static string MilkName(MilkType milk)
    {
        switch (milk)
        {
            case MilkType.Whole: return "Integral";
            case MilkType.Oat: return "de Aveia";
            case MilkType.Almond: return "de Amendoa";
            default: return "";
        }
    }
}
