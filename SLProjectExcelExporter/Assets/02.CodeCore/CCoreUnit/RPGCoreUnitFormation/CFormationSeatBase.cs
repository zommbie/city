using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;


//-----------------------------------------------
public abstract class CFormationSeatBase : CMonoBase
{
    private int m_iFormationIndex = 0;      public int GetFormationIndex() { return m_iFormationIndex; }
    private CUnitBase m_pUnitMember = null; public CUnitBase GetFormationMember() { return m_pUnitMember; }
    //------------------------------------------------------------------
    internal void InterFormationSeatReset()
    {
        m_pUnitMember = null;
        OnFormationSeatReset();
    }

    internal void InterFormationSeatUpdate(float fDelta)
    {
        OnFormationSeatUpdate(fDelta);
    }

    //---------------------------------------------------------------
    public void SetFormationSeatMember(CUnitBase pMemberUnit, int iFormationIndex)
    {
        m_pUnitMember = pMemberUnit;
        m_iFormationIndex = iFormationIndex;
        OnFormationSeatMemberUnit(pMemberUnit, iFormationIndex);
    }

  
    //------------------------------------------------------------------
    protected virtual void OnFormationSeatReset() { }
    protected virtual void OnFormationSeatMemberUnit(CUnitBase pMemberUnit, int iFormationIndex) { }  
    protected virtual void OnFormationSeatUpdate(float fDelta) { }
}
