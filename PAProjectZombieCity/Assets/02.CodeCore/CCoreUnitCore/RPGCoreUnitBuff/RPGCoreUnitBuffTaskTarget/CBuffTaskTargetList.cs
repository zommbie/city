using System.Collections;
using System.Collections.Generic;


public class CBuffTaskTargetList 
{
    private List<CBuffTaskTargetBase> m_listBuffTaskTarget = new List<CBuffTaskTargetBase>();
    //-------------------------------------------------------------------------------------------
    public void DoBuffTaskTarget(CBuffBase pBuffInstance, ref List<CUnitCoreBase> pListOutTargetUnit)
    {
        for (int i = 0; i < m_listBuffTaskTarget.Count; i++)
        {
            m_listBuffTaskTarget[i].DoBuffTaskTarget(pBuffInstance, ref pListOutTargetUnit);
        }
    }
   
    //--------------------------------------------------------------------------------------------
    public void SetBuffTaskTargetAdd(CBuffTaskTargetBase pBuffTaskTarget) { m_listBuffTaskTarget.Add(pBuffTaskTarget); }
}
