using System.Collections.Generic;
public interface IEventBuffHandler  // 버프의 작동 결과를 처리할 객체 
{
    void IEventBuffStart(ulong hUnitID, uint hBuffInstanceID, uint hBuffTableID, int eBuffType, float fDuration, int iStackCount);
    void IEventBuffTimeExpire(ulong hUnitID, uint hBuffInstanceID);
    void IEventBuffErase(ulong hUnitID, uint hBuffInstanceID, uint hBuffTableID);
    void IEventBuffEnd(ulong hUnitID, uint hBuffInstanceID, uint hBuffTableID);
    void IEventBuffChangeStack(ulong hUnitID, uint hBuffInstanceID, int iStackCount);
    void IEventBuffChangePower(ulong hUnitID, uint hBuffInstanceID, float fPower);
    void IEventBuffChangeDuration(ulong hUnitID, uint hBuffInstanceID, float fDuration);
}

// 버프 객체를 관리하고 이벤트를 중계
public abstract class COperatorUnitBuffBase : COperatorBase
{
    private IEventBuffHandler           m_pBuffEventHandler = null;
    private CLinkedList<CBuffBase>      m_listBuffInstance = new CLinkedList<CBuffBase>();
    private Dictionary<int, CLinkedList<CBuffBase>> m_mapBuffTypeList = new Dictionary<int, CLinkedList<CBuffBase>>();
    //------------------------------------------------------
    public void InitializeBuffOperator(IEventBuffHandler pBuffEventHandler)
    {
        m_pBuffEventHandler = pBuffEventHandler;
        OnBuffOperatorInitilize(pBuffEventHandler);
    }

    internal void InterBuffOperatorUpdate(float fDelta)
    {
        UpdateBuffOperatorAll(fDelta);
        OnBuffOperatorUpdate(fDelta);
    }

    internal void InterBuffOperatorTimeExpire(CBuffBase pBuff)
    {
        PrivBuffRemove(pBuff);
        OnBuffOperatorTimeExpire(pBuff);
        OnBuffOperatorEnd(pBuff);
    }

    internal void InterBuffOperatorStackZero(CBuffBase pBuff)
    {
        PrivBuffRemove(pBuff);
        OnBuffOperatorStackZero(pBuff);
        OnBuffOperatorEnd(pBuff);    
    }

    internal void InterBuffOperatorErase(CBuffBase pBuff)
    {
        PrivBuffErase(pBuff);
    }

    //------------------------------------------------------------------------
    protected void ProtBuffStart(CBuffBase pEnterBuff, COperatorUnitBuffBase pBuffOrigin, float fBuffDuration, float fBuffPower)
    {
        CBuffBase.SBuffMerge rBuffMerge = pEnterBuff.GetBuffMerge();
        CBuffBase.SBuffAttribute rBuffAttribute = pEnterBuff.GetBuffAttribute();
        CBuffBase pMergeTarget = FindBuffMergeGroup(pEnterBuff, rBuffAttribute.BuffMergeGroup);
        if (pMergeTarget != null)
        {
            PrivBuffMerge(pMergeTarget, pEnterBuff, ref rBuffMerge, pBuffOrigin, fBuffDuration, fBuffPower);
        }
        else
        {
            PrivBuffStart(pEnterBuff, pBuffOrigin, fBuffDuration, fBuffPower);
        }
    }

    protected void ProtBuffUpdateManual(int eBuffType, float fDelta)
    {
        if (m_mapBuffTypeList.ContainsKey(eBuffType) == false) return;

        CLinkedList<CBuffBase> pListBuffInstance = m_mapBuffTypeList[eBuffType];
        if (pListBuffInstance != null)
        {
            UpdateBuffOperator(pListBuffInstance, fDelta);
        }
    }
    
    protected void ProtBuffClearType(int eBuffType)
    {
        if (m_mapBuffTypeList.ContainsKey(eBuffType) == false) return; 

        CLinkedList<CBuffBase> pListBuffInstance = m_mapBuffTypeList[eBuffType];
        if (pListBuffInstance != null)
        {
            CLinkedList<CBuffBase>.Enumerator it = pListBuffInstance.GetEnumerator();
            while(it.MoveNext())
            {
                CBuffBase pBuff = it.Current;
                m_listBuffInstance.Remove(pBuff);

                pBuff.InterBuffErase();
                OnBuffOperatorErase(pBuff);
                OnBuffOperatorEnd(pBuff);

                CBuffBase.InstancePoolReturn(pBuff);
            }
            pListBuffInstance.Clear();
        }
    }

