using System.Collections;
using System.Collections.Generic;

public class CBuffTaskList 
{
    private List<CBuffTaskBase> m_listBuffTask = new List<CBuffTaskBase>();
    //--------------------------------------------------------------
    public void DoBuffTaskReset()
    {
        for (int i = 0; i < m_listBuffTask.Count; i++)
        {
            m_listBuffTask[i].DoBuffReset();
        }
    }

    public void DoBuffTaskListExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage, List<CUnitCoreBase> pListTaskTarget)
    {
        for (int i = 0; i < m_listBuffTask.Count; i++)
        {
            m_listBuffTask[i].DoBuffTaskExcute(pBuffInstance, pBuffEventUsage, pListTaskTarget);
        }
    }

    public void DoBuffTaskListEnd(CBuffBase pBuffInstance)
    {
        for (int i = 0; i < m_listBuffTask.Count; i++)
        {
            m_listBuffTask[i].DoBuffTaskEnd(pBuffInstance);
        }
    }

    //---------------------------------------------------------------
    public void SetBuffTaskAdd(CBuffTaskBase pBuffTask) { m_listBuffTask.Add(pBuffTask); }

}
