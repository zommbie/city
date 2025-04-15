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
            ProtSceneAttacherLoadAddressablePrefab(c_DLCPrefabDLCMainName, (bool bLoaded, GameObject pLoadedObject) =>
            {
                if (bLoaded)
                {
                    CDLCLoader pDLCLoader = pLoadedObject.GetComponent<CDLCLoader>();
                    if (pDLCLoader != null)
                    {
                        pDLCLoader.DoDLCLoaderActivate(() =>
                        {
                            PrivSceneAttacherLoadUISceneAndNetwork(delFinish);
                        });
                    }
                    else
                    {
                        PrivSceneAttacherLoadUISceneAndNetwork(delFinish);
                    }
                }
                else
                {
                    PrivSceneAttacherLoadUISceneAndNetwork(delFinish);
                }
            });



        });
    }

    private void PrivSceneAttacherLoadUISceneAndNetwork(UnityAction delFinish)
    {
        ProtSceneAttacherLoadUIScene(c_UIRootSceneName, c_UIRootScenePrefabName, () =>
        {
            ZCManagerLogin.Instance.DoMgrLoginStart(LoginType, (CManagerLoginBase.ELoginResult eResult) =>
            {
                if (eResult == CManagerLoginBase.ELoginResult.Sucess)
                {
                    delFinish.Invoke();
                    ProtSceneAttacherDestroy();
                }
            });
        });
    }

    //---------------------------------------------------------------------
    protected sealed override void ProtSceneAttacherDestroy()
    {
        base.ProtSceneAttacherDestroy();
    }

    protected sealed override void ProtSceneAttacherLoadAddressablePrefab(string strPrefabName, UnityAction<bool, GameObject> delFinish)
    {
        base.ProtSceneAttacherLoadAddressablePrefab(strPrefabName, delFinish);
    }

    protected sealed override void ProtSceneAttacherLoadComponent<TEMPLATE>(string strPrefabName, UnityAction<TEMPLATE> delFinish)
    {
        base.ProtSceneAttacherLoadComponent(strPrefabName, delFinish);
    }

    protected sealed override void ProtSceneAttacherLoadResourcePrefab(string strPrefabPath, string strPrefabName, UnityAction delFinish)
    {
        base.ProtSceneAttacherLoadResourcePrefab(strPrefabPath, strPrefabName, delFinish);
    }

    protected sealed override void ProtSceneAttacherLoadUIScene(string strUISceneName, string strUISceneRootPrefabName, UnityAction delFinish)
    {
        base.ProtSceneAttacherLoadUIScene(strUISceneName, strUISceneRootPrefabName, delFinish);
    }
}
