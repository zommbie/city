using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSkillTaskEventListBase 
{

    private CMultiDictionary<int, CSkillTaskContainer> m_mapSkillTaskContainer = new CMultiDictionary<int, CSkillTaskContainer>();
    //-------------------------------------------------
    internal void InterSkillTaskEventListReset(CFiniteStateMachineUnitSkillBase pFSMOnwer)
    {
        IEnumerator<List<CSkillTaskContainer>> it = m_mapSkillTaskContainer.value.GetEnumerator();
        while(it.MoveNext())
        {
            List<CSkillTaskContainer> pList = it.Current;
            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterSkillTaskContainerReset(pFSMOnwer);

            }
        }
        OnSkillTaskEventListReset();
    }

    internal void InterSkillTaskEvent(SSkillUsage pSkillUsage, int eTaskEventType, int iArg, float fArg, params object[] aParams)
    {
        if (m_mapSkillTaskContainer.ContainsKey(eTaskEventType))
        {
            List<CSkillTaskContainer> pList = m_mapSkillTaskContainer[eTaskEventType];

            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterSkillTaskContainerExcute(pSkillUsage, iArg, fArg, aParams);
            }
        }
    }

    internal void InterSkillTaskUpdate(float fDelta)
    {
        IEnumerator<List<CSkillTaskContainer>> it = m_mapSkillTaskContainer.value.GetEnumerator();
        while (it.MoveNext())
        {
            List<CSkillTaskContainer> pList = it.Current;
            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterSkillTaskContainerUpdate(fDelta);
            }
        }
    }

    internal void InterSkillTaskInterval(int iIntervalCount)
    {
        IEnumerator<List<CSkillTaskContainer>> it = m_mapSkillTaskContainer.value.GetEnumerator();
        while (it.MoveNext())
        {
            List<CSkillTaskContainer> pList = it.Current;
            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterSkillTaskContainerInterval(iIntervalCount);
            }
        }
    }

    internal void InterSkillTaskEnter(CSkillStateBase pEnterState, CSkillStateBase pPrevState)
    {
        IEnumerator<List<CSkillTaskContainer>> it = m_mapSkillTaskContainer.value.GetEnumerator();
        while (it.MoveNext())
        {
            List<CSkillTaskContainer> pList = it.Current;
            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterSkillTaskContainerEnter(pEnterState, pPrevState);
            }
        }
    }

    internal void InterSkillTaskExit(CSkillStateBase pExitState)
    {
        IEnumerator<List<CSkillTaskContainer>> it = m_mapSkillTaskContainer.value.GetEnumerator();
        while (it.MoveNext())
        {
            List<CSkillTaskContainer> pList = it.Current;
            for (int i = 0; i < pList.Count; i++)
            {
                pList[i].InterSkillTaskContainerExit(pExitState);
            }
        }
    }

    //-------------------------------------------------
    protected void ProtSkillTaskEventAdd(int eEventType, CSkillTaskContainer pSkillTaskContainer)
    {
        m_mapSkillTaskContainer.Add(eEventType, pSkillTaskContainer);
    }
    //-------------------------------------------------
    protected virtual void OnSkillTaskEventListReset() { }
    protected virtual void OnSkillTaskEventEnter() { }
    protected virtual void OnSkillTaskEventExit() { }
    protected virtual void OnSkillTaskEventAnimationEvent(SSkillUsage pUsage, string strAniName, string strAniEventName, int iArg, float fArg) { }
}
