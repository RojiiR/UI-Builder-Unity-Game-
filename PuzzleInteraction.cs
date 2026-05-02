using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleInteraction : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public GameObject puzzlePanel;
    public string playerTag = "Player";

    [Header("Prompt UI (optional)")]
    public GameObject interactPrompt;

    private bool playerInRange = false;
    private WeirdPuzzle weirdPuzzle;
    private PasswordPuzzle passwordPuzzle; // Tambahkan ini
    private FlowPuzzle flowPuzzle;
    private ReactionPuzzle reactionPuzzle;
    private MemoryPuzzle memoryPuzzle;
    private ColorMatchPuzzle colorMatchPuzzle;
    private MathPuzzle mathPuzzle;

    private PlayerController playerController;
   

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        if (puzzlePanel != null)
        {
            // Deteksi WeirdPuzzle
            weirdPuzzle = puzzlePanel.GetComponent<WeirdPuzzle>();
            if (weirdPuzzle != null) weirdPuzzle.interaction = this;

            // Deteksi PasswordPuzzle 
            passwordPuzzle = puzzlePanel.GetComponent<PasswordPuzzle>();
            if (passwordPuzzle != null) passwordPuzzle.interaction = this;

            // Deteksi FlowPuzzle 
            flowPuzzle = puzzlePanel.GetComponent<FlowPuzzle>();
            if (flowPuzzle != null) flowPuzzle.interaction = this;

            // Deteksi Reaction Puzzle 
            reactionPuzzle = puzzlePanel.GetComponent<ReactionPuzzle>();
            if (reactionPuzzle != null) reactionPuzzle.interaction = this;
            
            // Deteksi Memory Puzzle 
            memoryPuzzle = puzzlePanel.GetComponent<MemoryPuzzle>();
            if (memoryPuzzle != null) memoryPuzzle.interaction = this;

            // Deteksi Color Match Puzzle 
            colorMatchPuzzle = puzzlePanel.GetComponent<ColorMatchPuzzle>();
            if (colorMatchPuzzle != null) colorMatchPuzzle.interaction = this;
            
            // Deteksi Math Puzzle 
            mathPuzzle = puzzlePanel.GetComponent<MathPuzzle>();
            if (mathPuzzle != null) mathPuzzle.interaction = this;

            //password paper
            PasswordDisplayUI displayUI = puzzlePanel.GetComponent<PasswordDisplayUI>();
            if (displayUI != null) displayUI.SetInteraction(this);

            puzzlePanel.SetActive(false);
        }

        if (interactPrompt != null){
            interactPrompt.SetActive(false);
        }
            
        flowPuzzle = puzzlePanel.GetComponent<FlowPuzzle>();
        if (flowPuzzle != null) flowPuzzle.interaction = this;  
        reactionPuzzle = puzzlePanel.GetComponent<ReactionPuzzle>();
        if (reactionPuzzle != null) reactionPuzzle.interaction = this;  
    }

    void Update()
    {
        if (!playerInRange) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Cek apakah salah satu puzzle sudah selesai
            bool isAlreadyDone = (weirdPuzzle != null && weirdPuzzle.IsCompleted) || 
                     (passwordPuzzle != null && passwordPuzzle.IsCompleted) ||
                     (flowPuzzle != null && flowPuzzle.IsCompleted) ||
                     (reactionPuzzle != null && reactionPuzzle.IsCompleted) ||
                     (memoryPuzzle != null && memoryPuzzle.IsCompleted) ||
                     (colorMatchPuzzle != null && colorMatchPuzzle.IsCompleted) ||
                     (mathPuzzle != null && mathPuzzle.IsCompleted);
            
            if (isAlreadyDone) return;

            OpenPuzzle();
        }
    }

    void OpenPuzzle()
    {
        if (puzzlePanel == null) return;

        puzzlePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
            playerController.enabled = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    // Dipanggil oleh WeirdPuzzle ATAU PasswordPuzzle
    public void OnPuzzleCompleted()
    {
        ClosePuzzle();
        Debug.Log("[Puzzle] Selesai! Player kembali normal.");
    }

    // Tambahkan fungsi Public agar tombol "Close" di UI bisa memanggil ini
    public void ClosePuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.enabled = true;
            
        Debug.Log("[Puzzle] UI Ditutup, PlayerController Aktif: " + (playerController != null));
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;
        playerController = other.GetComponent<PlayerController>();

        bool isAlreadyDone = (weirdPuzzle != null && weirdPuzzle.IsCompleted) || 
                             (passwordPuzzle != null && passwordPuzzle.IsCompleted);

        if (interactPrompt != null && !isAlreadyDone)
            interactPrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (puzzlePanel != null && puzzlePanel.activeSelf)
            ClosePuzzle();
    }
}