using System.Collections;
using System.Collections.Generic;


public class CBuffTaskConditionList 
{

    private List<CBuffTaskConditionBase> m_listBuffTaskCondition = new List<CBuffTaskConditionBase>();
    //-------------------------------------------------------------------------------
    public int DoBuffTaskConditionList(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage)
    {
        int iResult = 0;

        for (int i = 0; i < m_listBuffTaskCondition.Count; i++)
        {
            iResult = m_listBuffTaskCondition[i].DoBuffTaskCondition(pBuffInstance, pBuffEventUsage);
        }

        return iResult;
    }


    //-------------------------------------------------------------------------------
    public void SetBuffTaskConditionAdd(CBuffTaskConditionBase pBuffTaskCondition) { m_listBuffTaskCondition.Add(pBuffTaskCondition);}
}
