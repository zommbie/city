using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffTaskConditionList 
{
    private List<CBuffTaskConditionBase> m_listBuffTaskCondition = new List<CBuffTaskConditionBase>();
    //-----------------------------------------------------------------
    internal int InterBuffTaskCondition(IUnitEventBuff pOnwer, IUnitEventBuff pOrigin)
    {
        int iResult = 0;
        for (int i = 0; i < m_listBuffTaskCondition.Count; i++)
        {
            iResult = m_listBuffTaskCondition[i].InterBuffTaskCondition(pOnwer, pOrigin);
        }
        return iResult;
    }

    //-----------------------------------------------------------------
    public void InstanceBuffTaskCondition(CBuffTaskConditionBase pBuffTaskCondition) { m_listBuffTaskCondition.Add(pBuffTaskCondition); }
}
