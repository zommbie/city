using System.Collections;
using System.Collections.Generic;


public class CSkillTaskList
{
    private List<CSkillTaskBase> m_listSkillTask = new List<CSkillTaskBase>();
    //--------------------------------------------------------------------------
    public void DoSkillTaskListReset()
    {
        for (int i = 0; i < m_listSkillTask.Count; i++)
        {
            m_listSkillTask[i].DoSkillTaskReset();
        }
    }

    internal void InterSkillTaskProcess(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill, ref List<CUnitCoreBase> pListTarget)
    {
        for (int i = 0; i < m_listSkillTask.Count; i++)
        {
            m_listSkillTask[i].DoSkillTaskProcess(pSkillUsage, pFSMSkill, ref pListTarget);
        }
    }

    internal void InterSkillTaskStateExit(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        for (int i = 0; i < m_listSkillTask.Count; i++)
        {
            m_listSkillTask[i].InterSkillTaskStateExit(pSkillUsage, pFSMSkill);
        }
    }

    internal void InterSkillTaskUpdate(float fDelta)
    {
        for (int i = 0; i < m_listSkillTask.Count; i++)
        {
            m_listSkillTask[i].InterSkillTaskUpdate(fDelta);
        }
    }

    //--------------------------------------------------------------------------
    public void SetSkillTaskAdd(CSkillTaskBase pSkillTask) { m_listSkillTask.Add(pSkillTask);}
    
}
