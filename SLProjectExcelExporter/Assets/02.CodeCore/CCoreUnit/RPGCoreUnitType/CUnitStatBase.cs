using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitStatBase : CUnitMovementBase
{
	private CAssistUnitStatBase m_pAssistStat = null;
    //---------------------------------------------------------------------

    //----------------------------------------------------------------------
    protected virtual void InstanceUnitStat(CAssistUnitStatBase pInstanceStat) { m_pAssistStat = pInstanceStat; }
}
