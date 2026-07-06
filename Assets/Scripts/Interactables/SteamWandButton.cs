using UnityEngine;

// Vaporizador de leite: é o que diferencia Cappuccino (com espuma) de Latte
// (sem espuma), por exemplo — mesma quantidade de leite, técnica diferente.
public class SteamWandButton : MonoBehaviour, IUsable
{
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

        DrinkBuilder.Instance.SetFoamed(true);
        UIManager.Instance.ShowPourFeedback("Leite vaporizado/espumado");
    }
}
