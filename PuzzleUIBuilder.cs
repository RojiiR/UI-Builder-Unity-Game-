using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class PuzzleUIBuilder : Editor
{
    [MenuItem("Tools/Puzzle/Build Full Canvas Puzzle")]
    static void Build()
    {
        GameObject existing = GameObject.Find("Canvas Puzzle");
        if (existing != null) { DestroyImmediate(existing); }

        // ── Canvas Setup ─────────────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas Puzzle");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // ── 1. WeirdPuzzle Panel ──────────────────────────────────────────
        GameObject weirdPanel = CreatePanel(canvasGO.transform, "WeirdPuzzle", 
            new Vector2(620, 440), new Color(0.08f, 0.08f, 0.12f, 0.96f));
        BuildWeirdPuzzleUI(weirdPanel);
        weirdPanel.SetActive(false);

        // ── 2. PasswordPuzzle Panel ───────────────────────────────────────
        GameObject passPanel = CreatePanel(canvasGO.transform, "PasswordPuzzle", 
            new Vector2(400, 550), new Color(0.1f, 0.1f, 0.1f, 0.98f));
        BuildPasswordPuzzleUI(passPanel);
        passPanel.SetActive(false);

        // ── 3. FlowPuzzle Panel (NEW!) ────────────────────────────────────
        GameObject flowPanel = CreatePanel(canvasGO.transform, "FlowPuzzle", 
            new Vector2(500, 500), new Color(0.15f, 0.15f, 0.15f, 0.98f));
        BuildFlowPuzzleUI(flowPanel);
        flowPanel.SetActive(false);

        // ── 4. BuildReaction Panel (NEW!) ──────────────────────────────────── 
        GameObject reactPanel = CreatePanel(canvasGO.transform, "ReactionPuzzle",
            new Vector2(500, 500), new Color(0.15f, 0.15f, 0.15f, 0.98f));
        BuildReactionPuzzleUI(reactPanel);
        reactPanel.SetActive(false);

        // ── 5. MemoryPuzzle Panel (NEW!) ──────────────────────────────────── 
        GameObject memoryPanel = CreatePanel(canvasGO.transform, "MemoryPuzzle",
            new Vector2(500, 500), new Color(0.15f, 0.15f, 0.15f, 0.98f));
        BuildMemoryPuzzleUI(memoryPanel);
        memoryPanel.SetActive(false);

        // ── 6. Color Match Panel (NEW!) ──────────────────────────────────── 
        GameObject colormatchPanel = CreatePanel(canvasGO.transform, "ColorMatchPuzzle",
            new Vector2(500, 500), new Color(0.15f, 0.15f, 0.15f, 0.98f));
        BuildColorMatchPuzzleUI(colormatchPanel);
        colormatchPanel.SetActive(false);

        // ── 7. MathPuzzle Panel (NEW!) ──────────────────────────────────── 
        GameObject mathpuzzlepanel = CreatePanel(canvasGO.transform, "MathPuzzlePanel",
            new Vector2(500, 500), new Color(0.15f, 0.15f, 0.15f, 0.98f));
        BuildMathPuzzleUI(mathpuzzlepanel);
        mathpuzzlepanel.SetActive(false);

        // ── 8. Interact Prompt ────────────────────────────────────────────
        GameObject promptGO = CreatePrompt(canvasGO.transform);
        promptGO.SetActive(false);

        Debug.Log("[PuzzleBuilder] ✅ Semua Panel (Weird, Password, Flow) Berhasil Dibuat!");
        Selection.activeGameObject = canvasGO;
    }

    static void BuildPasswordPuzzleUI(GameObject panel)
    {
        PasswordPuzzle script = panel.AddComponent<PasswordPuzzle>();
        CreateText(panel.transform, "Title", "SECURITY TERMINAL", new Vector2(0, 230), new Vector2(350, 40), 20, FontStyle.Bold, Color.green);

        GameObject dispGO = CreatePanel(panel.transform, "Display", new Vector2(300, 80), Color.black);
        dispGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 140);
        Text dispText = CreateText(dispGO.transform, "Value", "ENTER CODE", Vector2.zero, new Vector2(280, 70), 32, FontStyle.Bold, Color.green);
        script.displayBox = dispText;

        float startX = -100f;
        float startY = 40f;
        int num = 1;
        for (int y = 0; y < 3; y++) {
            for (int x = 0; x < 3; x++) {
                int currentNum = num;
                CreateButton(panel.transform, $"Btn_{num}", num.ToString(), 
                    new Vector2(startX + (x * 100), startY - (y * 90)), new Vector2(80, 80), new Color(0.2f, 0.2f, 0.2f));
                num++;
            }
        }

        CreateButton(panel.transform, "EnterBtn", "CHECK", new Vector2(0, -210), new Vector2(280, 60), new Color(0.1f, 0.4f, 0.1f));
        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(170, 240), new Vector2(40, 40), new Color(0.7f, 0.2f, 0.2f));
    }

    static void BuildWeirdPuzzleUI(GameObject panel) {
        WeirdPuzzle pScript = panel.AddComponent<WeirdPuzzle>();
        CreateText(panel.transform, "Title", "⚡ Weird Puzzle", new Vector2(0, 180), new Vector2(580, 50), 22, FontStyle.Bold, Color.white);
        
        GameObject lineContainerGO = new GameObject("LineContainer");
        lineContainerGO.transform.SetParent(panel.transform, false);
        RectTransform lineRT = lineContainerGO.AddComponent<RectTransform>();
        lineRT.anchorMin = Vector2.zero; lineRT.anchorMax = Vector2.one;
        lineRT.offsetMin = lineRT.offsetMax = Vector2.zero;
        pScript.lineContainer = lineRT;

        float[] yPos = { 70f, 0f, -70f };
        for (int i = 0; i < 3; i++) {
            CreateColorBox(panel.transform, $"Left_{i}", new Vector2(-210, yPos[i]));
            CreateColorBox(panel.transform, $"Right_{i}", new Vector2(210, yPos[i]));
        }

        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(280, 195), new Vector2(38, 38), Color.red);
    }

    static void BuildFlowPuzzleUI(GameObject panel)
    {
        FlowPuzzle script = panel.AddComponent<FlowPuzzle>();
        CreateText(panel.transform, "Title", "FLOW CONNECT", new Vector2(0, 220), new Vector2(400, 40), 20, FontStyle.Bold, Color.cyan);
        
        GameObject container = new GameObject("LineContainer");
        container.transform.SetParent(panel.transform, false);
        script.lineContainer = container.AddComponent<RectTransform>();
        script.lineContainer.sizeDelta = new Vector2(500, 500);

        Color[] pairColors = { Color.red, Color.blue, Color.yellow };
        Vector2[] positions = { 
            new Vector2(-150, 150), new Vector2(150, -150), 
            new Vector2(150, 150), new Vector2(-150, -150), 
            new Vector2(0, 180), new Vector2(0, -180) 
        };

        for (int i = 0; i < 6; i++)
        {
            GameObject go = new GameObject("Point_" + i);
            go.transform.SetParent(panel.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = positions[i];
            rt.sizeDelta = new Vector2(60, 60);

            var img = go.AddComponent<Image>();
            img.color = pairColors[i / 2];

            go.AddComponent<Button>();
            var p = go.AddComponent<FlowPoint>();
            p.color = img.color;
        }

        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(220, 220), new Vector2(40, 40), Color.red);
    }

    static void BuildReactionPuzzleUI(GameObject panel)
    {
        ReactionPuzzle script = panel.AddComponent<ReactionPuzzle>();
        
        // Spawn Area (Kotak transparan tengah)
        GameObject areaGO = CreatePanel(panel.transform, "SpawnArea", new Vector2(500, 400), new Color(0,0,0,0.3f));
        script.spawnArea = areaGO.GetComponent<RectTransform>();

        // Progress Bar Background
        GameObject barBg = CreatePanel(panel.transform, "ProgressBarBg", new Vector2(400, 30), Color.gray);
        barBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -220);

        // Progress Bar Fill
        GameObject barFill = CreatePanel(barBg.transform, "Fill", new Vector2(400, 30), Color.green);
        Image fillImg = barFill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        script.progressBar = fillImg;

        // Progress Text
        script.progressText = CreateText(panel.transform, "ProgressText", "PROGRESS: 0%", 
            new Vector2(0, -180), new Vector2(200, 30), 18, FontStyle.Bold, Color.white);

        // Close Button
        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(220, 220), new Vector2(40, 40), Color.red);
    }

    static void BuildMemoryPuzzleUI(GameObject panel)
    {
        MemoryPuzzle script = panel.AddComponent<MemoryPuzzle>();
        
        script.topBoxes = new Image[6];
        script.bottomButtons = new Button[6];

        // Layouting
        float startX = -190f;
        float spacing = 75f;

        // Baris Atas (Indikator)
        for (int i = 0; i < 6; i++)
        {
            GameObject box = CreatePanel(panel.transform, $"TopBox_{i}", new Vector2(60, 60), Color.gray);
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + (i * spacing), 80);
            script.topBoxes[i] = box.GetComponent<Image>();
        }

        // Baris Bawah (Input)
        for (int i = 0; i < 6; i++)
        {
            Button btn = CreateButton(panel.transform, $"BottomBtn_{i}", "", 
                new Vector2(startX + (i * spacing), -40), new Vector2(60, 60), Color.gray);
            script.bottomButtons[i] = btn;
        }

        // Info Text
        script.infoText = CreateText(panel.transform, "InfoText", "PRESS START", 
            new Vector2(0, 160), new Vector2(400, 40), 20, FontStyle.Bold, Color.white);

        // Start Button
        script.startButton = CreateButton(panel.transform, "StartBtn", "START", 
            new Vector2(0, -140), new Vector2(150, 50), Color.blue);

        // Close Button
        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(230, 230), new Vector2(40, 40), Color.red);
    }

    static void BuildColorMatchPuzzleUI(GameObject panel)
    {
        ColorMatchPuzzle script = panel.AddComponent<ColorMatchPuzzle>();
        script.targetBoxes = new Image[10];
        script.inputBoxes = new Image[10];

        float startX = -225f; // Mulai dari agak kiri
        float spacing = 50f;  // Jarak antar kotak

        // Label Atas
        CreateText(panel.transform, "TargetLabel", "TARGET PATTERN", new Vector2(0, 120), new Vector2(200, 30), 16, FontStyle.Bold, Color.white);

        // Baris Atas (Target)
        for (int i = 0; i < 10; i++)
        {
            GameObject box = CreatePanel(panel.transform, $"Target_{i}", new Vector2(40, 40), Color.gray);
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + (i * spacing), 70);
            script.targetBoxes[i] = box.GetComponent<Image>();
        }

        // Baris Bawah (Input)
        for (int i = 0; i < 10; i++)
        {
            Button btn = CreateButton(panel.transform, $"InputBtn_{i}", "", 
                new Vector2(startX + (i * spacing), -20), new Vector2(40, 40), Color.gray);
            script.inputBoxes[i] = btn.GetComponent<Image>();
        }

        // Finish Button
        script.finishButton = CreateButton(panel.transform, "FinishBtn", "FINISH", 
            new Vector2(0, -120), new Vector2(180, 50), new Color(0.1f, 0.6f, 0.1f));

        // Close Button
        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(240, 240), new Vector2(40, 40), Color.red);
    }

    static void BuildMathPuzzleUI(GameObject panel)
    {
        MathPuzzle script = panel.AddComponent<MathPuzzle>();
        script.progressBoxes = new Image[3];
        script.numButtons = new Button[10];

        // 1. Progress Boxes (Atas)
        for (int i = 0; i < 3; i++)
        {
            GameObject box = CreatePanel(panel.transform, $"Prog_{i}", new Vector2(50, 20), Color.gray);
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70 + (i * 70), 180);
            script.progressBoxes[i] = box.GetComponent<Image>();
        }

        // 2. Question Area
        GameObject qBox = CreatePanel(panel.transform, "QBox", new Vector2(200, 60), new Color(0, 0, 0, 0.5f));
        qBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(-110, 100);
        script.questionText = CreateText(qBox.transform, "Txt", "00 + 00 = ?", Vector2.zero, new Vector2(180, 50), 24, FontStyle.Bold, Color.white);

        // 3. Answer Area
        GameObject aBox = CreatePanel(panel.transform, "ABox", new Vector2(120, 60), Color.black);
        aBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(60, 100);
        script.answerText = CreateText(aBox.transform, "Txt", "", Vector2.zero, new Vector2(100, 50), 30, FontStyle.Bold, Color.green);

        // 4. Number Pad (0-9)
        float startX = -100f;
        float startY = 20f;
        for (int i = 0; i < 10; i++)
        {
            // Susun 5 kotak per baris
            float xPos = startX + (i % 5 * 50);
            float yPos = startY - (i / 5 * 55);
            string label = (i == 9) ? "0" : (i + 1).ToString(); // Susunan 1-9 lalu 0
            
            // Kita atur sedikit logikanya agar index i sesuai dengan angka tombol
            int currentNum = (i == 9) ? 0 : i + 1;
            
            Button btn = CreateButton(panel.transform, $"Num_{currentNum}", label, new Vector2(xPos, yPos), new Vector2(45, 50), Color.gray);
            script.numButtons[currentNum] = btn;
        }

        // 5. OK & Clear Button
        script.okButton = CreateButton(panel.transform, "OkBtn", "OK", new Vector2(0, -110), new Vector2(120, 45), Color.green);
        
        Button clr = CreateButton(panel.transform, "ClearBtn", "C", new Vector2(100, -110), new Vector2(50, 45), Color.red);
        script.clearButton = clr;

        // 6. Close Button
        CreateButton(panel.transform, "CloseBtn", "✕", new Vector2(240, 240), new Vector2(40, 40), Color.red);
    }

    // --- REUSABLE HELPERS ---
    static GameObject CreatePanel(Transform parent, string name, Vector2 size, Color bg) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = size;
        go.AddComponent<Image>().color = bg;
        return go;
    }

    static GameObject CreatePrompt(Transform parent) {
        GameObject promptGO = new GameObject("InteractPrompt");
        promptGO.transform.SetParent(parent, false);
        RectTransform rt = promptGO.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.1f);
        rt.sizeDelta = new Vector2(300, 48);
        promptGO.AddComponent<Image>().color = new Color(0, 0, 0, 0.72f);
        CreateText(promptGO.transform, "PromptText", "[E] Interaksi", Vector2.zero, new Vector2(290, 44), 14, FontStyle.Bold, Color.white);
        return promptGO;
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color color) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.color = color;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        Text txt = CreateText(go.transform, "Text", label, Vector2.zero, size, 18, FontStyle.Bold, Color.white);
        txt.raycastTarget = false; 
        return btn;
    }

    static Text CreateText(Transform parent, string name, string content, Vector2 pos, Vector2 size, int fontSize, FontStyle style, Color color) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var txt = go.AddComponent<Text>();
        txt.text = content;
        txt.fontSize = fontSize;
        txt.fontStyle = style;
        txt.color = color;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return txt;
    }

    static void CreateColorBox(Transform parent, string name, Vector2 pos) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(75, 52);
        rt.anchoredPosition = pos;
        go.AddComponent<Image>().color = Color.gray;
    }
}
#endif