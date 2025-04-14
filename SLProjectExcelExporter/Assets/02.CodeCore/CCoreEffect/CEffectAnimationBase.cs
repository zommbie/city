using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


abstract public class CEffectAnimationBase : CEffectBase
{


    private Animator m_pAnimator = null;
    private Dictionary<string, AnimationClip> m_mapAnimationClip = new Dictionary<string, AnimationClip>();
    //------------------------------------------------------
    protected override void OnEffectInitialize()
    {
        base.OnEffectInitialize();
        m_pAnimator = GetComponent<Animator>();
        if (m_pAnimator == null)
        {
            m_pAnimator = GetComponentInChildren<Animator>();
            CEffectAnimationHandler pHandler = m_pAnimator.gameObject.AddComponent<CEffectAnimationHandler>();
            pHandler.AnimEndFuction = HandleEffectAnimEnd;
        }

        PrivEffectAnimationInitialize(m_pAnimator.runtimeAnimatorController.animationClips);
    }

    protected override void OnEffectStart(float fDuration, params object[] aParams)
    {
        base.OnEffectStart(fDuration, aParams);
        m_pAnimator.Rebind();
        if (aParams.Length > 0)
        {
            if (aParams[0] is string)
            {
                m_pAnimator.Play(aParams[0] as string);
            }
        }    
    }

    //----------------------------------------------------------
   
    private void PrivEffectAnimationInitialize(AnimationClip[] aClips)
    {
        for (int i = 0; i < aClips.Length; i++)
        {
            AnimationClip pClip = aClips[i];
            m_mapAnimationClip[pClip.name] = pClip;
            AnimationEvent pEndEvent = new AnimationEvent();
            pEndEvent.time = pClip.length;
            pEndEvent.functionName = "HandleEffectAnimEnd";
            pEndEvent.stringParameter = pClip.name;
            pClip.AddEvent(pEndEvent);
        }
    }

    public void HandleEffectAnimEnd(string strAnimName)
    {
        DoEffectEnd();
    }

    //----------------------------------------------------------------------------

}

public class CEffectAnimationHandler : CMonoBase
{
    public UnityAction<string> AnimEndFuction = null;
    //--------------------------------------------------------
    private void HandleEffectAnimEnd(string strAnimName)
    {
        AnimEndFuction?.Invoke(strAnimName);
    }
}
