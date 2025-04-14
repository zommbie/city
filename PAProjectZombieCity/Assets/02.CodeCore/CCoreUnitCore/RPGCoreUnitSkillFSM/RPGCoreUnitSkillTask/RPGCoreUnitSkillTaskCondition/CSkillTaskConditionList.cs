using System.Collections;
using System.Collections.Generic;

public class CSkillTaskConditionList 
{
    private List<CSkillTaskConditionBase> m_listSkillTaskCondition = new List<CSkillTaskConditionBase>();
    //----------------------------------------------------------------------------
    internal int InterSkillTaskCondition(CSkillTaskEventBase pSkillTaskEvent, CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        int iResult = 0;
        for (int i = 0; i < m_listSkillTaskCondition.Count; i++)
        {
            iResult = m_listSkillTaskCondition[i].InterSkillTaskCondition(pSkillTaskEvent, pSkillUsage, pFSMSkill);
            if (iResult != 0)
            {
                break;
            }
        }

        return iResult;
    }

    //-----------------------------------------------------------------------------
    public void SetSkillTaskConditionAdd(CSkillTaskConditionBase pSkillTaskCondition) { m_listSkillTaskCondition.Add(pSkillTaskCondition);} 
}
