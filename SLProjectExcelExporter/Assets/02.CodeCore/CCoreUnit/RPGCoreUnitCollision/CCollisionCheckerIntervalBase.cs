using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 충돌해 있는 객체에 간격 만큼 이벤트를 발생 시킨다.
public abstract class CCollisionCheckerIntervalBase : CCollisionCheckerBase 
{
    [SerializeField]
    private float Interval = 0.2f;

    public class SCollisionCheckInfo
    {
        public float    CurrentInterval;
        public int      TickCount;
        public CMonoBase CollisionObject;
    }
     
    private CLinkedList<SCollisionCheckInfo> m_listCollisionCheckInfo = new CLinkedList<SCollisionCheckInfo>();
    //------------------------------------------------------
    protected override void OnCollisionCheckerReset()
    {
        m_listCollisionCheckInfo.Clear();       
    }

    protected override void OnCollisionCheckerUpdate(float fDelta)
    {
        UpdateCollisionInterval(fDelta);
    }

    protected override sealed void OnCollisionCheckerEnter(CMonoBase pObjectEnter)
    {
        PrivCollisionCheckerEnter(pObjectEnter);
    }

    protected sealed override void OnCollisionCheckerExit(CMonoBase pObjectExit)
    {
        PrivCollisionCheckerExit(pObjectExit);
    }

    //-------------------------------------------------
    private void UpdateCollisionInterval(float fDelta)
    {
        CLinkedList<SCollisionCheckInfo>.Enumerator it = m_listCollisionCheckInfo.GetEnumerator();
        while (it.MoveNext())
        {
            SCollisionCheckInfo pCheckInfo = it.Current;
            pCheckInfo.CurrentInterval += fDelta;

            if (pCheckInfo.CurrentInterval >= Interval)
            {
                pCheckInfo.CurrentInterval = 0;
                pCheckInfo.TickCount++;
                OnCollisionCheckerIntervalTick(pCheckInfo.CollisionObject, pCheckInfo.TickCount);
            }           
        }
    }
 
    private SCollisionCheckInfo FindCollisionCheckInfo(CMonoBase pCheckObject)
    {
        SCollisionCheckInfo pFind = null;
        CLinkedList<SCollisionCheckInfo>.Enumerator it = m_listCollisionCheckInfo.GetEnumerator();
        while(it.MoveNext())
        {
            if (it.Current.CollisionObject == pCheckObject)
            {
                pFind = it.Current;
                break;
            }
        }
        return pFind;
    }

    private void PrivCollisionCheckerEnter(CMonoBase pObjectEnter)
    {
        SCollisionCheckInfo pFindChecker = FindCollisionCheckInfo(pObjectEnter);
        if (pFindChecker == null)
        {
            pFindChecker = new SCollisionCheckInfo();
            pFindChecker.CollisionObject = pObjectEnter;
            m_listCollisionCheckInfo.AddLast(pFindChecker);
            OnCollisionCheckerIntervalEnter(pObjectEnter);
        }
    }

    private void PrivCollisionCheckerExit(CMonoBase pObjectExit)
    {
        SCollisionCheckInfo pFindChecker = FindCollisionCheckInfo(pObjectExit);
        if (pFindChecker != null)
        {
            m_listCollisionCheckInfo.Remove(pFindChecker);
            OnCollisionCheckerIntervalExit(pObjectExit);
        }
    }

    //--------------------------------------------------------
    protected virtual void OnCollisionCheckerIntervalTick(CMonoBase pObjectTick, int iTickCount) { }
    protected virtual void OnCollisionCheckerIntervalEnter(CMonoBase pObjectEnter) { }
    protected virtual void OnCollisionCheckerIntervalExit(CMonoBase pObjectExit) { }

}
