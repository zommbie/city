using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public abstract class CBuffTaskTargetListBase 
{
    private int m_eTaskTargetFilterType = 0;
    private List<CBuffTaskTargetBase> m_listBuffTaskTarget = new List<CBuffTaskTargetBase>();
    private List<IUnitEventBuff> m_listBuffTargetNoteIn = new List<IUnitEventBuff>();
    private List<IUnitEventBuff> m_listBuffTargetNoteOut = new List<IUnitEventBuff>();
    //-------------------------------------------------------
    internal void InterBuffTaskTarget(IUnitEventBuff pOwner, IUnitEventBuff pOrigin, List<IUnitEventBuff> pListOutTarget)
    {
        if (m_listBuffTaskTarget.Count == 0)
        {
            pListOutTarget.Add(pOwner);
        }
        else
        {
            ExtractBuffTaskTarget(m_eTaskTargetFilterType, pOwner, pOrigin, pListOutTarget);

            for (int i = 0; i < m_listBuffTaskTarget.Count; i++)
            {
                m_listBuffTaskTarget[i].InterBuffTaskTarget(pOwner, pOrigin, m_listBuffTargetNoteIn, m_listBuffTargetNoteOut);
                m_listBuffTargetNoteIn.Clear();
                
                for (int j = 0; j < m_listBuffTargetNoteOut.Count; j++)
                {
                    m_listBuffTargetNoteIn.Add(m_listBuffTargetNoteOut[j]);
                }
                m_listBuffTargetNoteOut.Clear();
            }

            for (int i = 0; i < m_listBuffTargetNoteIn.Count; i++)
            {
                pListOutTarget.Add(m_listBuffTargetNoteIn[i]);
            }
        }
    }
    //--------------------------------------------------------
    public void SetBuffTeskTargetFilter(int eTaskTargetFilter)
    {
        m_eTaskTargetFilterType = eTaskTargetFilter;
    }
    //----------------------------------------------------------
    protected virtual void ExtractBuffTaskTarget(int eTaskTargetFilter, IUnitEventBuff pOwner, IUnitEventBuff pOrigin, List<IUnitEventBuff> pListOutTarget) { }
    //-------------------------------------------------------
    public void InstanceBuffTaskTarget(CBuffTaskTargetBase pBuffTaskTarget) { m_listBuffTaskTarget.Add(pBuffTaskTarget); }
}
