using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 충돌 이벤트를 수신하여 

[RequireComponent(typeof(Rigidbody))]
public abstract class CCollisionRegidbodyBase : CMonoBase , IEventCollisionTrigger
{
    [SerializeField]
    private bool DefaultKinematic = true;
    [SerializeField]
    private CCollisionTriggerBase CollisionTrigger = null;

    private Rigidbody m_pRigidBody = null;
    private IEventCollisionTrigger m_pOwnerReceiver = null;  public IEventCollisionTrigger GetCollisionCheckerOwner() { return m_pOwnerReceiver;}
    private int m_hCollisionCheckID = 0;                     public int GetCollisionCheckID() { return m_hCollisionCheckID; }
    //--------------------------------------------------
    internal void InterCollisionCheckerInitialize(IEventCollisionTrigger pOwnerReceiver)
    {
        m_pRigidBody = GetComponent<Rigidbody>();
        m_pRigidBody.isKinematic = DefaultKinematic;       
        m_pOwnerReceiver = pOwnerReceiver;

        CollisionTrigger.InterCollisionTriggerInitialize(m_pOwnerReceiver);
        SetMonoActive(false);
    }
    //--------------------------------------------------
    public void IEventCollisionTriggerEnter(CCollisionTriggerBase pEnterTrigger)
    {
        OnCollisionClashEnter(m_pOwnerReceiver, pEnterTrigger);
    }

    public void IEventCollisionTriggerExit(CCollisionTriggerBase pExitTrigger)
    {
        OnCollisionClashRefresh(m_pOwnerReceiver, pExitTrigger);
    }

    public void IEventCollisionTriggerRefresh(CCollisionTriggerBase pRefreshTrigger)
    {
        OnCollisionClashExit(m_pOwnerReceiver, pRefreshTrigger);
    }
    //----------------------------------------------------
    public void DoCollisionRigidEnable(bool bEnable)
    {
        SetMonoActive(bEnable);
    }

    //-------------------------------------------------
    private void FixedUpdate()
    {
        OnCollisionCheckerUpdate(Time.fixedDeltaTime);
    }
    private void OnEnable()
    {       
        OnCollisionCheckerReset();
    }
    //-------------------------------------------------------
    protected virtual void OnCollisionCheckerUpdate(float fDelta) { }
    protected virtual void OnCollisionCheckerReset() { }
    protected virtual void OnCollisionCheckerEnter() { }

    protected virtual void OnCollisionClashEnter(IEventCollisionTrigger pEventReceiver, CCollisionTriggerBase pEnterTrigger) { }
    protected virtual void OnCollisionClashRefresh(IEventCollisionTrigger pEventReceiver, CCollisionTriggerBase pExitTrigger) { }
    protected virtual void OnCollisionClashExit(IEventCollisionTrigger pEventReceiver, CCollisionTriggerBase pRefreshTrigger) { }
}
