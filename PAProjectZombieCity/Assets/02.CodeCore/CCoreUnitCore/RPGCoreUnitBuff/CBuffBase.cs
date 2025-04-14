using System.Collections;
using System.Collections.Generic;

public abstract class CBuffBase : CObjectInstancePoolBase<CBuffBase>
{
    public enum EMargePriority
    {
        None,
        Exclusive,  // 각자의 버프가 별개로 취급된다.
        Exist,      // 기존에 있는 버프로 병합된다.
        New,        // 새로운 버프로 병합된다.
        Time,       // 시간이 높은쪽으로 병합된다.
        Power,      // 파워가 높은쪽으로 병합된다.
        Stack,      // 버프 스택이 높은쪽으로 병합된다. 
    }

    public struct SBuffRefresh          // 외부 UI참조용 
    {
        public uint     BuffID;
        public float    RemainTime;          // 버프의 남은 시간
        public int      StackCount;          // 현재 중첩 갯수
       
    }

    public struct SBuffAttribute
    {
        public uint         hBuffID;
        public int          BuffInstanceType;                           // 실드등 특정 상호작용을 하는 버프등의 인스턴스 구분 
        public int          BuffType;
        public int          BuffMergeGroup;                             // 이 그룹간 머지가 발생한다.
        public List<int>    BuffAttributeList;                          // 버프 속성 : 공격력 상승, 방어력 상승등, 
        public float        EventOverTime;                              // 이 주기 마다 이벤트 생성
        public float        Duration;
        public float        MaxDuration;                                // 최대 버프 시간      
        public float        DefaultPower;                               // 스킬 등에서 파워를 0 입력 했을때 사용할 고정값
    }

    public struct SBuffMerge
    {       
        public int StackMax;                // 스텍업의 최고 갯수
        public int StackCount;              // 초기 버프 스택

        public EMargePriority MergePriority;
        public bool MergeStackUp;             // 기존 버프의 스텍 카운트가 올라간다
        public bool MergePowerUp;             // 기존 버프의 파워가 더해진다.       
        public bool MergeTimeReset;           // 기존 버프의 타이머가 리셋된다.
        public bool MergeTimeUp;              // 기존 버프의 지속시간이 병합 버프의 지속시간만큼 더해진다. 
    }

    private float m_fDuration = 0;                        public float GetBuffDuration() { return m_fDuration; }
    private float m_fDurationCurrent = 0;                 public float GetBuffRemainTime() { return m_fDurationCurrent; } 
    private float m_fEventOverTimeCurrent = 0;
    private int   m_iEventOverTimeCount = 0;
    private int   m_iStackCount = 0;

    private int   m_eBuffType = 0;                        public int   GetBuffType() { return m_eBuffType; } 
    private float m_fBuffPower = 0;                       public float GetBuffPower() { return m_fBuffPower; }
    private bool  m_bBuffActive = false;                  public bool  IsBuffActive { get { return m_bBuffActive; } }
    private uint  m_hBuffID = 0;                          public uint  GetBuffTableID() { return m_hBuffID; }

    private SBuffRefresh        m_rBuffRefresh   = new SBuffRefresh();      public SBuffRefresh   GetBuffRefresh() { return m_rBuffRefresh; }
    private SBuffAttribute      m_rBuffAttribute = new SBuffAttribute();    public SBuffAttribute GetBuffAttribute() { return m_rBuffAttribute; } public List<int>.Enumerator GetBuffAttributeList() { return m_rBuffAttribute.BuffAttributeList.GetEnumerator(); }
    private SBuffMerge          m_rBuffMerge     = new SBuffMerge();        public SBuffMerge     GetBuffMerge()     { return m_rBuffMerge; } public int GetBuffMergeGroup() { return m_rBuffAttribute.BuffMergeGroup; }
                                                                            public ulong          GetBuffUnitInstanceID() { return m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(); }
                                                                           
    private COperatorUnitBuffBase   m_pBuffOperatorOwner = null;            //  버프의 소유자
    private COperatorUnitBuffBase   m_pBuffOperatorOrigin = null;           //  버프를 만든 객체 
    private IEventBuffHandler       m_pEventBuffHandler = null;
    private CBuffEventList          m_pBuffEventList = null;
    //-------------------------------------------------------------------------   
    internal void InterBuffUpdate(float fDelta)
    {
        UpdateBuff(fDelta);
        OnBuffUpdate(fDelta);
    }

    internal void InterBuffStart(IEventBuffHandler pBuffEventHandler, COperatorUnitBuffBase pBuffOperatorOwner, COperatorUnitBuffBase pBuffOperatorOrigin, float fDuration, float fBuffPower)
    {
        m_pBuffEventList.DoBuffEventReset();
        m_pEventBuffHandler = pBuffEventHandler;
        m_pBuffOperatorOwner = pBuffOperatorOwner;
        m_pBuffOperatorOrigin = pBuffOperatorOrigin;
        
        PrivBuffStart(fDuration, fBuffPower);
        OnBuffStart(pBuffEventHandler);
    }

