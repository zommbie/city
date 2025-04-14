using System.Collections;
using System.Collections.Generic;

public interface IEvenSkillHandler
{
    public void IEventSkillUsePassive(ulong hUnitID, uint hSkillID, int iSkillLevel);
    public void IEventSkillUseActive(ulong hUnitID, uint hSkillID, int iSkillLevel, int eSkillType);
    public void IEventSkillCondition(ulong hUnitID, uint hSkillID, int iConditionResult);
    public void IEventSkillOnOff(ulong hUnitID, uint hSkillID, int eSkillType, bool bOn);
}

public abstract class CFiniteStateMachineUnitSkillBase : CFiniteStateMachineBase
{
    private IEvenSkillHandler     m_pSkillHandler = null;

    private CCoolTime             m_pCoolTime = new CCoolTime();                protected CCoolTime GetFSMCoolTime() { return m_pCoolTime; }
    private CStateSkillBase       m_pCurrentState = null;                       protected ulong GetFSMUnitID() { return GetOperatorUnit().GetUnitInstanceID(); }
    private List<CStateSkillBase> m_listStateSkill = new List<CStateSkillBase>();
    private Dictionary<uint, CSkillContainerBase> m_mapSkillContainer = new Dictionary<uint, CSkillContainerBase>();
    //-------------------------------------------------------------------------
    protected override void OnFSMStateEnter(CStateBase pState)
    {
        base.OnFSMStateEnter(pState);
        m_pCurrentState = pState as CStateSkillBase;
    }

    protected override void OnFSMStateLeave(CStateBase pState)
    {
        base.OnFSMStateLeave(pState);
        PrivFSMSkillClear(pState as CStateSkillBase);
    }

    protected override void OnFSMStateRemove(CStateBase pState)
    {
        base.OnFSMStateRemove(pState);
        PrivFSMSkillClear(pState as CStateSkillBase);
    }

    protected override sealed void OnFSMStateAction(CStateBase pState, EStateEnterType eStateAction) { base.OnFSMStateAction(pState, eStateAction);}
    //---------------------------------------------------------------------------
    public void InitializeUnitFSMSkill(IEvenSkillHandler pEventSkillHandler)
    {
        m_pSkillHandler = pEventSkillHandler;
        OnFSMUnitSkillInitialize(pEventSkillHandler);
    }

    //--------------------------------------------------------------------------
    protected virtual void ProtFSMSkillAdd(CSkillContainerBase pSkillContainer)
    {
        if (m_mapSkillContainer.ContainsKey(pSkillContainer.SkillID) == false)
        {
            m_mapSkillContainer[pSkillContainer.SkillID] = pSkillContainer;
        }
    }

    protected int ProtFSMSkillCondition(CSkillUsageBase pSkillUsage)
    {
        int eConditionResult = 0;
        CSkillContainerBase pSkillContainer = FindSkillUsage(pSkillUsage.SkillID);
        if (pSkillContainer != null)
        {
            eConditionResult = pSkillContainer.SkillCondition.DoSkillCondition(pSkillUsage, this);
            m_pSkillHandler.IEventSkillCondition(GetFSMUnitID(), pSkillUsage.SkillID, eConditionResult);
        }
        return eConditionResult;
    }

    protected void ProtFSMSkillUsePassive(uint hSkillID)  // 발동 조건 체크를 하지 않고 바로 실행
    {
        CSkillContainerBase pSkillContainer = FindSkillUsage(hSkillID);
        if (pSkillContainer != null)
        {
            PrivFSMSkillUse(null, pSkillContainer);
            m_pSkillHandler.IEventSkillUsePassive(GetOperatorUnit().GetUnitInstanceID(), hSkillID, pSkillContainer.SkillLevel);
        }
    }

    protected void ProtFSMSkillUseActive(CSkillUsageBase pSkillUsage)
    {          
        CSkillContainerBase pSkillContainer = FindSkillUsage(pSkillUsage.SkillID);
        if (pSkillContainer != null)
        {
            m_pSkillHandler.IEventSkillUseActive(GetFSMUnitID(), pSkillUsage.SkillID, pSkillContainer.SkillLevel, pSkillContainer.SkillType);
            PrivFSMSkillUse(pSkillUsage, pSkillContainer);           
        }
    }


    //-----------------------------------------------------------------------------
    private CSkillContainerBase FindSkillUsage(uint hSkillID)
    {
        CSkillContainerBase pFindSkillContainer = null;
        m_mapSkillContainer.TryGetValue(hSkillID, out pFindSkillContainer);
        return pFindSkillContainer;
    }

    private void PrivFSMSkillUse(CSkillUsageBase pSkillUsage, CSkillContainerBase pSkillContainer)
    {
        for (int i = 0; i < pSkillContainer.SkillState.Count; i++)
        {
            CStateSkillBase pState = pSkillContainer.SkillState[i];
            pState.SetStateSkillUsage(pSkillUsage);
            OnFSMUnitSkillSkillUse(pSkillUsage, pSkillContainer);
            ProtFSMEnter(pState, EStateEnterType.Enter);
        }
    }

    private void PrivFSMSkillClear(CStateSkillBase pSkillState)
    {
        if (m_pCurrentState == pSkillState)
        {
            m_pCurrentState = null;
        }
    }

    //--------------------------------------------------------------------------
    protected virtual void OnFSMUnitSkillInitialize(IEvenSkillHandler pEventSkillHandler) { }
    protected virtual void OnFSMUnitSkillSkillUse(CSkillUsageBase pSkillUsage, CSkillContainerBase pSkillContainer) { }

}
