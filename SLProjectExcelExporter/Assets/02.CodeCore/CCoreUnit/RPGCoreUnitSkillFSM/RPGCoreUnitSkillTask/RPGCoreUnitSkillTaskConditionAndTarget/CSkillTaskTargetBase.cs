using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSkillTaskTargetBase
{
   
    //--------------------------------------------------------------
    internal void InterSkillTaskTarget(SSkillUsage pSkillUsage, List<IUnitEventSkill> pListInTarget, List<IUnitEventSkill> pListOutTarget)
    {
        OnSkillTaskTarget(pSkillUsage, pListInTarget, pListOutTarget);
    }

    //--------------------------------------------------------------
    protected virtual void OnSkillTaskTarget(SSkillUsage pSkillUsage, List<IUnitEventSkill> pListInTarget, List<IUnitEventSkill> pListOutTarget) { }
}
