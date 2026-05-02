using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MathPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;
    public PuzzleInteraction interaction;

    [Header("UI Elements")]
    public Image[] progressBoxes; // 3 Kotak indikator atas
    public Text questionText;     // Teks soal (10 + 20)
    public Text answerText;       // Teks jawaban input
    public Button[] numButtons;   // Tombol 0-9
    public Button okButton;
    public Button clearButton;    // Tombol hapus (opsional tapi penting)

    private int currentRound = 0;
    private int correctAnswer;
    private string currentInput = "";

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
        // Tombol Angka
        for (int i = 0; i < numButtons.Length; i++)
        {
            int val = i;
            numButtons[i].onClick.AddListener(() => AddNumber(val));
        }

        okButton.onClick.AddListener(CheckAnswer);
        
        if (clearButton != null)
            clearButton.onClick.AddListener(() => { currentInput = ""; answerText.text = ""; });
    }

    void ResetPuzzle()
    {
        currentRound = 0;
        currentInput = "";
        answerText.text = "";
        IsCompleted = false;

        foreach (var box in progressBoxes) box.color = Color.gray;
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        int a = Random.Range(10, 50);
        int b = Random.Range(10, 50);
        correctAnswer = a + b;
        questionText.text = $"{a} + {b} = ?";
        currentInput = "";
        answerText.text = "";
    }

    void AddNumber(int num)
    {
        if (currentInput.Length < 3) // Maksimal 3 digit
        {
            currentInput += num.ToString();
            answerText.text = currentInput;
        }
    }

    void CheckAnswer()
    {
        if (string.IsNullOrEmpty(currentInput)) return;

        int playerAnswer = int.Parse(currentInput);

        if (playerAnswer == correctAnswer)
        {
            // BENAR
            progressBoxes[currentRound].color = Color.yellow;
            currentRound++;

            if (currentRound >= 3)
            {
                IsCompleted = true;
                Invoke("FinishPuzzle", 0.5f);
            }
            else
            {
                GenerateQuestion();
            }
        }
        else
        {
            // SALAH
            Debug.Log("Salah hitung! Reset progres.");
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

    void FinishPuzzle()
    {
        if (interaction != null) interaction.OnPuzzleCompleted();
    }
}