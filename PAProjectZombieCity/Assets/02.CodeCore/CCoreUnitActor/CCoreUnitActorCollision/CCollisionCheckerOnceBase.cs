using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 같은 오브젝트의 경우 딜레이 만큼 무시한다. 
// 물리 모델에서 같은 오브젝트가 빠르게 진동 할 경우 과도한 충돌 이벤트 발생을 억제 하기 위해 사용된다

public abstract class CCollisionCheckerOnceBase : CCollisionRegidbodyBase 
{
    [SerializeField]
    private float ResetInterval = 1f;

    public class SCheckOnceinfo
    {
        public float CurrentInterval;
        public CCollisionTriggerBase CollisionTrigger;
    }

    private CLinkedList<SCheckOnceinfo> m_listCollisionDelayInfo = new CLinkedList<SCheckOnceinfo>();
    //---------------------------------------------------------
    protected override void OnCollisionCheckerReset()
    {
        m_listCollisionDelayInfo.Clear();
    }
  
    protected override void OnCollisionCheckerUpdate(float fDelta)
    {
        UpdateCollisionDelay(fDelta);
    }

    protected override void OnCollisionClashEnter(IEventCollisionTrigger pEventReceiver, CCollisionTriggerBase pEnterTrigger)
    {
        base.OnCollisionClashEnter(pEventReceiver, pEnterTrigger);
        pEventReceiver.IEventCollisionTriggerEnter(pEnterTrigger);
    }

    protected override void OnCollisionClashRefresh(IEventCollisionTrigger pEventReceiver, CCollisionTriggerBase pExitTrigger)
    {
        base.OnCollisionClashRefresh(pEventReceiver, pExitTrigger);
        pEventReceiver.IEventCollisionTriggerRefresh(pExitTrigger);
    }

    protected override void OnCollisionClashExit(IEventCollisionTrigger pEventReceiver, CCollisionTriggerBase pRefreshTrigger)
    {
        base.OnCollisionClashExit(pEventReceiver, pRefreshTrigger);
        pEventReceiver.IEventCollisionTriggerExit(pRefreshTrigger);
    }

    //-------------------------------------------------------
    private void UpdateCollisionDelay(float fDelta)
    {
        CLinkedList<SCheckOnceinfo>.Enumerator it = m_listCollisionDelayInfo.GetEnumerator();
        while (it.MoveNext())
        {
            SCheckOnceinfo pCheckInfo = it.Current;
            pCheckInfo.CurrentInterval += fDelta;

            if (pCheckInfo.CurrentInterval >= ResetInterval)
            {
                it.Remove();
            }
        }
    }

    private SCheckOnceinfo FindCollisionCheckInfo(CMonoBase pCheckObject)
    {
        SCheckOnceinfo pFind = null;
        CLinkedList<SCheckOnceinfo>.Enumerator it = m_listCollisionDelayInfo.GetEnumerator();
        while (it.MoveNext())
        {
            //if (it.Current.CollisionObject == pCheckObject)
            //{
            //    pFind = it.Current;
            //    break;
            //}
        }
        return pFind;
    }

    private void PrivCollisionCheckerEnter(CMonoBase pEnterObject)
    {
        SCheckOnceinfo pFindChecker = FindCollisionCheckInfo(pEnterObject);
        if (pFindChecker == null)
        {
            pFindChecker = new SCheckOnceinfo();
      //      pFindChecker.CollisionObject = pEnterObject;
            m_listCollisionDelayInfo.AddLast(pFindChecker);
            OnCollisionCheckerDelayEnter(pEnterObject);
        }
    }

    private void PrivCollisionCheckerExit(CMonoBase pExitOnject)
    {
        SCheckOnceinfo pFindChecker = FindCollisionCheckInfo(pExitOnject);
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
