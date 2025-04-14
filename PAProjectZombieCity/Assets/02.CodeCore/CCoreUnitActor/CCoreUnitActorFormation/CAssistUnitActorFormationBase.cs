using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 포메이션 구성의 기준이 되는 객체에 부착

public abstract class CAssistUnitActorFormationBase : CAssistUnitActorBase
{
    private bool m_bFormationReader = false;
    private List<CFormationSeatBase> m_listFormationSeat = new List<CFormationSeatBase>();
    //-------------------------------------------------------------------------
    protected sealed override void OnAssistFixedUpdate(float fDelta)
    {
        base.OnAssistFixedUpdate(fDelta);

        if (HasFormationSeat())
        {
            OnUnitFormationUpdate(fDelta);
        }
       
    }

    //-------------------------------------------------------------------------
    public bool HasFormationSeat() { return m_listFormationSeat.Count > 0 ? true : false;}
    public int GetFormationSeatCount() { return m_listFormationSeat.Count; }
    //-----------------------------------------------------------------------
    public List<CFormationSeatBase>.Enumerator IterUnitFormationSeat()
    {
        return m_listFormationSeat.GetEnumerator();
    }

    //---------------------------------------------------------------------
    protected void ProtUnitFormationReset()
    {
        for (int i = 0; i < m_listFormationSeat.Count; i++)
        {
 //           m_listFormationSeat[i].InterFormationSeatReset();
        }
    }

    protected void ProtUnitFormationRemove()
    {
        m_listFormationSeat.Clear();
    }

    protected void ProtUnitFormationSeatAdd(CFormationSeatBase pFormationSeat)
    {
        m_listFormationSeat.Add(pFormationSeat);
        OnUnitFormationSeat(pFormationSeat);
    }

    protected void ProtUnitFormationUpdateSeat(float fDelta)
    {
        for (int i = 0; i < m_listFormationSeat.Count; i++)
        {
  //          m_listFormationSeat[i].InterFormationSeatUpdate(fDelta);
        }
    }


    protected CFormationSeatBase GetUnitFormationSeat(int iSeatIndex)
    {
        CFormationSeatBase pFormationSeat = null;
        if (iSeatIndex < m_listFormationSeat.Count)
        {
            pFormationSeat = m_listFormationSeat[iSeatIndex];
        }
        return pFormationSeat;
    }

    //------------------------------------------------------------------------
    protected virtual void OnUnitFormationUpdate(float fDelta) { }  
    protected virtual void OnUnitFormationSeat(CFormationSeatBase pFormationSeat) { }
    
}
