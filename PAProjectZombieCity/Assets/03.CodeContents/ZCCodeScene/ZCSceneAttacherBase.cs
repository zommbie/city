using UnityEngine;
using UnityEngine.Events;

public abstract class ZCSceneAttacherBase : CSceneAttacherBase
{
    [SerializeField]
    private CManagerLoginBase.ELoginType LoginType = CManagerLoginBase.ELoginType.LoginVirtual;

    protected const string c_ManagerPrefabPath      = "FrontAsset";
    protected const string c_ManagerPrefabName      = "ZCPrefabManager";
    protected const string c_DLCPrefabDLCMainName   = "ZCPrefabDLCMain";
    protected const string c_UIRootSceneName        = "ZCSceneUIRoot";
    protected const string c_UIRootScenePrefabName  = "UISystemRoot";

    //-----------------------------------------------------------------------------------------------
    protected void ProtSceneAttacherBasicLoad(UnityAction delFinish)
    {
        ProtSceneAttacherLoadResourcePrefab(c_ManagerPrefabPath, c_ManagerPrefabName, () => { 
            
        
        
        
        });
    }
}
