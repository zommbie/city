using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [개요] 에니메이션 객체에 하나씩 장착되는 제어 객체로 각종 에니메이션 기능 추상화를 구현
// 1) 상하체 에니메이션 및 스켈레탈 + 스프라이트 + 스파인등의 다양한 에니 플렛폼을 그룹으로 관리  
 
public abstract class CAssistShapeAnimationBase : CAssistUnitActorBase
{
    public struct SAnimationUsage
    {
        public string strAniName;
        public bool bLoop;
        public bool bAutoIdle;       // 에니메이션 종료시 아이들 복귀
        public float fBlendTime;     // 이전 에니메이션과 블랜드 시간
        public float fDuration;      // 0 이 아닐경우 해당 시간이 되면 강제로 End가 발생한다. 
        public float fAniSpeed;      // 디폴트 1f
        public float fAniSpeedOrigin;        
        public int Layer;            // 
        public UnityAction AniFinish;
    }

    [Serializable]
    public class SAnimationGroupInfo
    {
        public string AnimationName;
        public bool Idle;
        public bool Loop;
        public bool AutoIdle;
        public float Duration;
        public float BlendTime;
        public int MainAniIndex;   // 대표 에니메이션 객체 인덱스
        public List<CShapeAnimationBase> AniInstance = new List<CShapeAnimationBase>();
    }
    [SerializeField]
    private List<SAnimationGroupInfo> AnimationGroup = null;

    private SAnimationGroupInfo     m_pGroupInfoPlay = null;
    private SAnimationGroupInfo     m_pGroupInfoIdle = null;
    private SAnimationGroupInfo     m_pGroupDelayWait = null;
    private SAnimationUsage         m_rAnimationUsageCache = new SAnimationUsage();

    private bool m_bPlaying = false;                public bool IsPlaying { get { return m_bPlaying; } }
    private bool m_bDelayWait = false;              public bool IsDelayWait { get { return m_bDelayWait; } }
    private int m_iSortOrder = 0;                   public int  GetAssistAniSortOrder() { return m_iSortOrder; }

    private float m_fAnimationScale = 1f;
    private float m_fDelayWait = 0f;
    private float m_fDelayWaitCurrent = 0;

