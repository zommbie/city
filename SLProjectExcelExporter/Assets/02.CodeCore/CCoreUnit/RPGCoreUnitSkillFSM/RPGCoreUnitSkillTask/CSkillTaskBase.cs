using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class CSkillTaskBase 
{
    private bool m_bTaskEnable = false; public bool IsTaskEnable { get { return m_bTaskEnable; } }

    private CSkillStateBase m_pStateOwner = null; protected CSkillStateBase GetSkillTaskStateOwner() { return m_pStateOwner; }
    private CFiniteStateMachineUnitSkillBase m_pFSMOwner = null;  protected CFiniteStateMachineUnitSkillBase GetSkillTaskFSMOwner() { return m_pFSMOwner; }
    private SSkillUsage m_pSkillUsage = null;                     protected SSkillUsage GetSkillTaskUsage() { return m_pSkillUsage; }
    private CSkillTaskConditionList  m_pTaskConditionList = null;   
    //---------------------------------------------------------
    internal void InterSkillTaskReset(CFiniteStateMachineUnitSkillBase pFSMOwner)
    {
        m_pFSMOwner = pFSMOwner;
        m_bTaskEnable = false;
        OnSkillTaskReset();
    }

    internal void InterSkillTaskEnter(CSkillStateBase pEnterState, CSkillStateBase pPrevState)
    {
        m_bTaskEnable = false;
        m_pStateOwner = pEnterState;
        OnSkillTaskEnter(pEnterState, pPrevState);
    }

    internal void InterSkillTaskExit(CSkillStateBase pExitState)
    {
        m_bTaskEnable = false;
        OnSkillTaskExit(pExitState, m_pSkillUsage);
        m_pStateOwner = null;
        m_pFSMOwner = null;
    }

    internal void InterSkillTaskUpdate(float fDelta)
    {
        OnSkillTaskUpdate(fDelta);
    }


    internal void InterSkillTaskExcute(SSkillUsage pSkillUsage, List<IUnitEventSkill> pListTarget, int iArg, float fArg, params object[] aParams)
    {
        m_bTaskEnable = true;
        m_pSkillUsage = pSkillUsage;
        if (m_pTaskConditionList != null)
        {
            int iResult = m_pTaskConditionList.InterSkillTaskCondition(m_pFSMOwner, pSkillUsage, iArg, fArg, aParams);
            if (iResult == 0)
            {
                PrivSkillTaskExcute(pSkillUsage, pListTarget, iArg, fArg, aParams);
            }
        }
        else
        {
            PrivSkillTaskExcute(pSkillUsage, pListTarget, iArg, fArg, aParams);
        }
    }

    internal void InterSkillTaskInterval(int iIntervalCount)
    {
        OnSkillTaskInterval(iIntervalCount);
    }

    //-----------------------------------------------------------
    private void PrivSkillTaskExcute(SSkillUsage pSkillUsage, List<IUnitEventSkill> pListTarget, int iArg, float fArg, params object[] aParams)
    {
        OnSkillTaskExcute(pSkillUsage, pListTarget, iArg, fArg, aParams);
    }

    //-----------------------------------------------------------
    public virtual void InstanceTaskConditionList(CSkillTaskConditionList pTaskConditionList) { m_pTaskConditionList = pTaskConditionList; }
  
    //--------------------------------------------------------
    protected virtual void OnSkillTaskReset() { }
    protected virtual void OnSkillTaskEnter(CSkillStateBase pEnterState, CSkillStateBase pPrevState) { }
    protected virtual void OnSkillTaskExit(CSkillStateBase pExitState, SSkillUsage pSkillUsage) { }
    protected virtual void OnSkillTaskUpdate(float fDelta) { }
    protected virtual void OnSkillTaskExcute(SSkillUsage pSkillUsage, List<IUnitEventSkill> pListTarget, int iArg, float fArg, params object[] aParams) { }
    protected virtual void OnSkillTaskInterval(int iIntervalCount) { }
}
