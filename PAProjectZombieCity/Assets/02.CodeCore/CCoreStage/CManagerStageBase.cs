using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class CManagerStageBase : CManagerTemplateBase<CManagerStageBase>
{
    private List<CStageBase> m_listStageInstance = new List<CStageBase>();
    //------------------------------------------------------------
    protected override void OnUnityAwake()
    {
        base.OnUnityAwake();
        PrivMgrStageInstance();
    }

    //--------------------------------------------------------------
    protected void ProtMgrStageLoad(uint hStageID, UnityAction delFinish, params object [] aParams)
    {
        PrivMgrStageLoad(hStageID, delFinish, aParams);
    }

    //-------------------------------------------------------------
    private void PrivMgrStageInstance()
    {
        if (m_listStageInstance.Count == 0)
        {
            GetComponentsInChildren(true, m_listStageInstance);
        }
    }


    private void PrivMgrStageLoad(uint hStageID, UnityAction delFinish, params object[] aParams)
    {
        PrivMgrStageInstance();

        int iComplete = 0;
        for (int i = 0; i < m_listStageInstance.Count; i++)
        {
//            m_listStageInstance[i]
        }
    }



    //-------------------------------------------------------------
    public CManagerStageBase() : base(false) { }
}
