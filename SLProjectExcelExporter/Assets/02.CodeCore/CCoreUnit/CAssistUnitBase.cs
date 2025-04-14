using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitBase : CMonoBase
{
    private bool m_bEnableUpdate = true;
    private CUnitBase.EUnitState m_eAssistOwnerState; protected CUnitBase.EUnitState GetAssistUnitState() { return m_eAssistOwnerState; }
    protected CUnitBase m_pAssistOwner = null; 
    
    //------------------------------------------------------
    private void Update()
    {
        if (m_bEnableUpdate)
        {
            OnAssistUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (m_bEnableUpdate)
        {
            OnAssistFixedUpdate(Time.fixedDeltaTime);
        }
    }

	private void LateUpdate()
	{
		if (m_bEnableUpdate)
		{
            OnAssistLateUpdate();
		}
	}

	//------------------------------------------------------
	internal void InterAssistInitialize(CUnitBase pOwner)
    {
        m_pAssistOwner = pOwner;
        OnAssistInitialize(pOwner);
    }

    internal void InterAssistUnitState(CUnitBase.EUnitState eState)
    {
        m_eAssistOwnerState = eState;
        OnAssistUnitState(eState);
    }

    internal void InterAssistUnitGameStart()
    {
        OnAssistUnitGameStart();
    }

    internal void InterAssistUnitGameEnd()
    {
        OnAssistUnitGameEnd();
    }

    //-------------------------------------------------------
    public void SetAssistUpdateEnable(bool bEnable)
    {
        m_bEnableUpdate = bEnable;
    }

    //---------------------------------------------------------
    protected virtual void OnAssistInitialize(CUnitBase pOwner) { }
    protected virtual void OnAssistUpdate(float fDelta) { }
    protected virtual void OnAssistFixedUpdate(float fDelta) { }
    protected virtual void OnAssistLateUpdate() { }
    protected virtual void OnAssistUnitState(CUnitBase.EUnitState eState) { }
    protected virtual void OnAssistUnitGameStart() { }
    protected virtual void OnAssistUnitGameEnd() { }
}
