using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DrinkType
{
    Espresso,
    Americano,
    Cappuccino,
    Latte,
    FlatWhite,
    Macchiato,
    Mocha,
    Cortado,
    Affogato,
    ChaiLatte,
    LondonFogLatte,
    MatchaLatte
}

public enum CupSize
{
    Small,
    Medium,
    Large
}

public enum MilkType
{
    None,
    Whole,
    Oat,
    Almond
}

// Quanto leite foi adicionado — junto com Foamed, é o que diferencia
// Macchiato (Splash) / Cortado (Half, sem espuma) / Flat White (Half, com
// espuma) / Latte (Full, sem espuma) / Cappuccino (Full, com espuma).
public enum MilkAmount
{
    None,
    Splash,
    Half,
    Full
}

// Base líquida da bebida: a maioria vem do espresso, mas Chai/Matcha/London
// Fog usam outra base (não passam pela máquina de espresso).
public enum BaseIngredient
{
    None,
    Espresso,
    Chai,
    Matcha,
    EarlGrey
}

// Representa tanto o pedido do catálogo (com Name/Id/Difficulty) quanto,
// por estrutura, o que o jogador entregou — os dois são comparados campo a
// campo pelo OrderValidator. O "tipo" da bebida não é escolhido diretamente
// pelo jogador: ele emerge da combinação desses ingredientes, igual na vida
// real (ex: espresso + bastante leite vaporizado = Cappuccino).
[System.Serializable]
public class CoffeeOrder
{
    public int Id;
    public string Name;
    public int Difficulty; // 1 = simples, 2 = médio, 3 = complexo
    public DrinkType Type; // só pra exibição/organização, não usado na validação

    public CupSize Size;
    public BaseIngredient Base;
    public int Shots;
    public MilkType Milk;
    public MilkAmount MilkAmount;
    public bool Foamed;
    public bool WaterAdded;
    public bool IceCreamAdded;
    public bool Iced;
    public string Flavor; // null/vazio quando não se aplica (ex: "Caramel", "Vanilla", "Dark Chocolate")
    public int Quantity;

    public CoffeeOrder(int id, string name, int difficulty, DrinkType type, CupSize size, BaseIngredient baseIngredient,
        int shots, MilkType milk = MilkType.None, MilkAmount milkAmount = MilkAmount.None, bool foamed = false,
        bool waterAdded = false, bool iceCreamAdded = false, bool iced = false, string flavor = null, int quantity = 1)
    {
        Id = id;
        Name = name;
        Difficulty = difficulty;
        Type = type;
        Size = size;
        Base = baseIngredient;
        Shots = shots;
        Milk = milk;
        MilkAmount = milkAmount;
        Foamed = foamed;
        WaterAdded = waterAdded;
        IceCreamAdded = iceCreamAdded;
        Iced = iced;
        Flavor = flavor;
        Quantity = quantity;
    }
}

