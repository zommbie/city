using System.Collections;
using System.Collections.Generic;


public abstract class CBuffTaskTargetBase 
{
    
    //----------------------------------------------------------------------------------
    public void DoBuffTaskTarget(CBuffBase pBuffInstance, ref List<CUnitCoreBase> pListOutTargetUnit)
    {
        OnBuffTaskTarget(pBuffInstance, ref pListOutTargetUnit);
    }

    //----------------------------------------------------------------------------------
    protected virtual void OnBuffTaskTarget(CBuffBase pBuffInstance, ref List<CUnitCoreBase> pListOutTargetUnit) { }
}
