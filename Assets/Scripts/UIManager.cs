using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Mostra pedido atual, receita de passos, nome do que está sendo mirado,
// avisos de ação (despejo), estado de validação, feedback e score na tela.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Pedido")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private Text orderNameText;
    [SerializeField] private Text recipeText;

    [Header("Mira")]
    [SerializeField] private Text hoverNameText;
    [SerializeField] private Text pourFeedbackText;

    [Header("Score")]
    [SerializeField] private Text scoreText;

    [Header("Validando")]
    [SerializeField] private GameObject validatingPanel;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Text feedbackScoreText;
    [SerializeField] private Image feedbackIcon;
    [SerializeField] private Sprite correctIcon;
    [SerializeField] private Sprite incorrectIcon;

    private Coroutine pourFeedbackRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowOrder(CoffeeOrder order)
    {
        feedbackPanel.SetActive(false);
        validatingPanel.SetActive(false);
        orderPanel.SetActive(true);
        orderNameText.text = order.Name;

        if (recipeText != null)
        {
            recipeText.text = string.Join("\n", RecipeFormatter.BuildSteps(order));
        }
    }

    public void ShowValidating()
    {
        orderPanel.SetActive(false);
        validatingPanel.SetActive(true);
    }

    public void ShowFeedback(ValidationResult result, int totalScore)
    {
        validatingPanel.SetActive(false);
        feedbackPanel.SetActive(true);

        feedbackText.text = result.feedback;
        feedbackScoreText.text = $"+{result.score} pontos (clique em espaco vazio pra continuar)";
        feedbackIcon.sprite = result.correct ? correctIcon : incorrectIcon;

        UpdateScore(totalScore);
    }

    public void UpdateScore(int totalScore)
    {
        scoreText.text = $"Score: {totalScore}";
    }

    // Nome do objeto que o jogador está mirando no centro da tela (ou vazio).
    public void ShowHoverName(string name)
    {
        if (hoverNameText == null)
        {
            return;
        }

        hoverNameText.text = string.IsNullOrEmpty(name) ? "" : name;
    }

    // Aviso temporário de ação (ex: "Leite adicionado"), some sozinho.
    public void ShowPourFeedback(string message)
    {
        if (pourFeedbackText == null)
        {
            return;
        }

        pourFeedbackText.text = message;
        pourFeedbackText.gameObject.SetActive(true);

        if (pourFeedbackRoutine != null)
        {
            StopCoroutine(pourFeedbackRoutine);
        }

        pourFeedbackRoutine = StartCoroutine(HidePourFeedbackAfterDelay());
    }

    private IEnumerator HidePourFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        pourFeedbackText.gameObject.SetActive(false);
    }
}
