using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitSkillBase : CUnitBuffBase, IUnitEventSkill
{
	private CFiniteStateMachineUnitSkillBase m_pFSMSkill = null;
    //------------------------------------------------------------------
    public void ISkillAnimation(ref SAnimationUsage rAnimUsage)
    {
        OnUnitSkillAnimation(ref rAnimUsage);
    }

    public void ISkillAnimationIdle()
    {
        OnUnitSkillAnimationIdle();
    }

    public void ISkillUseBuff(IUnitEventBuff pTargetUnit, uint hBuffID, float fDuration, float fPower)
    {
        OnUnitSkillUseBuff(pTargetUnit, hBuffID, fDuration, fPower);
    }

    public void ISkillCrushObject(IUnitEventSkill pOtherObject)
    {
        OnUnitSkillCrushObject(pOtherObject);
    }

    //-----------------------------------------------------------------
    public bool IsSkillAction()
    {
        return false; //m_pFSMSkill.IsSkillAction();
    }
    //-----------------------------------------------------------------
    protected virtual void OnUnitSkillAnimation(ref SAnimationUsage rAnimUsage) { }   
    protected virtual void OnUnitSkillAnimationIdle() { }   
    protected virtual void OnUnitSkillUseBuff(IUnitEventBuff pTargetUnit, uint hBuffID, float fDuration, float fPower) { }   
    protected virtual void OnUnitSkillCrushObject(IUnitEventSkill pOtherObject) { }
    //------------------------------------------------------------------
    protected virtual void InstanceUnitFSM(CFiniteStateMachineUnitSkillBase pInstanceSkillFSM) { m_pFSMSkill = pInstanceSkillFSM; }
}
