using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 위
public abstract class CUIWidgetVirtualJoyPadBase : CUIWidgetBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private CImage JoypadBack = null;       // 원형 이미지 사용
    [SerializeField]
    private CImage JoypadThumb = null;      // 원형 이미지 사용

    private float m_fHalfRadiusBack = 0;
    private float m_fHalfRadiusThumb = 0;
    private float m_fThumbLengthMax = 0;
    private float m_fJoyPadLength = 0;

    private bool m_bRefreshDirection = false;               public bool IsRefreshDirection { get { return m_bRefreshDirection; } }
    private Vector2 m_vecJoyPadPosition = Vector2.zero;
    private Vector2 m_vecJoyPadDirection = Vector2.zero;
     //-------------------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        if (JoypadBack.rectTransform.rect.width != JoypadBack.rectTransform.rect.height || JoypadThumb.rectTransform.rect.width != JoypadThumb.rectTransform.rect.height)
        {
            Debug.LogWarningFormat("[UI JoyPad] JoyPad Image width And Height. Must be Same length");
        }

        PrivVirtualJoyPadThumbReset();
    }
    private void FixedUpdate()
    {
        UpdateVirtualJoyPadEditorSimulate();

        if (m_bRefreshDirection)
        {
            OnVirtualJoyPadRefresh(m_vecJoyPadDirection, m_fJoyPadLength);
        }

        OnVirtualJoyPadFixedUpdate(Time.fixedDeltaTime);
    }

    //-------------------------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        m_bRefreshDirection = true;
        PrivVirtualJoyPadRefreshThumb(eventData.position);
        OnVirtualJoyPadStart();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_bRefreshDirection = false;
        PrivVirtualJoyPadThumbReset();
        OnVirtualJoyPadStop();
    }

    public void OnDrag(PointerEventData eventData)
    {
        PrivVirtualJoyPadRefreshThumb(eventData.position);
    }
    //------------------------------------------------------------------------
    private void PrivVirtualJoyPadThumbReset()
    {
        m_fHalfRadiusBack = JoypadBack.rectTransform.rect.width / 2f;
        m_fHalfRadiusThumb = JoypadThumb.rectTransform.rect.width / 2f;

        JoypadThumb.rectTransform.anchorMin = Vector2.zero;
        JoypadThumb.rectTransform.anchorMax = Vector2.zero;
        JoypadThumb.rectTransform.localPosition = Vector2.zero;

        m_vecJoyPadPosition = GetUIPosition();
        m_fThumbLengthMax = m_fHalfRadiusBack - m_fHalfRadiusThumb;

        m_vecJoyPadDirection = Vector2.zero;
        m_fJoyPadLength = 0f;
        m_bRefreshDirection = false;
    }

    private void PrivVirtualJoyPadRefreshThumb(Vector2 vecRefreshPosition)
    {
        Vector2 vecOffset = vecRefreshPosition - m_vecJoyPadPosition;
        float fLength = Vector2.Distance(Vector2.zero, vecOffset);
        fLength = Mathf.Clamp(fLength, -m_fThumbLengthMax, m_fThumbLengthMax);
        vecOffset.Normalize();
        Vector2 vecThumbPosition = vecOffset * fLength;     
        JoypadThumb.rectTransform.localPosition = vecThumbPosition;

        m_vecJoyPadDirection = vecOffset;
        m_fJoyPadLength = fLength;
    }
    //------------------------------------------------------------------------
    private void UpdateVirtualJoyPadEditorSimulate()
    {
#if UNITY_EDITOR
        PrivVirtualJoyPadKeyboadInputDetect();
#endif
    }

    private void PrivVirtualJoyPadKeyboadInputDetect()
    {
        if (Input.anyKey)
        {
            PrivVirtualJoyPadKeyboadInputKey();            
        }
        else if (m_bRefreshDirection)
        {
            PrivVirtualJoyPadThumbReset();
            OnVirtualJoyPadStop();
        }
    }

    private void PrivVirtualJoyPadKeyboadInputKey()
    {
        bool bKeyInput = false;
        float fValueX = 0;
        float fValueY = 0;
        float fAxisValue = m_fThumbLengthMax;
        if (Input.GetKey(KeyCode.W))
        {
            bKeyInput = true;
            fValueY += fAxisValue;         
        }

        if (Input.GetKey(KeyCode.S))
        {
            bKeyInput = true;
            fValueY -= fAxisValue;
        }

        if (Input.GetKey(KeyCode.A))
        {
            bKeyInput = true;
            fValueX -= fAxisValue;
        }

        if (Input.GetKey(KeyCode.D))
        {
            bKeyInput = true;
            fValueX += fAxisValue;
        }

        if (bKeyInput)
        {
            m_bRefreshDirection = true;
            Vector2 vecDirection = new Vector2(fValueX, fValueY);
            vecDirection += m_vecJoyPadPosition; 
            PrivVirtualJoyPadRefreshThumb(vecDirection);         
        }
    }

    //------------------------------------------------------------------------
    protected virtual void OnVirtualJoyPadStart() { }
    protected virtual void OnVirtualJoyPadRefresh(Vector2 VecDirection, float fLength) { }
    protected virtual void OnVirtualJoyPadStop() { }
    protected virtual void OnVirtualJoyPadFixedUpdate(float fDelta) { }
}
