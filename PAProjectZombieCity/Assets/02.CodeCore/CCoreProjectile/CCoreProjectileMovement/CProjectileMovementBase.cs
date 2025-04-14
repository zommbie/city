using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CProjectileMovementBase : CMonoBase
{
    protected CProjectileBase   m_pOwnerProjectile  = null;
    protected Transform         m_pMoveTransform    = null;
    protected Transform         m_pTargetTransform  = null;
    protected Vector3           m_vecTargetPosition = Vector3.zero;
    protected Vector3           m_vecTargetOffset   = Vector3.zero;
    protected Vector3           m_vecFirePosition   = Vector3.zero;
    protected Quaternion        m_quaFireRotation   = Quaternion.identity;

    private bool  m_bEnable = false;   
    private UnityAction m_delFinish = null;
    //-----------------------------------------------------------------
    internal void InterProjectileMovementInitialize(CProjectileBase pOwnerProjectile, Transform pMoveTransform)
    {
        m_pOwnerProjectile = pOwnerProjectile;
        m_pMoveTransform = pMoveTransform;
        OnProjectileMovementInitialize();
    }

    internal void InterProjectileMovementUpdate(float fDelta)
    {
        if (m_bEnable)
        {
            OnProjectileMovementUpdate(fDelta);
        }
    }

    internal void InterProjectileMovementToTransform(Vector3 vecFirePosition, Vector3 vecTargetOffset, Quaternion quaFireRotation, Transform pTargetTransform, UnityAction delFinish)
    {
        m_bEnable = true;
        m_vecFirePosition = vecFirePosition;
        m_quaFireRotation = quaFireRotation;
        m_pTargetTransform = pTargetTransform;
        m_vecTargetOffset = vecTargetOffset;
        m_delFinish = delFinish;
       
        OnProjectileMovementToTransform(vecFirePosition, vecTargetOffset, quaFireRotation, pTargetTransform);
    }

    //-------------------------------------------------------------------
    protected void ProtProjectileMovementArrive()
    {
        m_delFinish?.Invoke();
        m_bEnable = false;
    }

    protected void ProtProjectileMovementResetRotation()
    {
        m_pOwnerProjectile.transform.localEulerAngles = Vector3.zero;
        m_pOwnerProjectile.transform.eulerAngles = Vector3.zero;
    }

    //----------------------------------------------------------------------
    public void DoProjectileEnable(bool bEnable)
    {
        m_bEnable = false;
    }


    //---------------------------------------------------------------------
    protected virtual void OnProjectileMovementInitialize() { }
    protected virtual void OnProjectileMovementUpdate(float fDelta) { }
    protected virtual void OnProjectileMovementToTransform(Vector3 vecFirePosition, Vector3 vecTargetOffset, Quaternion quaFireRotation, Transform pTargetTransform) { }
  

}
