using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Utilitário HTTP pra falar com a Claude API. Só é usado pelo OrderValidator.
// Equivalente a um pequeno "fetch wrapper" que você faria em JS/Node.
public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    private const string ApiUrl = "https://api.anthropic.com/v1/messages";
    private const string AnthropicVersion = "2023-06-01";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SendClaudeMessage(string userPrompt, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendClaudeMessageCoroutine(userPrompt, onSuccess, onError));
    }

    private IEnumerator SendClaudeMessageCoroutine(string userPrompt, Action<string> onSuccess, Action<string> onError)
    {
        string apiKey = AppConfig.Instance != null ? AppConfig.Instance.AnthropicApiKey : null;

        if (string.IsNullOrEmpty(apiKey))
        {
            onError?.Invoke("API key da Anthropic não configurada.");
            yield break;
        }

        ClaudeRequestBody body = new ClaudeRequestBody
        {
            model = AppConfig.Instance.Model,
            max_tokens = AppConfig.Instance.MaxTokens,
            messages = new[]
            {
                new ClaudeMessage { role = "user", content = userPrompt }
            }
        };

        string jsonBody = JsonUtility.ToJson(body);

        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", apiKey);
            request.SetRequestHeader("anthropic-version", AnthropicVersion);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string body_ = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
                onError?.Invoke($"HTTP {request.responseCode} - {request.error}\n{body_}");
                yield break;
            }

            string textContent = ExtractTextFromResponse(request.downloadHandler.text);
            onSuccess?.Invoke(textContent);
        }
    }

    private string ExtractTextFromResponse(string rawResponseJson)
    {
        try
        {
            ClaudeResponseBody response = JsonUtility.FromJson<ClaudeResponseBody>(rawResponseJson);
            if (response?.content != null && response.content.Length > 0)
            {
                return response.content[0].text;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[ApiClient] Erro ao interpretar resposta da API: {e.Message}\n{rawResponseJson}");
        }

        return rawResponseJson;
    }

    [Serializable]
    private class ClaudeMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class ClaudeRequestBody
    {
        public string model;
        public int max_tokens;
        public ClaudeMessage[] messages;
    }

    [Serializable]
    private class ClaudeContentBlock
    {
        public string type;
        public string text;
    }

    [Serializable]
    private class ClaudeResponseBody
    {
        public ClaudeContentBlock[] content;
    }
}
