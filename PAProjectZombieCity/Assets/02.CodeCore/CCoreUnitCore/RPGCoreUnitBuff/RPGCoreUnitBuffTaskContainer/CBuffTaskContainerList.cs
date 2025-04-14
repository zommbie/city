using System.Collections;
using System.Collections.Generic;


public class CBuffTaskContainerList 
{
    private CBuffTaskTargetList     m_pBuffTaskTargetList = null;
    private CBuffTaskConditionList  m_pBuffTaskConditionList = null;
    private CBuffTaskList           m_pBuffTaskList = null;

    private List<CUnitCoreBase>     m_listTargetUnitInstance = new List<CUnitCoreBase>();
    //----------------------------------------------------------------------
    public void DoBuffTaskContainerReset()
    {
        m_pBuffTaskList.DoBuffTaskReset();
    }

    public void DoBuffTaskContainerExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage)
    {
        int iResult = 0;
        m_listTargetUnitInstance.Clear();
        if (m_pBuffTaskConditionList != null)
        {
            iResult = m_pBuffTaskConditionList.DoBuffTaskConditionList(pBuffInstance, pBuffEventUsage);
        }

        if (iResult == 0)
        {
            if (m_pBuffTaskTargetList != null)
            {
                m_pBuffTaskTargetList.DoBuffTaskTarget(pBuffInstance, ref m_listTargetUnitInstance);
            }

            m_pBuffTaskList.DoBuffTaskListExcute(pBuffInstance, pBuffEventUsage, m_listTargetUnitInstance);
        }
    }


    public void DoBuffTaskContainerEnd(CBuffBase pBuffInstance)
    {
        m_pBuffTaskList.DoBuffTaskListEnd(pBuffInstance);
    }


    //-----------------------------------------------------------------------
    public void SetBuffTaskContainerTargetList(CBuffTaskTargetList pBuffTaskTargetList) { m_pBuffTaskTargetList = pBuffTaskTargetList; }
    public void SetBuffTaskContainerConditionList(CBuffTaskConditionList pBuffTaskConditionList) { m_pBuffTaskConditionList = pBuffTaskConditionList;}
    public void SetBuffTaskContainerTaskList(CBuffTaskList pBuffTaskList) { m_pBuffTaskList = pBuffTaskList; }
}
