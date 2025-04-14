using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 같은 오브젝트의 경우 딜레이 만큼 무시한다. 
// 물리 모델에서 같은 오브젝트가 빠르게 진동 할 경우 과도한 충돌 이벤트 발생을 억제 하기 위해 사용된다
public abstract class CCollisionCheckerDelayBase : CCollisionCheckerBase 
{
    [SerializeField]
    private float Delay = 0.5f;

    public class SCollisionDelayInfo
    {
        public float CurrentInterval;
        public CMonoBase CollisionObject;
    }

    private CLinkedList<SCollisionDelayInfo> m_listCollisionDelayInfo = new CLinkedList<SCollisionDelayInfo>();
    //---------------------------------------------------------
    protected override void OnCollisionCheckerReset()
    {
        m_listCollisionDelayInfo.Clear();
    }

    protected override void OnCollisionCheckerEnter(CMonoBase pObjectEnter)
    {
        PrivCollisionCheckerEnter(pObjectEnter);
    }

    protected override void OnCollisionCheckerUpdate(float fDelta)
    {
        UpdateCollisionDelay(fDelta);
    }
    //-------------------------------------------------------
    private void UpdateCollisionDelay(float fDelta)
    {
        CLinkedList<SCollisionDelayInfo>.Enumerator it = m_listCollisionDelayInfo.GetEnumerator();
        while (it.MoveNext())
        {
            SCollisionDelayInfo pCheckInfo = it.Current;
            pCheckInfo.CurrentInterval += fDelta;

            if (pCheckInfo.CurrentInterval >= Delay)
            {
                it.Remove();
            }
        }
    }

    private SCollisionDelayInfo FindCollisionCheckInfo(CMonoBase pCheckObject)
    {
        SCollisionDelayInfo pFind = null;
        CLinkedList<SCollisionDelayInfo>.Enumerator it = m_listCollisionDelayInfo.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.CollisionObject == pCheckObject)
            {
                pFind = it.Current;
                break;
            }
        }
        return pFind;
    }

    private void PrivCollisionCheckerEnter(CMonoBase pEnterObject)
    {
        SCollisionDelayInfo pFindChecker = FindCollisionCheckInfo(pEnterObject);
        if (pFindChecker == null)
        {
            pFindChecker = new SCollisionDelayInfo();
            pFindChecker.CollisionObject = pEnterObject;
            m_listCollisionDelayInfo.AddLast(pFindChecker);
            OnCollisionCheckerDelayEnter(pEnterObject);
        }
    }

    private void PrivCollisionCheckerExit(CMonoBase pExitOnject)
    {
        SCollisionDelayInfo pFindChecker = FindCollisionCheckInfo(pExitOnject);
        if (pFindChecker != null)
        {
            m_listCollisionDelayInfo.Remove(pFindChecker);
            OnCollisionCheckerDelayExit(pExitOnject);
        }
    }

    //----------------------------------------------------------------
    
    protected virtual void OnCollisionCheckerDelayEnter(CMonoBase pObjectEnter) { }
    protected virtual void OnCollisionCheckerDelayExit(CMonoBase pObjectExit) { }


}
