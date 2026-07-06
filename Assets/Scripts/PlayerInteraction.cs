using UnityEngine;

// Base comum pra qualquer objeto que reage ao olhar do jogador (highlight
// visual e nome mostrado na tela).
public interface IHighlightable
{
    string DisplayName { get; }
    void OnHoverEnter();
    void OnHoverExit();
}

// Objetos que o jogador pega e carrega na mão (xícara, jarra de leite).
public interface IInteractable : IHighlightable
{
    void OnGrab(Transform handAnchor);
    void OnRelease(IHighlightable releaseTarget);
}

// Objetos fixos que o jogador aciona sem segurar (máquina de espresso, bomba
// de xarope, balde de gelo).
public interface IUsable : IHighlightable
{
    void Use();
}

// Raycast a partir da câmera POV pra detectar, segurar/soltar ou acionar
// objetos. Clicar em espaço vazio (nada no centro da tela) tenta entregar o
// pedido ou avançar pro próximo, dependendo do estado do jogo.
public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera povCamera;
    [SerializeField] private Transform handAnchor;
    [SerializeField] private HandAnimationController handAnimationController;

    [Header("Configuração")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private IHighlightable currentHoverTarget;
    private IInteractable grabbedObject;

    private void Update()
    {
        UpdateHoverTarget();
        HandleInput();
    }

    private void UpdateHoverTarget()
    {
        Ray ray = povCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        IHighlightable target = null;
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            target = hit.collider.GetComponent<IHighlightable>();
        }

        if (target != currentHoverTarget)
        {
            currentHoverTarget?.OnHoverExit();
            currentHoverTarget = target;
            currentHoverTarget?.OnHoverEnter();
            UIManager.Instance?.ShowHoverName(currentHoverTarget?.DisplayName);
        }
    }

    private void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        // Segurando algo e mirando num equipamento fixo (máquina, leite,
        // xarope, gelo): aciona sem soltar o que está na mão.
        if (grabbedObject != null && currentHoverTarget is IUsable usableWhileHolding)
        {
            usableWhileHolding.Use();
            return;
        }

        if (grabbedObject != null)
        {
            grabbedObject.OnRelease(currentHoverTarget);
            grabbedObject = null;
            handAnimationController?.PlayIdle();
            return;
        }

        if (currentHoverTarget is IInteractable interactable)
        {
            grabbedObject = interactable;
            grabbedObject.OnGrab(handAnchor);
            handAnimationController?.PlayGrab();
        }
        else if (currentHoverTarget is IUsable usable)
        {
            usable.Use();
        }
        else if (currentHoverTarget == null)
        {
            if (DrinkBuilder.Instance == null)
            {
                Debug.LogError("[PlayerInteraction] DrinkBuilder.Instance esta null. Confira se o GameObject 'DrinkBuilder' existe na cena (rode Tools > Coffee Barista > Montar Cena de Teste fora do Play).");
                return;
            }

            DrinkBuilder.Instance.TryAdvanceAfterFeedback();
        }
    }
}
