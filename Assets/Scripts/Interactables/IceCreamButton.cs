using UnityEngine;

// Sorvete — usado pro Affogato (shot de espresso despejado por cima).
public class IceCreamButton : MonoBehaviour, IUsable
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

        DrinkBuilder.Instance.SetIceCreamAdded(true);
        UIManager.Instance.ShowPourFeedback("Sorvete adicionado");
    }
}
