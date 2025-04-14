using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitFormationBase : CUnitSummonBase
{
    private CAssistUnitFormationBase m_pAssistUnitFormation = null;

    //-----------------------------------------------------------
    protected virtual void InstanceUnitFormation(CAssistUnitFormationBase pUnitFormation) { m_pAssistUnitFormation = pUnitFormation; }
}
