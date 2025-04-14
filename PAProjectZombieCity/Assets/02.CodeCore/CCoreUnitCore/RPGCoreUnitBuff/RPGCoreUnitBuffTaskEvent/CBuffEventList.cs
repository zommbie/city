using System.Collections;
using System.Collections.Generic;

public class CBuffEventList 
{
    private List<CBuffEventBase> m_listBuffEvent = new List<CBuffEventBase>();
    //-----------------------------------------------------------------------
    public void DoBuffEventReset()
    {
        for (int i = 0; i < m_listBuffEvent.Count; i++)
        {
            m_listBuffEvent[i].DoBuffEventReset();
        }
    }

    public void DoBuffEventExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage)
    {
        for (int i = 0; i < m_listBuffEvent.Count; i++)
        {
            m_listBuffEvent[i].DoBuffEventExcute(pBuffInstance, pBuffEventUsage);
        }
    }

    public void DoBuffEventEnd(CBuffBase pBuffInstance)
    {
        for (int i = 0; i < m_listBuffEvent.Count; i++)
        {
            m_listBuffEvent[i].DoBuffEventEnd(pBuffInstance);
        }
    }

    //-----------------------------------------------------------------------
    public void SetBuffEvent(CBuffEventBase pBuffEvent) { m_listBuffEvent.Add(pBuffEvent); }
}
