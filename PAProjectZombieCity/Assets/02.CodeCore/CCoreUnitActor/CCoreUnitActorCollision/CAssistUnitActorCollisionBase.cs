using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유닛들을 구성하는 다양한 파츠의 충돌 체크를 담당한다. 
// 대부분 파츠 하나로 사용할 것이다. 

public abstract class CAssistUnitActorCollisionBase : CAssistUnitActorBase, IEventCollisionTrigger
{
    private Dictionary<int, CCollisionRegidbodyBase> m_mapCollisionCheckInstance = new Dictionary<int, CCollisionRegidbodyBase>();
    //---------------------------------------------------------
    protected override void OnAssistInitialize(CUnitActorBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        CCollisionRegidbodyBase[] aCollisionChecker = GetComponentsInChildren<CCollisionRegidbodyBase>(true);
        for (int i = 0; i < aCollisionChecker.Length; i++)
        {
            aCollisionChecker[i].InterCollisionCheckerInitialize(this);
            m_mapCollisionCheckInstance[aCollisionChecker[i].GetCollisionCheckID()] = aCollisionChecker[i];
        }
    }
    //--------------------------------------------------------
    public void IEventCollisionTriggerEnter(CCollisionTriggerBase pEnterChecker)
    {
        CCollisionRegidbodyBase pCollisionChecker = pEnterChecker.GetCollisionTriggerOwner<CCollisionRegidbodyBase>();
        if (pCollisionChecker != null)
        {
            OnAssistCollisionClashStart(pCollisionChecker, pEnterChecker);
        }
    }
    public void IEventCollisionTriggerExit(CCollisionTriggerBase pExitChecker)
    {
        CCollisionRegidbodyBase pCollisionChecker = pExitChecker.GetCollisionTriggerOwner<CCollisionRegidbodyBase>();
        if (pCollisionChecker != null)
        {
            OnAssistCollisionClashExit(pCollisionChecker, pExitChecker);
        }
    }
    public void IEventCollisionTriggerRefresh(CCollisionTriggerBase pRefeshChecker)
    {
        CCollisionRegidbodyBase pCollisionChecker = pRefeshChecker.GetCollisionTriggerOwner<CCollisionRegidbodyBase>();
        if (pCollisionChecker != null)
        {
            OnAssistCollisionClashRefresh(pCollisionChecker, pRefeshChecker);
        }
    }

  
    //----------------------------------------------------------
    protected virtual void OnAssistCollisionClashStart(CCollisionRegidbodyBase pClashChecker, CCollisionTriggerBase pClashTrigger) { }
    protected virtual void OnAssistCollisionClashRefresh(CCollisionRegidbodyBase pClashChecker, CCollisionTriggerBase pClashTrigger) { }
    protected virtual void OnAssistCollisionClashExit(CCollisionRegidbodyBase pClashChecker, CCollisionTriggerBase pClashTrigger) { }

}
