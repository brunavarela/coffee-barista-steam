using System;
using UnityEngine;

[Serializable]
public class ValidationResult
{
    public bool correct;
    public string feedback;
    public int score;
}

// Único ponto do jogo que fala com IA: recebe o pedido solicitado e o que o
// jogador entregou, e pede pra Claude julgar se está certo.
public class OrderValidator : MonoBehaviour
{
    public void Validate(string requestedOrder, string deliveredOrder, Action<ValidationResult> onComplete)
    {
        string prompt =
            $"Pedido solicitado: {requestedOrder}\n" +
            $"Pedido entregue: {deliveredOrder}\n" +
            "O barista preparou corretamente?\n" +
            "Responda em JSON VÁLIDO:\n" +
            "{\n" +
            "\"correct\": true ou false,\n" +
            "\"feedback\": \"explicação curta em português\",\n" +
            "\"score\": número de 0 a 100\n" +
            "}\n" +
            "Responda APENAS com JSON válido.";

        ApiClient.Instance.SendClaudeMessage(
            prompt,
            onSuccess: response => onComplete?.Invoke(ParseResponse(response)),
            onError: error =>
            {
                Debug.LogError($"[OrderValidator] Falha na validação: {error}");
                onComplete?.Invoke(new ValidationResult
                {
                    correct = false,
                    feedback = "Não foi possível validar o pedido agora. Tente novamente.",
                    score = 0
                });
            });
    }

    private ValidationResult ParseResponse(string rawText)
    {
        try
        {
            string cleanJson = ExtractJson(rawText);
            return JsonUtility.FromJson<ValidationResult>(cleanJson);
        }
        catch (Exception e)
        {
            Debug.LogError($"[OrderValidator] Erro ao parsear JSON: {e.Message}\nResposta: {rawText}");
            return new ValidationResult
            {
                correct = false,
                feedback = "Erro ao interpretar resposta da validação.",
                score = 0
            };
        }
    }

    // A Claude API às vezes envolve o JSON com texto extra apesar do prompt pedir
    // "apenas JSON" — isolamos o trecho entre a primeira '{' e a última '}'.
    private string ExtractJson(string text)
    {
        int start = text.IndexOf('{');
        int end = text.LastIndexOf('}');

        if (start >= 0 && end > start)
        {
            return text.Substring(start, end - start + 1);
        }

        return text;
    }
}
