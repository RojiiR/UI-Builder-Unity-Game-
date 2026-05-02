using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PasswordPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;
    
    [Header("UI References")]
    public Text displayBox; 
    public PuzzleInteraction interaction; 

    private string correctPassword;
    private string currentInput = "";
    private bool isPasswordGenerated = false;

    void Start()
    {
        GenerateNewPassword();
        SetupButtons(); // <--- Tambahkan ini untuk mendaftarkan klik otomatis
    }
    

    // Fungsi sakti untuk mengisi OnClick yang kosong secara otomatis
    void SetupButtons()
    {
        // 1. Cari semua tombol yang ada di bawah panel ini
        Button[] allButtons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in allButtons)
        {
            string btnName = btn.gameObject.name;

            // Bersihkan listener lama biar ngga dobel
            btn.onClick.RemoveAllListeners();

            // Jika namanya mengandung "Btn_", ambil angkanya
            if (btnName.StartsWith("Btn_"))
            {
                string numStr = btnName.Replace("Btn_", "");
                if (int.TryParse(numStr, out int number))
                {
                    btn.onClick.AddListener(() => InputNumber(number));
                }
            }
            // Jika namanya EnterBtn
            else if (btnName == "EnterBtn")
            {
                btn.onClick.AddListener(() => CheckPassword());
            }
            else if (btnName == "CloseBtn")
            {
                btn.onClick.AddListener(() => {
                    if (interaction != null) interaction.ClosePuzzle();
                    else gameObject.SetActive(false);
                });
            }
        }
        Debug.Log("[PasswordPuzzle] Semua tombol berhasil didaftarkan via Code!");
    }

    public void GenerateNewPassword()
    {
        string code = "";
        for (int i = 0; i < 4; i++)
        {
            code += Random.Range(1, 10).ToString(); 
        }
        correctPassword = code;
        
        currentInput = "";
        UpdateDisplay("ENTER CODE");
        isPasswordGenerated = true; // Tandai sudah ada password
        Debug.Log($"[PasswordPuzzle] Password baru: {correctPassword}");
    }

    public void InputNumber(int number)
    {
        Debug.Log("Tombol diklik: " + number); 
        if (IsCompleted || currentInput.Length >= 4) return;

        currentInput += number.ToString();
        UpdateDisplay(currentInput);
    }

    public void CheckPassword()
    {
        if (IsCompleted) return;
        if (currentInput == correctPassword)
            StartCoroutine(SuccessSequence());
        else
            StartCoroutine(FailSequence());
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay("");
    }

    void UpdateDisplay(string text)
    {
        if (displayBox != null) displayBox.text = text;
    }

    IEnumerator SuccessSequence()
    {
        IsCompleted = true;
        UpdateDisplay("CORRECT!");
        yield return new WaitForSeconds(1f);
        
        // 1. Panggil callback completion
        if (interaction != null) interaction.OnPuzzleCompleted();

        // 2. KEMBALIKAN KONTROL PLAYER DI SINI
        // Ganti 'PlayerMovement' dengan nama script movement kamu
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Jika kamu menggunakan script movement biasa:
            // player.GetComponent<PlayerMovement>().enabled = true;

            // Jika kamu menggunakan sistem kursor:
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        gameObject.SetActive(false);
    }

    IEnumerator FailSequence()
    {
        UpdateDisplay("WRONG!");
        yield return new WaitForSeconds(1f);
        ClearInput();
        UpdateDisplay("ENTER CODE");
    }

    public string GetPassword()
    {
        if (!isPasswordGenerated) return "NOTHING"; 
        return correctPassword;
    }
}