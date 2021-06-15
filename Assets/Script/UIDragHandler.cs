using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;



public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UnityAction<PointerEventData> _beginCall;
    public UnityAction<PointerEventData> _dragCall;
    public UnityAction<PointerEventData> _endCall;

    public void SetEnable(bool isEnable)
    {
        this.enabled = isEnable;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this._beginCall != null)
            this._beginCall.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this._dragCall != null)
            this._dragCall.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this._endCall != null)
            this._endCall.Invoke(eventData);
    }
}

