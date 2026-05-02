using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlowPoint : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Color color;
    public FlowPuzzle puzzle;
    public bool isConnected = false;
    private Outline outline;

    void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.effectDistance = new Vector2(5, 5);
        outline.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        puzzle.StartDragging(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        puzzle.OnDragging(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        puzzle.StopDragging();
    }

    public void Highlight(bool state) => outline.enabled = state;
}