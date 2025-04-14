using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 외부 요청에 의해 실행되며 스테이트 작동에 의해 취소 될수 있다.
// 글로벌 쿨타임에 영향을 받는다.
public abstract class CSkillTypeActiveBase : CSkillTypeBase
{
    private CSkillConditionList         m_pSkillCondition = null;
    private List<CSkillStateBase>       m_listSkillStateInstance = new List<CSkillStateBase>();
    //------------------------------------------------------------
    internal int InterSkillActiveCondition(SSkillUsage pSkillUsage , CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        int iResult = 0;
        if (m_pSkillCondition != null)
        {
            iResult = m_pSkillCondition.DoConditionCheck(pSkillUsage, pFSMSkill);
        }
        return iResult;
    }
    //-------------------------------------------------------------
    public List<CSkillStateBase>.Enumerator IterSkillState()
    {
        return m_listSkillStateInstance.GetEnumerator();
    }

    //--------------------------------------------------------------
    public virtual void InstanceSkillCondition(CSkillConditionList pSkillCondition) { m_pSkillCondition = pSkillCondition;}
    public virtual void InstanceSkillState(CSkillStateBase pSkillState) { m_listSkillStateInstance.Add(pSkillState); }
}
