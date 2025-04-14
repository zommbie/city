using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillResourceList 
{
    private List<CSkillResourceBase> m_listSkillResource = new List<CSkillResourceBase>();
    //------------------------------------------------------------------
    public int DoSkillResourceCheckAndConsume(SSkillUsage pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        int iResult = 0;
        for (int i = 0; i < m_listSkillResource.Count; i++)
        {
            iResult = m_listSkillResource[i].DoSkillResourceCheckAndConsume(pSkillUsage, pFSMSkill);
            if (iResult != 0)
            {
                break;
            }
        }
        return iResult;
    }

    //-------------------------------------------------------------
    public virtual void InstanceSkillResource(CSkillResourceBase pSkillResource) { m_listSkillResource.Add(pSkillResource); }
}
