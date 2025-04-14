using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컨디션 체크를 해서 성공할 경우 해당 리소스를 소모한다.

public abstract class CSkillResourceBase 
{
    public int DoSkillResourceCheckAndConsume(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        return OnSkillResourceCheckAndConsume(pSkillUsage, pFSMSkill);
    }
    //-----------------------------------------------------------------------------
    protected virtual int OnSkillResourceCheckAndConsume(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { return 0; }
}
