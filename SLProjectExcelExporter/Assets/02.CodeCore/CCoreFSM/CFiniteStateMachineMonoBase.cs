using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FSM을 유니티에서 사용 할 수 있도록 하는 브릿지
public abstract class CFiniteStateMachineMonoBase : CAssistUnitBase
{

	private CFiniteStateMachineBase m_pFSMInstance = null;
	//------------------------------------------------------------
	protected override void OnAssistInitialize(CUnitBase pOwner)
	{
		base.OnAssistInitialize(pOwner);
		m_pFSMInstance = MakeFSMInstance();
	}

	protected override sealed void OnAssistLateUpdate()
	{
		//일반 업데이트가 끝난 이후 스테이트 교체가 발생
		m_pFSMInstance.InterFSMCheckChange();

		
	}

	//-------------------------------------------------------------
	protected abstract CFiniteStateMachineBase MakeFSMInstance();
}
