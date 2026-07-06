using UnityEngine;

// Qualquer objeto que o jogador possa apontar/segurar (xícara, jarra de leite,
// colher etc.) implementa essa interface.
public interface IInteractable
{
    void OnHoverEnter();
    void OnHoverExit();
    void OnGrab(Transform handAnchor);
    void OnRelease();
}

// Raycast a partir da câmera POV pra detectar e segurar/soltar objetos.
public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera povCamera;
    [SerializeField] private Transform handAnchor;
    [SerializeField] private HandAnimationController handAnimationController;

    [Header("Configuração")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private IInteractable currentHoverTarget;
    private IInteractable grabbedObject;

    private void Update()
    {
        UpdateHoverTarget();
        HandleInput();
    }

    private void UpdateHoverTarget()
    {
        Ray ray = povCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            IInteractable target = hit.collider.GetComponent<IInteractable>();

            if (target != currentHoverTarget)
            {
                currentHoverTarget?.OnHoverExit();
                currentHoverTarget = target;
                currentHoverTarget?.OnHoverEnter();
            }
        }
        else if (currentHoverTarget != null)
        {
            currentHoverTarget.OnHoverExit();
            currentHoverTarget = null;
        }
    }

    private void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (grabbedObject == null && currentHoverTarget != null)
        {
            grabbedObject = currentHoverTarget;
            grabbedObject.OnGrab(handAnchor);
            handAnimationController?.PlayGrab();
        }
        else if (grabbedObject != null)
        {
            grabbedObject.OnRelease();
            grabbedObject = null;
            handAnimationController?.PlayIdle();
        }
    }
}
