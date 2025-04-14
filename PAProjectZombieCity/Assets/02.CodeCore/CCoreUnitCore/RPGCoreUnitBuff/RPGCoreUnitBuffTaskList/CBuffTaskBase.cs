using System.Collections;
using System.Collections.Generic;

public abstract class CBuffTaskBase
{
    private bool m_bActiveTask = false;
    //-----------------------------------------------------------------------------------
    public void DoBuffReset()
    {
        m_bActiveTask = false;
        OnBuffTaskReset();
    }
    
    public void DoBuffTaskExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage, List<CUnitCoreBase> pListTaskTarget)
    {
        m_bActiveTask = true;
        OnBuffTaskExcute(pBuffInstance, pBuffEventUsage, pListTaskTarget);
    }

    public void DoBuffTaskEnd(CBuffBase pBuffInstance)
    {
        if (m_bActiveTask)
        {
            OnBuffTaskEnd(pBuffInstance);
        }
    }


    //------------------------------------------------------------------------------------
    protected virtual void OnBuffTaskExcute(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage, List<CUnitCoreBase> pListTaskTarget) { }
    protected virtual void OnBuffTaskEnd(CBuffBase pBuffInstance) { }
    protected virtual void OnBuffTaskReset() { }
}
