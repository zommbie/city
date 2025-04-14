using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FSM에 의해 운동되는 객체로 하나의 스킬타입은 복수의 스테이트로 구성된다. 
// 스킬 태스크를 실행 시키며 생성된 태스크 이벤트를 처리하는 주체 이다.
public abstract class CSkillStateBase : CStateBase
{
    private CSkillResourceList m_pSkillResourceList = null;
    private SSkillUsage m_pSkillUsage = null; protected SSkillUsage GetSkillStateUsage() { return m_pSkillUsage; }
    private CSkillTaskEventListBase m_pSkillTaskEventList = null;

    private List<CSkillTaskBase> m_listUpdateTask = new List<CSkillTaskBase>();
    //--------------------------------------------------------------------
    protected override sealed void OnStateEnter(CStateBase pStatePrev)
    {
        base.OnStateEnter(pStatePrev);
        m_listUpdateTask.Clear();
        if (m_pSkillResourceList != null)
        {
            if (m_pSkillResourceList.DoSkillResourceCheckAndConsume(m_pSkillUsage, GetStateFSMOwner() as CFiniteStateMachineUnitSkillBase) == 0)
            {
                m_pSkillTaskEventList.InterSkillTaskEnter(this, pStatePrev as CSkillStateBase);
                OnSkillStateStart();
            }
            else
            {
                ProtStateSelfRemove();
            }
        }
    }

    protected sealed override void OnStateLeave(CStateBase pStatePrev)
    {
        base.OnStateLeave(pStatePrev);
        m_pSkillTaskEventList.InterSkillTaskExit(this);
        OnSkillStateEnd();
    }

    //-----------------------------------------------------------------
    internal void InterSkillStateReset(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMOwner)
    {
        m_pSkillUsage = pSkillUsage;
        m_pSkillTaskEventList.InterSkillTaskEventListReset(pFSMOwner);
        OnSkillStateReset();
    }

    internal void InterSkillStateUpdate(float fDelta)
    {
        OnSkillStateUpdate(fDelta);
    }

    internal void InterSkillStateAnimationEvent(string strAniName, string strEventKey, int iArg, float fArg)
    {
        OnSkillStateAnimationEvent(strAniName, strEventKey, iArg, fArg);
    }

    internal void InterSkillStateAddTarget(IUnitEventSkill pAddTarget)
    {
        if (m_pSkillUsage != null)
        {
            m_pSkillUsage.SkillTargetList.Add(pAddTarget);
        }
    }

    //------------------------------------------------------------------
    protected void ProtSkillStateTaskEvent(int eTaskEventType, int iArg, float fArg, params object[] aParams)
    {
        m_pSkillTaskEventList.InterSkillTaskEvent(m_pSkillUsage, eTaskEventType, iArg, fArg, aParams);
    }

    protected void ProtSkillStateTaskUpdate(float fDelta)
    {
        m_pSkillTaskEventList.InterSkillTaskUpdate(fDelta);
    }

    protected void ProtSkillStateTaskInterval(int iIntervalCount)
    {
        m_pSkillTaskEventList.InterSkillTaskInterval(iIntervalCount);
    }

    //------------------------------------------------------------------
    public virtual void InstanceSkillResourceList(CSkillResourceList pSkillResourceList) { m_pSkillResourceList = pSkillResourceList; }
    public virtual void InstanceSkillTaskEventList(CSkillTaskEventListBase pSkillTaskEventList) { m_pSkillTaskEventList = pSkillTaskEventList; }
    //------------------------------------------------------------------
    protected virtual void OnSkillStateStart() { }
    protected virtual void OnSkillStateEnd() { }
    protected virtual void OnSkillStateReset() { }
    protected virtual void OnSkillStateUpdate(float fDelta) { }
    protected virtual void OnSkillStateAnimationEvent(string strAniName, string strEventKey, int iArg, float fArg) { }   
}
