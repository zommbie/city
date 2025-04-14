using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSkillTaskEventConditionBase 
{
    private CSkillTaskTargetListBase m_pTaskTargetList = null;
    private List<IUnitEventSkill> m_listTargetNote = new List<IUnitEventSkill>();
    private List<CSkillTaskBase> m_listSkillTaskInstance = new List<CSkillTaskBase>();
    //-----------------------------------------------------------------------
    internal void InterSkillTaskEventConditionReset(CFiniteStateMachineUnitSkillBase pFSMOnwer)
    {
        for (int i = 0; i < m_listSkillTaskInstance.Count; i++)
        {
            m_listSkillTaskInstance[i].InterSkillTaskReset(pFSMOnwer);
        }
        OnSkillTaskEventConditionReset();
    }

    internal void InterSkillTaskEventConditionEnter(CSkillStateBase pEnterState, CSkillStateBase pPrevState)
    {
        for (int i = 0; i < m_listSkillTaskInstance.Count; i++)
        {
            m_listSkillTaskInstance[i].InterSkillTaskEnter(pEnterState, pPrevState);
        }
    }

    internal void InterSkillTaskEventConditionExit(CSkillStateBase pExitState)
    {
        for (int i = 0; i < m_listSkillTaskInstance.Count; i++)
        {
            m_listSkillTaskInstance[i].InterSkillTaskExit(pExitState);
        }
    }

    internal void InterSkillTaskConditionExcute(SSkillUsage pSkillUsage, int iArg, float fArg, params object[] aParams)
    {
        if (OnSkillTaskEventCondition(pSkillUsage, iArg, fArg, aParams))
        {
            PrivSkillTaskContainerExcute(pSkillUsage, iArg, fArg, aParams);
        }
    }

    internal void InterSkillTaskConditionInterval(int iIntervalCount)
    {
        for (int i = 0; i < m_listSkillTaskInstance.Count; i++)
        {
            m_listSkillTaskInstance[i].InterSkillTaskInterval(iIntervalCount);
        }
    }

    internal void InterSkillTaskConditionUpdate(float fDelta)
    {
        for (int i = 0; i < m_listSkillTaskInstance.Count; i++)
        {
            m_listSkillTaskInstance[i].InterSkillTaskUpdate(fDelta);
        }
    }

    //---------------------------------------------------------------------
    private void PrivSkillTaskContainerExcute(SSkillUsage pSkillUsage, int iArg, float fArg, params object[] aParams)
    {
        m_listTargetNote.Clear();

        if (m_pTaskTargetList != null)
        {
            m_pTaskTargetList.InterSkillTaskTarget(pSkillUsage, m_listTargetNote);
        }
        else
        {
            for (int i = 0; i < pSkillUsage.SkillTargetList.Count; i++)
            {
                m_listTargetNote.Add(pSkillUsage.SkillTargetList[i]);
            }
        }

        for (int i = 0; i < m_listSkillTaskInstance.Count; i++)
        {
            m_listSkillTaskInstance[i].InterSkillTaskExcute(pSkillUsage, m_listTargetNote, iArg, fArg, aParams);
        }
    }
    //------------------------------------------------------------------------
    public void InstanceTaskAdd(CSkillTaskBase pSkillTask)
    {
        m_listSkillTaskInstance.Add(pSkillTask);
    }

    public void InstanceTaskTargetList(CSkillTaskTargetListBase pSkillTaskTargetList)
    {
        m_pTaskTargetList = pSkillTaskTargetList;
    }
    
    //-------------------------------------------------------------------------
    protected virtual bool OnSkillTaskEventCondition(SSkillUsage pSkillUsage, int iArg, float fArg, params object[] aParams) { return true; }
    protected virtual void OnSkillTaskEventConditionReset() { }
    protected virtual void OnSkillTaskEventConditionEnter(CSkillStateBase pEnterState) { }
}
