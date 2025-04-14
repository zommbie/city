using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBuffTaskConditionBase 
{

    //-----------------------------------------------------------
    internal int InterBuffTaskCondition(IUnitEventBuff pOnwer, IUnitEventBuff pOrigin)
    {
        return OnBuffTaskCondition(pOnwer, pOrigin);
    }

    //----------------------------------------------------------
    protected virtual int OnBuffTaskCondition(IUnitEventBuff pOnwer, IUnitEventBuff pOrigin) { return 0; }
}
