using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Monta uma cena de teste com objetos placeholder (cubos) pra jogar o loop
// completo antes de existir arte 3D de verdade. Roda quantas vezes quiser —
// reaproveita objetos já criados em vez de duplicar.
public static class SceneSetup
{
    [MenuItem("Tools/Coffee Barista/Montar Cena de Teste")]
    public static void BuildTestScene()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("[SceneSetup] Pare o Play antes de rodar esse menu. Nada foi alterado.");
            return;
        }

        RemoveAllMissingScripts();

        // Objetos de versões antigas da cena que não existem mais no código.
        RemoveStaleObject("Balcao de Entrega");
        RemoveStaleObject("Estacao de Preparo");
        RemoveStaleObject("Balcao");
        string[] staleTypeButtons =
        {
            "Tipo Espresso", "Tipo Americano", "Tipo Cappuccino", "Tipo Latte", "Tipo Flat White",
            "Tipo Macchiato", "Tipo Mocha", "Tipo Cortado", "Tipo Affogato", "Tipo Chai Latte",
            "Tipo London Fog Latte", "Tipo Matcha Latte"
        };
        foreach (string staleName in staleTypeButtons)
        {
            RemoveStaleObject(staleName);
        }

        SetupPlayer(out Camera cam, out Transform handAnchor, out PlayerInteraction playerInteraction);

        CreatePrimitiveWithCollider("Chao", new Vector3(0f, -0.05f, 0f), new Vector3(8f, 0.1f, 8f), new Color(0.5f, 0.5f, 0.5f));

        CreatePrimitiveWithCollider("Balcao Norte", new Vector3(0f, 0.45f, 3.2f), new Vector3(6.4f, 0.9f, 0.6f), new Color(0.55f, 0.35f, 0.2f));
        CreatePrimitiveWithCollider("Balcao Sul", new Vector3(0f, 0.45f, -3.2f), new Vector3(6.4f, 0.9f, 0.6f), new Color(0.55f, 0.35f, 0.2f));
        CreatePrimitiveWithCollider("Balcao Leste", new Vector3(3.2f, 0.45f, 0f), new Vector3(0.6f, 0.9f, 6.4f), new Color(0.55f, 0.35f, 0.2f));
        CreatePrimitiveWithCollider("Balcao Oeste", new Vector3(-3.2f, 0.45f, 0f), new Vector3(0.6f, 0.9f, 6.4f), new Color(0.55f, 0.35f, 0.2f));

        GameManager gameManager = GetOrCreateComponent<GameManager>("GameManager");
        DrinkBuilder drinkBuilder = GetOrCreateComponent<DrinkBuilder>("DrinkBuilder");

        UIManager uiManager = BuildUI();

        float y = 0.95f;

        // Balcao Norte: xícaras + máquina de espresso
        CreateCup("Xicara Pequena", CupSize.Small, new Vector3(-2.0f, y, 3f), new Color(0.9f, 0.9f, 0.9f));
        CreateCup("Xicara Media", CupSize.Medium, new Vector3(-1.4f, y, 3f), new Color(0.85f, 0.85f, 0.85f));
        CreateCup("Xicara Grande", CupSize.Large, new Vector3(-0.8f, y, 3f), new Color(0.8f, 0.8f, 0.8f));
        CreateUsable<EspressoMachineButton>("Maquina de Espresso", new Vector3(0.6f, y, 3f), new Color(0.3f, 0.2f, 0.1f));

        // Balcao Leste: leites
        CreateMilkDispenser("Leite Integral", MilkType.Whole, new Vector3(3f, y, -1.5f), Color.white);
        CreateMilkDispenser("Leite de Aveia", MilkType.Oat, new Vector3(3f, y, -0.3f), new Color(0.9f, 0.8f, 0.6f));
        CreateMilkDispenser("Leite de Amendoa", MilkType.Almond, new Vector3(3f, y, 0.9f), new Color(0.95f, 0.9f, 0.8f));

        // Balcao Sul: xaropes + gelo
        CreateFlavorPump("Xarope Caramelo", "Caramel", new Vector3(-1.8f, y, -3f), new Color(0.7f, 0.4f, 0.1f));
        CreateFlavorPump("Xarope Baunilha", "Vanilla", new Vector3(-0.6f, y, -3f), new Color(0.95f, 0.9f, 0.7f));
        CreateFlavorPump("Xarope Chocolate", "Chocolate", new Vector3(0.6f, y, -3f), new Color(0.35f, 0.2f, 0.1f));
        CreateFlavorPump("Xarope Chocolate Amargo", "Dark Chocolate", new Vector3(1.8f, y, -3f), new Color(0.2f, 0.1f, 0.05f));
        CreateUsable<IceBinButton>("Balde de Gelo", new Vector3(2.8f, y, -3f), new Color(0.6f, 0.85f, 1f));

        // Balcao Oeste: água (Americano), sorvete (Affogato), vaporizador
        // (diferencia Cappuccino/Latte/Cortado/Flat White) e bases alternativas
        // (Chai, Matcha, Earl Grey) que não passam pela máquina de espresso.
        CreateUsable<WaterTapButton>("Torneira de Agua", new Vector3(-3f, y, -2.6f), new Color(0.6f, 0.8f, 0.95f));
        CreateUsable<IceCreamButton>("Sorvete", new Vector3(-3f, y, -1.56f), new Color(0.95f, 0.95f, 0.9f));
        CreateUsable<SteamWandButton>("Vaporizador de Leite", new Vector3(-3f, y, -0.52f), new Color(0.7f, 0.7f, 0.75f));
        CreateBaseBrewButton(BaseIngredient.Chai, "Base Chai", new Vector3(-3f, y, 0.52f), new Color(0.8f, 0.5f, 0.3f));
        CreateBaseBrewButton(BaseIngredient.Matcha, "Base Matcha", new Vector3(-3f, y, 1.56f), new Color(0.4f, 0.6f, 0.3f));
        CreateBaseBrewButton(BaseIngredient.EarlGrey, "Base Earl Grey", new Vector3(-3f, y, 2.6f), new Color(0.7f, 0.6f, 0.5f));

        var gmSO = new SerializedObject(gameManager);
        gmSO.FindProperty("uiManager").objectReferenceValue = uiManager;
        gmSO.ApplyModifiedPropertiesWithoutUndo();

        var piSO = new SerializedObject(playerInteraction);
        piSO.FindProperty("povCamera").objectReferenceValue = cam;
        piSO.FindProperty("handAnchor").objectReferenceValue = handAnchor;
        piSO.FindProperty("interactionRange").floatValue = 2.5f;
        piSO.FindProperty("interactableLayer").intValue = 1; // layer "Default"
        piSO.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[SceneSetup] Cena de teste montada. Aperte Play: WASD anda, mouse olha, clique interage.");
    }

    private static void RemoveAllMissingScripts()
    {
        GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in all)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }
    }

    private static void RemoveStaleObject(string name)
    {
        GameObject stale = GameObject.Find(name);
        if (stale != null)
        {
            Undo.DestroyObjectImmediate(stale);
        }
    }

    private static void SetupPlayer(out Camera cam, out Transform handAnchor, out PlayerInteraction playerInteraction)
    {
        GameObject playerGo = GameObject.Find("Player");
        if (playerGo == null)
        {
            playerGo = new GameObject("Player", typeof(CharacterController), typeof(PlayerMovement));
            Undo.RegisterCreatedObjectUndo(playerGo, "Montar Cena de Teste");
        }

        playerGo.transform.position = new Vector3(0f, 0f, 0f);
        playerGo.transform.rotation = Quaternion.identity;

        CharacterController controller = playerGo.GetComponent<CharacterController>();
        controller.center = new Vector3(0f, 0.9f, 0f);
        controller.height = 1.8f;
        controller.radius = 0.35f;

        cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camGo.tag = "MainCamera";
            Undo.RegisterCreatedObjectUndo(camGo, "Montar Cena de Teste");
            cam = camGo.GetComponent<Camera>();
        }

        cam.transform.SetParent(playerGo.transform);
        cam.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        cam.transform.localRotation = Quaternion.identity;

        if (cam.GetComponent<SimpleMouseLook>() == null)
        {
            cam.gameObject.AddComponent<SimpleMouseLook>();
        }

        playerInteraction = cam.GetComponent<PlayerInteraction>();
        if (playerInteraction == null)
        {
            playerInteraction = cam.gameObject.AddComponent<PlayerInteraction>();
        }

        Transform existingAnchor = cam.transform.Find("HandAnchor");
        if (existingAnchor == null)
        {
            GameObject handGo = new GameObject("HandAnchor");
            handGo.transform.SetParent(cam.transform);
            handGo.transform.localPosition = new Vector3(0.3f, -0.3f, 0.6f);
            existingAnchor = handGo.transform;
        }

        handAnchor = existingAnchor;
    }

    private static T GetOrCreateComponent<T>(string name) where T : Component
    {
        T existing = Object.FindAnyObjectByType<T>();
        if (existing != null)
        {
            return existing;
        }

        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Montar Cena de Teste");
        return go.AddComponent<T>();
    }

    private static GameObject CreatePrimitiveWithCollider(string name, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(go, "Montar Cena de Teste");
        }

        go.name = name;
        go.transform.position = position;
        go.transform.localScale = scale;

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(rend.sharedMaterial) { color = color };
        }

        return go;
    }

    private static void CreateCup(string name, CupSize size, Vector3 position, Color color)
    {
        GameObject go = CreatePrimitiveWithCollider(name, position, new Vector3(0.25f, 0.3f, 0.25f), color);
        CupPickup cup = go.GetComponent<CupPickup>();
        if (cup == null)
        {
            cup = go.AddComponent<CupPickup>();
        }

        var so = new SerializedObject(cup);
        so.FindProperty("size").enumValueIndex = (int)size;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateMilkDispenser(string name, MilkType milk, Vector3 position, Color color)
    {
        GameObject go = CreatePrimitiveWithCollider(name, position, new Vector3(0.3f, 0.4f, 0.3f), color);
        MilkDispenserButton dispenser = go.GetComponent<MilkDispenserButton>();
        if (dispenser == null)
        {
            dispenser = go.AddComponent<MilkDispenserButton>();
        }

        var so = new SerializedObject(dispenser);
        so.FindProperty("milkType").enumValueIndex = (int)milk;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateFlavorPump(string name, string flavor, Vector3 position, Color color)
    {
        GameObject go = CreatePrimitiveWithCollider(name, position, new Vector3(0.2f, 0.4f, 0.2f), color);
        FlavorPumpButton pump = go.GetComponent<FlavorPumpButton>();
        if (pump == null)
        {
            pump = go.AddComponent<FlavorPumpButton>();
        }

        var so = new SerializedObject(pump);
        so.FindProperty("flavor").stringValue = flavor;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateUsable<T>(string name, Vector3 position, Color color) where T : Component
    {
        GameObject go = CreatePrimitiveWithCollider(name, position, new Vector3(0.4f, 0.5f, 0.4f), color);
        if (go.GetComponent<T>() == null)
        {
            go.AddComponent<T>();
        }
    }

    private static void CreateBaseBrewButton(BaseIngredient baseIngredient, string name, Vector3 position, Color color)
    {
        GameObject go = CreatePrimitiveWithCollider(name, position, new Vector3(0.4f, 0.5f, 0.4f), color);
        BaseBrewButton button = go.GetComponent<BaseBrewButton>();
        if (button == null)
        {
            button = go.AddComponent<BaseBrewButton>();
        }

        var so = new SerializedObject(button);
        so.FindProperty("baseIngredient").enumValueIndex = (int)baseIngredient;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static UIManager BuildUI()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        GameObject canvasGo;
        if (canvas == null)
        {
            canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGo, "Montar Cena de Teste");
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else
        {
            canvasGo = canvas.gameObject;
        }

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemGo = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(eventSystemGo, "Montar Cena de Teste");
        }

        UIManager uiManager = canvasGo.GetComponent<UIManager>();
        if (uiManager == null)
        {
            uiManager = canvasGo.AddComponent<UIManager>();
        }

        GameObject orderPanel = CreatePanel(canvasGo.transform, "OrderPanel", new Vector2(0, -40), new Vector2(500, 100));
        Text orderNameText = CreateText(orderPanel.transform, "OrderNameText", "Pedido: ...", 26);

        Text recipeText = CreateText(canvasGo.transform, "RecipeText", "", 20);
        RectTransform recipeRt = recipeText.GetComponent<RectTransform>();
        recipeRt.anchorMin = new Vector2(0, 1);
        recipeRt.anchorMax = new Vector2(0, 1);
        recipeRt.pivot = new Vector2(0, 1);
        recipeRt.sizeDelta = new Vector2(360, 280);
        recipeRt.anchoredPosition = new Vector2(20, -20);
        recipeText.alignment = TextAnchor.UpperLeft;

        GameObject validatingPanel = CreatePanel(canvasGo.transform, "ValidatingPanel", new Vector2(0, -40), new Vector2(500, 80));
        CreateText(validatingPanel.transform, "ValidatingText", "Validando...", 24);
        validatingPanel.SetActive(false);

        GameObject feedbackPanel = CreatePanel(canvasGo.transform, "FeedbackPanel", new Vector2(0, -160), new Vector2(600, 160));
        Text feedbackText = CreateText(feedbackPanel.transform, "FeedbackText", "", 20);
        Text feedbackScoreText = CreateText(feedbackPanel.transform, "FeedbackScoreText", "", 18);
        feedbackScoreText.rectTransform.anchoredPosition = new Vector2(0, -50);

        GameObject iconGo = new GameObject("FeedbackIcon", typeof(RectTransform), typeof(Image));
        iconGo.transform.SetParent(feedbackPanel.transform, false);
        Image feedbackIcon = iconGo.GetComponent<Image>();
        RectTransform iconRt = iconGo.GetComponent<RectTransform>();
        iconRt.sizeDelta = new Vector2(40, 40);
        iconRt.anchoredPosition = new Vector2(-270, 60);
        feedbackPanel.SetActive(false);

        GameObject crosshairGo = canvasGo.transform.Find("Crosshair")?.gameObject
            ?? new GameObject("Crosshair", typeof(RectTransform), typeof(Image));
        crosshairGo.transform.SetParent(canvasGo.transform, false);
        RectTransform crosshairRt = crosshairGo.GetComponent<RectTransform>();
        crosshairRt.anchorMin = new Vector2(0.5f, 0.5f);
        crosshairRt.anchorMax = new Vector2(0.5f, 0.5f);
        crosshairRt.pivot = new Vector2(0.5f, 0.5f);
        crosshairRt.sizeDelta = new Vector2(8, 8);
        crosshairRt.anchoredPosition = Vector2.zero;
        crosshairGo.GetComponent<Image>().color = Color.white;

        Text hoverNameText = CreateText(canvasGo.transform, "HoverNameText", "", 18);
        RectTransform hoverRt = hoverNameText.GetComponent<RectTransform>();
        hoverRt.anchorMin = new Vector2(0.5f, 0.5f);
        hoverRt.anchorMax = new Vector2(0.5f, 0.5f);
        hoverRt.pivot = new Vector2(0.5f, 1f);
        hoverRt.sizeDelta = new Vector2(300, 30);
        hoverRt.anchoredPosition = new Vector2(0, -20);

        Text pourFeedbackText = CreateText(canvasGo.transform, "PourFeedbackText", "", 22);
        RectTransform pourRt = pourFeedbackText.GetComponent<RectTransform>();
        pourRt.anchorMin = new Vector2(0.5f, 0f);
        pourRt.anchorMax = new Vector2(0.5f, 0f);
        pourRt.pivot = new Vector2(0.5f, 0f);
        pourRt.sizeDelta = new Vector2(500, 40);
        pourRt.anchoredPosition = new Vector2(0, 60);
        pourFeedbackText.gameObject.SetActive(false);

        Text scoreText = CreateText(canvasGo.transform, "ScoreText", "Score: 0", 22);
        RectTransform scoreRt = scoreText.GetComponent<RectTransform>();
        scoreRt.anchorMin = new Vector2(1, 1);
        scoreRt.anchorMax = new Vector2(1, 1);
        scoreRt.pivot = new Vector2(1, 1);
        scoreRt.sizeDelta = new Vector2(200, 40);
        scoreRt.anchoredPosition = new Vector2(-20, -20);

        var so = new SerializedObject(uiManager);
        so.FindProperty("orderPanel").objectReferenceValue = orderPanel;
        so.FindProperty("orderNameText").objectReferenceValue = orderNameText;
        so.FindProperty("recipeText").objectReferenceValue = recipeText;
        so.FindProperty("hoverNameText").objectReferenceValue = hoverNameText;
        so.FindProperty("pourFeedbackText").objectReferenceValue = pourFeedbackText;
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("validatingPanel").objectReferenceValue = validatingPanel;
        so.FindProperty("feedbackPanel").objectReferenceValue = feedbackPanel;
        so.FindProperty("feedbackText").objectReferenceValue = feedbackText;
        so.FindProperty("feedbackScoreText").objectReferenceValue = feedbackScoreText;
        so.FindProperty("feedbackIcon").objectReferenceValue = feedbackIcon;
        so.ApplyModifiedPropertiesWithoutUndo();

        return uiManager;
    }

    private static GameObject CreatePanel(Transform parent, string name, Vector2 anchoredPos, Vector2 size)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        return go;
    }

    private static Text CreateText(Transform parent, string name, string content, int fontSize)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Text text = go.GetComponent<Text>();
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.font = font;
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        return text;
    }
}
