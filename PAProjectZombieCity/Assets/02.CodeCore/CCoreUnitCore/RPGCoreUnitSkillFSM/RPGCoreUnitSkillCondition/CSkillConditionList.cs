using System.Collections;
using System.Collections.Generic;

public class CSkillConditionList 
{
    private List<CSkillConditionBase> m_listSkillCondition = new List<CSkillConditionBase>();
    //-----------------------------------------------------------------------------------
    public int DoSkillCondition(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMUnitSkill)
    {
        int iConditionResult = 0;

        for (int i = 0; i < m_listSkillCondition.Count; i++)
        {
            iConditionResult = m_listSkillCondition[i].DoSkillConditionWork(pSkillUsage, pFSMUnitSkill);
            if (iConditionResult != 0)
            {
                break;
            }
        }

        return iConditionResult;
    }

    public void SetSkillConditionAdd(CSkillConditionBase pSkillCondition) { m_listSkillCondition.Add(pSkillCondition);}


}
