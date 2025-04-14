using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [개요] 에니메이션 객체에 하나씩 장착되는 제어 객체로 각종 에니메이션 기능 추상화를 구현하였다
// 1) 상하체 에니메이션 및 스켈레탈 + 스프라이트 + 스파인등의 다양한 에니 플렛폼을 그룹으로 관리한다.    
// 2) 메카님을 적용하기 어려운 에니메이션 객체들을 통합 제어하기 위한 레이어이다. 

public interface IEventAnimationNotify
{   
    void IAnimationEnd(string strAniName);  
    void IAnimationKeyEvent(string strAniName, string strKeyName, int iArgument, float fArgument);
    void IAnimationSound(string strSoundName, int iArgument);       // 에니메이션에 사운드가 걸려 있어야 네트워크 상에서 동기화가 쉽다.
}

public struct SAnimationUsage
{
    public string strAniGroupName;
    public bool   bLoop;
    public bool   bAutoIdle;      // 에니메이션 종료시 아이들 복귀
    public float  fBlend;         // 이전 에니메이션과 블랜드 해줌
    public float  fDuration;      // 0이 아닐경우 해당 시간이 되면 강제로 End가 발생한다. 
    public float  fAniSpeed;      // 디폴트 1f
    public int    Layer;          // 
    public UnityAction Finish;  
}

public abstract class CAssistShapeAnimationBase : CAssistUnitBase
{  
    [Serializable]
    public class SAnimationGroupInfo
    {
        public string AnimationName;
        public bool   Idle;
        public List<CShapeAnimationBase> AniInstance = new List<CShapeAnimationBase>();
    }
    [SerializeField]
    private List<SAnimationGroupInfo> AnimationGroup = null;

    private int m_iShapeSortOrder = 0; public int p_ShapeSortOrder { get { return m_iShapeSortOrder; } }
    private int m_iShapeSortOrderOffset = 0;

    private SAnimationGroupInfo     m_pGroupInfoPlay = null;
    private SAnimationGroupInfo     m_pGroupInfoIdle = null;
    private IEventAnimationNotify   m_pAniNotifyProcessor = null;

    private List<CShapeAnimationBase> m_listShapeAnimation = new List<CShapeAnimationBase>();
    private List<CShapeAnimationPartsBase> m_listShapeParts = new List<CShapeAnimationPartsBase>(); //ToDo
    //---------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        m_pAniNotifyProcessor = pOwner as IEventAnimationNotify;
        PrivShapeAnimationCollectAniGroup(m_pAniNotifyProcessor);
    }

    protected override void OnAssistUnitState(CUnitBase.EUnitState eState)
    {
        base.OnAssistUnitState(eState);
        if (eState == CUnitBase.EUnitState.Spawning)
        {
            ProtAssistShapeAnimationIdle();
        }
    }

    protected override void OnAssistUpdate(float fDelta)
    {
        base.OnAssistUpdate(fDelta);
        PrivShapeAnimationUpdate(fDelta);
    }

    //--------------------------------------------------------------------
    protected void ProtAssistShapeAnimationPlay(ref SAnimationUsage rAniUsage)
    {
        SAnimationGroupInfo pAnimationGroup = FindShapeAnimationGroup(rAniUsage.strAniGroupName);
        if (m_pGroupInfoPlay == pAnimationGroup)
        {
            return;
        }

        if (pAnimationGroup != null)
        {
            PrivShpaeAnimationPlay(pAnimationGroup, ref rAniUsage, false);
        }
        else
        {
            //Error!
        }
    }

    protected void ProtAssistShapeAnimationIdle(float fBlend = 0f)
    {
        if (m_pGroupInfoIdle != null)
        {
            SAnimationUsage rAniUsage = new SAnimationUsage();
            rAniUsage.strAniGroupName = m_pGroupInfoIdle.AnimationName;
            rAniUsage.fDuration = 0f;
            rAniUsage.bLoop = true;
            rAniUsage.fAniSpeed = 1f;
            rAniUsage.fBlend = fBlend;
            PrivShpaeAnimationPlay(m_pGroupInfoIdle, ref rAniUsage, true);
        }
    }

    //----------------------------------------------------------------------
    public void SetAssistShapeSortOrder(int iSortOrder)
    {
        PrivShapeAnimationSortOrder(iSortOrder);
    }

    public void SetAssistShapeSortOrderOffset(int iSortOrderOffset)
    {
        m_iShapeSortOrderOffset = iSortOrderOffset;
    }

	//-------------------------------------------------------------------------
    private void PrivShapeAnimationUpdate(float fDelta)
    {
        for (int i = 0; i < m_listShapeAnimation.Count; i++)
        {
            m_listShapeAnimation[i].InterShapeAnimationUpdate(fDelta);
        }
    }

    private void PrivShapeAnimationCollectAniGroup(IEventAnimationNotify pAniNotifyProcessor)
    {
        for (int i = 0; i < AnimationGroup.Count; i++)
        {
            for (int j = 0; j < AnimationGroup[i].AniInstance.Count; j++)
            {
                CShapeAnimationBase pShapeAnimation = AnimationGroup[i].AniInstance[j];
                if (m_listShapeAnimation.Contains(pShapeAnimation) == false)
                {
                    pShapeAnimation.InterShapeAnimationInitilize(pAniNotifyProcessor);
                    m_listShapeAnimation.Add(pShapeAnimation);
                }
            }

            if (AnimationGroup[i].Idle)
            {
                m_pGroupInfoIdle = AnimationGroup[i];
            }
        }
    }

    private void PrivShapeAnimationSortOrder(int iSortOrder)
    {
        for (int i = 0; i < m_listShapeAnimation.Count; i++)
        {
            m_listShapeAnimation[i].InterShapeAnimationSort(iSortOrder, m_iShapeSortOrderOffset);
        }
    }


    private SAnimationGroupInfo FindShapeAnimationGroup(string strAniGroupName)
    {
        SAnimationGroupInfo pFind = null;

        for (int i = 0; i < AnimationGroup.Count; i++)
        {
            if (AnimationGroup[i].AnimationName == strAniGroupName)
            {
                pFind = AnimationGroup[i];
            }
        }

        return pFind;
    }

    private void PrivShpaeAnimationPlay(SAnimationGroupInfo pAniGroupInfo, ref SAnimationUsage rAniUsage, bool bIdle)
    {
        m_pGroupInfoPlay = pAniGroupInfo;
        for (int i = 0; i < pAniGroupInfo.AniInstance.Count; i++)
        {
            pAniGroupInfo.AniInstance[i].InterShapeAnimationPlay(ref rAniUsage, bIdle);
        }
    }
    //--------------------------------------------------------------------------

}
