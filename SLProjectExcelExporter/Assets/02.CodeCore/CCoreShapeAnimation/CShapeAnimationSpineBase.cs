using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public abstract class CShapeAnimationSpineBase : CShapeAnimationBase
{
    [SerializeField]
    private SkeletonAnimation SpineAnimation;
    [SerializeField]
    private int SortOrderOffsetRate = 0;

    private Spine.AnimationState m_pSpineControl = null;
    private MeshRenderer m_pMeshRenderer = null;
    private SAnimationUsage m_rAnimationUsage;
    private Dictionary<string, Spine.Animation> m_mapSpineAnimationTrack = new Dictionary<string, Spine.Animation>();
    //-----------------------------------------------------
    protected override void OnShapeAnimationInitialize(IEventAnimationNotify pAniNotifyProcessor)
    {
        base.OnShapeAnimationInitialize(pAniNotifyProcessor);
        m_pSpineControl = SpineAnimation.AnimationState;
        m_pMeshRenderer = GetComponent<MeshRenderer>();

        ExposedList<Spine.Animation>.Enumerator it = m_pSpineControl.Data.SkeletonData.Animations.GetEnumerator();
        while(it.MoveNext())
        {
            m_mapSpineAnimationTrack[it.Current.Name] = it.Current;
        }

        m_pSpineControl.Start += HandleShapeAnimationSpineStart;
        m_pSpineControl.Interrupt += HandleShapeAnimationInterrupt;
        m_pSpineControl.End += HandleShapeAnimationSpineEnd;
        m_pSpineControl.Dispose += HandleShapeAnimationSpineDispose;
        m_pSpineControl.Complete += HandleShapeAnimationSpineComplete;
        m_pSpineControl.Event += HandleShapeAnimationSpineEvent;
    }

    protected override void OnShapeAnimationUpdate(float fDelta)
    {
        base.OnShapeAnimationUpdate(fDelta);

    }

    protected override void OnShapeAnimationPlay(ref SAnimationUsage rAniUsage, bool bIdle)
    {       
        if (m_mapSpineAnimationTrack.ContainsKey(rAniUsage.strAniGroupName) == false)
        {
            return;
        }

        m_rAnimationUsage = rAniUsage;
        m_pSpineControl.SetAnimation(rAniUsage.Layer, rAniUsage.strAniGroupName, rAniUsage.bLoop);       
    }

    protected override void OnShapeAnimationSortOrder(int iSortOrder, int iSortOrderOffset)
    {
        int iSortOrderApply = iSortOrder + (iSortOrderOffset * SortOrderOffsetRate);
        PrivShapeSpineSortOrder(iSortOrderApply);
    }

    //-----------------------------------------------------
    protected void ProtShapeAnimationFlipX(bool bLeft, float fScale = 1f)
    {
        if (bLeft)
        {
            SpineAnimation.skeleton.ScaleX = -fScale;
        }
        else
        {
            SpineAnimation.skeleton.ScaleX = fScale;
        }
    }

    //---------------------------------------------------------------
    private void PrivShapeSpineSortOrder(int iSortOrder)
    {
        m_pMeshRenderer.sortingOrder = iSortOrder;
    }


    //---------------------------------------------------------------
    private void HandleShapeAnimationSpineStart(TrackEntry pEntry)
    {

    }

    private void HandleShapeAnimationInterrupt(TrackEntry pEntry)
    {

    }

    private void HandleShapeAnimationSpineEnd(TrackEntry pEntry)
    {
        if(pEntry.Loop == false)
        {
            
        }
    }

    private void HandleShapeAnimationSpineDispose(TrackEntry pEntry)
    {

    }

    private void HandleShapeAnimationSpineComplete(TrackEntry pEntry)
    {
        if (pEntry.Loop == false)
        {
            m_rAnimationUsage.Finish?.Invoke();
            m_pAniNotifyProcessor.IAnimationEnd(pEntry.Animation.Name);
            OnShapeAnimationSpineFinish(pEntry);
        }
    }

    private void HandleShapeAnimationSpineEvent(TrackEntry pEntry, Spine.Event pEvent)
    {
        m_pAniNotifyProcessor.IAnimationKeyEvent(pEntry.Animation.Name, pEvent.Data.Name, pEvent.Data.Int, pEvent.Float);
        OnShapeAnimationSpineEvent(pEntry, pEvent);
    }
    //----------------------------------------------------------------------
    protected virtual void OnShapeAnimationSpineFinish(TrackEntry pEntry) { }
    protected virtual void OnShapeAnimationSpineEvent(TrackEntry pEntry, Spine.Event pEvent) { }
}
