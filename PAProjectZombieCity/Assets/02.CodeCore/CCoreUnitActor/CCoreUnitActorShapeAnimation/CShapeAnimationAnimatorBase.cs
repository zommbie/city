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

    private string m_strIdleName = string.Empty; public string GetShapeAnimationName() { return m_rAnimationUsage.strAniName; }   
    private float m_fCurrentAniDuration = 0;
    private bool m_bPlayAnimation = false;
   
    private CAssistShapeAnimationBase.SAnimationUsage m_rAnimationUsage = new CAssistShapeAnimationBase.SAnimationUsage();
    //------------------------------------------------------------

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

    protected override float OnShapeAnimationPlay(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage) 
    {
        if (AniControl.HasState(0, Animator.StringToHash(rAniUsage.strAniName)))
        {
            m_rAnimationUsage = rAniUsage;
            PrivShapeAnimationPlay(ref rAniUsage);
        }
        else
        {
            Debug.LogWarningFormat("[Aniamtion] Invalid Ani Name {0}", rAniUsage.strAniName);
        }

        return 0f;
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
        if (rAniStateInfo.IsName(m_rAnimationUsage.strAniName))
        {
            if (rAniStateInfo.normalizedTime >= 1f)
            {
                PrivShapeAnimationEnd();
            }
        }
    }
   
    private void PrivShapeAnimationEnd()
    {
        AniControl.SetBool(m_rAnimationUsage.strAniName, false);

        if (m_rAnimationUsage.bAutoIdle)
        {
            AniControl.SetBool(m_strIdleName, true);
        }
        else
        {
            AniControl.speed = 0;
        }

        m_rAnimationUsage.AniFinish?.Invoke();       
        m_bPlayAnimation = false;
    }

    private void PrivShapeAnimationPlay(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage)
    {    
        if (m_strIdleName != string.Empty)
        {
            AniControl.SetBool(m_strIdleName, false);
        }

        if (rAniUsage.fBlendTime != 0)
        {          
            AniControl.SetBool(rAniUsage.strAniName, true);
        }
        else
        {            
            AniControl.Play(rAniUsage.strAniName, rAniUsage.Layer);
        }
        AniControl.speed = rAniUsage.fAniSpeed == 0 ? 1f : rAniUsage.fAniSpeed;      

        m_fCurrentAniDuration = 0f;
        m_bPlayAnimation = true;
    }

    private void PrivShapeAnimationIdle(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage)
    {
        AniControl.SetBool(rAniUsage.strAniName, true);      
        AniControl.speed = 1f;

        m_fCurrentAniDuration = 0;
        m_strIdleName = rAniUsage.strAniName;
        m_bPlayAnimation = false; 
    }
}
