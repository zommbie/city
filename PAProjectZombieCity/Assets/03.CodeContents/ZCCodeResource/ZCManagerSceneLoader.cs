using UnityEngine;

public class ZCManagerSceneLoader : CManagerSceneLoaderBase
{   public static new ZCManagerSceneLoader Instance { get { return CManagerSceneLoaderBase.Instance as ZCManagerSceneLoader; } }
    public const string c_MasterSceneName = "ZCSceneMaster";
    //------------------------------------------------------------------------
    public enum ESceneName
    {
        None,
        ZCSceneLogin,
        ZCSceneLobby,
        ZCSceneBattle,
    }



}
