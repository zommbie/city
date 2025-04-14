using System.Collections;

public abstract class CStateBase
{
    private IEnumerator m_pEnumerator = null;          public IEnumerator GetStateEnumerator() { return m_pEnumerator; }   // 외부에서 코루틴을 종료할때
    private CFiniteStateMachineBase m_pFSMOwnr = null; protected CFiniteStateMachineBase GetStateFSMOwner() { return m_pFSMOwnr; }
    //------------------------------------------------------------------------
    internal void InterStateInitialize(CFiniteStateMachineBase pFSMOwner)
    {
        m_pFSMOwnr = pFSMOwner;
        OnStateInitialize(pFSMOwner);
    }
    internal void InterStateEnterAnother(CStateBase pStatePrev)
    {
        OnStateEnterAnother(pStatePrev);
    }
    internal void InterStateEnter(CStateBase pStatePrev)
    {
        OnStateEnter(pStatePrev);
    }
    internal void InterStateLeave(CStateBase pStatePrev)
    {
        OnStateLeave(pStatePrev);
    }
    internal void InterStateRemove()
    {
        OnStateRemove();
    }
    internal void InterStateInterrupted(CStateBase pStateInterrupt)
    {
        OnStateInterrupted(pStateInterrupt);
    }
    internal void InterStateInterruptedResume(CStateBase pStateInterrupt)
    {
        OnStateInterruptResume(pStateInterrupt);
    }
    internal void InterStateForceClear() // 스테이트가 강제로 시스템에서 제거될때. 스테이트에서 할당된 리소스나 레퍼 제거
	{
        OnStateForceClear();
    }
    internal void InterStateUpdate(float fDelta)
	{
        OnStateUpdate(fDelta);
	}
    internal void InterStateUpdateInStack(float fDelta)
	{
        OnStateUpdateInStack(fDelta);
	}
    internal void InterStateUpdateInInterrupt(float fDelta)
	{
        OnStateUpdateInInterrupt(fDelta);
	}
    internal IEnumerator InterStateActivateCoroutine()
	{
        m_pEnumerator = OnStateActivateCoroutine();
        return m_pEnumerator;
	}
    
    //--------------------------------------------------------------------------
    protected void ProtStateSelfEnd()
	{
        m_pFSMOwnr?.InterFSMLeave(this);
    } 
    protected void ProtStateSelfRemove()
    {
        m_pFSMOwnr?.InterFSMRemove(this);
    }
	//--------------------------------------------------------------------------	
    protected virtual void OnStateInitialize(CFiniteStateMachineBase pFSMOwner) {}
    protected virtual void OnStateEnterAnother(CStateBase pStatePrev) {}
	protected virtual void OnStateEnter(CStateBase pStatePrev) {}
    protected virtual void OnStateRemove() { }
    protected virtual void OnStateLeave(CStateBase pStatePrev) {}	
    protected virtual void OnStateInterrupted(CStateBase pStateInterrupt) {}
	protected virtual void OnStateInterruptResume(CStateBase pStateInterrupt) { }
    protected virtual void OnStateUpdate(float fDelta) { }
    protected virtual void OnStateUpdateInInterrupt(float fDelta) { }
    protected virtual void OnStateUpdateInStack(float fDelta) { }
    protected virtual void OnStateForceClear() { }
    protected virtual IEnumerator OnStateActivateCoroutine() { yield break; }	
	//---------------------------------------------------------------------------
}
