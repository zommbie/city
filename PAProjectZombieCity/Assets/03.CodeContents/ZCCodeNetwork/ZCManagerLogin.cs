using UnityEngine;
using UnityEngine.Events;

public class ZCManagerLogin : CManagerLoginBase
{   public static new ZCManagerLogin Instance { get { return CManagerLoginBase.Instance as ZCManagerLogin; } }

    //---------------------------------------------------------------------------
    protected override CLoginBase OnMgrLoginGuest(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
    {
        CLoginBase pLoginInstance = new ZCLoginGuest();
        pLoginInstance.DoOAuthLogIn(strReserveToken, delFinish);
        return pLoginInstance;
    }

    protected override CLoginBase OnMgrLoginVirtual(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
    {
        CLoginBase pLoginInstance = new ZCLoginVirtual();
        pLoginInstance.DoOAuthLogIn(strReserveToken, delFinish);
        return pLoginInstance;
    }

    protected override CLoginBase OnMgrLoginDevelop(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
    {
        CLoginBase pLoginInstance = new ZCLoginDevelop();
        pLoginInstance.DoOAuthLogIn(strReserveToken, delFinish);
        return pLoginInstance;
    }

    protected override CLoginBase OnMgrLoginGoogle(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
    {
        CLoginBase pLoginInstance = new ZCLoginGooglePlay();
        pLoginInstance.DoOAuthLogIn(strReserveToken, delFinish);
        return pLoginInstance;
    }

    protected override CLoginBase OnMgrLoginAppleStore(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
    {
        CLoginBase pLoginInstance = new ZCLoginAppleStore();
        pLoginInstance.DoOAuthLogIn(strReserveToken, delFinish);
        return pLoginInstance;
    }

    protected override CLoginBase OnMgrLoginGuestLocal(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
    {
        CLoginBase pLoginInstance = new ZCLoginGuestLocal();
        pLoginInstance.DoOAuthLogIn(strReserveToken, delFinish);
        return pLoginInstance;
    }
    //-----------------------------------------------------------------------------
}
