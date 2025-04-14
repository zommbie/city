using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSkillConditionBase 
{

    //----------------------------------------------------
    public int DoConditionCheckFactor(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        return OnConditionCheckFactor(pSkillUsage, pFSMSkill);
    }

    //-------------------------------------------------
    protected virtual int OnConditionCheckFactor(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { return 0; }
}
