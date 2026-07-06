using UnityEngine;

// Torneira de água quente — usada pro Americano.
public class WaterTapButton : MonoBehaviour, IUsable
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

        DrinkBuilder.Instance.SetWaterAdded(true);
        UIManager.Instance.ShowPourFeedback("Agua quente adicionada");
    }
}
