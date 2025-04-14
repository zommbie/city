using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class CLoginVirtualBase : CLoginBase
{
    //---------------------------------------------------------------------
    protected override void OnOAuthLogIn(string strReserveToken, UnityAction<CManagerLoginBase.ELoginResult, string, long> delFinish) 
    { 
        delFinish?.Invoke(CManagerLoginBase.ELoginResult.Sucess, "None", 0);     
    }
    
    protected override void OnOAuthLogOut() 
    {
    
    }
}
