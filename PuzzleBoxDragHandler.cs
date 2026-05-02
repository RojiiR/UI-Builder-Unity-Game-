using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to each color box. Handles pointer down (left = start drag)
/// and pointer enter (right = try drop).
/// Auto-added by WeirdPuzzle.SetupButtons() — do NOT add manually.
/// </summary>
public class PuzzleBoxDragHandler : MonoBehaviour,
    IPointerDownHandler, IPointerEnterHandler
{
    [HideInInspector] public WeirdPuzzle puzzle;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public bool isLeft;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLeft)
            puzzle.StartDrag(slotIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLeft)
            puzzle.TryDrop(slotIndex);
    }
}