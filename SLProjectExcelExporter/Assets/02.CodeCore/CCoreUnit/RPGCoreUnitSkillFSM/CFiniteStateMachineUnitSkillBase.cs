using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


public interface IUnitEventSkill  // 인터페이스로 호출된 작동을 패킷으로 송수신하면 간단하게 네트워크 게임이 된다. 
{
    public void             ISkillAnimation(ref SAnimationUsage rAnimUsage);
    public void             ISkillAnimationIdle();
    public void             ISkillUseBuff(IUnitEventBuff pTargetUnit, uint hBuffID, float fDuration, float fPower);
    public void             ISkillCrushObject(IUnitEventSkill pOtherObject);

}

public abstract class CFiniteStateMachineUnitSkillBase : CFiniteStateMachineBase
{
    private IUnitEventSkill m_pFSMOwner = null;  
    protected CSkillStateBase GetFSMCurrentSkillState() { return GetFSMCurrentState() as CSkillStateBase; }
    private Dictionary<uint, CSkillTypeBase> m_mapSkillInstance = new Dictionary<uint, CSkillTypeBase>();
    private List<CSkillTypeAutoCastingBase> m_listSkillAutoCasting = new List<CSkillTypeAutoCastingBase>();
    
    //public bool IsSkillAction() { return !IsEmpty(); }
  
    //--------------------------------------------------------
    //protected override void OnAssistUpdate(float fDelta)
    //{
    //    base.OnAssistUpdate(fDelta);
    //    PrivFSMSkillUpdateAutoCast();
    //}

    //protected override void OnAssistFixedUpdate(float fDelta)
    //{
    //    base.OnAssistFixedUpdate(fDelta);
    //    PrivFSMSkillUpdateState(fDelta);
    //}

    //protected override void OnAssistUnitState(CUnitBase.EUnitState eState)
    //{
    //    base.OnAssistUnitState(eState);
    //    if (eState == CUnitBase.EUnitState.Spawning)
    //    {
    //        DoFSMUnitSkillReset();
    //    }
    //}

    //------------------------------------------------------
    protected override void OnFSMStateEnter(CStateBase pState)
    {
        base.OnFSMStateEnter(pState);
    }

    protected override void OnFSMStateLeave(CStateBase pState)
    {
        base.OnFSMStateLeave(pState);
    }

    //--------------------------------------------------------
    internal void InterFSMUnitSkillInitialize(IUnitEventSkill pOwner)
    {
        m_pFSMOwner = pOwner;
        OnFSMUnitSkillInitialize(pOwner);
    }

    internal void InterFSMUnitSkillTimer(float fDelay, UnityAction delWorkFunction)
    {
   //     StartCoroutine(CoroutineSkillTimer(fDelay, delWorkFunction));
    }

   

    //-------------------------------------------------------
    public void DoFSMUnitSkillReset()
    {
    //    StopAllCoroutines();
  //      PrivFSMClearAll();
        OnFSMUnitSkillReset();
    }

    public void DoFSMSkillUsageTargetAdd(IUnitEventSkill pAddTarget)
    {
        CSkillStateBase pSkillState = GetFSMCurrentSkillState();
        if (pSkillState != null)
        {
            pSkillState.InterSkillStateAddTarget(pAddTarget);
        }
    }

    //---------------------------------------------------------
    private IEnumerator CoroutineSkillTimer(float fDelay, UnityAction delWorkFunction)  // 유닛이 리셋될 때 강제로 제거하여 사이드 이펙트를 방지하기 위해 
    {
        yield return new WaitForSeconds(fDelay);
        delWorkFunction?.Invoke();
        yield break;
    }

    private void PrivFSMSkillUpdateAutoCast()
    {
        for (int i = 0; i < m_listSkillAutoCasting.Count; i++)
        {
            m_listSkillAutoCasting[i].InterSkillTypeUpdateAutoCasting();
        }
    }

    private void PrivFSMSkillUpdateState(float fDelta)
    {
        CSkillStateBase pCurrentState = GetFSMCurrentSkillState(); ;
        if (pCurrentState != null)
        {
            pCurrentState.InterSkillStateUpdate(fDelta);
        }
    }

    //---------------------------------------------------------
    protected void ProtFSMUnitSkillAdd(CSkillTypeBase pAddSkillType)
    {
        uint hSkillID = pAddSkillType.GetSkillID();
        if (m_mapSkillInstance.ContainsKey(hSkillID))
        {
            //Error!
            return;
        }

        if (pAddSkillType is CSkillTypeActiveBase)
        {
           
        }
        else if (pAddSkillType is CSkillTypePassiveBase)
        {
            
        }
        else if (pAddSkillType is CSkillTypeAutoCastingBase)
        {
            m_listSkillAutoCasting.Add(pAddSkillType as CSkillTypeAutoCastingBase);
        }

        m_mapSkillInstance[hSkillID] = pAddSkillType;
    }

    protected int ProtFSMUnitSkillUse(SSkillUsage pUsage)
    {
        int eResult = 0;
        CSkillTypeBase pSkillType = FindFSMSkill(pUsage.SkillID);
        if (pSkillType != null)
        {
            eResult = PrivFSMUnitSkillUse(pSkillType, pUsage);
        }
        else
        {
            eResult = -1;
        }

        return eResult;
    }

    protected void ProtFSMUnitSkillAnimationEvent(string strAniName, string strEventKey, int iArg, float fArg)
    {
        CSkillStateBase pCurrentState = GetFSMCurrentSkillState();
        if (pCurrentState != null)
        {
            pCurrentState.InterSkillStateAnimationEvent(strAniName, strEventKey, iArg, fArg);
        }
    }

    //-------------------------------------------------------
    private CSkillTypeBase FindFSMSkill(uint hSkillID)
    {
        CSkillTypeBase pFindSkill = null;
        m_mapSkillInstance.TryGetValue(hSkillID, out pFindSkill);
        return pFindSkill;
    }

    private int PrivFSMUnitSkillUse(CSkillTypeBase pSkillType, SSkillUsage rUsage)
    {
        int iResult = 0;
        if (pSkillType is CSkillTypeActiveBase)
        {
            iResult = PrivFSMUnitSkillUseActive(pSkillType as CSkillTypeActiveBase, rUsage);
        }
        else if (pSkillType is CSkillTypePassiveBase)
        {
            PrivFSMUnitSkillUsePassive(pSkillType as CSkillTypePassiveBase);
        }
        return iResult;
    }

    private int PrivFSMUnitSkillUseActive(CSkillTypeActiveBase pSkillActive, SSkillUsage pUsage)
    {
        int iResult = 0;
        iResult = pSkillActive.InterSkillActiveCondition(pUsage, this);
        if (iResult == 0)
        {            
            List<CSkillStateBase>.Enumerator it = pSkillActive.IterSkillState();
            while(it.MoveNext())
            {
                it.Current.InterSkillStateReset(pUsage, this);
          //      ProtStateAction(it.Current, EStateEnterType.Enter);
            }
        }
        return iResult;
    }

    private void PrivFSMUnitSkillUsePassive(CSkillTypePassiveBase pSkillPassive)
    {

    }

    //--------------------------------------------------------
    protected virtual void OnFSMUnitSkillInitialize(IUnitEventSkill pOwner) { }
    protected virtual void OnFSMUnitSkillReset() { }
    //--------------------------------------------------------
    //protected override sealed void ProtStateAction(CStateBase pState, EStateEnterType eStateAction) { base.InterFSMAction(pState, eStateAction); }
    //protected override sealed void PrivFSMLeave(CStateBase pState) { base.PrivFSMLeave(pState);}
}
