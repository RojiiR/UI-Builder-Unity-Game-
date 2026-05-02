using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorMatchPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;
    public PuzzleInteraction interaction;

    [Header("UI Elements")]
    public Image[] targetBoxes; // Kotak Atas
    public Image[] inputBoxes;  // Kotak Bawah
    public Button finishButton;

    private Color[] colors = { Color.red, Color.blue, Color.yellow };
    private int[] targetColorIndices = new int[10];
    private int[] inputColorIndices = new int[10];

    void OnEnable()
    {
        ResetPuzzle();
    }

    void Start()
    {
        SetupButtons();
        SetupExitButton();
    }

    void SetupButtons()
    {
        for (int i = 0; i < inputBoxes.Length; i++)
        {
            int index = i;
            Button btn = inputBoxes[i].GetComponent<Button>();
            btn.onClick.AddListener(() => CycleColor(index));
        }

        finishButton.onClick.AddListener(CheckMatch);
    }

    void ResetPuzzle()
    {
        IsCompleted = false;
        
        // Acak warna target (Atas)
        for (int i = 0; i < targetBoxes.Length; i++)
        {
            targetColorIndices[i] = Random.Range(0, colors.Length);
            targetBoxes[i].color = colors[targetColorIndices[i]];
        }

        // Reset warna input (Bawah) jadi abu-abu
        for (int i = 0; i < inputBoxes.Length; i++)
        {
            inputColorIndices[i] = -1; // Belum dipilih
            inputBoxes[i].color = Color.gray;
        }
    }

    void CycleColor(int index)
    {
        // Ganti warna saat diklik (Abu -> Merah -> Biru -> Kuning -> Merah...)
        inputColorIndices[index] = (inputColorIndices[index] + 1) % colors.Length;
        inputBoxes[index].color = colors[inputColorIndices[index]];
    }

    void CheckMatch()
    {
        bool allMatch = true;
        for (int i = 0; i < 10; i++)
        {
            if (inputColorIndices[i] != targetColorIndices[i])
            {
                allMatch = false;
                break;
            }
        }

        if (allMatch)
        {
            IsCompleted = true;
            if (interaction != null) interaction.OnPuzzleCompleted();
        }
        else
        {
            // Jika salah, acak ulang semuanya
            Debug.Log("Salah! Mengacak ulang...");
            ResetPuzzle();
        }
    }

    void SetupExitButton()
    {
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name == "CloseBtn")
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    if (interaction != null) interaction.ClosePuzzle();
                    else gameObject.SetActive(false);
                });
                break;
            }
        }
    }
}