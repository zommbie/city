using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// [셋팅] 에니메이터 설정에서 트랜지션 설정은 Idle만 해 놓는다. idle condition = true
//        코드에서 수동으로 제어하므로 파라메터를 통한 자동 트랜지션은 사용하지 않는다


public abstract class CShapeAnimationAnimatorBase : CShapeAnimationBase
{
    [SerializeField]
    private Animator AniControl;

    private string m_strIdleName = string.Empty; public string GetShapeAnimationName() { return m_rAnimationUsage.strAniGroupName; }   
    private float m_fCurrentAniDuration = 0;
    private bool m_bPlayAnimation = false;
   
    private SAnimationUsage m_rAnimationUsage = new SAnimationUsage();
    //-------------------------------------------------------------
    protected override void OnShapeAnimationInitialize(IEventAnimationNotify pAniNotifyProcessor)
    {
        base.OnShapeAnimationInitialize(pAniNotifyProcessor);
    }

    protected override void OnShapeAnimationUpdate(float fDelta)
    {
        base.OnShapeAnimationUpdate(fDelta);

        if (m_bPlayAnimation)
        {
            if (m_rAnimationUsage.fDuration != 0 && m_rAnimationUsage.bLoop)
            {
                UpdateShapeAnimationDuration(fDelta);
            }
            else if (m_rAnimationUsage.bLoop == false)
            {
                UpdateShapeAnimationEnd();
            }   
        }
    }

    protected override void OnShapeAnimationPlay(ref SAnimationUsage rAniUsage, bool bIdle) 
    {
        if (AniControl.HasState(0, Animator.StringToHash(rAniUsage.strAniGroupName)))
        {
            m_rAnimationUsage = rAniUsage;
            if (bIdle)
            {
                PrivShapeAnimationIdle(ref rAniUsage);
            }
            else
            {
                PrivShapeAnimationPlay(ref rAniUsage);
            }
        }
        else
        {
            Debug.LogWarningFormat("[Aniamtion] Invalid Ani Name {0}", rAniUsage.strAniGroupName);
        }
    }

    //-----------------------------------------------------------------------
    private void UpdateShapeAnimationDuration(float fDelta)
    {
        m_fCurrentAniDuration += fDelta;

        if (m_fCurrentAniDuration >= m_rAnimationUsage.fDuration)
        {
            PrivShapeAnimationEnd();
        }
    }

    private void UpdateShapeAnimationEnd()
    {
        AnimatorStateInfo rAniStateInfo = AniControl.GetCurrentAnimatorStateInfo(0);
        if (rAniStateInfo.IsName(m_rAnimationUsage.strAniGroupName))
        {
            if (rAniStateInfo.normalizedTime >= 1f)
            {
                PrivShapeAnimationEnd();
            }
        }
    }
   
    private void PrivShapeAnimationEnd()
    {
        AniControl.SetBool(m_rAnimationUsage.strAniGroupName, false);

        if (m_rAnimationUsage.bAutoIdle)
        {
            AniControl.SetBool(m_strIdleName, true);
        }
        else
        {
            AniControl.speed = 0;
        }

        m_rAnimationUsage.Finish?.Invoke();
        m_pAniNotifyProcessor.IAnimationEnd(m_rAnimationUsage.strAniGroupName);
        m_bPlayAnimation = false;
    }

    private void PrivShapeAnimationPlay(ref SAnimationUsage rAniUsage)
    {    
        if (m_strIdleName != string.Empty)
        {
            AniControl.SetBool(m_strIdleName, false);
        }

        if (rAniUsage.fBlend != 0)
        {          
            AniControl.SetBool(rAniUsage.strAniGroupName, true);
        }
        else
        {            
            AniControl.Play(rAniUsage.strAniGroupName, rAniUsage.Layer);
        }
        AniControl.speed = rAniUsage.fAniSpeed == 0 ? 1f : rAniUsage.fAniSpeed;      

        m_fCurrentAniDuration = 0f;
        m_bPlayAnimation = true;
    }

    private void PrivShapeAnimationIdle(ref SAnimationUsage rAniUsage)
    {
        AniControl.SetBool(rAniUsage.strAniGroupName, true);      
        AniControl.speed = 1f;

        m_fCurrentAniDuration = 0;
        m_strIdleName = rAniUsage.strAniGroupName;
        m_bPlayAnimation = false; 
    }
}