    protected void ProtBuffEvent(CBuffEventUsage pBuffEventUsage)
    {
        CLinkedList<CBuffBase>.Enumerator it = m_listBuffInstance.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.InterBuffEvent(pBuffEventUsage);
        }
    }

    protected CLinkedList<CBuffBase>.Enumerator IterBuffInstance()
    {
        return m_listBuffInstance.GetEnumerator();
    }

    protected CLinkedList<CBuffBase>.Enumerator IterBuffInstance(int eBuffType)
    {
        CLinkedList<CBuffBase>.Enumerator it = m_listBuffInstance.GetEnumerator();
        if (m_mapBuffTypeList.ContainsKey(eBuffType))
        {
            it = m_mapBuffTypeList[eBuffType].GetEnumerator();
        }
        return it;
    }

    protected void ProtBuffErase(uint hBuffID)
    {
        CBuffBase pEraseBuff = FindBuffFromID(hBuffID);
        if (pEraseBuff != null)
        {
            PrivBuffErase(pEraseBuff);
        }
    }
    //----------------------------------------------------------------------------
    private void PrivBuffErase(CBuffBase pEraseBuff)
    {
        pEraseBuff.InterBuffErase();
        PrivBuffRemove(pEraseBuff);
        OnBuffOperatorErase(pEraseBuff);
        OnBuffOperatorEnd(pEraseBuff);
    }

    //----------------------------------------------------------------------------
    private void UpdateBuffOperatorAll(float fDelta)
    {
        Dictionary<int, CLinkedList<CBuffBase>>.Enumerator it = m_mapBuffTypeList.GetEnumerator();
        while(it.MoveNext())
        {
            UpdateBuffOperator(it.Current.Value, fDelta);
        }
    }

    private void UpdateBuffOperator(CLinkedList<CBuffBase> pListBuffInstance, float fDelta)
    {
        CLinkedList<CBuffBase>.Enumerator it = pListBuffInstance.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.InterBuffUpdate(fDelta);
        }
    }

    //-----------------------------------------------------------------------------
    private CBuffBase FindBuffMergeGroup(CBuffBase pEnterBuff, int iBuffMergeGroup)
    {
        if (iBuffMergeGroup == 0) return null;      
        return OnBuffOperatorMatchMergeGroup(pEnterBuff, iBuffMergeGroup, m_listBuffInstance.GetEnumerator());
    }

    private CBuffBase FindBuffFromID(uint hBuffID)
    {
        CBuffBase pFindBuff = null;
        CLinkedList<CBuffBase>.Enumerator it = m_listBuffInstance.GetEnumerator();
        while (it.MoveNext())
        {
            CBuffBase pBuff = it.Current;
            if (hBuffID == pBuff.GetBuffTableID())
            {
                pFindBuff = pBuff;
                break;
            }
        }
        return pFindBuff;
    }

    //------------------------------------------------------------------------------
    private void PrivBuffRemove(CBuffBase pBuff)
    {
        int iBuffType = pBuff.GetBuffType();
        CLinkedList<CBuffBase> pListBuff = m_mapBuffTypeList[iBuffType];
        if (pListBuff != null)
        {
            pListBuff.Remove(pBuff);
        }
        m_listBuffInstance.Remove(pBuff);
        CBuffBase.InstancePoolReturn(pBuff);
    }

    private void PrivBuffMerge(CBuffBase pOldBuff, CBuffBase pNewBuff, ref CBuffBase.SBuffMerge rBuffMerge, COperatorUnitBuffBase pBuffOrigin, float fBuffDuration, float fBuffPower)
    {
        if (rBuffMerge.MergePriority == CBuffBase.EMargePriority.Exclusive)
        {
            PrivBuffStart(pNewBuff, pBuffOrigin, fBuffDuration, fBuffPower);
        }
        else if (rBuffMerge.MergePriority == CBuffBase.EMargePriority.Exist)
        {
            pOldBuff.InterBuffMerge(pNewBuff, fBuffDuration, fBuffPower);
        }
        else if (rBuffMerge.MergePriority == CBuffBase.EMargePriority.Power)
        {
            CBuffBase pMergeBuff = null;
            float fPowerOld = pOldBuff.GetBuffPower();
            if (fPowerOld >= fBuffPower)
            {
                pMergeBuff = pOldBuff;
            }
            else
            {
                pMergeBuff = pNewBuff;
            }
            pMergeBuff.InterBuffMerge(pNewBuff, fBuffDuration, fBuffPower);
        }
        else if (rBuffMerge.MergePriority == CBuffBase.EMargePriority.Time)
        {

        }
        else if (rBuffMerge.MergePriority == CBuffBase.EMargePriority.Stack)
        {

        }
    }
 
    private void PrivBuffStart(CBuffBase pBuff, COperatorUnitBuffBase pBuffOrigin, float fBuffDuration, float fBuffPower)
    {
        m_listBuffInstance.AddLast(pBuff);

        CLinkedList<CBuffBase> pListBuff = null;
        int iBuffType = pBuff.GetBuffType();
        if (m_mapBuffTypeList.ContainsKey(iBuffType))
        {
            pListBuff = m_mapBuffTypeList[iBuffType];
        }
        else
        {
            pListBuff = new CLinkedList<CBuffBase>();
            m_mapBuffTypeList[iBuffType] = pListBuff;
        }

        pListBuff.AddLast(pBuff);

        pBuff.InterBuffStart(m_pBuffEventHandler, this, pBuffOrigin, fBuffDuration, fBuffPower);
        OnBuffOperatorStart(pBuff);
    }

    //------------------------------------------------------------------------
    protected virtual void OnBuffOperatorInitilize(IEventBuffHandler pBuffEventHandler) { }
    protected virtual void OnBuffOperatorUpdate(float fDelta) { }
    protected virtual void OnBuffOperatorTimeExpire(CBuffBase pBuff) { }
    protected virtual void OnBuffOperatorEnd(CBuffBase pBuff) { }
    protected virtual void OnBuffOperatorErase(CBuffBase pBuff) { }
    protected virtual void OnBuffOperatorStackZero(CBuffBase pBuff) { }
    protected virtual void OnBuffOperatorStart(CBuffBase pBuff) { }
    protected virtual CBuffBase OnBuffOperatorMatchMergeGroup(CBuffBase pEnterBuff, int iMergeGroup, CLinkedList<CBuffBase>.Enumerator it) { return null; }

}
