using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CCollisionCheckerBase : CMonoBase
{
    [SerializeField]
    private CMonoBase CollisionOwner; public CMonoBase GetCollisionCheckerOwner() { return CollisionOwner; } 

    private Rigidbody m_pRigidBody = null;
    private bool m_bEnable = true; public void SetCollisionCheckerEnable(bool bEnable) { m_bEnable = bEnable; }
    //----------------------------------------------
    protected override void OnUnityAwake()
    {
        m_pRigidBody = GetComponent<Rigidbody>();
        m_pRigidBody.isKinematic = true;
    }
    //-------------------------------------------------
    private void FixedUpdate()
    {
        if (m_bEnable)
        {
            OnCollisionCheckerUpdate(Time.fixedDeltaTime);
        }
    }
    private void OnEnable()
    {       
        OnCollisionCheckerReset();
    }
    //------------------------------------------------
    internal void InterCollisionEnter(CMonoBase pTriggerEnter)
    {
        OnCollisionCheckerEnter(pTriggerEnter);
    }

    internal void InterCollisionExit(CMonoBase pTriggerExit)
    {
        OnCollisionCheckerExit(pTriggerExit);
    }

    //-------------------------------------------------------
    protected virtual void OnCollisionCheckerUpdate(float fDelta) { }
    protected virtual void OnCollisionCheckerEnter(CMonoBase pTriggerEnter) { }
    protected virtual void OnCollisionCheckerExit(CMonoBase pTriggerExit) { }
    protected virtual void OnCollisionCheckerReset() { }


}
