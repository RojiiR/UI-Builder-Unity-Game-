using UnityEngine;
using UnityEngine.UI;

public class PasswordDisplayUI : MonoBehaviour
{
    public Text textDisplay; 
    public PasswordPuzzle sourcePuzzle;
    
    // Referensi ke sistem interaksi di Kotak B
    private PuzzleInteraction interaction;

    void Start()
    {
        // Cari tombol CloseBtn di dalam panel ini
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name == "CloseBtn")
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    if (interaction != null) 
                        interaction.ClosePuzzle(); // Tutup lewat sistem interaction
                    else 
                        gameObject.SetActive(false); // Backup jika interaction tidak ketemu
                });
            }
        }
    }

    void OnEnable()
    {
        if (sourcePuzzle != null && textDisplay != null)
        {
            string pass = sourcePuzzle.GetPassword();
            
            if (pass == "NOTHING")
            {
                textDisplay.text = "NOTHING";
                textDisplay.color = Color.red; // Opsional: kasih warna merah kalau belum ada
            }
            else
            {
                textDisplay.text = "CODE: " + pass;
                textDisplay.color = Color.green; // Warna hijau kalau sudah ada
            }
        }
    }

    // Fungsi ini akan dipanggil otomatis oleh PuzzleInteraction saat Start
    // karena kita memasukkan script ini ke dalam panel puzzle
    public void SetInteraction(PuzzleInteraction inter)
    {
        interaction = inter;
    }
}