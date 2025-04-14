using System.Collections;
using System.Collections.Generic;


public abstract class CSkillConditionBase 
{
    //----------------------------------------------------------------
    public int DoSkillConditionWork(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        return OnSkillConditionWork(pSkillUsage, pFSMSkill);
    }

    //-----------------------------------------------------------------
    protected virtual int OnSkillConditionWork(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { return 0; }
}
