using System.Collections;
using System.Collections.Generic;

// 원본 스텟에 장비나 패시브, 버프등에 의해 수정되는 값.  스텟은 이 체인을 참조하여 최종 값을 산출한다.
public abstract class CStatModifierBase 
{
    private int m_hModifierType = 0;            public int GetModifierType()   { return m_hModifierType; }
    private bool m_bModifierActive = false;     public bool IsModifierActive { get { return m_bModifierActive; } }
    private CStatBase m_pOwnerStat = null;
    //---------------------------------------------------
    internal float InterStatModifierRefresh(CStatBase pOwnerStat, CStatBase pRootChainStat)
    {
        return OnStatModifierRefresh(pOwnerStat, pRootChainStat);
    }

    internal void InterStatModifierAdd(CStatBase pOwnerStat)
    {
        m_bModifierActive = true;
        m_pOwnerStat = pOwnerStat;
        OnStatModifierAdd(pOwnerStat);
    }

    internal void InterStatModifierRemove(CStatBase pOwnerStat)
    {
        m_bModifierActive = false;
        m_pOwnerStat = null;
        OnStatModifierRemove(pOwnerStat);
    }
    //-----------------------------------------------
    protected void ProtStatModifierRefresh()
    {
        if (m_pOwnerStat != null)
        {
            m_pOwnerStat.DoStatRefresh();
        }
    }

    //------------------------------------------------
    protected void SetStatModifierType(int eModifierType) { m_hModifierType = eModifierType; }
    //-------------------------------------------------
    protected virtual float OnStatModifierRefresh(CStatBase pOwnerStat, CStatBase pRootChainStat) { return 0; }
    protected virtual void  OnStatModifierAdd(CStatBase pOwnerStat) { }
    protected virtual void  OnStatModifierRemove(CStatBase pOwnerStat) { }
}
