using UnityEngine;

// Estação de preparo de base alternativa (Chai, Matcha, Earl Grey) — usada
// nas bebidas que não passam pela máquina de espresso.
public class BaseBrewButton : MonoBehaviour, IUsable
{
    [SerializeField] private BaseIngredient baseIngredient = BaseIngredient.Chai;

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

        DrinkBuilder.Instance.SetBase(baseIngredient);
        UIManager.Instance.ShowPourFeedback($"Base preparada: {CoffeeOrderData.BaseName(baseIngredient)}");
    }
}
