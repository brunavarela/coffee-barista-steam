using UnityEngine;

// Bomba de xarope (caramelo, baunilha, chocolate...). Usar despeja o sabor
// na xícara que estiver na estação de preparo.
public class FlavorPumpButton : MonoBehaviour, IUsable
{
    [SerializeField] private string flavor = "Caramel";

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

        DrinkBuilder.Instance.SetFlavor(flavor);
        UIManager.Instance.ShowPourFeedback($"Sabor adicionado: {flavor}");
    }
}
