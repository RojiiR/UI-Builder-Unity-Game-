using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FlowPuzzle : MonoBehaviour
{
    public bool IsCompleted { get; private set; } = false;
    public PuzzleInteraction interaction;
    public RectTransform lineContainer;
    
    private List<FlowPoint> allPoints = new List<FlowPoint>();
    private FlowPoint currentStartPoint;
    private List<Vector2> occupiedGrids = new List<Vector2>(); // Database koordinat tali
    private List<GameObject> currentLineSegments = new List<GameObject>();
    private Vector2 lastGridPos;

    public float gridSize = 60f; // Sesuaikan dengan ukuran kotakmu

    void Start()
    {
        allPoints.AddRange(GetComponentsInChildren<FlowPoint>());
        foreach (var p in allPoints) p.puzzle = this;
    }

    public void StartDragging(FlowPoint point)
    {
        if (IsCompleted || point.isConnected) return;
        
        currentStartPoint = point;
        lastGridPos = SnapToGrid(point.GetComponent<RectTransform>().anchoredPosition);
        
        // Highlight titik awal
        point.Highlight(true);
    }

    public void OnDragging(Vector2 mousePos)
    {
        if (currentStartPoint == null) return;

        // 1. Konversi posisi mouse ke posisi lokal UI
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineContainer, mousePos, null, out Vector2 localPos);
        
        // 2. CEK BATAS: Jika posisi mouse keluar dari lineContainer, RESET!
        if (!IsPositionInsideContainer(localPos))
        {
            Debug.Log("Keluar Batas! Reset Puzzle.");
            ResetPuzzle();
            return;
        }

        Vector2 snappedPos = SnapToGrid(localPos);

        // 3. Jika gerak ke grid baru
        if (snappedPos != lastGridPos)
        {
            // CEK TABRAKAN
            if (occupiedGrids.Contains(snappedPos))
            {
                Debug.Log("TABRAKAN! Reset.");
                ResetPuzzle();
                return;
            }

            CreateLineSegment(lastGridPos, snappedPos);
            occupiedGrids.Add(snappedPos);
            lastGridPos = snappedPos;

            CheckForTargetPoint(snappedPos);
        }
    }

    // Fungsi Pembantu untuk cek apakah koordinat ada di dalam Rect
    bool IsPositionInsideContainer(Vector2 localPos)
    {
        Rect rect = lineContainer.rect;
        // Kita beri sedikit toleransi (offset) agar tidak terlalu sensitif di ujung garis
        return rect.Contains(localPos);
    }

    public void StopDragging()
    {
        if (currentStartPoint == null) return;

        // Jika dilepas tapi belum sampai ke pasangan warna yang sama
        ResetCurrentLine();
        currentStartPoint.Highlight(false);
        currentStartPoint = null;
    }

    void CreateLineSegment(Vector2 start, Vector2 end)
    {
        GameObject line = new GameObject("LineSeg");
        line.transform.SetParent(lineContainer, false);
        Image img = line.AddComponent<Image>();
        img.color = currentStartPoint.color;
        img.raycastTarget = false;

        RectTransform rt = img.rectTransform;
        Vector2 dir = end - start;
        float length = dir.magnitude;

        rt.anchoredPosition = start + (dir / 2);
        rt.sizeDelta = new Vector2(length + 5f, 10f); // 10f adalah tebal tali
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);

        currentLineSegments.Add(line);
    }

    void CheckForTargetPoint(Vector2 pos)
    {
        foreach (var p in allPoints)
        {
            if (p == currentStartPoint) continue;

            Vector2 pPos = SnapToGrid(p.GetComponent<RectTransform>().anchoredPosition);
            if (pos == pPos)
            {
                if (p.color == currentStartPoint.color)
                {
                    // BERHASIL CONNECT!
                    p.isConnected = true;
                    currentStartPoint.isConnected = true;
                    currentLineSegments.Clear(); // Garis jadi permanen
                    currentStartPoint = null;
                    CheckWinCondition();
                }
                else
                {
                    // Nabrak warna lain
                    ResetPuzzle();
                }
                return;
            }
        }
    }

    void ResetCurrentLine()
    {
        foreach (var line in currentLineSegments) Destroy(line);
        currentLineSegments.Clear();
        // Hapus koordinat yang baru saja dilewati (logika tambahan bisa diatur di sini)
    }

    public void ResetPuzzle()
    {
        foreach (Transform child in lineContainer) Destroy(child.gameObject);
        occupiedGrids.Clear();
        currentLineSegments.Clear();
        foreach (var p in allPoints) p.isConnected = false;
        currentStartPoint = null;
    }

    Vector2 SnapToGrid(Vector2 pos)
    {
        float x = Mathf.Round(pos.x / gridSize) * gridSize;
        float y = Mathf.Round(pos.y / gridSize) * gridSize;
        return new Vector2(x, y);
    }

    void CheckWinCondition()
    {
        int count = 0;
        foreach (var p in allPoints) if (p.isConnected) count++;
        if (count >= allPoints.Count)
        {
            IsCompleted = true;
            interaction.OnPuzzleCompleted();
        }
    }
}