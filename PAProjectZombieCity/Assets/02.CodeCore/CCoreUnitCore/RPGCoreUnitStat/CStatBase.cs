using System.Collections;
using System.Collections.Generic;

// 메모리 치팅을 방지하기 위해 핵심 결과값은 보안 필드로 보호한다.

public interface IEventStatHandler
{
    public void IEventStatUpdate(ulong hUnitID, int eStatType, float fStatValue);
}

public abstract class CStatBase 
{
    private int   m_eStatType = 0;                  public int GetStatType() { return m_eStatType; } 
    
    private EnFloat m_fStatBasic  = new EnFloat();  // 외부에서 주어지는 값 
    private EnFloat m_fStatResult = new EnFloat();  // CStatModifierBase 등에 의해 보정된 최종 값
    private float m_fStatMin = 0;                   // 출력되는 최소값
    private float m_fStatMax = 0;                   // 이 값 이상 Result에 적용되지 않는다.
    public  float Value { get { return m_fStatResult; } set { ProtStatValue(value); } }

    private CStatBase                      m_pRootChainStat = null;
    private COperatorStatBase              m_pStatOperator = null; protected COperatorStatBase GetStatOperaOwner() { return m_pStatOperator; }
    private List<CStatBase>                m_listChainStat  = new List<CStatBase>();
    private CLinkedList<CStatModifierBase> m_listModifier   = new CLinkedList<CStatModifierBase>();
    //---------------------------------------------------------------------------------------------
    internal void InterStatInitilize(COperatorStatBase pStatOperator)
    {      
        m_pStatOperator = pStatOperator;
        OnStatInitialize(pStatOperator);
    }

    internal void InterStatChainRefresh(CStatBase pPrevStat)
    {
        m_pRootChainStat = pPrevStat;
        OnStatRefreshChain(pPrevStat);    
    } 

    //--------------------------------------------------------------------------------------------
    public float DoStatRefresh()
    {
        PrivStatRefresh();
        return m_fStatResult; 
    }

    //---------------------------------------------------------------------------------------------
    protected void ProtStatChainAdd(CStatBase pChainStat)
    {
        if (m_listChainStat.Contains(pChainStat) == false)
        {
            m_listChainStat.Add(pChainStat);
            PrivStatRefresh();
        }
    }

    protected void ProtStatValue(float fValue)
    {
        m_fStatBasic.Value = fValue;
        PrivStatRefresh();
    }

    //--------------------------------------------------------------------------
    public void DoStatModifierAdd(CStatModifierBase pAddModifier)
    {
        if (m_listModifier.Contains(pAddModifier) == false)
        {
            m_listModifier.AddLast(pAddModifier);
            pAddModifier.InterStatModifierAdd(this);
            PrivStatRefresh();
        }
    }

    public void DoStatModifierRemove(CStatModifierBase pRemoveModifier)
    {
        m_listModifier.Remove(pRemoveModifier);
        pRemoveModifier.InterStatModifierRemove(this);
        PrivStatRefresh();
    }

    public void DoStatClear()
    {
        m_fStatBasic.Value = 0;
        m_fStatMin = 0;
        m_fStatMax = 0;
        m_fStatResult.Value = 0;
        m_listChainStat.Clear();
        m_listModifier.Clear();
    }

    public void DoStatReset()
    {
        m_fStatBasic.Value = 0;
        PrivStatRefresh();
    }

    public void SetStatMinMax(float fMin, float fMax = 0)
    {
        m_fStatMin = fMin;
        m_fStatMax = fMax;        
    }

    //----------------------------------------------------------------------------------------------   
    private void PrivStatRefresh()
    {
        float fResultValue = m_fStatBasic + m_fStatMin;

        CLinkedList<CStatModifierBase>.Enumerator it = m_listModifier.GetEnumerator();
        while (it.MoveNext())
        {
            fResultValue += it.Current.InterStatModifierRefresh(this, m_pRootChainStat);
        }

        if (fResultValue > m_fStatMax && m_fStatMax != 0)
        {
            fResultValue = m_fStatMax;
        }
     
        PrivStatApply(fResultValue);
    }

    private void PrivStatApply(float fValue)
    {
        m_fStatResult.Value = fValue;
        for (int i = 0; i < m_listChainStat.Count; i++)
        {
            m_listChainStat[i].InterStatChainRefresh(this);
        }

        m_pStatOperator?.InterStatOperatorStatChange(this, fValue);
        OnStatRefreshValue();
    }

    //----------------------------------------------------------------------------------------------
    protected virtual void OnStatInitialize(COperatorStatBase pStatOperator) { }
    protected virtual void OnStatRefreshChain(CStatBase pStat) { }
    protected virtual void OnStatRefreshValue() { }
    //-----------------------------------------------------------------------------------------------
    public CStatBase(int hStatType)
    {
        m_eStatType = hStatType;
    }

    static public implicit operator float(CStatBase pStat)
    {
        return pStat.m_fStatResult;
    }

    static public implicit operator int(CStatBase pStat)
    {
        return (int)pStat.m_fStatResult;
    }

    //---------------------------------------------------------------------------
    public static float operator +(CStatBase pStat, int Value)
    {
        int iResult = (int)pStat.Value + Value;
        return iResult;
    }

    public static float operator -(CStatBase pStat, int Value)
    {
        int iResult = (int)pStat.Value - Value;
        return iResult;
    }

    public static float operator +(CStatBase pStat, float Value)
    {
        float fResult = pStat.Value + Value;
        return fResult;
    }

    public static float operator -(CStatBase pStat, float Value)
    {
        float fResult = pStat.Value - Value;
        return fResult;
    }

    public static float operator +(CStatBase pStat, CStatBase pValue)
    {
        float fResult = pStat.Value + pValue.Value;
        return fResult;
    }

    public static float operator -(CStatBase pStat, CStatBase pValue)
    {
        float fResult = pStat.Value - pValue.Value;
        return fResult;
    }
    //------------------------------------------------------------------------------
    public static float operator *(CStatBase pStat, int Value)
    {
        float fResult = pStat.Value * Value;
        return fResult;
    }

    public static float operator /(CStatBase pStat, int Value)
    {
        float fResult = pStat.Value / Value;
        return fResult;
    }

    public static float operator *(CStatBase pStat, float Value)
    {
        float fResult = pStat.Value * Value;
        return fResult;
    }

    public static float operator /(CStatBase pStat, float Value)
    {
        float fResult = pStat.Value / Value;
        return fResult;
    }

    public static float operator *(CStatBase pStat, CStatBase pValue)
    {
        float fResult = pStat.Value * pValue.Value;
        return fResult;
    }

    public static float operator /(CStatBase pStat, CStatBase pValue)
    {
        float fResult = pStat.Value / pValue.Value;
        return fResult;
    }

   
}
