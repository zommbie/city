using System.Collections;
using System.Collections.Generic;

public class CSkillTaskTargetList 
{
    private List<CSkillTaskTargetBase> m_listSkillTaskTarget = new List<CSkillTaskTargetBase>();
    //---------------------------------------------------------
    public void DoSkillTaskTargetList(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill, ref List<CUnitCoreBase> pListOutTarget)
    {
        for (int i = 0; i < m_listSkillTaskTarget.Count; i++)
        {
            m_listSkillTaskTarget[i].DoSkillTaskTarget(pSkillUsage, pFSMSkill, ref pListOutTarget);
        }
    }

    //---------------------------------------------------------
    public void SetSkillTaskTarget(CSkillTaskTargetBase pSkillTaskTarget) { m_listSkillTaskTarget.Add(pSkillTaskTarget); }
   
}
