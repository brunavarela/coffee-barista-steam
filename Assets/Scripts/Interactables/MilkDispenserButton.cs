using UnityEngine;

// Dispenser de leite fixo (integral, aveia, amêndoa). Cada uso aumenta a
// quantidade de leite na bebida (pitada -> meio a meio -> cheio).
public class MilkDispenserButton : MonoBehaviour, IUsable
{
    [SerializeField] private MilkType milkType = MilkType.Whole;

    public string DisplayName => gameObject.name;

    public void OnHoverEnter() { }
    public void OnHoverExit() { }

    public void Use()
    {
        if (!DrinkBuilder.Instance.HasCup)
        {
            UIManager.Instance.ShowPourFeedback("Pegue uma xicara primeiro.");
            return;
        }

        DrinkBuilder.Instance.AddMilk(milkType);
        string amount = CoffeeOrderData.MilkAmountName(DrinkBuilder.Instance.Current.MilkAmount);
        UIManager.Instance.ShowPourFeedback($"Leite ({gameObject.name}): {amount}");
    }
}
