using UnityEngine;

// Botão/alavanca da máquina de espresso: cada uso puxa mais um shot na
// xícara que estiver na mão (e marca a base como Espresso).
public class EspressoMachineButton : MonoBehaviour, IUsable
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

        DrinkBuilder.Instance.AddShot();
        UIManager.Instance.ShowPourFeedback("+1 shot de espresso");
    }
}
