using System.Collections;
using System.Collections.Generic;

public interface IEventUnitHandler
{

}

public abstract class CUnitCoreBase
{
    private IEventUnitHandler m_pEventUnitHandler = null;
    private ulong m_hUnitID = 0;           public ulong GetUnitInstanceID() { return m_hUnitID; }            // 서버의 인스턴스 아이디 이다.
    private uint m_hTableID = 0;          public uint GetBattleUnitTableID() { return m_hTableID; }      // 유닛 테이블 등의 아이디 이다
    private bool m_bAlive = false;        public bool IsAlive { get { return m_bAlive; } }               // 타겟등의 상호작용을 하는가?
    private bool m_bResurrect = false;    public bool IsResurrect { get { return m_bResurrect; } }       // 부활 가능한가?
    private bool m_bRemove = false;       public bool IsRemove { get { return m_bRemove; } }             // 전장에서 제거되었나? (일체 상호작용 안됨)
    private bool m_bControl = true;       public bool IsControl { get { return m_bControl; } }     // CC 등에 의해서 조작이 불가능 한지
    private List<COperatorBase> m_listOperator = new List<COperatorBase>();
    //-----------------------------------------------------------------------------------
    public void UpdateUnit(float fDelta)
    {
        for (int i = 0; i < m_listOperator.Count; i++)
        {
            m_listOperator[i].InterOperatorUpdate(fDelta);
        }
        OnBattleUnitUpdate(fDelta);
    }

    public void UpdateUnitLate()
    {
        for (int i = 0; i < m_listOperator.Count; i++)
        {
            m_listOperator[i].InterOperatorUpdatelate();
        }
    }

    //------------------------------------------------------------------------------------
    protected virtual void ProtBattleUnitInitialize(ulong hInstanceID, uint hUnitID, IEventUnitHandler pEventUnitHandler)
    {
        m_hUnitID = hInstanceID;
        m_hTableID = hUnitID;
        m_bAlive = false;
        m_bRemove = false;
        m_pEventUnitHandler = pEventUnitHandler;
        OnBattleUnitInitialize(pEventUnitHandler);
    }

    protected virtual void ProtBattleUnitOperatorAdd(COperatorBase pOperator)
    {
        pOperator.InterOperatorInitialize(this);
        m_listOperator.Add(pOperator);
    }

    //-----------------------------------------------------------------------------------
    protected void ProtBattleUnitStateChange(int eUnitState, int iValue = 0, float fValue = 0f)
    {
        for (int i = 0; i < m_listOperator.Count; i++)
        {
            m_listOperator[i].InterOperatorStateChange(eUnitState, iValue, fValue);
        }
        OnBattleUnitStateChange(eUnitState, iValue, fValue);
    }

    protected virtual void SetUnitAlive(bool bAlive) { m_bAlive = bAlive; }
    protected virtual void SetUnitResurrect(bool bResurrect) { m_bResurrect = bResurrect; }
    protected virtual void SetUnitRemove(bool bRemove) { m_bRemove = bRemove; }
    protected virtual void SetUnitControl(bool bControl) { m_bControl = bControl; }

    //------------------------------------------------------------------------------------
    protected virtual void OnBattleUnitInitialize(IEventUnitHandler pEventUnitHandler) { }
    protected virtual void OnBattleUnitStateChange(int eUnitState, int iValue, float fValue) { }
    protected virtual void OnBattleUnitUpdate(float fDelta) { }
    protected virtual void OnBattleUnitUpdateLate() { }
}
