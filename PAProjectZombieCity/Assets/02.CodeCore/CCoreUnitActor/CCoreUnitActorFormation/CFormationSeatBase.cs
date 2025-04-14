using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;


//-----------------------------------------------
public abstract class CFormationSeatBase : CMonoBase
{
    private int m_iFormationIndex = 0;      public int GetFormationIndex() { return m_iFormationIndex; }
    //private CUnitBaseOld m_pUnitMember = null; public CUnitBaseOld GetFormationMember() { return m_pUnitMember; }
    ////------------------------------------------------------------------
    //internal void InterFormationSeatReset()
    //{
    //    m_pUnitMember = null;
    //    OnFormationSeatReset();
    //}

    //internal void InterFormationSeatUpdate(float fDelta)
    //{
    //    OnFormationSeatUpdate(fDelta);
    //}

    ////---------------------------------------------------------------
    //public void SetFormationSeatMember(CUnitBaseOld pMemberUnit, int iFormationIndex)
    //{
    //    m_pUnitMember = pMemberUnit;
    //    m_iFormationIndex = iFormationIndex;
    //    OnFormationSeatMemberUnit(pMemberUnit, iFormationIndex);
    //}

  
    ////------------------------------------------------------------------
    //protected virtual void OnFormationSeatReset() { }
    //protected virtual void OnFormationSeatMemberUnit(CUnitBaseOld pMemberUnit, int iFormationIndex) { }  
    //protected virtual void OnFormationSeatUpdate(float fDelta) { }
}