// Equivalente a um arquivo de constants em JavaScript:
// const ORDERS = [...]; export const getRandomOrder = () => ORDERS[Math.floor(Math.random() * ORDERS.length)];
public static class CoffeeOrderData
{
    public static readonly List<CoffeeOrder> AllOrders = new List<CoffeeOrder>
    {
        new CoffeeOrder(1, "Espresso Single Shot", 1, DrinkType.Espresso, CupSize.Small, BaseIngredient.Espresso, shots: 1),
        new CoffeeOrder(2, "Espresso Double Shot", 1, DrinkType.Espresso, CupSize.Small, BaseIngredient.Espresso, shots: 2),
        new CoffeeOrder(3, "Americano", 1, DrinkType.Americano, CupSize.Medium, BaseIngredient.Espresso, shots: 2, waterAdded: true),
        new CoffeeOrder(4, "Americano Large", 1, DrinkType.Americano, CupSize.Large, BaseIngredient.Espresso, shots: 2, waterAdded: true),
        new CoffeeOrder(5, "Cappuccino Small", 2, DrinkType.Cappuccino, CupSize.Small, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(6, "Cappuccino Medium", 2, DrinkType.Cappuccino, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(7, "Cappuccino Large", 2, DrinkType.Cappuccino, CupSize.Large, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(8, "Latte Whole Milk", 2, DrinkType.Latte, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: false),
        new CoffeeOrder(9, "Latte Oat Milk", 2, DrinkType.Latte, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Oat, milkAmount: MilkAmount.Full, foamed: false),
        new CoffeeOrder(10, "Latte Almond Milk", 2, DrinkType.Latte, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Almond, milkAmount: MilkAmount.Full, foamed: false),
        new CoffeeOrder(11, "Flat White", 2, DrinkType.FlatWhite, CupSize.Medium, BaseIngredient.Espresso, shots: 2, milk: MilkType.Whole, milkAmount: MilkAmount.Half, foamed: true),
        new CoffeeOrder(12, "Flat White Large", 2, DrinkType.FlatWhite, CupSize.Large, BaseIngredient.Espresso, shots: 2, milk: MilkType.Whole, milkAmount: MilkAmount.Half, foamed: true),
        new CoffeeOrder(13, "Macchiato", 2, DrinkType.Macchiato, CupSize.Small, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Splash, foamed: false),
        new CoffeeOrder(14, "Dark Chocolate Mocha", 3, DrinkType.Mocha, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true, flavor: "Dark Chocolate"),
        new CoffeeOrder(15, "Dark Chocolate Mocha Whole", 3, DrinkType.Mocha, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true, flavor: "Dark Chocolate"),
        new CoffeeOrder(16, "Caramel Mocha", 3, DrinkType.Mocha, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true, flavor: "Caramel"),
        new CoffeeOrder(17, "Caramel Mocha Oat", 3, DrinkType.Mocha, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Oat, milkAmount: MilkAmount.Full, foamed: true, flavor: "Caramel"),
        new CoffeeOrder(18, "Cortado", 2, DrinkType.Cortado, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Half, foamed: false),
        new CoffeeOrder(19, "Cortado Small", 2, DrinkType.Cortado, CupSize.Small, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Half, foamed: false),
        new CoffeeOrder(20, "Affogato", 3, DrinkType.Affogato, CupSize.Small, BaseIngredient.Espresso, shots: 1, iceCreamAdded: true),
        new CoffeeOrder(21, "Affogato Vanilla", 3, DrinkType.Affogato, CupSize.Small, BaseIngredient.Espresso, shots: 1, iceCreamAdded: true, flavor: "Vanilla"),
        new CoffeeOrder(22, "Affogato Chocolate", 3, DrinkType.Affogato, CupSize.Small, BaseIngredient.Espresso, shots: 1, iceCreamAdded: true, flavor: "Chocolate"),
        new CoffeeOrder(23, "Chai Latte", 2, DrinkType.ChaiLatte, CupSize.Medium, BaseIngredient.Chai, shots: 0, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(24, "Chai Latte Oat", 2, DrinkType.ChaiLatte, CupSize.Medium, BaseIngredient.Chai, shots: 0, milk: MilkType.Oat, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(25, "London Fog Latte", 3, DrinkType.LondonFogLatte, CupSize.Medium, BaseIngredient.EarlGrey, shots: 0, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(26, "Matcha Latte", 2, DrinkType.MatchaLatte, CupSize.Medium, BaseIngredient.Matcha, shots: 0, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(27, "Matcha Latte Almond", 2, DrinkType.MatchaLatte, CupSize.Medium, BaseIngredient.Matcha, shots: 0, milk: MilkType.Almond, milkAmount: MilkAmount.Full, foamed: true),
        new CoffeeOrder(28, "Iced Cappuccino", 2, DrinkType.Cappuccino, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: true, iced: true),
        new CoffeeOrder(29, "Iced Latte Vanilla", 3, DrinkType.Latte, CupSize.Medium, BaseIngredient.Espresso, shots: 1, milk: MilkType.Whole, milkAmount: MilkAmount.Full, foamed: false, iced: true, flavor: "Vanilla"),
        new CoffeeOrder(30, "2x Espresso Double Shot (2 shots)", 3, DrinkType.Espresso, CupSize.Small, BaseIngredient.Espresso, shots: 2, quantity: 2),
    };

    public static CoffeeOrder GetRandomOrder()
    {
        int index = Random.Range(0, AllOrders.Count);
        return AllOrders[index];
    }

    // Usado pela progressão: conforme o jogador avança, libera pedidos mais difíceis.
    public static CoffeeOrder GetRandomOrder(int maxDifficulty)
    {
        List<CoffeeOrder> pool = AllOrders.Where(o => o.Difficulty <= maxDifficulty).ToList();
        if (pool.Count == 0)
        {
            pool = AllOrders;
        }

        int index = Random.Range(0, pool.Count);
        return pool[index];
    }

    public static string BaseName(BaseIngredient baseIngredient)
    {
        switch (baseIngredient)
        {
            case BaseIngredient.Espresso: return "Espresso";
            case BaseIngredient.Chai: return "Chai";
            case BaseIngredient.Matcha: return "Matcha";
            case BaseIngredient.EarlGrey: return "Earl Grey";
            default: return "Nenhuma";
        }
    }

    public static string MilkAmountName(MilkAmount amount)
    {
        switch (amount)
        {
            case MilkAmount.Splash: return "so uma pitada";
            case MilkAmount.Half: return "meio a meio";
            case MilkAmount.Full: return "bastante";
            default: return "nenhum";
        }
    }
}
