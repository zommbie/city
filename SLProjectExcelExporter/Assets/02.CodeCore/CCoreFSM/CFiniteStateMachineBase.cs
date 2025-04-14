
using System.Collections.Generic;
// 플렛폼 독립적인 실행을 위해서 유니티 요소를 제거한 버전

public abstract class CFiniteStateMachineBase 
{
	public enum EStateEnterType
	{
		Enter,          //새로운 스테이트는 큐에 대기
		Interrupt,      //새로운 스테이트는 스텍에 보존되며 새로운 스테이트가 활성화된다.
		EnterForce,     //새로운 스테이트는 즉시 활성화되며 모든 큐와 스텍이 강제 종료된다.
	}

	private Stack<CStateBase> m_stackInterruptedState = new Stack<CStateBase>();
	private Queue<CStateBase> m_queueState = new Queue<CStateBase>();

	private CStateBase m_pStateCurrent = null;  protected CStateBase GetFSMCurrentState() { return m_pStateCurrent; }
	private CStateBase m_pStatePrev = null;
	
	//-------------------------------------------------------------
	internal void InterFSMUpdate(float fDelta)
	{
		UpdateFSMState(fDelta);
		OnFSMUpdate(fDelta);
	}

	internal void InterFSMCheckChange()
	{
		if (m_pStateCurrent == null)
		{
			PrivFSMChecChange();
		}
	}

	internal void InterFSMLeave(CStateBase pStateLeave)
	{
		PrivFSMLeave(pStateLeave);
		OnFSMStateLeave(pStateLeave);
	}

    internal void InterFSMRemove(CStateBase pStateRemove)  // 외부에서 강제 종료 되었을때
    {
        PrivFSMRemove(pStateRemove);
		OnFSMStateRemove(pStateRemove);
	}

	internal void InterFSMAction(CStateBase pState, EStateEnterType eStateAction)
	{
		PrivFSMAction(pState, eStateAction);
		OnFSMStateAction(pState, eStateAction);
	}

	internal void InterFSMClearAll()
	{
		PrivFSMClearAll();
		OnFSMStateClearAll();
	}

	public bool IsStateEmpty()
	{
		bool bEmpty = false;
		if (m_queueState.Count == 0 && m_stackInterruptedState.Count == 0 && m_pStateCurrent == null)
		{
			bEmpty = true;
		}
		return bEmpty;
	}

	//----------------------------------------------------------
	private void PrivFSMAction(CStateBase pState, EStateEnterType eStateAction)
	{
		pState.InterStateInitialize(this);

		switch (eStateAction)
		{
			case EStateEnterType.Enter:
				PrivStateActionEnter(pState);
				break;
			case EStateEnterType.EnterForce:
				PrivStateActionEnterForce(pState);
				break;
			case EStateEnterType.Interrupt:
				PrivStateActionInterrupt(pState);
				break;
		}
	}

	private void PrivFSMLeave(CStateBase pState)
	{
		if (m_pStateCurrent != pState) return;

		pState.InterStateLeave(m_pStateCurrent);
		m_pStateCurrent = null;
		
	}

    private void PrivFSMRemove(CStateBase pState)
    {
        if (m_pStateCurrent != pState) return;
        pState.InterStateRemove(m_pStateCurrent);
        m_pStateCurrent = null;
    }

	private void PrivFSMClearAll()
	{
		Stack<CStateBase>.Enumerator it = m_stackInterruptedState.GetEnumerator();
		while(it.MoveNext())
		{
			it.Current.InterStateForceClear();
		}
		m_stackInterruptedState.Clear();

		Queue<CStateBase>.Enumerator it2 = m_queueState.GetEnumerator();
		while(it2.MoveNext())
		{
			it2.Current.InterStateForceClear();
		}
		m_queueState.Clear();

		if (m_pStateCurrent != null)
		{
			m_pStateCurrent.InterStateLeave(m_pStatePrev);
			m_pStateCurrent.InterStateRemove(m_pStatePrev);
			m_pStateCurrent.InterStateForceClear();
		}

		m_pStateCurrent = null;
		m_pStatePrev = null;
	}

