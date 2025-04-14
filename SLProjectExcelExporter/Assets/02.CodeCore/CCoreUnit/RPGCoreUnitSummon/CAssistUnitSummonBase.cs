using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitEventSummon
{

}


public abstract class CAssistUnitSummonBase : CAssistUnitBase
{
     private IUnitEventSummon m_pUnitSummoner = null;
     private List<IUnitEventSummon> m_listMinion = new List<IUnitEventSummon>();
    //-------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        m_pUnitSummoner = pOwner as IUnitEventSummon;
    }

}
