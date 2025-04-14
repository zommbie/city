using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CCollisionTriggerBase : CMonoBase
{
    [SerializeField]
    private CCollisionCheckerBase CollisionChecker;

    private Collider m_pCollider = null;
    //-----------------------------------------------------------------------
    protected override void OnUnityAwake()
    {
        m_pCollider = GetComponent<Collider>();
        m_pCollider.isTrigger = true;
    }

    //-------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {    
        CCollisionTriggerBase pTrigger = other.gameObject.GetComponent<CCollisionTriggerBase>();
        if (pTrigger != null)
        {
            CMonoBase pEnterObject = pTrigger.GetTriggerOwner();
            if (pEnterObject != null)
            {
                CollisionChecker.InterCollisionEnter(pEnterObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CCollisionTriggerBase pTrigger = other.gameObject.GetComponent<CCollisionTriggerBase>();
        if (pTrigger != null)
        {
            CMonoBase pExitObject = pTrigger.GetTriggerOwner();
            if (pExitObject != null)
            {
                CollisionChecker.InterCollisionExit(pExitObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

    }
    //-----------------------------------------------------
    public CMonoBase GetTriggerOwner() { return CollisionChecker.GetCollisionCheckerOwner(); }


    //---------------------------------------------------------
    protected virtual void OnCollisionTriggerEnter(CCollisionTriggerBase pCollisionEnter) { }
    protected virtual void OnCollisionTriggerExit(CCollisionTriggerBase pCollisionExit) { }
    protected virtual void OnCollisionTriggerStay(CCollisionTriggerBase pCollisionStay) { }
}