    //------------------------------------------------------------
	private void UpdateFSMState(float fDelta)
	{
		if (m_pStateCurrent != null)
		{
			m_pStateCurrent.InterStateUpdate(fDelta);
		}
		UpdateFSMStateInInterrupt(fDelta);
		UpdateFSMStateInStack(fDelta);
	}

	private void UpdateFSMStateInStack(float fDelta)
	{
		Queue<CStateBase>.Enumerator it = m_queueState.GetEnumerator();
		while (it.MoveNext())
		{
			it.Current.InterStateUpdateInStack(fDelta);
		}
	}

	private void UpdateFSMStateInInterrupt(float fDelta)
	{
		Stack<CStateBase>.Enumerator it = m_stackInterruptedState.GetEnumerator();
		while (it.MoveNext())
		{
			it.Current.InterStateUpdateInInterrupt(fDelta);
		}
	}

	//----------------------------------------------------------
    private void PrivStateActionEnter(CStateBase pState)
	{
		if (m_pStateCurrent != null)
		{
			m_pStateCurrent.InterStateEnterAnother(pState);
		}

		PrivStateActivate(pState);
	}

	private void PrivStateActionEnterForce(CStateBase pState)
	{
		PrivStateClearAll(pState);
		PrivStateActivate(pState);
	}

	private void PrivStateActionInterrupt(CStateBase pState)
	{
		if (m_pStateCurrent != null)
		{
			m_stackInterruptedState.Push(m_pStateCurrent);
			m_pStateCurrent.InterStateInterrupted(pState);
		}

		PrivStateCurrentActivate(pState);
	}

	private void PrivStateClearAll(CStateBase pStateNew)
	{
		Stack<CStateBase>.Enumerator itStack = m_stackInterruptedState.GetEnumerator();
		while (itStack.MoveNext())
		{
			itStack.Current.InterStateRemove(pStateNew);
		}
		m_stackInterruptedState.Clear();

		Queue<CStateBase>.Enumerator itQueue = m_queueState.GetEnumerator();
		while (itQueue.MoveNext())
		{
			itQueue.Current.InterStateRemove(pStateNew);
		}
		m_queueState.Clear();

		if (m_pStateCurrent != null)
		{
			m_pStateCurrent.InterStateLeave(pStateNew);
			m_pStateCurrent.InterStateRemove(pStateNew);
		}
	}

	private void PrivStateActivate(CStateBase pState)
	{		
        if (m_queueState.Count == 0 && m_stackInterruptedState.Count == 0 && m_pStateCurrent == null)
        {
            PrivStateCurrentActivate(pState);
        }
        else
        {
            m_queueState.Enqueue(pState);
        }
    }

	//-------------------------------------------------------------
	private void PrivFSMChecChange()
	{
		if (PrivStateUpdateInterrupt() == false)
		{
			PrivStateUpdateQueue();
		}
	}

	private bool PrivStateUpdateInterrupt()
	{
		bool bUpdate = false;
		if (m_stackInterruptedState.Count > 0)
		{
			bUpdate = true;
			CStateBase pState = m_stackInterruptedState.Pop();
			pState.InterStateInterruptedResume(m_pStatePrev);
			PrivStateCurrentActivate(pState);
		}
		return bUpdate;
	}

	private void PrivStateUpdateQueue()
	{
		if (m_queueState.Count > 0)
		{
			CStateBase pState = m_queueState.Dequeue();			
			PrivStateCurrentActivate(pState);
		}
		else
		{			
			OnFSMStateEmpty();
		}
	}

	private void PrivStateCurrentActivate(CStateBase pState)
	{
        m_pStatePrev = m_pStateCurrent;
        m_pStateCurrent = pState;       
        pState.InterStateEnter(m_pStatePrev);

        OnFSMStateEnter(pState);
	}

	//---------------------------------------------------------------------
	protected virtual void OnFSMStateEnter(CStateBase pState) { }
	protected virtual void OnFSMStateLeave(CStateBase pState) { }
    protected virtual void OnFSMStateRemove(CStateBase pState) { }
	protected virtual void OnFSMStateEmpty() { }
	protected virtual void OnFSMStateAction(CStateBase pState, EStateEnterType eStateAction) { }
	protected virtual void OnFSMStateClearAll() { }
	protected virtual void OnFSMUpdate(float fDelta) { }
}
