using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class CButtonPointDown : CButton
{
    [SerializeField]
    private UnityEvent PointDown = new UnityEvent();
    //---------------------------------------------------
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        PointDown?.Invoke();
    }
}
  