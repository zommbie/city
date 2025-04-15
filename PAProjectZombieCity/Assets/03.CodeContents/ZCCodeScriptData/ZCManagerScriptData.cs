using UnityEngine;

public class ZCManagerScriptData : CManagerScriptDataBase
{
    public static new ZCManagerScriptData Instance { get { return CManagerScriptDataBase.Instance as ZCManagerScriptData; } }



    //-------------------------------------------------------------------------------------
    protected override void OnMgrScriptDataLoaded(CScriptDataBase pLoadedScriptData)
    {

    }
}
