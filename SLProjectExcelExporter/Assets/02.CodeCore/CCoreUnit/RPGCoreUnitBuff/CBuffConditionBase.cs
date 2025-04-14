using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBuffConditionBase 
{

    internal int InterBuffCondition(IUnitEventBuff pAssistBuffOrigin, IUnitEventBuff pAssistBuffOwner)
    {
        return OnBuffCondition(pAssistBuffOrigin, pAssistBuffOwner);
    }

    //-----------------------------------------------------------------
    protected virtual int OnBuffCondition(IUnitEventBuff pAssistBuffOrigin, IUnitEventBuff pAssistBuffOwner) { return 0; }
}
