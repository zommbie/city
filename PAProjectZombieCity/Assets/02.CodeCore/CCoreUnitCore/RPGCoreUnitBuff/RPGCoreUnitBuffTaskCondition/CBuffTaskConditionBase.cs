using System.Collections;
using System.Collections.Generic;


public abstract class CBuffTaskConditionBase 
{
    
    //-------------------------------------------------------------------------
    public int DoBuffTaskCondition(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage)
    {
        return OnBuffTaskCondition(pBuffInstance, pBuffEventUsage);
    }

    //-------------------------------------------------------------------------
    protected virtual int OnBuffTaskCondition(CBuffBase pBuffInstance, CBuffEventUsage pBuffEventUsage) { return 0; }
}
