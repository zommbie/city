using System.Collections.Generic;
using System.Linq;
// 플렛폼 독립적인 실행을 위해서 유니티 요소를 제거한 버전

public abstract class CFiniteStateMachineBase : COperatorBase
{
	public enum EStateEnterType
	{
		Enter,          //새로운 스테이트는 큐에 대기. 활성 스테이트는 이벤트를 받음. 아이들 스테이트 등에 사용. 각 스테이트는 자신이 언제 언스택 될지 결정한다.
		Interrupt,      //활성 스테이트는 스텍에 보존되며 새로운 스테이트가 활성화된다.
		EnterForce,     //새로운 스테이트는 즉시 활성화되며 모든 큐와 스텍이 강제 종료된다. 
	}

	private Stack<CStateBase> m_stackInterruptedState	    = new Stack<CStateBase>();
	private Queue<CStateBase> m_queueWaitState				= new Queue<CStateBase>();

    private bool m_bActiveState = false;
	private CStateBase m_pStateCurrent = null;			    protected CStateBase GetFSMCurrentState() { return m_pStateCurrent; }
	private CStateBase m_pStatePrev = null;

    //-------------------------------------------------------------
    protected override sealed void OnOperatorUpdate(float fDelta)
    {
        base.OnOperatorUpdate(fDelta);
        UpdateFSMCheckChange();
        UpdateFSMState(fDelta);
        OnFSMUpdate(fDelta);
    }

    //------------------------------------------------------------
    internal void InterFSMLeave(CStateBase pLeaveState)
    {
        ProtFSMLeave(pLeaveState);
    }
    internal void InterFSMRemove(CStateBase pRemoveState)
    {
        ProtFSMRemove(pRemoveState);
    }

    //-------------------------------------------------------------
	protected virtual void ProtFSMLeave(CStateBase pStateLeave)
	{
		PrivFSMLeave(pStateLeave);
		OnFSMStateLeave(pStateLeave);
	}

    protected virtual void ProtFSMRemove(CStateBase pStateRemove)  // 외부에서 강제 종료 되었을때. 생존스킬을 사용했을때 모든 CC 스태이트를 제거등
    {
        PrivFSMRemove(pStateRemove);
		OnFSMStateRemove(pStateRemove);
	}

	protected virtual void ProtFSMEnter(CStateBase pState, EStateEnterType eEnterType)
	{
        m_bActiveState = true;
		PrivFSMEnter(pState, eEnterType);
		OnFSMStateAction(pState, eEnterType);
	}

	protected virtual void ProtFSMClearAll()
	{
		PrivFSMClearAll();
		OnFSMStateClearAll();
	}
    //--------------------------------------------------------
	public bool IsFSMEmpty()
	{
		bool bEmpty = false;
		if (m_queueWaitState.Count == 0 && m_stackInterruptedState.Count == 0 && m_pStateCurrent == null)
		{
			bEmpty = true;
		}
		return bEmpty;
	}

	//----------------------------------------------------------
	private void PrivFSMEnter(CStateBase pState, EStateEnterType eStateAction)
	{
		pState.InterStateInitialize(this);

		switch (eStateAction)
		{
			case EStateEnterType.Enter:
				PrivFSMActionEnter(pState);
				break;
			case EStateEnterType.EnterForce:
				PrivFSMActionEnterForce(pState);
				break;
			case EStateEnterType.Interrupt:
				PrivFSMActionInterrupt(pState);
				break;
		}
	}

	private void PrivFSMLeave(CStateBase pState)
	{
		if (m_pStateCurrent != pState) return;

		pState.InterStateLeave(m_pStatePrev);
		m_pStateCurrent = null;
	}

    private void PrivFSMRemove(CStateBase pState)
    {
        if (m_pStateCurrent == pState)
		{
			pState.InterStateRemove();
			m_pStateCurrent = null;
		}
		else
		{
			if (m_queueWaitState.Contains(pState))
			{
				m_queueWaitState = new Queue<CStateBase>(m_queueWaitState.Where(x => x != pState)); // 스택은 제거가 불가능하므로 새로 할당한다.
			}

			if (m_stackInterruptedState.Contains(pState))
			{
				m_stackInterruptedState = new Stack<CStateBase>(m_stackInterruptedState.Where(x => x != pState));
			}
		}
    }

	private void PrivFSMClearAll()
	{
		Stack<CStateBase>.Enumerator itInterrupt = m_stackInterruptedState.GetEnumerator();
		while(itInterrupt.MoveNext())
		{
			itInterrupt.Current.InterStateForceClear();
		}
		m_stackInterruptedState.Clear();

		Queue<CStateBase>.Enumerator itState = m_queueWaitState.GetEnumerator();
		while(itState.MoveNext())
		{
			itState.Current.InterStateForceClear();
		}
		m_queueWaitState.Clear();

		if (m_pStateCurrent != null)
		{
			m_pStateCurrent.InterStateRemove();
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

    private void UpdateFSMCheckChange()
    {
        if (m_pStateCurrent != null) return;

        if (PrivFSMCheckChangeInterrupt() == false)
        {
            PrivFSMCheckChangeQueue();
        }
    }

    private void UpdateFSMStateInStack(float fDelta)
	{
		Queue<CStateBase>.Enumerator it = m_queueWaitState.GetEnumerator();
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
    private void PrivFSMActionEnter(CStateBase pState)
	{
		if (m_pStateCurrent != null)
		{
			m_pStateCurrent.InterStateEnterAnother(pState);
		}

		PrivFSMEnter(pState);
	}

	private void PrivFSMActionEnterForce(CStateBase pState)
	{
		PrivFSMClearAll();
		PrivFSMEnter(pState);
	}

	private void PrivFSMActionInterrupt(CStateBase pState)
	{
		if (m_pStateCurrent != null)
		{
			m_stackInterruptedState.Push(m_pStateCurrent);
			m_pStateCurrent.InterStateInterrupted(pState);
		}

		PrivFSMActivate(pState);
	}

	private void PrivFSMEnter(CStateBase pState)
	{		
        if (m_queueWaitState.Count == 0 && m_stackInterruptedState.Count == 0 && m_pStateCurrent == null)
        {
            PrivFSMActivate(pState);
        }
        else
        {
            m_queueWaitState.Enqueue(pState);
        }
    }

	//-------------------------------------------------------------


	private bool PrivFSMCheckChangeInterrupt()
	{
		bool bUpdate = false;
		if (m_stackInterruptedState.Count > 0)
		{
			bUpdate = true;
			CStateBase pState = m_stackInterruptedState.Pop();
			pState.InterStateInterruptedResume(m_pStatePrev);
			PrivFSMActivate(pState);
		}
		return bUpdate;
	}

	private void PrivFSMCheckChangeQueue()
	{
		if (m_queueWaitState.Count > 0)
		{
			CStateBase pState = m_queueWaitState.Dequeue();			
			PrivFSMActivate(pState);
		}
		else
		{			
            if (m_bActiveState)
            {
                m_bActiveState = false;
                OnFSMStateEmpty();
            }
        }
	}

	private void PrivFSMActivate(CStateBase pState)
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
