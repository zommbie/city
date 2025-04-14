using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBuffBase : CObjectInstancePoolBase<CBuffBase>
{
    public struct SBuffRefresh          // 외부 UI참조용 
    {
        public uint BuffID;
        public float RemainTime;        // 버프의 남은 시간
        public int StackCount;          // 현재 중첩 갯수
        public int BuffType;            // 패시브 버프, 쇼우 버프등 컨텐츠에서 구분할 수 있는 형태 
    }

    public class SBuffAttribute
    {
        public int BuffMergeGroup;
        public float EventOverTime = 0f;    // 이 주기 마다 이벤트 생성
        public int StackUpCount = 1;        // 스텍업을 할때마다 증가하는 값
        public int StackUpMax = 1;          // 스텍업의 최고 갯수

        public bool MergeExclusive = false;      // 기존 버프와는 별도로 생성된다.
        public bool MergeStackUp = false;        // 기존 버프의 스텍 카운트가 올라간다
        public bool MergePowerUp = false;        // 기존 버프의 파워(위력)이 증가한다.
        public bool MergeTimeReset = false;      // 기존 버프의 타이머가 리셋된다.
        public bool MergeTimePlus = false;       // 기존 버프의 지속시간이 병합 버프의 지속시간만큼 더해진다. 
    }
       
    private float m_fDuration = 0;
    private float m_fEventOverTimeCurrent = 0;
    private float m_fBuffPower = 0;                 public float GetBuffPower() { return m_fBuffPower; }    public void SetBuffPower(float fPower) { m_fBuffPower = fPower; }
    private bool  m_bBuffActive = false;            public bool  IsBuffActive { get { return m_bBuffActive; } }
    private int   m_iEventOverTimeCount = 0;

    private SBuffRefresh m_rBuffRefresh     = new SBuffRefresh();           public SBuffRefresh GetBuffRefreshInfo() { return m_rBuffRefresh; }
    private SBuffAttribute m_pBuffAttribute = new SBuffAttribute();         public SBuffAttribute GetBuffAttribute() { return m_pBuffAttribute; }

    private CAssistUnitBuffBase m_pBuffOnwerAssist = null;
    private IUnitEventBuff      m_pBuffOrigin = null;         //  버프의 출처
    private IUnitEventBuff      m_pBuffOwner = null;          //  버프를 소유하고 있는 주체  
    private CBuffConditionList  m_pBuffConditionList = null;  //  버프가 발동되기 위한 조건 (무적상태라 실패, 화염 면역등)
    private CBuffTaskEventList  m_pBuffTaskEventList = null;  //  이벤트에 따른 버프의 실제 작동 내용
    //--------------------------------------------------------------------------------------
    protected override void OnObjectPoolDeactivate()
    {
        m_pBuffTaskEventList.InterBuffTaskReset();
        PrivBuffReset();
    }

    //-------------------------------------------------------------------------------------
    internal void InterBuffUpdate(float fDeltaTime)
    {
        UpdateBuffEventOverTime(fDeltaTime);
        UpdateBuffDuration(fDeltaTime);
    }

    internal int InterBuffStart(CAssistUnitBuffBase pBuffOwnerAssist, IUnitEventBuff pOnwer, IUnitEventBuff pOrigin, float fDuration, float fBuffPower)
    {
        PrivBuffReset();
        int eResult = m_pBuffConditionList.InterBuffConditon(m_pBuffOrigin, m_pBuffOwner);
        if (eResult == 0)
        {
            m_pBuffOwner = pOnwer;
            m_pBuffOrigin = pOrigin;
            m_pBuffOnwerAssist = pBuffOwnerAssist;
            m_fDuration = fDuration;

            PrivBuffTaskEventStart();
        }

        return eResult;
    }

    internal void InterBuffErase(CBuffBase pFromBuff)
    {
        PrivBuffTaskEventErase(pFromBuff);
        PrivBuffEventEnd();
    }

    internal void InterBuffPowerUp(CBuffBase pFromBuff, float fPowerValue)
    {
        m_fBuffPower += fPowerValue;
        OnBuffEventPowerUP(pFromBuff, fPowerValue);
    }

    internal void InterBuffStackUp(CBuffBase pFromBuff, int iStackValue)
    {
        m_rBuffRefresh.StackCount += iStackValue;
        if (m_rBuffRefresh.StackCount >= m_pBuffAttribute.StackUpMax)
        {
            m_rBuffRefresh.StackCount = m_pBuffAttribute.StackUpMax;
        }
        OnBuffEventStackUP(pFromBuff, m_rBuffRefresh.StackCount);
    }

    internal void InterBuffTimeUp(CBuffBase pFromBuff, float fTimeValue)
    {
        m_rBuffRefresh.RemainTime += fTimeValue;
        OnBuffEventTimeUP(pFromBuff, fTimeValue);
    }

    internal void InterBuffTimeReset(CBuffBase pFromBuff)
    {
        m_rBuffRefresh.RemainTime = m_fDuration;
        OnBuffEventTimeReset(pFromBuff);
    }

    internal void InterBuffEvent(CBuffTaskBase.EBuffTaskEvent eTaskEvent, int iArg, float fArg, params object[] aParams)
    {
        m_pBuffTaskEventList.InterBuffTaskEvent(this, eTaskEvent, m_pBuffOwner, m_pBuffOrigin, iArg, fArg, aParams);
    }

    //---------------------------------------------------------------------------------------
    private void PrivBuffReset()
    {
        m_rBuffRefresh.RemainTime = 0;
        m_fEventOverTimeCurrent = 0;
        m_fDuration = 0;
        m_iEventOverTimeCount = 0;
    }

    private void UpdateBuffDuration(float fDelta)
    {
        if (m_fDuration != 0)
        {
            m_rBuffRefresh.RemainTime -= fDelta;
            if (m_rBuffRefresh.RemainTime <= 0)
            {
                PrivBuffTaskEventExpire();
                PrivBuffEventEnd();
            }
        }
    }

    private void UpdateBuffEventOverTime(float fDelta)
    {
        if (m_pBuffAttribute.EventOverTime == 0) return;

        m_fEventOverTimeCurrent += fDelta;

        if (m_fEventOverTimeCurrent >= m_pBuffAttribute.EventOverTime)
        {
            m_fEventOverTimeCurrent = 0f;
            m_iEventOverTimeCount++;
            PrivBuffTaskEvent(CBuffTaskBase.EBuffTaskEvent.EventOverTime, m_iEventOverTimeCount, m_pBuffAttribute.EventOverTime);
        }
    }

    private void PrivBuffTaskEventStart()
    {       
        PrivBuffTaskEvent(CBuffTaskBase.EBuffTaskEvent.Start);

        m_bBuffActive = true;
        m_rBuffRefresh.RemainTime = m_fDuration;
        OnBuffEventStart();
    }

    private void PrivBuffTaskEventExpire()
    {
        PrivBuffTaskEvent(CBuffTaskBase.EBuffTaskEvent.Expire);
        OnBuffEventExpire();
    }

    private void PrivBuffTaskEventErase(CBuffBase pFromBuff)
    {
        PrivBuffTaskEvent(CBuffTaskBase.EBuffTaskEvent.Erase);
        OnBuffEventErase(pFromBuff);
    }

    private void PrivBuffEventEnd()
    {
        m_bBuffActive = false;
        PrivBuffTaskEvent(CBuffTaskBase.EBuffTaskEvent.End);
        m_pBuffOnwerAssist.InterAssistBuffEnd(this);
        OnBuffEventEnd();
    }

    private void PrivBuffTaskEvent(CBuffTaskBase.EBuffTaskEvent eEventType, int iArg = 0, float fArg = 0, params object[] aParams)
    {
        m_pBuffTaskEventList.InterBuffTaskEvent(this, eEventType, m_pBuffOwner, m_pBuffOrigin, iArg, fArg, aParams);
    }
    //-----------------------------------------------------------------------------------
    protected virtual void OnBuffEventStart() { }
    protected virtual void OnBuffEventExpire() { }
    protected virtual void OnBuffEventErase(CBuffBase pFromBuff) { }
    protected virtual void OnBuffEventPowerUP(CBuffBase pFromBuff, float fPowerValue) { }
    protected virtual void OnBuffEventStackUP(CBuffBase pFromBuff, int iStackCount) { }
    protected virtual void OnBuffEventTimeUP(CBuffBase pFromBuff, float fTimeValue) { }
    protected virtual void OnBuffEventTimeReset(CBuffBase pFromBuff) { }
    protected virtual void OnBuffEventEnd() { }
    //-------------------------------------------------------------------------------------  
    public SBuffRefresh GetBuffRefresh() { return m_rBuffRefresh; }
    public uint GetBuffID()              { return m_rBuffRefresh.BuffID; }
    public int  GetBuffInstanceID()      { return GetObjectInstanceID(); }
    //--------------------------------------------------------------------------------------
    public void InstanceBuffInfo(SBuffAttribute pBuffAttribute, SBuffRefresh rRefreshInfo) { m_pBuffAttribute = pBuffAttribute;  m_rBuffRefresh = rRefreshInfo; }
    public void InstanceBuffConditionList(CBuffConditionList pBuffConditionList) { m_pBuffConditionList = pBuffConditionList; }
    public void InstanceBuffTaskEventList(CBuffTaskEventList pBuffTaskEventList) { m_pBuffTaskEventList = pBuffTaskEventList; }

    protected sealed override int GetObjectInstanceID(){ return base.GetObjectInstanceID();}
}
