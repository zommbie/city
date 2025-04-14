using System.Collections;
using System.Collections.Generic;


public abstract class CSkillTaskConditionBase 
{
    

    //-------------------------------------------------------------------
    internal int InterSkillTaskCondition(CSkillTaskEventBase pSkillTaskEvent, CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        return OnSkillTaskCondition(pSkillTaskEvent, pSkillUsage, pFSMSkill);
    }

    //-------------------------------------------------------------------
    protected virtual int OnSkillTaskCondition(CSkillTaskEventBase pSkillTaskEvent, CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { return 0; }

}
