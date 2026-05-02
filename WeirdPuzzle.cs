using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WeirdPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;

    [Header("Auto-assigned by PuzzleUIBuilder")]
    public RectTransform lineContainer;
    public PuzzleInteraction interaction;

    private readonly Color[] palette = new Color[]
    {
        new Color(0.92f, 0.25f, 0.25f),
        new Color(0.25f, 0.55f, 0.92f),
        new Color(0.95f, 0.85f, 0.20f)
    };

    private int[] leftOrder;
    private int[] rightOrder;

    private RectTransform[] leftRects  = new RectTransform[3];
    private RectTransform[] rightRects = new RectTransform[3];
    private Image[]         leftImages  = new Image[3];
    private Image[]         rightImages = new Image[3];

    private bool[]  connected;
    private Image[] lineObjects;

    private bool    isDragging         = false;
    private int     draggingColorIndex = -1;
    private Image   dragLine           = null;
    private Vector2 dragStartLocal;

    private Canvas      canvas;

    void Awake()
    {
        canvas      = GetComponentInParent<Canvas>();
        connected   = new bool[3];
        lineObjects = new Image[3];
    }

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            Transform lt = transform.Find($"Left_{i}");
            Transform rt = transform.Find($"Right_{i}");

            if (lt != null)
            {
                leftRects[i]  = lt.GetComponent<RectTransform>();
                leftImages[i] = lt.GetComponent<Image>();
                var h = lt.gameObject.AddComponent<PuzzleBoxDragHandler>();
                h.puzzle = this; h.slotIndex = i; h.isLeft = true;
            }
            if (rt != null)
            {
                rightRects[i]  = rt.GetComponent<RectTransform>();
                rightImages[i] = rt.GetComponent<Image>();
                var h = rt.gameObject.AddComponent<PuzzleBoxDragHandler>();
                h.puzzle = this; h.slotIndex = i; h.isLeft = false;
            }
        }
        ResetPuzzle();
    }

    void OnEnable()
    {
        if (leftRects[0] != null && !IsCompleted)
            ResetPuzzle();
    }

    void Update()
    {
        if (!isDragging || dragLine == null) return;

        Vector2 mouseLocal = MouseToLineContainer();
        DrawLine(dragLine, dragStartLocal, mouseLocal, palette[draggingColorIndex]);

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            CancelDrag();
    }

    public void ResetPuzzle()
    {
        IsCompleted        = false;
        isDragging         = false;
        draggingColorIndex = -1;
        dragLine           = null;
        connected          = new bool[3];

        if (lineContainer != null)
            foreach (Transform child in lineContainer)
                Destroy(child.gameObject);
        
        lineObjects = new Image[3];

        leftOrder  = Shuffle(new int[] { 0, 1, 2 });
        rightOrder = Shuffle(new int[] { 0, 1, 2 });

        for (int i = 0; i < 3; i++)
        {
            if (leftImages[i]  != null) leftImages[i].color  = palette[leftOrder[i]];
            if (rightImages[i] != null) rightImages[i].color = palette[rightOrder[i]];
            SetOutline(leftRects[i],  false);
            SetOutline(rightRects[i], false);
        }
    }

    public void StartDrag(int slotIndex)
    {
        int colorIndex = leftOrder[slotIndex];
        if (connected[colorIndex]) return;

        if (lineObjects[colorIndex] != null)
        {
            Destroy(lineObjects[colorIndex].gameObject);
            lineObjects[colorIndex] = null;
        }

        isDragging         = true;
        draggingColorIndex = colorIndex;
        dragLine           = CreateLineImage();
        lineObjects[colorIndex] = dragLine;

        dragStartLocal = RectToLineContainer(leftRects[slotIndex]);
    }

    public void TryDrop(int slotIndex)
    {
        if (!isDragging) return;

        int targetColor = rightOrder[slotIndex];

        if (targetColor == draggingColorIndex)
        {
            Vector2 endLocal = RectToLineContainer(rightRects[slotIndex]);
            DrawLine(dragLine, dragStartLocal, endLocal, palette[draggingColorIndex]);

            connected[draggingColorIndex] = true;
            SetOutline(leftRects[GetLeftSlotOfColor(draggingColorIndex)], true);
            SetOutline(rightRects[slotIndex], true);

            isDragging         = false;
            dragLine           = null;
            draggingColorIndex = -1;

            CheckAllConnected();
        }
        else
        {
            CancelDrag();
        }
    }

    void CancelDrag()
    {
        if (dragLine != null)
        {
            Destroy(dragLine.gameObject);
            if (draggingColorIndex >= 0)
                lineObjects[draggingColorIndex] = null;
            dragLine = null;
        }
        isDragging         = false;
        draggingColorIndex = -1;
    }

    void CheckAllConnected()
    {
        if (connected[0] && connected[1] && connected[2])
        {
            IsCompleted = true;
            StartCoroutine(CompleteAfterDelay(0.6f));
        }
    }

    IEnumerator CompleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (interaction != null) interaction.OnPuzzleCompleted();
        else gameObject.SetActive(false);
    }

    // --- KOORDINAT FIX ---
    Vector2 RectToLineContainer(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;

        // Menghasilkan posisi relatif terhadap PIVOT lineContainer
        return lineContainer.InverseTransformPoint(worldCenter);
    }

    Vector2 MouseToLineContainer()
    {
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineContainer,
            Mouse.current.position.ReadValue(),
            cam,
            out Vector2 local);
        return local;
    }

    // --- DRAWING FIX ---
    Image CreateLineImage()
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(lineContainer, false);
        Image img = go.AddComponent<Image>();
        img.raycastTarget = false;

        RectTransform rt = go.GetComponent<RectTransform>();
        
        // KRUSIAL: Anchor harus di tengah (0.5) agar sinkron dengan InverseTransformPoint 
        // yang berbasis pada pivot parent-nya (Container)
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        
        // Pivot (0, 0.5) artinya pangkal rotasi ada di ujung kiri tengah image garis
        rt.pivot = new Vector2(0f, 0.5f); 
        
        return img;
    }

    void DrawLine(Image line, Vector2 from, Vector2 to, Color color)
    {
        if (line == null) return;
        RectTransform rt = line.GetComponent<RectTransform>();
        
        Vector2 dir = to - from;
        rt.anchoredPosition = from;
        
        // Atur panjang garis sesuai jarak antar titik
        rt.sizeDelta = new Vector2(dir.magnitude, 6f); 
        
        // Atur rotasi berdasarkan arah
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
        
        line.color = color;
    }

    int GetLeftSlotOfColor(int colorIndex)
    {
        for (int i = 0; i < leftOrder.Length; i++)
            if (leftOrder[i] == colorIndex) return i;
        return 0;
    }

    void SetOutline(RectTransform rt, bool on)
    {
        if (rt == null) return;
        Outline o = rt.GetComponent<Outline>() ?? rt.gameObject.AddComponent<Outline>();
        o.effectColor    = Color.white;
        o.effectDistance = new Vector2(2, -2);
        o.enabled        = on;
    }

    int[] Shuffle(int[] arr)
    {
        int[] c = (int[])arr.Clone();
        for (int i = c.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = c[i]; c[i] = c[j]; c[j] = tmp;
        }
        return c;
    }
}