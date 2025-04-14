using System.Collections;
using System.Collections.Generic;

public abstract class CSkillTaskBase 
{
    private CStateSkillBase m_pOwnerSkillState = null;
    //----------------------------------------------------------
    internal void InterSkillTaskInitialize(CStateSkillBase pOwnerSkillState)
    {
        m_pOwnerSkillState = pOwnerSkillState;
        OnSkillTaskInitialize(pOwnerSkillState);
    }
    //------------------------------------------------------------
    public void DoSkillTaskReset()
    {
        OnSkillTaskReset();
    }

    public void DoSkillTaskProcess(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill, ref List<CUnitCoreBase> pListTarget)
    {
        OnSkillTaskProcess(pSkillUsage, pFSMSkill, ref pListTarget);
    }

    internal void InterSkillTaskStateExit(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill)
    {
        OnSkillTaskStateExit(pSkillUsage, pFSMSkill);
    }

    internal void InterSkillTaskUpdate(float fDelta)
    {
        OnSkillTaskUpdate(fDelta);
    }

    //------------------------------------------------------------
    protected virtual void OnSkillTaskInitialize(CStateSkillBase pOwnerSkillState) { }
    protected virtual void OnSkillTaskReset() { }
    protected virtual void OnSkillTaskProcess(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill, ref List<CUnitCoreBase> pListTarget) { }
    protected virtual void OnSkillTaskStateExit(CSkillUsageBase pSkillUsage, CFiniteStateMachineUnitSkillBase pFSMSkill) { }
    protected virtual void OnSkillTaskUpdate(float fDelta) { }
}
