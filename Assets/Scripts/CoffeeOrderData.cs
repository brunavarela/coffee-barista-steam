using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CoffeeOrder
{
    public int Id;
    public string Name;
    public int Difficulty; // 1 = simples, 2 = médio, 3 = complexo

    public CoffeeOrder(int id, string name, int difficulty)
    {
        Id = id;
        Name = name;
        Difficulty = difficulty;
    }
}

// Equivalente a um arquivo de constants em JavaScript:
// const ORDERS = [...]; export const getRandomOrder = () => ORDERS[Math.floor(Math.random() * ORDERS.length)];
public static class CoffeeOrderData
{
    public static readonly List<CoffeeOrder> AllOrders = new List<CoffeeOrder>
    {
        new CoffeeOrder(1, "Espresso Single Shot", 1),
        new CoffeeOrder(2, "Espresso Double Shot", 1),
        new CoffeeOrder(3, "Americano", 1),
        new CoffeeOrder(4, "Americano Large", 1),
        new CoffeeOrder(5, "Cappuccino Small", 2),
        new CoffeeOrder(6, "Cappuccino Medium", 2),
        new CoffeeOrder(7, "Cappuccino Large", 2),
        new CoffeeOrder(8, "Latte Whole Milk", 2),
        new CoffeeOrder(9, "Latte Oat Milk", 2),
        new CoffeeOrder(10, "Latte Almond Milk", 2),
        new CoffeeOrder(11, "Flat White", 2),
        new CoffeeOrder(12, "Flat White Large", 2),
        new CoffeeOrder(13, "Macchiato", 2),
        new CoffeeOrder(14, "Dark Chocolate Mocha", 3),
        new CoffeeOrder(15, "Dark Chocolate Mocha Whole", 3),
        new CoffeeOrder(16, "Caramel Mocha", 3),
        new CoffeeOrder(17, "Caramel Mocha Oat", 3),
        new CoffeeOrder(18, "Cortado", 2),
        new CoffeeOrder(19, "Cortado Small", 2),
        new CoffeeOrder(20, "Affogato", 3),
        new CoffeeOrder(21, "Affogato Vanilla", 3),
        new CoffeeOrder(22, "Affogato Chocolate", 3),
        new CoffeeOrder(23, "Chai Latte", 2),
        new CoffeeOrder(24, "Chai Latte Oat", 2),
        new CoffeeOrder(25, "London Fog Latte", 3),
        new CoffeeOrder(26, "Matcha Latte", 2),
        new CoffeeOrder(27, "Matcha Latte Almond", 2),
        new CoffeeOrder(28, "Iced Cappuccino", 2),
        new CoffeeOrder(29, "Iced Latte Vanilla", 3),
        new CoffeeOrder(30, "2x Espresso Double Shot (2 shots)", 3),
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
}
