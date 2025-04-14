using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBuffTaskTargetBase 
{

    internal void InterBuffTaskTarget(IUnitEventBuff pOwner, IUnitEventBuff pOrigin, List<IUnitEventBuff> pListTargetIn, List<IUnitEventBuff> pListTargetOut) 
    {
        OnBuffTaskTarget(pOwner, pOrigin, pListTargetIn, pListTargetOut);
    }

    protected virtual void OnBuffTaskTarget(IUnitEventBuff pOwner, IUnitEventBuff pOrigin, List<IUnitEventBuff> pListTargetIn, List<IUnitEventBuff> pListTargetOut) { }
}
