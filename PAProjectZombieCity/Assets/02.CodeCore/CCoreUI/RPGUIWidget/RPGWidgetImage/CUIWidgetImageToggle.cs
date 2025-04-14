using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 크기나 형태가 다른 이미지의 토글 
// Enable 기반의 제어
public class CUIWidgetImageToggle : CUIWidgetBase
{
    [SerializeField]
    private CImage ToggleOn = null;
    [SerializeField]
    private CImage ToggleOff = null;
    [SerializeField]
    private bool DefaultOn = false;

    private bool m_bToggleOn = false;
    //-----------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
        if (ToggleOn != null)
        {
            ToggleOn.raycastTarget = false;
        }

        if (ToggleOff != null)
        {
            ToggleOff.raycastTarget = false;
        }

        PrivImageToggle(DefaultOn);
    }

    //------------------------------------------------------------
    public void DoImageToggle(bool bOn)
    {
        PrivImageToggle(bOn);
    }

    public void DoImageToggle()
    {
        PrivImageToggle(!m_bToggleOn);
    }

    //------------------------------------------------------------
    private void PrivImageToggle(bool bOn)
    {
        m_bToggleOn = bOn;
        if (bOn)
        {
            if (ToggleOn != null)
            ToggleOn.enabled = true;
            if (ToggleOff != null)
            ToggleOff.enabled = false;
        }
        else
        {
            if (ToggleOn != null)
            ToggleOn.enabled = false;
            if (ToggleOff != null)
            ToggleOff.enabled = true;
        }
    }
}

