using System.Collections;
using System.Collections.Generic;


public abstract class CBuffEventBase
{
    private int m_iEventCount;
    private int m_iCurrentEventCount;
    private List<CBuffTaskContainerList> m_listBuffTaskContainer = new List<CBuffTaskContainerList>();
    //-------------------------------------------------------------
    public void DoBuffEventReset()
    {
        m_iCurrentEventCount = m_iEventCount;

        for (int i = 0; i < m_listBuffTaskContainer.Count; i++)
        {
            m_listBuffTaskContainer[i].DoBuffTaskContainerReset();
        }

        OnBuffEventReset();
    }

    public void DoBuffEventExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage)
    {
        if (m_iEventCount > 0)
        {
            if (m_iCurrentEventCount <= 0)
            {
                return;
            }
        }

        if (OnBuffEventExcute(pBuffInstance, pBuffEventUsage))
        {
            m_iCurrentEventCount--;
            PrivBuffEventExcute(pBuffInstance, pBuffEventUsage);
        }
    }

    public void DoBuffEventEnd(CBuffBase pBuffInstance)
    {
        for (int i = 0; i < m_listBuffTaskContainer.Count; i++)
        {
            m_listBuffTaskContainer[i].DoBuffTaskContainerEnd(pBuffInstance);
        }
    }

    //----------------------------------------------------------
    private void PrivBuffEventExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage)
    {       
        for (int i = 0; i < m_listBuffTaskContainer.Count; i++)
        {
            m_listBuffTaskContainer[i].DoBuffTaskContainerExcute(pBuffInstance, pBuffEventUsage);           
        }
    }
    
    //----------------------------------------------------------
    protected virtual void OnBuffEventReset() {  }
    protected virtual bool OnBuffEventExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage) { return false; }
    //----------------------------------------------------------
    public void SetBuffEventCount(int iEventCount)
    {
        m_iEventCount = iEventCount;
    }
    public void SetBuffTaskContainerAdd(CBuffTaskContainerList pBuffTaskContainer) { m_listBuffTaskContainer.Add(pBuffTaskContainer); }
}
