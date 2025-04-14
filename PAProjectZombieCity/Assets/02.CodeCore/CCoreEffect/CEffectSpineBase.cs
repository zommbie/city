using UnityEngine;
using Spine.Unity;
using Spine;

public abstract class CEffectSpineBase : CEffectBase
{    
    [SerializeField]
    private bool LoopPlay = false;
    [SerializeField]
    private SkeletonAnimation SpineAnimation;

    private string m_strFirstAnimationName = null;
    private Spine.AnimationState m_pSpineControl = null;
    private MeshRenderer m_pSpineMeshRenderer = null;
    //----------------------------------------------------------
    protected override void OnEffectInitialize()
    {
        base.OnEffectInitialize();
       
        m_pSpineControl = SpineAnimation.AnimationState;
        m_pSpineMeshRenderer = SpineAnimation.gameObject.GetComponent<MeshRenderer>();
        m_pSpineControl.Complete += HandleShapeAnimationSpineComplete;
        m_pSpineControl.Event += HandleShapeAnimationSpineEvent;

        if (SpineAnimation.AnimationName == null)
        {
            ExposedList<Spine.Animation>.Enumerator it = m_pSpineControl.Data.SkeletonData.Animations.GetEnumerator();
            while (it.MoveNext())
            {
                m_strFirstAnimationName = it.Current.Name;
                break;
            }
        }
        else
        {
            m_strFirstAnimationName = SpineAnimation.AnimationName;
        }
    }

    protected override void OnEffectStartActivate(params object[] aParams)
    {      
        base.OnEffectStartActivate(aParams);
        if (m_strFirstAnimationName != null)
        {
            PrivShapeSpineAnimationStart(m_strFirstAnimationName, GetEffectSpineSpeed(), LoopPlay);
        }
    }

    protected override void OnEffectShowHide(bool bShow)
    {
        base.OnEffectShowHide(bShow);
        m_pSpineMeshRenderer.enabled = bShow;
    }

    protected override void OnEffectTimeScale(float fTimeScale)
    {
        m_pSpineControl.TimeScale = fTimeScale;
    }

    //--------------------------------------------------------------------
    protected void ProtEffectSpineAnimationStart(string strAniName, float fSpeed, bool bLoop)
    {
        PrivShapeSpineAnimationStart(strAniName, fSpeed, bLoop);
    }

    protected void ProtEffectSpineSortOrder(int iSortOder)
    {
        m_pSpineMeshRenderer.sortingOrder = iSortOder;
    }

    protected void ProtEffectSpineFlip(bool bFilp)
    {
        float fScaleX = Mathf.Abs(SpineAnimation.Skeleton.ScaleX);
        if (bFilp)
        {
            SpineAnimation.Skeleton.ScaleX = -fScaleX;
        }
        else
        {
            SpineAnimation.Skeleton.ScaleX = fScaleX;
        }
    }

    //-------------------------------------------------------------------
    private void PrivShapeSpineAnimationEnd(TrackEntry pEntry)
    {
        if (pEntry.Loop == true) return;

        DoEffectEnd();
        OnEffectSpineEnd(pEntry);
    }

    private void PrivShapeSpineAnimationStart(string strAniName, float fPlaySpeed, bool bLoop)
    {      
        TrackEntry pTrackEntry = m_pSpineControl.SetAnimation(0, strAniName, bLoop);
        pTrackEntry.TimeScale = fPlaySpeed;
    }    

    //---------------------------------------------------------------------
    private void HandleShapeAnimationSpineComplete(TrackEntry pEntry)
    {
        PrivShapeSpineAnimationEnd(pEntry);
    }

    private void HandleShapeAnimationSpineEvent(TrackEntry pEntry, Spine.Event pEvent)
    {
        OnEffectSpineEvent(pEntry, pEvent);
    }

    //-------------------------------------------------------------------
    protected virtual void OnEffectSpineEvent(TrackEntry pEntry, Spine.Event pEvent) { }
    protected virtual void OnEffectSpineEnd(TrackEntry pTrackEntry) { }
    protected abstract float GetEffectSpineSpeed();
}
