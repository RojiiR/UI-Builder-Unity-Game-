using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ReactionPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;
    public PuzzleInteraction interaction;

    [Header("UI References")]
    public RectTransform spawnArea;
    public Image progressBar;
    public Text progressText;

    [Header("Settings")]
    public int totalTargets = 10;
    private int targetsClicked = 0;
    private float spawnInterval = 0.8f;
    private Color[] colors = { Color.red, Color.blue, Color.yellow };

    void Start()
    {
        SetupExitButton();
    }

    void SetupExitButton()
    {
        // Mencari tombol dengan nama CloseBtn di dalam panel ini
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name == "CloseBtn")
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    if (interaction != null) 
                    {
                        StopAllCoroutines(); // Hentikan spawn saat tutup
                        interaction.ClosePuzzle();
                    }
                    else 
                    {
                        gameObject.SetActive(false);
                    }
                });
                break;
            }
        }
    }

    void OnEnable()
    {
        ResetPuzzle();
        StartCoroutine(SpawnRoutine());
    }

    void ResetPuzzle()
    {
        IsCompleted = false;
        targetsClicked = 0;
        UpdateUI();
        // Bersihkan sisa kotak jika ada
        foreach (Transform child in spawnArea) {
            if (child.name.Contains("Target")) Destroy(child.gameObject);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (targetsClicked < totalTargets)
        {
            SpawnTarget();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnTarget()
    {
        GameObject go = new GameObject("Target");
        go.transform.SetParent(spawnArea, false);
        
        // Random Posisi
        RectTransform rt = go.AddComponent<RectTransform>();
        float x = Random.Range(spawnArea.rect.xMin + 50, spawnArea.rect.xMax - 50);
        float y = Random.Range(spawnArea.rect.yMin + 50, spawnArea.rect.yMax - 50);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(80, 80);

        // Visual & Warna
        Image img = go.AddComponent<Image>();
        img.color = colors[Random.Range(0, colors.Length)];
        
        // Button Logic
        Button btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => OnTargetClicked(go));

        // Animasi Membesar
        StartCoroutine(AnimateScale(rt));
    }

    IEnumerator AnimateScale(RectTransform rt)
    {
        rt.localScale = Vector3.zero;
        float t = 0;
        while (t < 1f && rt != null)
        {
            t += Time.deltaTime * 3f; // Kecepatan membesar
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
    }

    void OnTargetClicked(GameObject target)
    {
        Destroy(target);
        targetsClicked++;
        UpdateUI();

        if (targetsClicked >= totalTargets)
        {
            FinishPuzzle();
        }
    }

    void UpdateUI()
    {
        float progress = (float)targetsClicked / totalTargets;
        if (progressBar != null) progressBar.fillAmount = progress;
        if (progressText != null) progressText.text = $"PROGRESS: {(progress * 100):0}%";
    }

    void FinishPuzzle()
    {
        IsCompleted = true;
        Debug.Log("Reaction Puzzle Selesai!");
        if (interaction != null) interaction.OnPuzzleCompleted();
    }
}