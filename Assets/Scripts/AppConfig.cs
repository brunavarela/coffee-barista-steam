using System;
using System.IO;
using UnityEngine;

// Singleton de configuração geral. A API key NUNCA deve ficar hardcoded no
// código-fonte (isso vaza no Git). Ordem de prioridade pra carregar a key:
// 1) variável de ambiente ANTHROPIC_API_KEY
// 2) arquivo local Assets/StreamingAssets/anthropic_api_key.txt (ignorado pelo git)
// 3) campo preenchido no Inspector (útil só em builds de teste)
public class AppConfig : MonoBehaviour
{
    public static AppConfig Instance { get; private set; }

    [Header("Claude API")]
    [Tooltip("Evite preencher aqui em builds versionadas. Prefira a variável de ambiente ou o arquivo local.")]
    [SerializeField] private string anthropicApiKeyInspector;

    [Header("Claude API - Configs Gerais")]
    [SerializeField] private string model = "claude-sonnet-5";
    [SerializeField] private int maxTokens = 300;

    private const string LocalKeyFileName = "anthropic_api_key.txt";

    public string AnthropicApiKey { get; private set; }
    public string Model => model;
    public int MaxTokens => maxTokens;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadApiKey();
    }

    private void LoadApiKey()
    {
        string envKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (!string.IsNullOrEmpty(envKey))
        {
            AnthropicApiKey = envKey;
            return;
        }

        string localFilePath = Path.Combine(Application.streamingAssetsPath, LocalKeyFileName);
        if (File.Exists(localFilePath))
        {
            AnthropicApiKey = File.ReadAllText(localFilePath).Trim();
            return;
        }

        AnthropicApiKey = anthropicApiKeyInspector;

        if (string.IsNullOrEmpty(AnthropicApiKey))
        {
            Debug.LogWarning(
                "[AppConfig] Nenhuma API key da Anthropic encontrada. Configure a variável de ambiente " +
                "ANTHROPIC_API_KEY ou crie o arquivo Assets/StreamingAssets/anthropic_api_key.txt (ambos fora do git).");
        }
    }
}
