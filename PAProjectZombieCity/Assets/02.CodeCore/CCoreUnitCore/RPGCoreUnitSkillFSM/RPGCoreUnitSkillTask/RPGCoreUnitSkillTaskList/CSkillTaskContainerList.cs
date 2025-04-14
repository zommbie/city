using System.Collections;
using System.Collections.Generic;

public class CSkillTaskContainerList 
{
    private CSkillTaskConditionList     m_pTaskConditionList = null;
    private CSkillTaskTargetList        m_pTaskTargetList = null;
    private CSkillTaskList              m_pTaskList = null;
    private List<CUnitCoreBase>         m_listTaskTargetNote = new List<CUnitCoreBase>();
    //----------------------------------------------------------
    public void DoSkillTaskContainerReset()
    {
        PrivSkillTaskContainerReset();
    }

    internal void InterSkillTaskContainerTaskProcess(CSkillTaskEventBase pSkillTaskEvent, CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        int iResult = 0;
        if (m_pTaskConditionList != null)
        {
            iResult = m_pTaskConditionList.InterSkillTaskCondition(pSkillTaskEvent, pSkillUsage, pFSMSkill);
            if (iResult == 0)
            {
                PrivSkillTaskTaskProcess(pSkillUsage, pFSMSkill);
            }
        }       
    }

    internal void InterSkillTaskContainerTaskProcessForce(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        PrivSkillTaskTaskProcess(pSkillUsage, pFSMSkill);
    }

    internal void InterSkillTaskContainerStateExit(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        m_pTaskList.InterSkillTaskStateExit(pSkillUsage, pFSMSkill);
    }

    //----------------------------------------------------------
    private void PrivSkillTaskContainerReset()
    {
        m_listTaskTargetNote.Clear();        
        m_pTaskList?.DoSkillTaskListReset();
    }

    private void PrivSkillTaskTaskProcess(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        m_listTaskTargetNote.Clear();
        m_pTaskTargetList?.DoSkillTaskTargetList(pSkillUsage, pFSMSkill, ref m_listTaskTargetNote);
        m_pTaskList?.InterSkillTaskProcess(pSkillUsage, pFSMSkill, ref m_listTaskTargetNote);
    }
   
    //---------------------------------------------------------
    public void SetSkillTaskConditionList(CSkillTaskConditionList pTaskConditionList) { m_pTaskConditionList = pTaskConditionList;}
    public void SetSkillTaskTargetList(CSkillTaskTargetList pTaskTargetList) { m_pTaskTargetList = pTaskTargetList;}
    public void SetSkillTaskList(CSkillTaskList pTaskList) { m_pTaskList = pTaskList; }
}
