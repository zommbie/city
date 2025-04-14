using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetAnimationGaugeDestSectionBase : CUIWidgetAnimationGaugeBase
{
    [SerializeField]
    private float SectionSpeed = 1f;  // 초당 이동 속도 

    [SerializeField]
    private CSlider DestSlider;

    private bool m_bGaugeDestStart = false;
    private bool m_bGaugeSectionForward = false;
    private bool m_bGaugeBackwardStart = false;
    private float m_fCurrentDestRate = 0f;
    private float m_fDestRate = 0f;
    //----------------------------------------------------------------------------
    protected override void OnUIGaugeStart(float fDestValue, float fGaugeDestRate, bool bFoward)
    {
        base.OnUIGaugeStart(fDestValue, fGaugeDestRate, bFoward);

        if (m_bGaugeDestStart == false)
        {
            m_bGaugeDestStart = true;
            PrivDestSectionStart(fGaugeDestRate);
        }
        else
        {
            PrivDestSectionRefresh(fGaugeDestRate);
        }
    }

    protected override void OnUIGaugeUpdate(float fDelta)
    {
        base.OnUIGaugeUpdate(fDelta);
        if (m_bGaugeDestStart)
        {
            if (m_bGaugeSectionForward)
            {
                UpdateDestSectionForward(fDelta);
            }
        }     
    }

    protected override void OnUIUnityUpdate(float fDelta)
    {
        base.OnUIUnityUpdate(fDelta);
        if (m_bGaugeDestStart == false && m_bGaugeBackwardStart)
        {
            UpdateDestSectionBackward(fDelta);
        }
    }

    protected override void OnUIGaugeReset(float fMax, float fStartValue)
    {
        base.OnUIGaugeReset(fMax, fStartValue);
        if (DestSlider != null)
        {
            DestSlider.value = 0f;
        }
    }

    protected override void OnUIGaugeEnd(float fMoveLength, bool bFoward)
    {
        base.OnUIGaugeEnd(fMoveLength, bFoward);

        if (m_bGaugeDestStart == true)
        {
            m_bGaugeDestStart = false;
            if (DestSlider != null)
            {
                DestSlider.value = 0f;
            }
        }

        if (m_bGaugeSectionForward == false)
        {
            m_bGaugeBackwardStart = true;
        }
    }

    //--------------------------------------------------------------------------------
    private void PrivDestSectionStart(float fDestValue)
    {        
        m_fDestRate = fDestValue;
        m_fCurrentDestRate = GetUIGaugeValueCurrentRate();
        if (DestSlider != null)
        {
            DestSlider.value = m_fCurrentDestRate;
        }
        PrivDestSectionDirection(m_fCurrentDestRate, m_fDestRate);

        if (m_bGaugeSectionForward == false)
        {
            m_bGaugeBackwardStart = false;
        }
    }

    private void PrivDestSectionDirection(float fCurrentRate, float fDestRate)
    {
        if (fCurrentRate <= fDestRate)
        {
            m_bGaugeSectionForward = true;
        }
        else if (fCurrentRate > fDestRate)
        {
            m_bGaugeSectionForward = false;
        }
    }

    private void PrivDestSectionRefresh(float fDestValue)
    {       
        m_fDestRate = fDestValue;
        PrivDestSectionDirection(m_fCurrentDestRate, m_fDestRate);
    }

    private void UpdateDestSectionForward(float fDelta)
    {
        float fMoveValue = fDelta * SectionSpeed;
        m_fCurrentDestRate += fMoveValue;

        if (m_fCurrentDestRate > m_fDestRate)
        {
            m_fCurrentDestRate = m_fDestRate;
        }

        if (DestSlider != null)
        {
            DestSlider.value = m_fCurrentDestRate;
        }
    }

    private void UpdateDestSectionBackward(float fDelta)
    {
        float fMoveValue = fDelta * SectionSpeed;
        m_fCurrentDestRate -= fMoveValue;

        if (m_fCurrentDestRate < m_fDestRate)
        {
            m_fCurrentDestRate = m_fDestRate;
            m_bGaugeBackwardStart = false;
        }

        if (DestSlider != null)
        {
            DestSlider.value = m_fCurrentDestRate;
        }
    }
}
