using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 객체로 부터 각종 정보를 읽어서 판정을 내리는 기능

public class CSkillConditionList 
{

    private List<CSkillConditionBase> m_listCheckFactor = new List<CSkillConditionBase>();
    //-----------------------------------------
    public int DoConditionCheck(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        int iResult = 0;
        for (int i = 0; i < m_listCheckFactor.Count; i++)
        {
            iResult = m_listCheckFactor[i].DoConditionCheckFactor(pSkillUsage, pFSMSkill);
            if (iResult != 0)
            {
                break;
            }
        }
        return iResult;
    }

    //-----------------------------------------
    public virtual void InstanceConditionChecker(CSkillConditionBase pChecker) { m_listCheckFactor.Add(pChecker);}
}
