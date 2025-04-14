using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillTaskConditionList 
{
    private List<CSkillTaskConditionBase> m_listSkillTaskCondition = new List<CSkillTaskConditionBase>();
    //-----------------------------------------------------------------
    internal int InterSkillTaskCondition(CFiniteStateMachineUnitSkillBase pFSMOwner,  SSkillUsage pSkillUsage, int iArg, float fArg, params object[] aParams)
    {
        return OnSkillTaskCondition(pFSMOwner, pSkillUsage, iArg, fArg, aParams);
    }
    //------------------------------------------------------------------
    protected virtual int OnSkillTaskCondition(CFiniteStateMachineUnitSkillBase pFSMOwner, SSkillUsage pSkillUsage, int iArg, float fArg, params object[] aParams) { return 0; }
    //-----------------------------------------------------------------
    public void InstanceSkillTaskCondition(CSkillTaskConditionBase pSkillTaskCondition) { m_listSkillTaskCondition.Add(pSkillTaskCondition); }
    
}
