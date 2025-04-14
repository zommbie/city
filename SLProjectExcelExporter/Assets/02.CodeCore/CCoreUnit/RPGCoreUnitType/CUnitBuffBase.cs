using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitBuffBase : CUnitStatBase, IUnitEventBuff
{
    private CAssistUnitBuffBase m_pAssistBuff = null;
    //---------------------------------------------------------------------
    public void IBuffReceive(IUnitEventBuff pBuffOrigin, uint hBuffID, float fBuffDuration, float fBuffPower)
    {
        OnUnitBuffReceive(pBuffOrigin, hBuffID, fBuffDuration, fBuffPower);
    }

    //--------------------------------------------------------------------
    protected virtual void OnUnitBuffReceive(IUnitEventBuff pBuffOrigin, uint hBuffID, float fBuffDuration, float fBuffPower) { }

    //---------------------------------------------------------------------
    protected virtual void InstanceUnitBuff(CAssistUnitBuffBase pUnitBuff) { m_pAssistBuff = pUnitBuff; }
    
}
