using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(Canvas))]
public abstract class CUIWidgetSpineController : CUIWidgetBase
{
    [SerializeField]
    private SkeletonGraphic SpineInstance = null;
    [SerializeField]
    private Transform       SpineRootPivot = null;

    private int m_iBaseSortOrder = 0;
    private int m_iUIFrameSortOrder = 0;
    private bool m_bInitialize = false;
    private bool m_bFlipX = false;                  public bool IsFlipX { get { return m_bFlipX; } }
    private bool m_bFlipY = false;                  public bool IsFlipY { get { return m_bFlipY; } }
    private string m_strPlayAnimationName;
    private Material m_pMaterialCopy = null;
    private Canvas m_pSpineCanvas = null;
    private Spine.AnimationState m_pSpineControl = null;
    private Dictionary<string, Spine.Animation> m_mapTrackInstance = new Dictionary<string, Spine.Animation>();
    //---------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
        PrivSpineControllerInitialize();
    }

    protected override void OnUIEntryChangeOrder(int iOrder)
    {
        base.OnUIEntryChangeOrder(iOrder);
        m_iUIFrameSortOrder = iOrder;
        ProtSpineControllerSortOrder(0);        
    }

    //---------------------------------------------------------
    protected void ProtSpineControllerAnimation(string strAniName, float fPlaySpeed = 1f,  bool bLoop = false)
    {
        if (m_mapTrackInstance.ContainsKey(strAniName) == false) return;

        m_strPlayAnimationName = strAniName;
        TrackEntry pTrackEntry = m_pSpineControl.SetAnimation(0, strAniName, bLoop);
        pTrackEntry.TimeScale = fPlaySpeed;
    }

    protected void ProtSpineControllerColor(Color rColor)
    {
        SpineInstance.Skeleton.SetColor(rColor);
    }

    protected void ProtSpineControllerSortOrder(int iSortOderOffset)
    {
        if (m_pSpineCanvas == null) return;

        int iSortOrder = m_iBaseSortOrder + iSortOderOffset + m_iUIFrameSortOrder;
        m_pSpineCanvas.sortingOrder = iSortOrder;
    }

    protected void ProtSpineControllerMaterialValue(string strPropertyName, float fValue)
    {
        m_pMaterialCopy.SetFloat(strPropertyName, fValue);       
    }

    protected Transform GetSpineControllerRootPivot()
    {
        return SpineRootPivot;
    }

    protected void ProtSpineControllerFlipX(bool bFlipX)
    {
        if (bFlipX)
        {
            SpineInstance.Skeleton.ScaleX = -1f;
            m_bFlipX = true;
        }
        else
        {
            SpineInstance.Skeleton.ScaleX = 1f;
            m_bFlipX = false;
        }
    }
    //-----------------------------------------------------
  
    private void PrivSpineControllerInitialize()
    {
        if (m_bInitialize) return;
        m_bInitialize = true;

        m_pSpineCanvas = GetComponent<Canvas>();
        if (m_pSpineCanvas != null)
        {
            m_iBaseSortOrder = m_pSpineCanvas.sortingOrder;
        }

        SpineInstance.Initialize(true);

        m_pMaterialCopy = Instantiate(SpineInstance.material);
        SpineInstance.material = m_pMaterialCopy;
        SpineInstance.SetAllDirty();

        m_pSpineControl = SpineInstance.AnimationState;
        m_pSpineControl.Start += HandleSpineEventStart;
        m_pSpineControl.End += HandleSpineEventEnd;
        m_pSpineControl.Dispose += HandleSpineEventDispose;
        m_pSpineControl.Complete += HandleSpineEventComplete;
        m_pSpineControl.Event += HandleSpineEventCustom;

        ExposedList<Spine.Animation>.Enumerator it = m_pSpineControl.Data.SkeletonData.Animations.GetEnumerator();
        while (it.MoveNext())
        {
            m_mapTrackInstance.Add(it.Current.Name, it.Current);
        }
    }
    //----------------------------------------------------------
    private void HandleSpineEventStart(TrackEntry trackEntry)
    {

    }
 

    private void HandleSpineEventEnd(TrackEntry trackEntry)
    {

    }

    private void HandleSpineEventDispose(TrackEntry trackEntry)
    {

    }

    private void HandleSpineEventComplete(TrackEntry trackEntry)
    {
        if (m_strPlayAnimationName != trackEntry.Animation.Name) return;
        OnSpineControllerAnimationEnd(trackEntry.Animation.Name);
    }

    private void HandleSpineEventCustom(TrackEntry trackEntry, Spine.Event eventType)
    {

    }

    ////---------------------------------------------------------------------------------
    protected virtual void OnSpineControllerAnimationEnd(string strAniName) { }
	
}
