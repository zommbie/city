using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitActorBase : CMonoBase
{
    private bool m_bEnableUpdate = true;
    private CUnitActorBase m_pOwnerUnitChar = null;  public CUnitActorBase GetAssistOwnerActor() { return m_pOwnerUnitChar; }
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
	internal void InterUnitActorMessage(int eMessageType, int iArg = 0, float fArg = 0f, string strArg = null, params object [] aParams)
    {
        OnAssistUnitActorMessage(eMessageType, iArg, fArg, strArg, aParams);
    }

    internal void InterUnitActorStatus(CUnitActorBase.EActorStatus eStaus)
    {
        OnAssistUnitActorStatus(eStaus);
    }

    internal void InterUnitActorInitialize(CUnitActorBase pOwnerUnitChar)
    {
        m_pOwnerUnitChar = pOwnerUnitChar;
        OnAssistInitialize(pOwnerUnitChar);
    }

    internal void InterUnitActorRemove()
    {
        OnAssistUnitActorRemove();
    }

    //-------------------------------------------------------
    public void SetAssistUpdateEnable(bool bEnable)
    {
        m_bEnableUpdate = bEnable;
    }

    //---------------------------------------------------------
    protected virtual void OnAssistInitialize(CUnitActorBase pOwner) { }
    protected virtual void OnAssistUpdate(float fDelta) { }
    protected virtual void OnAssistFixedUpdate(float fDelta) { }
    protected virtual void OnAssistLateUpdate() { }
    protected virtual void OnAssistUnitActorMessage(int eMessageType, int iArg = 0, float fArg = 0f, string strArg = null, params object[] aParams) { }
    protected virtual void OnAssistUnitActorStatus(CUnitActorBase.EActorStatus eStaus) { }
    protected virtual void OnAssistUnitActorRemove() { }
   
}
