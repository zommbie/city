using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class CManagerStageBase : CManagerTemplateBase<CManagerStageBase>
{
    private List<CStageBase> m_listStageInstance = new List<CStageBase>();
    //--------------------------------------------------------------
    protected override void OnManagerUISceneLoaded()
    {
        PrivMgrStageInstance();
    }

    //--------------------------------------------------------------
    public void DoMgrStageLoad(uint hStageID, UnityAction delFinish, params object [] aParams)
    {
        PrivMgrStageLoad(hStageID, delFinish, aParams);
        OnMgrStageLoad(hStageID, delFinish, aParams);
    }

    public void DoMgrStageStart(int iOption = 0)
    {

    }

    public void DoMgrStageEnd(int iResult = 0)
    {

    }

    public void DoMgrStageReset(int iOption = 0)
    {

    }

    public void DoMgrStageExit()
    {

    }

    //-------------------------------------------------------------
    private void PrivMgrStageInstance()
    {
        if (m_listStageInstance.Count == 0)
        {
            GetComponentsInChildren(true, m_listStageInstance);
            for (int i = 0; i < m_listStageInstance.Count; i++)
            {
                m_listStageInstance[i].InterStageInitialize();
            }
        }
    }


    private void PrivMgrStageLoad(uint hStageID, UnityAction delFinish, params object[] aParams)
    {
        PrivMgrStageInstance();

        int iComplete = 0;
        for (int i = 0; i < m_listStageInstance.Count; i++)
        {
            m_listStageInstance[i].InterStageLoad(hStageID, () => {
                iComplete++;
                if (iComplete == m_listStageInstance.Count)
                {
                    delFinish?.Invoke();
                }
            }, aParams);
        }
    }


    //------------------------------------------------------------
    protected virtual void OnMgrStageLoad(uint hStageID, UnityAction delFinish, params object[] aParams) { }
    protected virtual void OnMgrStageStart(int iOption) { }
    protected virtual void OnMgrStageEnd(int iResult) { }
    protected virtual void OnMgrStageReset(int iOption) { }
    protected virtual void OnMgrStageExit() { }
    //-------------------------------------------------------------
    public CManagerStageBase() : base(false) { }
}
