using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

abstract public class CManagerScriptDataBase : CManagerTemplateBase<CManagerScriptDataBase>, CDLCLoader.IDLCLoadAction
{
    private List<CScriptDataBase> m_listScriptData = new List<CScriptDataBase>();
    //-----------------------------------------------------------------
    public void IPreviousLoadContents(UnityAction delFinish)
    {
        GetComponentsInChildren(true, m_listScriptData);

        for (int i = 0; i < m_listScriptData.Count; i++)
        {
            m_listScriptData[i].InterScriptDataLoad();
            OnMgrScriptDataLoaded(m_listScriptData[i]);
        }

        GlobalManagerScriptDataLoaded();
        delFinish.Invoke();
    }
    //------------------------------------------------------------------
    

    protected virtual void OnMgrScriptDataLoaded(CScriptDataBase pLoadedScriptData) { }
	
}
