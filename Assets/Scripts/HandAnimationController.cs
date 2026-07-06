using UnityEngine;

// Encapsula os triggers do Animator das mãos. Chamado pelo PlayerInteraction
// e por outros scripts de interação (máquina de espresso, jarra de leite etc.).
public class HandAnimationController : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;

    private static readonly int GrabHash = Animator.StringToHash("Grab");
    private static readonly int PourHash = Animator.StringToHash("Pour");
    private static readonly int StirHash = Animator.StringToHash("Stir");
    private static readonly int IdleHash = Animator.StringToHash("Idle");

    public void PlayGrab()
    {
        handAnimator.SetTrigger(GrabHash);
    }

    public void PlayPour()
    {
        handAnimator.SetTrigger(PourHash);
    }

    public void PlayStir()
    {
        handAnimator.SetTrigger(StirHash);
    }

    public void PlayIdle()
    {
        handAnimator.SetTrigger(IdleHash);
    }
}
