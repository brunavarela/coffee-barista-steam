using UnityEngine;

// Xícara de um tamanho fixo. Pegue pra começar uma bebida nova, carregue até
// os equipamentos (máquina, leite, xarope, gelo) e use-os sem soltá-la. Solte
// em espaço vazio pra entregar; solte em qualquer outro lugar e ela volta
// sozinha pro lugar de origem.
public class CupPickup : PickupProp, IInteractable
{
    [SerializeField] private CupSize size = CupSize.Medium;

    public string DisplayName => gameObject.name;

    public void OnHoverEnter() { }
    public void OnHoverExit() { }

    public void OnGrab(Transform handAnchor)
    {
        DrinkBuilder.Instance.StartNewCup(size);

        transform.SetParent(handAnchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnRelease(IHighlightable releaseTarget)
    {
        if (releaseTarget == null)
        {
            DrinkBuilder.Instance.Deliver(this);
        }
        else
        {
            ReturnToOriginalSpot();
        }
    }
}
