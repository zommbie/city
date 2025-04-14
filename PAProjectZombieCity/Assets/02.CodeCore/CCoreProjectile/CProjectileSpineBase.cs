using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.Events;

public abstract class CProjectileSpineBase : CProjectileBase
{
    [SerializeField]
    private SkeletonAnimation SpineAnimation;
    [SerializeField]
    private Transform SpineRoot;

    private Spine.AnimationState m_pSpineControl = null;
    private MeshRenderer m_pSpineMeshRenderer = null;
    private string m_strAnimationCurrent = null;
    private UnityAction m_delFinish = null;
    //------------------------------------------------------
    protected override void OnProjectileInitialize()
    {
        m_pSpineControl = SpineAnimation.AnimationState;
        m_pSpineMeshRenderer = SpineAnimation.gameObject.GetComponent<MeshRenderer>();
        m_pSpineControl.Complete += HandleProjectileSpineAnimationEnd;
        m_pSpineControl.Event += HandleProjectileSpineAnimationEvent;
    }

    //-------------------------------------------------------
    public sealed override Transform GetProjctileTransform()
    {
        return SpineRoot;
    }

    //--------------------------------------------------------
    protected void ProtProjectileSpineAnimation(string strAniName, float fPlaySpeed, bool bLoop, UnityAction delFinish)
    {
        if (SpineAnimation.skeleton.Data.FindAnimation(strAniName) != null)
        {
            m_strAnimationCurrent = strAniName;
            TrackEntry pTrackEntry = m_pSpineControl.SetAnimation(0, strAniName, bLoop);
            pTrackEntry.TimeScale = fPlaySpeed;
            m_delFinish = delFinish;
        }
    }

    //-----------------------------------------------------------
    public void HandleProjectileSpineAnimationEnd(TrackEntry trackEntry)
    {
        if (trackEntry.Loop == true) return;
        m_strAnimationCurrent = null;
        m_delFinish?.Invoke();
        m_delFinish = null;
    }

    public void HandleProjectileSpineAnimationEvent(TrackEntry trackEntry, Spine.Event pEvent)
    {
        OnProjectileSpineEvent(m_strAnimationCurrent, pEvent.String);
    }
    //--------------------------------------------------------------
    protected virtual void OnProjectileSpineEvent(string strAnimationName, string strSpineEvent) { }
    
}