    internal void InterBuffErase() // 버프 스텍 카운트 등으로 종료 되었다.
    {
        m_pBuffEventList.DoBuffEventEnd(this);
        m_pEventBuffHandler.IEventBuffErase(m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(), GetBuffInstanceID(), GetBuffTableID());
        m_pEventBuffHandler.IEventBuffEnd(m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(), GetBuffInstanceID(), GetBuffTableID());
        OnBuffErase();
        OnBuffEnd();
    }

    internal void InterBuffMerge(CBuffBase pTargetBuff, float fTargetDuration, float fTargetBuffPower)
    {
        PrivBuffMerge(pTargetBuff, fTargetDuration, fTargetBuffPower);
    }

    internal void InterBuffEvent(CBuffEventUsage pBuffEventUsage)
    {
        m_pBuffEventList.DoBuffEventExcute(this, pBuffEventUsage);
    }

    //-----------------------------------------------------------------------
    protected override sealed void OnObjectPoolDeactivate() {}
    protected override sealed void OnObjectPoolActivate() { PrivBuffReset(); }
    //------------------------------------------------------------------------
    protected void ProtBuffEvent(CBuffEventUsage pBuffEventUsage)
    {
        m_pBuffEventList.DoBuffEventExcute(this, pBuffEventUsage);
    }

    protected void ProtBuffStackUp(int iStackValue)
    {
        PrivBuffStackUp(iStackValue);
    }

    protected void ProtBuffTimeUp(float fTimeValue)
    {
        m_fDurationCurrent += fTimeValue;
        m_pEventBuffHandler.IEventBuffChangeDuration(GetBuffUnitInstanceID(), GetBuffInstanceID(), m_fDurationCurrent);
        OnBuffChangeDuration(fTimeValue);
    }

    protected void ProtBuffPowerUp(float fPowerUpValue)
    {
        m_fBuffPower += fPowerUpValue;
        m_pEventBuffHandler.IEventBuffChangePower(GetBuffUnitInstanceID(), GetBuffInstanceID(), m_fBuffPower);
        OnBuffChangePower(fPowerUpValue);
    }

    protected void ProtBuffTimeReset()
    {
        m_fDurationCurrent = m_fDuration;
        m_pEventBuffHandler.IEventBuffChangeDuration(GetBuffUnitInstanceID(), GetBuffInstanceID(), m_fDurationCurrent);
        OnBuffTimeReset();
    }

    protected void ProtBuffSelfErase()
    {
        m_pBuffOperatorOwner.InterBuffOperatorErase(this);
    }

    //------------------------------------------------------------------------
    private void PrivBuffReset()
    {       
        m_fEventOverTimeCurrent = 0;
        m_iEventOverTimeCount = 0;
        m_fDurationCurrent = 0;
        m_fDuration = 0;
        m_iStackCount = 0;

        m_fBuffPower = 0;
        m_eBuffType = 0;
        m_bBuffActive = false;
        m_hBuffID = 0;

        m_pBuffOperatorOwner = null;
        m_pBuffOperatorOrigin = null;
        m_pBuffEventList = null;
        m_pEventBuffHandler = null;
    }

    private void PrivBuffStart(float fDuration, float fBuffPower)
    {        
        m_fBuffPower = fBuffPower == 0 ? m_rBuffAttribute.DefaultPower : fBuffPower;
       
        m_bBuffActive = true;
        if (fDuration != 0)
        {
            m_fDuration = fDuration > m_rBuffAttribute.MaxDuration ? m_rBuffAttribute.MaxDuration : fDuration;
        }
        m_fDurationCurrent = m_fDuration;
        m_pEventBuffHandler.IEventBuffStart(m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(), GetBuffInstanceID(), m_hBuffID, m_eBuffType, m_fDuration, m_iStackCount);
        PrivBuffStackChange(m_rBuffMerge.StackCount == 0 ? 1 : m_rBuffMerge.StackCount);
    }

    private void PrivBuffTimeExpire()
    {
        m_pBuffEventList.DoBuffEventEnd(this);
        m_pBuffOperatorOwner.InterBuffOperatorTimeExpire(this);

        m_pEventBuffHandler.IEventBuffTimeExpire(m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(), GetBuffInstanceID());
        m_pEventBuffHandler.IEventBuffEnd(m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(), GetBuffInstanceID(), GetBuffTableID());

        OnBuffTimeExpire();
        OnBuffEnd();      
    }

