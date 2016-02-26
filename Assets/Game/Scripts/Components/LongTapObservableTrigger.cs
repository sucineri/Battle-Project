using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class LongTapObservableTrigger : ObservableTriggerBase, IPointerDownHandler, IPointerUpHandler 
{
    [SerializeField] private float _longTapInterval = 1f;

    Subject<bool> _onPointerUp;

    private float _raiseTime = 0f;

    public IObservable<bool> OnPointerUpAsObserable()
    {
        return this._onPointerUp ?? (this._onPointerUp = new Subject<bool>());
    }

    protected override void RaiseOnCompletedOnDestroy()
    {
        if (this._onPointerUp != null)
        {
            this._onPointerUp.OnCompleted();
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (this._raiseTime <= 0f)
        {
            this._raiseTime = Time.realtimeSinceStartup + this._longTapInterval;
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (this._onPointerUp != null)
        {
            this._onPointerUp.OnNext(this._raiseTime <= Time.realtimeSinceStartup);
        }
        this._raiseTime = 0f;
    }

}
