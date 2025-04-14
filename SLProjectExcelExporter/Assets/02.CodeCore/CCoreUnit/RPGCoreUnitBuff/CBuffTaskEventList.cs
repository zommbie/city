using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffTaskEventList 
{
    private CMultiDictionary<CBuffTaskBase.EBuffTaskEvent, CBuffTaskBase> m_mapTaskEventList = new CMultiDictionary<CBuffTaskBase.EBuffTaskEvent, CBuffTaskBase>();
    //-----------------------------------------------------------------------------------
    internal void InterBuffTaskEvent(CBuffBase pBuffInstance, CBuffTaskBase.EBuffTaskEvent eTaskEvent, IUnitEventBuff pOwner, IUnitEventBuff pOrigin, int iArg, float fArg, params object[] aParams)
    {
        if (m_mapTaskEventList.ContainsKey(eTaskEvent))
        {
            List<CBuffTaskBase> pList = m_mapTaskEventList[eTaskEvent];

            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterBuffTaskEvent(pBuffInstance, pOwner, pOrigin, iArg, fArg, aParams);
            }
        }
    }

    internal void InterBuffTaskReset()
    {
        IEnumerator<List<CBuffTaskBase>> it = m_mapTaskEventList.value.GetEnumerator();
        while(it.MoveNext())
        {
            List<CBuffTaskBase> pList = it.Current;

            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterBuffTaskReset();
            }
        }
    }

    //-----------------------------------------------------------------------------------
    public void InstanceTaskEvent(int eTaskEvent, CBuffTaskBase pBuffTask) { m_mapTaskEventList.Add((CBuffTaskBase.EBuffTaskEvent)eTaskEvent, pBuffTask); }
}
