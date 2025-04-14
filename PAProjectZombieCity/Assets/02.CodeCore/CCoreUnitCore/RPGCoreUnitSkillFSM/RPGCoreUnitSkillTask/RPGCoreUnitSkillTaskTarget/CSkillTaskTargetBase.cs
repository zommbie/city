using System.Collections;
using System.Collections.Generic;

public abstract class CSkillTaskTargetBase 
{
    //-----------------------------------------------------------------------------
    public void DoSkillTaskTarget(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill, ref List<CUnitCoreBase> pListOutTarget)
    {
        OnSkillTaskTarget(pSkillUsage, pFSMSkill, ref pListOutTarget);
    }

    //-----------------------------------------------------------------------------
    protected virtual void OnSkillTaskTarget(CSkillUsageBase pUsage, CFiniteStateMachineUnitSkillBase pFSMSkill, ref List<CUnitCoreBase> pListOutTarget) { }
}