    private List<CShapeAnimationBase>      m_listShapeAnimation = new List<CShapeAnimationBase>();
    private List<CShapeAnimationPartsBase> m_listShapeParts = new List<CShapeAnimationPartsBase>(); //ToDo
    //---------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitActorBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        PrivShapeAnimationInitializeShapeAnimation();
        PrivShapeAnimationChangeAniSpeed(1f);       
    }


    protected override void OnAssistUpdate(float fDelta)
    {
        base.OnAssistUpdate(fDelta);
        float fDeltaScale = fDelta * m_fAnimationScale;
        UpdateShapeAnimation(fDeltaScale);
        UpdateShapeAnimationDelayWait(fDeltaScale);
    }
    
    //----------------------------------------------------------------------
    internal void InterAssistShapeAnimationEnd(string strAniGroupName, CShapeAnimationBase pEndAnimation, bool bIdle)
    {
        SAnimationGroupInfo pAnimationGroup = FindShapeAnimationGroup(strAniGroupName);
        if (pAnimationGroup != null)
        {
            bool bPlaying = false;
            for (int i = 0; i < pAnimationGroup.AniInstance.Count; i++)
            {
                if (pAnimationGroup.AniInstance[i].IsPlaying())
                {
                    bPlaying = true;
                    break;
                }
            }

            if (bPlaying == false)
            {
                PrivShpaeAnimationEnd();
            }

            if (bIdle)
            {
                ProtAssistShapeAnimationIdle();
            }
        }
    }

    internal void InterAssistShapeAnimationEvent(CShapeAnimationBase pEventAnimation, string strEventName, int iArg, float fArg)
    {
        OnAssistShapeAnimationEvent(strEventName, iArg, fArg);
    }
    //-------------------------------------------------------------
    public void DoAssistShapeAnimationScale(float fAniScale)
    {
        PrivShapeAnimationChangeAniSpeed(fAniScale);
        OnAssistShapeAnimationSpeed(fAniScale);
    }
   
    //--------------------------------------------------------------------
    protected void ProtAssistShapeAnimationPlay(string strAnimationName, float fPlaySpeed, float fPlayDelay, UnityAction delFinish)
    {       
        SAnimationGroupInfo pAnimationGroup = FindShapeAnimationGroup(strAnimationName);
        if (m_pGroupInfoPlay == pAnimationGroup)
        {
            return;
        }
     
        if (pAnimationGroup != null)
        {
            PrivShapeAnimationDelayWaitCancel();
            if (fPlayDelay == 0)
            {
                PrivShapeAnimationPlayGroup(pAnimationGroup, fPlaySpeed, delFinish);
            }
            else
            {
                PrivShapeAnimationDelayWait(pAnimationGroup, fPlaySpeed, fPlayDelay, delFinish);
            }
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
            rAniUsage.strAniName = m_pGroupInfoIdle.AnimationName;
            rAniUsage.fDuration = 0f;
            rAniUsage.bLoop = true;
            rAniUsage.fAniSpeed = m_fAnimationScale;
            rAniUsage.fBlendTime = fBlend;
            PrivShpaeAnimationPlayUsage(m_pGroupInfoIdle, ref rAniUsage);
        }
    }

    protected void ProtAssistShapeAnimationSortOrderChange(int iSortOrderOffser, bool bReset = false)
    {
        PrivShapeAnimationSortOrder(iSortOrderOffser, bReset);
    }

 	//-------------------------------------------------------------------------
    private void UpdateShapeAnimation(float fDelta)
    {
        for (int i = 0; i < m_listShapeAnimation.Count; i++)
        {
            m_listShapeAnimation[i].InterShapeAnimationUpdate(fDelta);
        }
    }

    private void UpdateShapeAnimationDelayWait(float fDelta)
    {
        if (m_bDelayWait == false) return;

        m_fDelayWaitCurrent += fDelta;
        if (m_fDelayWaitCurrent >= m_fDelayWait)
        {
            PrivShapeAnimationDelayWaitStart();
        }
    }

    private void PrivShapeAnimationInitializeShapeAnimation()
    {
        GetComponentsInChildren(true, m_listShapeAnimation);

        for (int i = 0; i < m_listShapeAnimation.Count; i++)
        {
            m_listShapeAnimation[i].InterShapeAnimationInitilize(this);
        }

        for (int i = 0; i < AnimationGroup.Count; i++)
        {           
            if (AnimationGroup[i].Idle)
            {
                m_pGroupInfoIdle = AnimationGroup[i];
            }
        }
    }

    private void PrivShapeAnimationSortOrder(int iSortOrder, bool bReset)
    {
        m_iSortOrder = iSortOrder;
        for (int i = 0; i < m_listShapeAnimation.Count; i++)
        {
            m_listShapeAnimation[i].InterShapeAnimationSort(iSortOrder, bReset);
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

    private void PrivShpaeAnimationPlayUsage(SAnimationGroupInfo pAniGroupInfo, ref SAnimationUsage rAniUsage)
    {
        m_bPlaying = true;
        m_pGroupInfoPlay = pAniGroupInfo;
        float fAniLength = 0;
        for (int i = 0; i < pAniGroupInfo.AniInstance.Count; i++)
        {
           float fLength = pAniGroupInfo.AniInstance[i].InterShapeAnimationPlay(ref rAniUsage);
           if (fLength < fAniLength)
            {
                fAniLength = fLength;
            }
        }

        OnAssistShapeAnimationStart(pAniGroupInfo.AnimationName, fAniLength);
    }
    
    private void PrivShapeAnimationPlayGroup(SAnimationGroupInfo pAnimationGroup, float fPlaySpeed, UnityAction delFinish)
    {
        SAnimationUsage rAniUsage = new SAnimationUsage();
        rAniUsage.AniFinish = delFinish;
        rAniUsage.fAniSpeed = fPlaySpeed;
        rAniUsage.fDuration = pAnimationGroup.Duration;
        rAniUsage.fBlendTime = pAnimationGroup.BlendTime;
        rAniUsage.bAutoIdle = pAnimationGroup.AutoIdle;
        rAniUsage.strAniName = pAnimationGroup.AnimationName;
        rAniUsage.bLoop = pAnimationGroup.Loop;
        PrivShpaeAnimationPlayUsage(pAnimationGroup, ref rAniUsage);
    }

    private void PrivShapeAnimationDelayWait(SAnimationGroupInfo pAnimationGroup, float fPlaySpeed, float fPlayDelay, UnityAction delFinish)
    {
        m_rAnimationUsageCache.AniFinish = delFinish;
        m_rAnimationUsageCache.fAniSpeed = fPlaySpeed;
        m_rAnimationUsageCache.fDuration = pAnimationGroup.Duration;
        m_rAnimationUsageCache.fBlendTime = pAnimationGroup.BlendTime;
        m_rAnimationUsageCache.bAutoIdle = pAnimationGroup.AutoIdle;
        m_rAnimationUsageCache.strAniName = pAnimationGroup.AnimationName;
        m_rAnimationUsageCache.bLoop = pAnimationGroup.Loop;

        m_pGroupDelayWait = pAnimationGroup;
        m_bDelayWait = true;
        m_fDelayWait = fPlayDelay;

        ProtAssistShapeAnimationIdle();
    }

    private void PrivShapeAnimationDelayWaitStart()
    {
        PrivShpaeAnimationPlayUsage(m_pGroupDelayWait, ref m_rAnimationUsageCache);
        PrivShapeAnimationDelayWaitCancel();
    }

    private void PrivShapeAnimationDelayWaitCancel()
    {
        m_bDelayWait = false;
        m_fDelayWait = 0;
        m_fDelayWaitCurrent = 0;
        m_pGroupDelayWait = null;
    }

    private void PrivShpaeAnimationEnd()
    {
        m_bPlaying = false;
        OnAssistShapeAnimationEnd();
    }

    private void PrivShapeAnimationChangeAniSpeed(float fAniSpeed)
    {
        m_fAnimationScale = fAniSpeed;
       
        for (int i = 0; i < AnimationGroup.Count; i++)
        {
            for (int j = 0; j < AnimationGroup[i].AniInstance.Count; j++)
            {
                CShapeAnimationBase pAniInstance = AnimationGroup[i].AniInstance[j];
                pAniInstance.InterShapeAnimationChangeSpeed(fAniSpeed);
            }
        }
    }
    //--------------------------------------------------------------------------
    protected virtual void OnAssistShapeAnimationStart(string strAniName, float fDuration) { }
    protected virtual void OnAssistShapeAnimationEnd() { }
    protected virtual void OnAssistShapeAnimationEvent(string strEventName, int iArg, float fArg) { }
    protected virtual void OnAssistShapeAnimationSpeed(float fAniSpeed) { }
}
