using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MemoryPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;
    public PuzzleInteraction interaction;

    [Header("UI Elements")]
    public Image[] topBoxes;
    public Button[] bottomButtons;
    public Button startButton;
    public Text infoText;

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool isDisplaying = false;
    private bool canInput = false;

    // PENTING: Reset setiap kali panel diaktifkan (OnEnable)
    void OnEnable()
    {
        StopAllCoroutines(); // Hentikan semua proses lama
        ResetPuzzleState();
    }

    void Start()
    {
        SetupButtons();
        SetupExitButton();
    }

    void ResetPuzzleState()
    {
        isDisplaying = false;
        canInput = false;
        IsCompleted = false;
        sequence.Clear();
        playerInput.Clear();
        if(infoText != null) infoText.text = "PRESS START";
        
        // Kembalikan semua warna ke abu-abu
        foreach (var img in topBoxes) if(img != null) img.color = Color.gray;
        foreach (var btn in bottomButtons) if(btn != null) btn.GetComponent<Image>().color = Color.gray;
    }

    void SetupButtons()
    {
        for (int i = 0; i < bottomButtons.Length; i++)
        {
            int index = i;
            bottomButtons[i].onClick.AddListener(() => OnButtonClick(index));
        }
        startButton.onClick.AddListener(StartSequence);
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

    void StartSequence()
    {
        if (isDisplaying) return;
        StartCoroutine(DisplaySequenceRoutine());
    }

    IEnumerator DisplaySequenceRoutine()
    {
        isDisplaying = true;
        canInput = false;
        playerInput.Clear();
        sequence.Clear();
        infoText.text = "WATCHING...";

        // Reset warna kotak bawah ke abu-abu saat mulai sequence baru
        foreach (var btn in bottomButtons) btn.GetComponent<Image>().color = Color.gray;

        for (int i = 0; i < 6; i++)
        {
            sequence.Add(Random.Range(0, 6));
        }

        yield return new WaitForSeconds(0.5f);

        foreach (int idx in sequence)
        {
            topBoxes[idx].color = Color.yellow;
            yield return new WaitForSeconds(0.6f);
            topBoxes[idx].color = Color.gray;
            yield return new WaitForSeconds(0.3f);
        }

        infoText.text = "YOUR TURN!";
        isDisplaying = false;
        canInput = true;
    }

    void OnButtonClick(int index)
    {
        if (!canInput || IsCompleted) return;

        // Visual: Kotak bawah jadi kuning saat diklik
        StartCoroutine(FlashButtonClick(index));

        playerInput.Add(index);
        
        if (playerInput[playerInput.Count - 1] != sequence[playerInput.Count - 1])
        {
            infoText.text = "WRONG! TRY AGAIN";
            StartCoroutine(FlashFeedback(Color.red));
            canInput = false; // Kunci input sampai di-reset lewat Start lagi
            return;
        }

        if (playerInput.Count == sequence.Count)
        {
            infoText.text = "SUCCESS!";
            StartCoroutine(FlashFeedback(Color.green));
            IsCompleted = true;
            Invoke("FinishPuzzle", 1.0f);
        }
    }

    // Coroutine untuk feedback klik tombol bawah
    IEnumerator FlashButtonClick(int index)
    {
        bottomButtons[index].GetComponent<Image>().color = Color.yellow;
        yield return new WaitForSeconds(0.2f);
        // Tetap kuning atau balik abu-abu? 
        // Kalau mau tetap kuning sampai sequence selesai, hapus baris di bawah:
        // bottomButtons[index].GetComponent<Image>().color = Color.gray; 
    }

    IEnumerator FlashFeedback(Color c)
    {
        foreach (var btn in bottomButtons) btn.GetComponent<Image>().color = c;
        yield return new WaitForSeconds(0.5f);
        if (c == Color.red) 
        {
            foreach (var btn in bottomButtons) btn.GetComponent<Image>().color = Color.gray;
        }
    }

    void FinishPuzzle()
    {
        if (interaction != null) interaction.OnPuzzleCompleted();
    }
}