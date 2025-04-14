using System.Collections;
using System.Collections.Generic;


// 컨디션 체크를 해서 성공할 경우 해당 리소스를 소모한다.

public abstract class CSkillResourceBase 
{
    public int DoSkillResourceCheck(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        return OnSkillResourceCheck(pSkillUsage, pFSMSkill);
    }

    public void DoSkillResourceConsume(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        OnSkillResourceConsume(pSkillUsage, pFSMSkill);
    }
    
    //-----------------------------------------------------------------------------
    protected virtual int OnSkillResourceCheck(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { return 0; }
    protected virtual void OnSkillResourceConsume(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { }
   
}
