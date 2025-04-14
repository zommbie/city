using UnityEngine.Events;
using UnityEngine;
public abstract class CLoginBase
{

	//----------------------------------------------------------------------
	public void DoOAuthLogIn(string strReserveToken, UnityAction<CManagerLoginBase.ELoginResult, string, long> delFinish)
	{
		OnOAuthLogIn(strReserveToken, delFinish);
	}

	public void DoOAuthLogOut()
	{
		OnOAuthLogOut();
	}



	//-------------------------------------------------------------	
	protected virtual void OnOAuthLogIn(string strReserveToken, UnityAction<CManagerLoginBase.ELoginResult, string, long> delFinish) { delFinish?.Invoke(CManagerLoginBase.ELoginResult.Sucess, "None", 0); }
	protected virtual void OnOAuthLogOut() { }
}
