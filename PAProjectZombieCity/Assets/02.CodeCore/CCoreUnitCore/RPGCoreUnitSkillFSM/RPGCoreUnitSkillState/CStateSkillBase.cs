using System.Collections;
using System.Collections.Generic;

public abstract class CStateSkillBase : CStateBase
{
    private CSkillUsageBase                     m_pSkillUsage = null;
    private CFiniteStateMachineUnitSkillBase    m_pFSMSkill = null;
    private CSkillResourceList                  m_pSkillResourceList = null;  
    private List<CSkillTaskContainerList>       m_listTaskContainerList = new List<CSkillTaskContainerList>();
    //-----------------------------------------------------------------------
    protected override void OnStateInitialize(CFiniteStateMachineBase pFSMOwner)
    {
        m_pFSMSkill = pFSMOwner as CFiniteStateMachineUnitSkillBase;
    }

    protected override sealed void OnStateEnter(CStateBase pStatePrev)
    {
        base.OnStateEnter(pStatePrev);

        PrivStateSkillTaskReset();

        int iResourceResult = 0;
        if (m_pSkillResourceList != null)
        {
            iResourceResult = m_pSkillResourceList.DoSkillResourceCheck(m_pSkillUsage, m_pFSMSkill); // 스테이트 발동 조건을 체크
        }
     
        if (iResourceResult == 0)
        {
            m_pSkillResourceList?.DoSkillResourceConsume(m_pSkillUsage, m_pFSMSkill);
            OnStateSkillEnter(pStatePrev as CStateSkillBase, m_pSkillUsage);
        }
        else
        {
            ProtStateSelfRemove(); // Remove는 Leave를 발생시키지 않으므로 스킬 스테이트 Leave로직을 작동 시키지 않는다.
            OnStateSkillFail(iResourceResult);
        }
    }

    protected override sealed void OnStateLeave(CStateBase pStatePrev)
    {
        base.OnStateLeave(pStatePrev);
        for (int i = 0; i < m_listTaskContainerList.Count; i++)
        {
            m_listTaskContainerList[i].InterSkillTaskContainerStateExit(m_pSkillUsage, m_pFSMSkill);
        }
        OnStateSkillLeave(pStatePrev as CStateSkillBase, m_pSkillUsage);
    }

    protected sealed override void OnStateRemove()
    {
        base.OnStateRemove();
        OnStateSkillRemove(m_pSkillUsage);
    }

    //------------------------------------------------------------------------
    public void SetStateSkillUsage(CSkillUsageBase pSkillUsage)
    {
        m_pSkillUsage = pSkillUsage;
    }

    //---------------------------------------------------------------------
    protected void ProtStateSkillTaskEvent(CSkillTaskEventBase pSkillTaskEvent)
    {
        for (int i = 0; i < m_listTaskContainerList.Count; i++)
        {
            m_listTaskContainerList[i].InterSkillTaskContainerTaskProcess(pSkillTaskEvent, m_pSkillUsage, m_pFSMSkill);
        }
    }

    protected void ProtStateSkillTaskEventForce()
    {
        for (int i = 0; i < m_listTaskContainerList.Count; i++)
        {
            m_listTaskContainerList[i].InterSkillTaskContainerTaskProcessForce(m_pSkillUsage, m_pFSMSkill);
        }
    }

    //-----------------------------------------------------------------------
    private void PrivStateSkillTaskReset()
    {
        for (int i = 0; i < m_listTaskContainerList.Count; i++)
        {
            m_listTaskContainerList[i].DoSkillTaskContainerReset();
        }
    }

    //---------------------------------------------------------------------
    public void SetSkillResourceList(CSkillResourceList pSkillResourceList) { m_pSkillResourceList = pSkillResourceList;}
    public void SetSkillTaskContainerListAdd(CSkillTaskContainerList pSkillTaskContainerList) { m_listTaskContainerList.Add(pSkillTaskContainerList);}
    //---------------------------------------------------------------------   
    protected virtual void OnStateSkillEnter(CStateSkillBase pPrevState, CSkillUsageBase pSkillUsage) { }
    protected virtual void OnStateSkillLeave(CStateSkillBase pPrevState, CSkillUsageBase pSkillUsage) { }
    protected virtual void OnStateSkillRemove(CSkillUsageBase pSkillUsage) { }
    protected virtual void OnStateSkillFail(int iResult) { }    
}
