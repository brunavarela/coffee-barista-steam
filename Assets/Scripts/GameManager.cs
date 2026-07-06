using UnityEngine;

public enum GameState
{
    PreparingOrder,
    ValidatingOrder,
    ShowingFeedback
}

// Controla o fluxo geral: pega pedido aleatório, aciona validação, guarda score.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referências")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private OrderValidator orderValidator;

    [Header("Progressão")]
    [Tooltip("A cada N pedidos concluídos, libera pedidos de maior dificuldade.")]
    [SerializeField] private int ordersPerDifficultyTier = 3;

    public GameState CurrentState { get; private set; }
    public CoffeeOrder CurrentOrder { get; private set; }
    public int OrdersCompleted { get; private set; }
    public int TotalScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RequestNewOrder();
    }

    public void RequestNewOrder()
    {
        int maxDifficulty = Mathf.Clamp(1 + OrdersCompleted / ordersPerDifficultyTier, 1, 3);
        CurrentOrder = CoffeeOrderData.GetRandomOrder(maxDifficulty);
        CurrentState = GameState.PreparingOrder;
        uiManager.ShowOrder(CurrentOrder);
    }

    // Chamado pelo PlayerInteraction quando o jogador finaliza a preparação
    // e entrega a bebida (deliveredDescription descreve o que foi montado).
    public void SubmitOrder(string deliveredDescription)
    {
        if (CurrentState != GameState.PreparingOrder)
        {
            return;
        }

        CurrentState = GameState.ValidatingOrder;
        uiManager.ShowValidating();

        orderValidator.Validate(CurrentOrder.Name, deliveredDescription, OnValidationComplete);
    }

    private void OnValidationComplete(ValidationResult result)
    {
        CurrentState = GameState.ShowingFeedback;
        TotalScore += result.score;

        if (result.correct)
        {
            OrdersCompleted++;
        }

        uiManager.ShowFeedback(result, TotalScore);
    }

    public void ProceedToNextOrder()
    {
        RequestNewOrder();
    }
}
