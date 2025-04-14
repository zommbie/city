using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillTaskContainer 
{
    private CSkillTaskEventConditionBase m_pTaskEventCondition = null;  
    //-----------------------------------------------------------
    internal void InterSkillTaskContainerReset(CFiniteStateMachineUnitSkillBase pFSMOnwer)
    {
        m_pTaskEventCondition?.InterSkillTaskEventConditionReset(pFSMOnwer);
    }

    internal void InterSkillTaskContainerExcute(SSkillUsage pSkillUsage, int iArg, float fArg, params object[] aParams)
    {
        m_pTaskEventCondition?.InterSkillTaskConditionExcute(pSkillUsage, iArg, fArg, aParams);
    }

    internal void InterSkillTaskContainerUpdate(float fDelta)
    {
        m_pTaskEventCondition?.InterSkillTaskConditionUpdate(fDelta);
    }

    internal void InterSkillTaskContainerEnter(CSkillStateBase pEnterState, CSkillStateBase pPrevState)
    {
        m_pTaskEventCondition?.InterSkillTaskEventConditionEnter(pEnterState, pPrevState);
    }

    internal void InterSkillTaskContainerExit(CSkillStateBase pExitState)
    {
        m_pTaskEventCondition?.InterSkillTaskEventConditionExit(pExitState);
    }

    internal void InterSkillTaskContainerInterval(int iIntervalCount)
    {
        m_pTaskEventCondition?.InterSkillTaskConditionInterval(iIntervalCount);
    }

    //-----------------------------------------------------------
    public void InstanceTaskEventCondition(CSkillTaskEventConditionBase pTaskEventCondition)
    {
        m_pTaskEventCondition = pTaskEventCondition;
    }

}
