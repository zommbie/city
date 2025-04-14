using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// 프로젝트 마다 오버라이드 해서 사용할 것

public enum EButtonSound
{
    None,
    Click,
    ClickPlus,
    ClickCancel,
}


[AddComponentMenu("UICustom/CButton", 1)]
public class CButton : Button
{
    private static PointerEventData g_LastPointerEventData = null;
    private static UnityAction      g_LastPointerClickOnce = null;

    [SerializeField]
    private EButtonSound ButtonSound;

    //---------------------------------------------------------
    public override void OnPointerClick(PointerEventData eventData)
    {
        g_LastPointerEventData = eventData;
        PrivButtonLastClickEventOnce();
        base.OnPointerClick(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        g_LastPointerEventData = eventData;
        base.OnPointerDown(eventData);
    }

    //---------------------------------------------------------
    public static Vector3 GetButtonLastPressPointer()
    {
        Vector3 vecScreenPosition = Vector3.zero;
        if (g_LastPointerEventData != null)
        {
            vecScreenPosition = g_LastPointerEventData.pressPosition;
        }
        return vecScreenPosition;
    }

    public static void SetButtonLastClickEventOnce(UnityAction delFinish)
    {
        g_LastPointerClickOnce = delFinish;
    }

    public void DoButtonClick()
    {
        OnSubmit(null);
    }

    //------------------------------------------------------------------
    private void PrivButtonLastClickEventOnce()
    {
        if (g_LastPointerClickOnce != null)
        {
            UnityAction pLastPointClickOnce = g_LastPointerClickOnce;  // 버튼 이벤트 체인에 의한 사이드 이펙트를 방지
            g_LastPointerClickOnce = null;
            pLastPointClickOnce.Invoke();
        }
    }
}  
