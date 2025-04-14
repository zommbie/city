using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CSessionBase
{
	private MonoBehaviour m_pCoroutineObject = null;
	private UnityAction<int, string, int, float> m_delError = null; //  ID / Message / Data (토큰등)
    //--------------------------------------------------------------------
    internal void InterSessionInitialize(MonoBehaviour pCoroutineOwner, UnityAction<int, string, int, float> delError)
    {
        m_pCoroutineObject = pCoroutineOwner;
        m_delError = delError;
        OnSessionInitialize();
    }

    internal void InterSessionUpdate(float fDeltaTime)
	{
        OnSessionUpdate(fDeltaTime);
    }
   
    internal void InterSessionFocusOn()     // 프로세스가 다시 복귀 되었을 때
    {
        OnSessionFocusOn();
    }

    internal void InterSessionFocusOff()    // 홈버튼을 누르는 등 다른 앱으로 스위칭 되었을때 
    {
        OnSessionFocusOff();
    }

    internal void InterSessionFocusRemove()  // 어플리케이션이 종료될때 
    {
        OnSessionFocusRemove();
    }

    internal void InterSessionReset()
    {
        OnSessionReset();
    }
    //------------------------------------------------------------------
    public void SetSessionDestURL(string strDestURL)
	{
        OnSessionDestURL(strDestURL);
    }

	public void SetSessionPassPort(long pPassPort)
	{
		OnSessionPassPort(pPassPort);
	}

	//--------------------------------------------------------
	protected void ProtSessionCoroutineStart(IEnumerator pFunction)
	{
		if (pFunction == null || m_pCoroutineObject == null) return;
		m_pCoroutineObject.StartCoroutine(pFunction);
	}

	protected void ProtSessionError(int iErrorCode, string strMessage = null, int iArg = 0, float fArg = 0)
	{
		m_delError?.Invoke(iErrorCode, strMessage, iArg, fArg);
	}

	//---------------------------------------------------------
	protected virtual void OnSessionInitialize() { }
	protected virtual void OnSessionUpdate(float fDeltaTime) { }
    protected virtual void OnSessionFocusOn() { }
    protected virtual void OnSessionFocusOff() { }
    protected virtual void OnSessionFocusRemove() { }
    protected virtual void OnSessionReset() { }
    protected virtual void OnSessionDestURL(string strDestURL) { }
    protected virtual void OnSessionPassPort(long strSessionToken) { }
}
