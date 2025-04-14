using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CManagerUnitActorBase : CManagerTemplateBase<CManagerUnitActorBase>
{
    private Dictionary<ulong, CUnitActorBase> m_mapUnitActorInstance = new Dictionary<ulong, CUnitActorBase>();
    //---------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------
    protected void ProtMgrUnitActorAdd(ulong hUnitInstanceID, uint hUnitTableID, CUnitActorBase pAddUnitChar)
    {
        if (m_mapUnitActorInstance.ContainsKey(hUnitInstanceID))
        {
            //Error!
        }
        else
        {
            m_mapUnitActorInstance[hUnitInstanceID] = pAddUnitChar;
            pAddUnitChar.InterUnitActorInitialize(hUnitInstanceID, hUnitTableID);
            OnMgrUnitActorAdd(hUnitInstanceID, hUnitTableID, pAddUnitChar);
        }
    }

    protected void ProtMgrUnitActorRemove(ulong hUnitInstanceID)
    {
        CUnitActorBase pUnitActor = null;
        m_mapUnitActorInstance.TryGetValue(hUnitInstanceID, out pUnitActor);
        if (pUnitActor != null)
        {
            m_mapUnitActorInstance.Remove(hUnitInstanceID);
            pUnitActor.InterUnitActorRemove();
            OnMgrUnitActorRemove(pUnitActor);
        }      
    }

    protected void ProtMgrUnitActorRemoveAll()
    {
        Dictionary<ulong, CUnitActorBase>.Enumerator it = m_mapUnitActorInstance.GetEnumerator();
        while(it.MoveNext())
        {
            CUnitActorBase pRemoveActor = it.Current.Value;
            pRemoveActor.InterUnitActorRemove();
            OnMgrUnitActorRemove(pRemoveActor);
        }
        m_mapUnitActorInstance.Clear();
    }


    protected Dictionary<ulong, CUnitActorBase>.Enumerator IterUnitActorList() { return m_mapUnitActorInstance.GetEnumerator(); }

    //---------------------------------------------------------------------------------
    protected CUnitActorBase FindMgrUnitActor(ulong hUnitID)
    {
        CUnitActorBase pFindActor = null;
        m_mapUnitActorInstance.TryGetValue(hUnitID, out pFindActor);
        return pFindActor;
    }

    //--------------------------------------------------------------------------------
    protected virtual void OnMgrUnitActorAdd(ulong hUnitInstanceID, uint hUnitTableID, CUnitActorBase pAddUnitChar) { }
    protected virtual void OnMgrUnitActorRemove(CUnitActorBase pAddUnitChar) { }
}
