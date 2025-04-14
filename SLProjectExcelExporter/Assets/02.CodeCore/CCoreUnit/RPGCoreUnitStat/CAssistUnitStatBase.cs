using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitStatBase : CAssistUnitBase
{

    private Dictionary<uint, CStatBase> m_mapStatInstance = new Dictionary<uint, CStatBase>();
	//----------------------------------------------
    protected void ProtAssistUnitStatAdd(CStatBase pStat)
    {
        uint hStatType = pStat.GetStatType();
        if (m_mapStatInstance.ContainsKey(hStatType))
        {
            //Error!
        }
        else
        {
            m_mapStatInstance[hStatType] = pStat;
        }
    }

    protected void ProtAssistUnitStatRefresh()
    {
        Dictionary<uint, CStatBase>.Enumerator it = m_mapStatInstance.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.Value.InterStatRefresh();
        }
        OnAssistUnitStatRefresh();
    }

    //---------------------------------------------------
    protected CStatBase FindAssistUnitStat(uint hStatType)
    {
        CStatBase pFind = null;
        m_mapStatInstance.TryGetValue(hStatType, out pFind);
        return pFind;
    }
    //-------------------------------------------------------------
    protected virtual void OnAssistUnitStatRefresh() { }

}
