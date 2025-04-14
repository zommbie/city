using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitSummonBase : CUnitSkillBase
{
    private CAssistUnitSummonBase m_pAssistSummon = null;

    //---------------------------------------------------------------------
    protected virtual void InstanceUnitSummon(CAssistUnitSummonBase pInstanceSummon) { m_pAssistSummon = pInstanceSummon; }
}
