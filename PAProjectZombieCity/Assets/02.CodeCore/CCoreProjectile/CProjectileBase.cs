using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CProjectileBase : CPrefabTemplateItemBase , IEventCollisionTrigger
{
    [SerializeField]
    private CCollisionTriggerBase CollisionTrigger;         protected CCollisionTriggerBase GetProjectileCollisionTrigger() { return CollisionTrigger; }
    [SerializeField]
    private CEffectBase           EffectHit;

    private CMuzzleBase    m_pFireMuzzle = null;            public CMuzzleBase GetProjectileFireMuzzle() { return m_pFireMuzzle; }
    private CUnitActorBase m_pProjectileOwner = null;       public CUnitActorBase GetProjectileOwner() { return m_pProjectileOwner; }

    private bool m_bUpdateMovement = false;
    private CProjectileMovementBase m_pProjectileMovement = null;
    private UnityAction m_delFinish = null;
    private Dictionary<int, CProjectileMovementBase> m_mapProjectileMovementInstance = new Dictionary<int, CProjectileMovementBase>();
    //----------------------------------------------------------
    protected override sealed void OnPrefabTemplateAllocated()
    {
        CollisionTrigger.InterCollisionTriggerInitialize(this);
        OnProjectileInitialize();
    } 
    //-----------------------------------------------------------
    internal void InterProjectileUpdate(float fDelta)
    {
        if (m_bUpdateMovement)
        {
            if (m_pProjectileMovement != null)
            {
                m_pProjectileMovement.InterProjectileMovementUpdate(fDelta);
            }

            OnProjectileUpdate(fDelta);
        }
    }

    //----------------------------------------------------------
    public void DoProjectileFireTarget(CUnitActorBase pProjectileOwner, CUnitActorBase pProjectileTarget, int eMovementType, CMuzzleBase pFireMuzzle, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, UnityAction delMoveFinish, params object [] aParams)
    {
        m_pFireMuzzle = pFireMuzzle;
        m_pProjectileOwner = pProjectileOwner;
        m_delFinish = delMoveFinish;
        m_bUpdateMovement = true;

        SetMonoActive(true);

        PrivProjectileMovementTargetTransformStart(eMovementType, pFireMuzzle, pTargetSocket, vecTargetOffset, ()=> { PrivProjectileArriveDest(); });

        OnProjectileFireTarget(pProjectileOwner, pProjectileTarget,  eMovementType, pTargetSocket, vecTargetOffset, aParams);
    }

    public void DoProjectileFireDirection()
    {
        OnProjectileFireDirection();
    }

    public void DoProjectileDestroy()
    {
        PrivProjectileReturn();
    }

    public virtual Transform GetProjctileTransform() { return transform; }
    //---------------------------------------------------------
    protected void ProtProjectileMovementInstanceAdd(int iMovementType, CProjectileMovementBase pMovementInstance)
    {
        m_mapProjectileMovementInstance[iMovementType] = pMovementInstance;
        pMovementInstance.InterProjectileMovementInitialize(this, GetProjctileTransform());
    }

    //------------------------------------------------------------
    public void IEventCollisionTriggerEnter(CCollisionTriggerBase pEnterTrigger)
    {
        OnProjectileCollisionTriggerEnter(pEnterTrigger);
    }
    public void IEventCollisionTriggerExit(CCollisionTriggerBase pExitTrigger)
    {
        OnProjectileCollisionTriggerExit(pExitTrigger);
    }
    public void IEventCollisionTriggerRefresh(CCollisionTriggerBase pRefreshTrigger)
    {
        OnProjectileCollisionTriggerRefresh(pRefreshTrigger);
    }
    //------------------------------------------------------------
    private void PrivProjectileMovementTargetTransformStart(int eMovementType, CMuzzleBase pFireMuzzle,  CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, UnityAction delMoveFinish)
    {
        if (m_mapProjectileMovementInstance.ContainsKey(eMovementType))
        {
            CProjectileMovementBase pMovementInstance = m_mapProjectileMovementInstance[eMovementType];
            if (pMovementInstance != null)
            {
                Vector3 vecMuzzlePosition = pFireMuzzle.transform.position;
                Quaternion quaMuzzleRoation = pFireMuzzle.transform.rotation;
                pMovementInstance.InterProjectileMovementToTransform(vecMuzzlePosition, vecTargetOffset, quaMuzzleRoation, pTargetSocket.GetShapeSocketTransform(), delMoveFinish);
                m_pProjectileMovement = pMovementInstance;
            }
        }
    }

    private void PrivProjectileReturn()
    {
        m_bUpdateMovement = false;
        SetMonoActive(false);        
        transform.SetParent(m_pFireMuzzle.transform);        
        ProtPrefabTemplateReturn();
        m_delFinish?.Invoke();
    }

    private void PrivProjectileArriveDest()
    {
        m_bUpdateMovement = false;
        OnProjectileArriveDest();
    }

    //-----------------------------------------------------------
    protected virtual void OnProjectileInitialize() { }
    protected virtual void OnProjectileFireTarget(CUnitActorBase pProjectileOwner, CUnitActorBase pProjectileTarget, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecOffset, params object[] aParams) { }
    protected virtual void OnProjectileFireDirection() { }
    protected virtual void OnProjectileArriveDest() { }
    protected virtual void OnProjectileUpdate(float fDelta) { }
    protected virtual void OnProjectileCollisionTriggerEnter(CCollisionTriggerBase pEnterChecker) { }
    protected virtual void OnProjectileCollisionTriggerRefresh(CCollisionTriggerBase pEnterChecker) { }
    protected virtual void OnProjectileCollisionTriggerExit(CCollisionTriggerBase pEnterChecker) { }
}

