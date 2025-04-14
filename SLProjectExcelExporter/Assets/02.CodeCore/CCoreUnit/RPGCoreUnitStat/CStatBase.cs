using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Status의 약자이다. Status는 다른 객체에서 예약되는 경우가 많아 여기서는 고유 대명사로 사용한다.

public interface IUnitEventStat
{
    public void      IStatUpdate(uint hStatType, float fStatValue);
    public CStatBase IStatFind(uint hStatType);
    public float     IStatValue(uint hStatType);
}

public abstract class CStatBase
{
    private uint m_hStatType = 0;                   public uint GetStatType() { return m_hStatType; }  protected void SetStatType(uint hStatType) { m_hStatType = hStatType; }
    private float m_fStatBasic = 0;                 // 외부에서 주어지는 값 
    private float m_fStatResult = 0;                // CStatModifierBase 등에 의해 보정된 최종 값
    private float m_fStatConstant = 0;              // 계산시 주어지는 상수 값. 변경하면 안되는 상수 값(기본 명중률 95%등)

    private IUnitEventStat m_pStatOwner = null;             protected IUnitEventStat GetStatOwner() { return m_pStatOwner; }
    private List<CStatBase>               m_listChainStat = new List<CStatBase>();
    private LinkedList<CStatModifierBase> m_listModifier = new LinkedList<CStatModifierBase>();
    //--------------------------------------------------------------
    internal void InterStatChangeChainStat(CStatBase pRootStat)
    {
        OnChangeChainStat(pRootStat);
    }

    internal void InterStatRefresh()
    {
        PrivStatValueRefresh();
    }

    //--------------------------------------------------------------
    protected void ProtStatModifierRemove(CStatModifierBase pModifier)
    {
        m_listModifier.Remove(pModifier);
        PrivStatValueRefresh();
    }

    protected void ProtStatModifierAdd(CStatModifierBase pModifier)
    {
        if (m_listModifier.Contains(pModifier)) return;

        m_listModifier.AddLast(pModifier);
        PrivStatValueRefresh();
    }

    protected void ProtStatBasicReset(float fResetValue)
    {
        m_fStatBasic = fResetValue;
        PrivStatValueRefresh();
    }

    protected void ProtStatBasicPlusMinus(float fPlusMinusValue)
    {
        m_fStatBasic += fPlusMinusValue;
        PrivStatValueRefresh();
    }

    protected void ProtStatInitilize(float fConstValue, IUnitEventStat pStatOwner)
    {
        PrivStatClear();       
        m_fStatConstant = fConstValue;
        m_pStatOwner = pStatOwner;
        
        PrivStatValueRefresh();
    }

    protected void ProtStatChain(List<CStatBase> pListChainStat)
    {
        m_listChainStat.Clear();
        for (int i = 0; i < pListChainStat.Count; i++)
        {
            m_listChainStat.Add(pListChainStat[i]);
        }
    }

    //---------------------------------------------------------------
    private void PrivStatValueRefresh()
    {
        float fResultValue = m_fStatBasic + m_fStatConstant;

        LinkedList<CStatModifierBase>.Enumerator it = m_listModifier.GetEnumerator();
        while(it.MoveNext())
        {
            fResultValue += it.Current.InterStatModifierRefresh(this);
        }

        PrivStatApply(fResultValue);
    }

    private void PrivStatClear()
    {
        m_fStatBasic = 0;
        m_fStatConstant = 0;
        m_listChainStat.Clear();
        m_listModifier.Clear();
    }

    private void PrivStatApply(float fValue)
    {
        m_fStatResult = fValue;
        for (int i = 0; i < m_listChainStat.Count; i++)
        {
            m_listChainStat[i].InterStatChangeChainStat(this);
        }

        m_pStatOwner?.IStatUpdate(m_hStatType, m_fStatResult);
    }

    static public implicit operator float(CStatBase pStat)
    {
        return pStat.m_fStatResult;
    }

    //-------------------------------------------------------------------------------
    protected virtual void OnChangeChainStat(CStatBase pStat) { }
}