    private void PrivBuffStackUp(int iStackValue)
    {
        if (iStackValue == 0) return;

        m_iStackCount += iStackValue;

        if (m_iStackCount > m_rBuffMerge.StackMax)
        {
            m_iStackCount = m_rBuffMerge.StackMax;
        }

        if (m_iStackCount <= 0)
        {
            m_iStackCount = 0;
            PrivBuffStackChange(m_iStackCount);
            PrivBuffStackZero();
        }
        else
        {
            PrivBuffStackChange(m_iStackCount);
        }
    }

    private void PrivBuffStackZero()
    {
        PrivBuffStackChange(0);
        m_pBuffOperatorOwner.InterBuffOperatorStackZero(this);       
    }

    private void PrivBuffStackChange(int iStackCount)
    {
        m_iStackCount = iStackCount;
        m_pEventBuffHandler.IEventBuffChangeStack(m_pBuffOperatorOwner.GetOperatorUnit().GetUnitInstanceID(), GetBuffInstanceID(), m_iStackCount);
        OnBuffChangeStack(m_iStackCount);
    }

    private void PrivBuffMerge(CBuffBase pTargetBuff, float fTargetDuration, float fTargetBuffPower)
    {
        if (m_rBuffMerge.MergePowerUp)
        {
            float fPowerUp = fTargetBuffPower != 0 ? fTargetBuffPower : pTargetBuff.m_rBuffAttribute.DefaultPower;
            ProtBuffPowerUp(fPowerUp);
        }

        if (m_rBuffMerge.MergeTimeUp)
        {
            float fTimeUp = fTargetDuration != 0 ? fTargetDuration : pTargetBuff.m_rBuffAttribute.Duration; 
            ProtBuffTimeUp(fTimeUp);
        }

        if (m_rBuffMerge.MergeTimeReset)
        {
            ProtBuffTimeReset();
        }

        if (m_rBuffMerge.MergeStackUp)
        {
            ProtBuffStackUp(pTargetBuff.m_rBuffMerge.StackCount);
        }

        OnBuffMerge(pTargetBuff);
    }

    private void PrivBuffEventOverTime()
    {
        OnBuffEventOverTime(m_iEventOverTimeCount);
        m_iEventOverTimeCount++;
    }

    //-----------------------------------------------------------------------
    private void UpdateBuff(float fDelta)
    {
        UpdateBuffEventOverTime(fDelta);
        UpdateBuffTime(fDelta);
    }

    private void UpdateBuffEventOverTime(float fDelta)
    {
        if (m_rBuffAttribute.EventOverTime == 0) return;

        m_fEventOverTimeCurrent += fDelta;
        if (m_fEventOverTimeCurrent >= m_rBuffAttribute.EventOverTime)
        {
            m_fEventOverTimeCurrent = 0;
            PrivBuffEventOverTime();
        }
    }

    private void UpdateBuffTime(float fDelta)
    {
        if (m_fDuration == 0) return; 

        m_fDurationCurrent -= fDelta;
        if (m_fDurationCurrent <= 0)
        {         
            PrivBuffTimeExpire();
        }
    }

    //-------------------------------------------------------------------------
    protected virtual void OnBuffStart(IEventBuffHandler pEventBuffHandler) { }
    protected virtual void OnBuffUpdate(float fDelta) { }
    protected virtual void OnBuffTimeExpire() { }
    protected virtual void OnBuffTimeReset() { }
    protected virtual void OnBuffEnd() { }
    protected virtual void OnBuffErase() { }
    protected virtual void OnBuffChangeStack(int iStackCurrent) { }
    protected virtual void OnBuffChangeDuration(float fDuration) { }
    protected virtual void OnBuffChangePower(float fBuffPowerUp) { }
    protected virtual void OnBuffEventOverTime(int iEventCount) { }
    protected virtual void OnBuffMerge(CBuffBase pTargetBuff) { }
    //------------------------------------------------------------------------
    public void SetBuffInformation(SBuffAttribute rBuffAttribute, SBuffMerge rBuffMerge) { m_rBuffAttribute = rBuffAttribute; m_rBuffMerge = rBuffMerge; m_hBuffID = m_rBuffAttribute.hBuffID; m_eBuffType = m_rBuffAttribute.BuffType;}
    public void SetBuffEventList(CBuffEventList pBuffEventList) { m_pBuffEventList = pBuffEventList; }
    public CUnitCoreBase GetBuffUnitOwner() { return m_pBuffOperatorOwner.GetOperatorUnit(); }
    public CUnitCoreBase GetBuffUnitOrigin() { return m_pBuffOperatorOrigin.GetOperatorUnit(); }
    protected override sealed uint GetObjectInstanceID() { return base.GetObjectInstanceID();}
    public uint GetBuffInstanceID() { return base.GetObjectInstanceID(); }   
    //-------------------------------------------------------------------------

}
