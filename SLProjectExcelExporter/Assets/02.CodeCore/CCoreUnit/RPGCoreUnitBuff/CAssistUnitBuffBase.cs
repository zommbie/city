using System.Collections.Generic;
using UnityEngine;

public interface IUnitEventBuff
{
    public void IBuffReceive(IUnitEventBuff pBuffOrigin, uint hBuffID, float fBuffDuration, float fBuffPower);
}

public abstract class CAssistUnitBuffBase : CAssistUnitBase
{
    private IUnitEventBuff m_pUnitBuffOnwer = null;  public IUnitEventBuff GetCtrUnitBuffProcessor() { return m_pUnitBuffOnwer; }
    private CLinkedList<CBuffBase> m_listBuffInstance = new CLinkedList<CBuffBase>();
    private List<CBuffBase> m_listBuffEraseNote = new List<CBuffBase>();
    private List<CBuffBase> m_listBuffMergeNote = new List<CBuffBase>();
    //---------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        m_pUnitBuffOnwer = pOwner as IUnitEventBuff;
    }

    protected override void OnAssistUpdate(float fDelta)
    {
        PrivAssistBuffEndProcess();
        PrivAssistBuffUpdate(fDelta);        
    }
    //--------------------------------------------------------------
    internal void InterAssistBuffEnd(CBuffBase pBuffInstance)
    {
        m_listBuffEraseNote.Add(pBuffInstance);
    }

    //--------------------------------------------------------------
    protected void ProtAssistBuffStart(IUnitEventBuff pBuffOrigin, CBuffBase pBuffInstance, float fBuffDuration, float fBuffPower)
    {
        List<CBuffBase> pListBuffMergeNote = FindAssistBuffMergeGroup(pBuffInstance.GetBuffAttribute().BuffMergeGroup);
        if (pListBuffMergeNote.Count > 0)
        {
            for (int i = 0; i < pListBuffMergeNote.Count; i++)
            {
                PrivAssistBuffMergeProcess(pListBuffMergeNote[i], pBuffOrigin, pBuffInstance, fBuffDuration, fBuffPower);
            }
        }
        else
        {
            PrivAssistBuffStart(pBuffOrigin, pBuffInstance, fBuffDuration, fBuffPower);
        }
    }

    protected void ProtAssistBuffEvent(CBuffTaskBase.EBuffTaskEvent eEventType, int iArg, float fArg, params object[] aParams)
    {
        CLinkedList<CBuffBase>.Enumerator it = m_listBuffInstance.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.InterBuffEvent(eEventType, iArg, fArg, aParams);
        }
    }
    //--------------------------------------------------------------
    private void PrivAssistBuffUpdate(float fDelta)
    {
        CLinkedList<CBuffBase>.Enumerator it = m_listBuffInstance.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.InterBuffUpdate(fDelta);
        }
    }

    private void PrivAssistBuffEndProcess()
    {
        if (m_listBuffEraseNote.Count == 0) return;

        for (int i = 0; i < m_listBuffEraseNote.Count; i++)
        {
            CBuffBase pBuffInstance = m_listBuffEraseNote[i];
            if (m_listBuffInstance.Contains(pBuffInstance))
            {
                PrivAssistBuffRemove(pBuffInstance);
            }
        }
        m_listBuffEraseNote.Clear();
    }

    private void PrivAssistBuffStart(IUnitEventBuff pBuffOrigin, CBuffBase pBuffInstance, float fBuffDuration, float fBuffPower)
    {        
        if (pBuffInstance.InterBuffStart(this, m_pUnitBuffOnwer, pBuffOrigin, fBuffDuration, fBuffPower) == 0)
        {
            m_listBuffInstance.AddLast(pBuffInstance);
            OnAssistUnitBuffStart(pBuffInstance);
        }
    }

    private void PrivAssistBuffRemove(CBuffBase pRemoveBuff)
    {
        m_listBuffInstance.Remove(pRemoveBuff);
        OnAssistUnitBuffEnd(pRemoveBuff);
        CObjectInstancePoolBase<CBuffBase>.InstancePoolReturn(pRemoveBuff);
    }

    private void PrivAssistBuffMergeProcess(CBuffBase pMergeTargetBuff, IUnitEventBuff pBuffOrigin, CBuffBase pBuffInstance, float fBuffDuration, float fBuffPower)
    {
        CBuffBase.SBuffAttribute pBuffAtt = pBuffInstance.GetBuffAttribute();
        if (pBuffAtt.MergeExclusive == true) // 기존 버프는 강제 종료되고 새로운 버프가 활성
        {
            pMergeTargetBuff.InterBuffErase(pBuffInstance);
            PrivAssistBuffRemove(pMergeTargetBuff);
            PrivAssistBuffStart(pBuffOrigin, pBuffInstance, fBuffDuration, fBuffPower);
            return;
        }
        
        if (pBuffAtt.MergePowerUp == true)
        {  // 기존 버프의 파워가 올린다.
            pMergeTargetBuff.InterBuffPowerUp(pBuffInstance, fBuffPower);
        }

        if (pBuffAtt.MergeStackUp == true)
        {   // 기존 버프의 스텍을 올린다
            pMergeTargetBuff.InterBuffPowerUp(pBuffInstance, pBuffAtt.StackUpCount);
        }

        if (pBuffAtt.MergeTimePlus == true)
        {
            pMergeTargetBuff.InterBuffTimeUp(pBuffInstance, fBuffDuration);
        }

        if (pBuffAtt.MergeTimeReset == true)
        {
            pMergeTargetBuff.InterBuffTimeReset(pBuffInstance);
        }
    }

    private List<CBuffBase> FindAssistBuffMergeGroup(int iMergeGroup)
    {
        m_listBuffMergeNote.Clear();
        CLinkedList<CBuffBase>.Enumerator it = m_listBuffInstance.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.GetBuffAttribute().BuffMergeGroup == iMergeGroup)
            {
                m_listBuffMergeNote.Add(it.Current);               
            }
        }
        return m_listBuffMergeNote;
    }
    //-------------------------------------------------------------
    protected virtual void OnAssistUnitBuffStart(CBuffBase pBuffInstance) { }
    protected virtual void OnAssistUnitBuffEnd(CBuffBase pBuffInstance) { }
}
