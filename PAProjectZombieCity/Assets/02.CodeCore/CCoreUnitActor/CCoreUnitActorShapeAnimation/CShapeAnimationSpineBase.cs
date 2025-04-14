using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public abstract class CShapeAnimationSpineBase : CShapeAnimationBase
{
    [System.Serializable]
    private class SSubMaterialInfo 
    {
        public string         SubMaterlalName;
        public List<Material> MaterialInstance;         // 변경 메터리얼은 원본과 동일해야 하며 _Name을 붙여서 구분한다.
    }

    [SerializeField]
    private List<SSubMaterialInfo> SubMaterialList = null;

    private MeshRenderer m_pMeshRenderer = null;
    private bool m_bPlaying = false;
     
    private int  m_iBaseSortOrder = 0;
    private Spine.AnimationState m_pSpineControl = null;
    private SkeletonAnimation    m_pSpineAnimation;
    private CAssistShapeAnimationBase.SAnimationUsage m_rAnimationUsage;

    private Dictionary<string, Spine.Animation> m_mapSpineAnimationTrack = new Dictionary<string, Spine.Animation>();
    //-----------------------------------------------------
    protected override void OnShapeAnimationInitialize(CAssistShapeAnimationBase pAssistOwner)
    {
        base.OnShapeAnimationInitialize(pAssistOwner);
        m_pSpineAnimation = GetComponent<SkeletonAnimation>();
        m_pSpineControl = m_pSpineAnimation.AnimationState;
        m_pMeshRenderer = GetComponent<MeshRenderer>();
        m_iBaseSortOrder = m_pMeshRenderer.sortingOrder;
        ExposedList<Spine.Animation>.Enumerator it = m_pSpineControl.Data.SkeletonData.Animations.GetEnumerator();
        while (it.MoveNext())
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

    protected override float OnShapeAnimationPlay(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage)
    {
        if (m_mapSpineAnimationTrack.ContainsKey(rAniUsage.strAniName) == false)
        {
            return 0;
        }
        m_rAnimationUsage = rAniUsage;
        return PrivShapeSpineAnimationStart(ref rAniUsage);
    }

    protected override void OnShapeAnimationSortOrder(int iSortOrderOffset, bool bReset)
    {
        PrivShapeSpineSortOrder(iSortOrderOffset, bReset);
    }

    protected override void OnShapeAnimationMaterialRefresh(MaterialPropertyBlock pRefreshPropertyBlock)
    {
        PrivShapeSpineRefreshPropertyBlock(pRefreshPropertyBlock);
    }

    public override sealed bool IsPlaying()
    {
        return m_bPlaying;
    }

    protected override void OnShapeAnimationChangeSpeed(float fAniSpeed)
    {
        m_pSpineControl.TimeScale = fAniSpeed;      
    }
    //------------------------------------------------------------
    public int GetShapeAnimationSpineSortOrder() { return m_pMeshRenderer.sortingOrder; }
    //-----------------------------------------------------
    protected void ProtShapeAnimationSpineFlipX(bool bLeft, float fScale = 1f)
    {
        if (bLeft)
        {
            m_pSpineAnimation.skeleton.ScaleX = -fScale;
        }
        else
        {
            m_pSpineAnimation.skeleton.ScaleX = fScale;
        }
    }

    protected void ProtShapeAnimationSpineMaterialChange(string strSubMaterialName)
    {
        SSubMaterialInfo pSubMaterialInfo = FindShapeSpineSubMaterial(strSubMaterialName);
        if (pSubMaterialInfo != null)
        {
            PrivShapeSpineChangeSubMaterialList(pSubMaterialInfo);
        }
    }
   
    protected void ProtShapeAnimationSpineMaterialReset(bool bMain, bool bSlot)
    {
        if (bMain)
        {
            m_pSpineAnimation.CustomMaterialOverride.Clear();
        }
        if (bSlot)
        {
            m_pSpineAnimation.CustomSlotMaterials.Clear();
        }
    }

    //---------------------------------------------------------------
    private void PrivShapeSpineSortOrder(int iSortOrderOffset, bool bReset)
    {
        if (bReset)
        {
            m_pMeshRenderer.sortingOrder = iSortOrderOffset;
        }
        else
        {
            m_pMeshRenderer.sortingOrder = m_iBaseSortOrder + iSortOrderOffset; 
        }
    }

    private float  PrivShapeSpineAnimationStart(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage)
    {
        m_bPlaying = true;        
        TrackEntry pTrackEntry = m_pSpineControl.SetAnimation(rAniUsage.Layer, rAniUsage.strAniName, rAniUsage.bLoop);
        pTrackEntry.TimeScale = rAniUsage.fAniSpeed;
        return pTrackEntry.AnimationTime;
    }
  
    private void PrivShapeSpineAnimationEnd(TrackEntry pEntry)
    {
        if (pEntry.Loop == true) return;

        m_bPlaying = true;
        m_rAnimationUsage.AniFinish?.Invoke();      
        m_pAssistOwner.InterAssistShapeAnimationEnd(m_rAnimationUsage.strAniName, this, m_rAnimationUsage.bAutoIdle);
        OnShapeAnimationSpineFinish(pEntry);
    }

    private void PrivShapeSpineChangeMaterial(Material pChangeMaterial)
    {
        string strChangeMatName = pChangeMaterial.name;
        int iUnderBarIndex = strChangeMatName.LastIndexOf("_");
        strChangeMatName = strChangeMatName.Remove(iUnderBarIndex, strChangeMatName.Length - iUnderBarIndex);

        Material[] aMaterialShare = m_pMeshRenderer.sharedMaterials;
        for (int i = 0; i < aMaterialShare.Length; i++)
        {          
            if (strChangeMatName == aMaterialShare[i].name)
            {
                m_pSpineAnimation.CustomMaterialOverride.Add(aMaterialShare[i], pChangeMaterial);
                break;
            }
        }
    }

    private void PrivShapeSpineChangeSubMaterialList(SSubMaterialInfo pSubMaterialInfo)
    {
        for (int i = 0; i < pSubMaterialInfo.MaterialInstance.Count; i++)
        {          
            PrivShapeSpineChangeMaterial(pSubMaterialInfo.MaterialInstance[i]);
        }
    }

    private void PrivShapeSpineRefreshPropertyBlock(MaterialPropertyBlock pPropertyBlock)
    {
        Material[] aMaterialShare = m_pMeshRenderer.sharedMaterials;
        for (int i = 0; i < aMaterialShare.Length; i++)
        {
            m_pMeshRenderer.SetPropertyBlock(pPropertyBlock, i);
        }
    }

    private SSubMaterialInfo FindShapeSpineSubMaterial(string strSubMaterialName)
    {
        SSubMaterialInfo pFindSubMaterial = null;
        for (int i = 0; i < SubMaterialList.Count; i++)
        {
            if (SubMaterialList[i].SubMaterlalName == strSubMaterialName)
            {
                pFindSubMaterial = SubMaterialList[i];
                break;
            }
        }

        return pFindSubMaterial;
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
        PrivShapeSpineAnimationEnd(pEntry);
    }

    private void HandleShapeAnimationSpineEvent(TrackEntry pEntry, Spine.Event pEvent)
    {
        m_pAssistOwner.InterAssistShapeAnimationEvent(this, pEvent.Data.Name, pEvent.Data.Int, pEvent.Float);
    }
    //----------------------------------------------------------------------
    protected virtual void OnShapeAnimationSpineFinish(TrackEntry pEntry) { }
  
}
