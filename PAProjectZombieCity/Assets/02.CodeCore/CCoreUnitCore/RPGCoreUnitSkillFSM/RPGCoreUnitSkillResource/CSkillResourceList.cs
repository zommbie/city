using System.Collections;
using System.Collections.Generic;


public class CSkillResourceList 
{
    private List<CSkillResourceBase> m_listSkillResource = new List<CSkillResourceBase>();
    //------------------------------------------------------------------
    public int DoSkillResourceCheck(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        int iResult = 0;
        for (int i = 0; i < m_listSkillResource.Count; i++)
        {
            iResult = m_listSkillResource[i].DoSkillResourceCheck(pSkillUsage, pFSMSkill);
            if (iResult != 0)
            {
                break;
            }
        }
        return iResult;
    }

    public void DoSkillResourceConsume(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        for (int i = 0; i < m_listSkillResource.Count; i++)
        {
            m_listSkillResource[i].DoSkillResourceConsume(pSkillUsage, pFSMSkill);           
        }
    }

    //-------------------------------------------------------------
    public void SetSkillResourceAdd(CSkillResourceBase pSkillResource) { m_listSkillResource.Add(pSkillResource); }
}
