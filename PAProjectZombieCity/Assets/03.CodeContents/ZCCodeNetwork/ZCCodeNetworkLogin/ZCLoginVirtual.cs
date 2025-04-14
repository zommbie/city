using UnityEngine.Events;

public class ZCLoginVirtual : CLoginBase
{
    protected override void OnOAuthLogIn(string strReserveToken, UnityAction<CManagerLoginBase.ELoginResult, string, long> delFinish)
    {
     //   SSManagerGameDB.Instance.DoMgrGameDBInitialize(SSManagerGameDB.EGameDBType.Virtual, () =>
        {
            delFinish.Invoke(CManagerLoginBase.ELoginResult.Sucess, "None", 0);
        });
    }
}
