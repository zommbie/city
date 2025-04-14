using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffConditionList
{
    private List<CBuffConditionBase> m_listBuffCondition = new List<CBuffConditionBase>();
    //------------------------------------------------------------
    internal int InterBuffConditon(IUnitEventBuff pAssistBuffOrigin, IUnitEventBuff pAssistBuffOwner)
    {
        int iResult = 0;
        for (int i = 0; i < m_listBuffCondition.Count; i++)
        {
            iResult = m_listBuffCondition[i].InterBuffCondition(pAssistBuffOrigin, pAssistBuffOwner);
            if (iResult != 0)
            {
                break;
            }
        }

        return iResult;
    }

    //------------------------------------------------------------
    
    //------------------------------------------------------------
    public void InstanceBuffCondition(CBuffConditionBase pBuffCondition) { m_listBuffCondition.Add(pBuffCondition); }
}
