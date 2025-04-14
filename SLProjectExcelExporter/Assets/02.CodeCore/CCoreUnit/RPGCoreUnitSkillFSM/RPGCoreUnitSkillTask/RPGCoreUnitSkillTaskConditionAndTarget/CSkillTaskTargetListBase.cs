using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSkillTaskTargetListBase
{
    private int m_eTargetFilterType = 0;
    private List<IUnitEventSkill> m_listUnitNoteIn = new List<IUnitEventSkill>();
    private List<IUnitEventSkill> m_listUnitNoteOut = new List<IUnitEventSkill>();
    private List<CSkillTaskTargetBase> m_listSkillTaskTarget = new List<CSkillTaskTargetBase>();
    //----------------------------------------------------------------------------
    internal void InterSkillTaskTarget(SSkillUsage pSkillUsage, List<IUnitEventSkill> pListOutTarget)
    {
        m_listUnitNoteIn.Clear();
        m_listUnitNoteOut.Clear();
        ExtractSkillTaskTargetList(pSkillUsage, m_eTargetFilterType, m_listUnitNoteIn);

        for (int i = 0; i < m_listSkillTaskTarget.Count; i++)
        {
            m_listSkillTaskTarget[i].InterSkillTaskTarget(pSkillUsage, m_listUnitNoteIn, m_listUnitNoteOut);

            m_listUnitNoteIn.Clear();
            for (int j = 0; j < m_listUnitNoteOut.Count; j++)
            {
                m_listUnitNoteIn.Add(m_listUnitNoteOut[j]);
            }
            m_listUnitNoteOut.Clear();
        }

        for (int i = 0; i < m_listUnitNoteIn.Count; i++)
        {
            pListOutTarget.Add(m_listUnitNoteIn[i]);
        }
    }
    //----------------------------------------------------------------------------
    public void SetTaskTargetFilter(int eTaskTargetFilter)
    {
        m_eTargetFilterType = eTaskTargetFilter;
    }
    //---------------------------------------------------------------------------
    protected virtual void ExtractSkillTaskTargetList(SSkillUsage pSkillUsage, int eTargetFilterType, List<IUnitEventSkill> pListOutTarget) { }
    //---------------------------------------------------------------------------
    public void InstanceTaskTarget(CSkillTaskTargetBase pSkillTaskTarget) { m_listSkillTaskTarget.Add(pSkillTaskTarget); }
}
