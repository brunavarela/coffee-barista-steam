using UnityEngine;

// Balde de gelo: usar marca a bebida da estação de preparo como gelada.
public class IceBinButton : MonoBehaviour, IUsable
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

        DrinkBuilder.Instance.SetIced(true);
        UIManager.Instance.ShowPourFeedback("Gelo adicionado");
    }
}
