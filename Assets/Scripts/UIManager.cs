using UnityEngine;
using UnityEngine.UI;

// Mostra pedido atual, estado de validação, feedback e score na tela.
public class UIManager : MonoBehaviour
{
    [Header("Pedido")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private Text orderNameText;

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

    public void ShowOrder(CoffeeOrder order)
    {
        feedbackPanel.SetActive(false);
        validatingPanel.SetActive(false);
        orderPanel.SetActive(true);
        orderNameText.text = order.Name;
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
        feedbackScoreText.text = $"+{result.score} pontos";
        feedbackIcon.sprite = result.correct ? correctIcon : incorrectIcon;

        UpdateScore(totalScore);
    }

    public void UpdateScore(int totalScore)
    {
        scoreText.text = $"Score: {totalScore}";
    }
}
