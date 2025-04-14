using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CMuzzleBase : CMonoBase
{
    [SerializeField]
    private CEffectBase EffectMuzzleFire = null;

    private CUnitBase m_pMuzzleOwnerUnit = null;   public CUnitBase GetMuzzleOwnerUnit() { return m_pMuzzleOwnerUnit; }
    //---------------------------------------------------------
    internal void InterMuzzleInitialize(CUnitBase pMuzzleOwnerUnit)
    {
        m_pMuzzleOwnerUnit = pMuzzleOwnerUnit;
        OnMuzzleInitialize(pMuzzleOwnerUnit);
    }

    private void Update()
    {
        OnMuzzleUpdate();
    }

    //---------------------------------------------------------
    public void DoMuzzleFireProjectileDirection(float fLength, int iFireType, Vector3 vecDirectionOffset)
    {
        PrivMuzzleEffect();
        OnMuzzleFireProjectileDirection(m_pMuzzleOwnerUnit, ExtractMuzzleFireDirection(vecDirectionOffset), fLength, iFireType);
    }

    public void DoMuzzleFireProjectileTarget(float fMoveSpeed, CUnitBase pTargetUnit, int iFireType, Vector3 vecDirectionOffset)
    {
        PrivMuzzleEffect();
        OnMuzzleFireProjectileTarget(m_pMuzzleOwnerUnit, ExtractMuzzleFireDirection(vecDirectionOffset), fMoveSpeed, iFireType, pTargetUnit);
    }   

    public void DoMuzzleFireEnd()
    {
        OnMuzzleFireEnd();
    }
    
    public void DoMuzzleFireHitScanTarget(CUnitBase pHitScanTarget, int eHitScanType, Vector3 vecDirectionOffset)
    {
        PrivMuzzleEffect();
        OnMuzzleFireHitScanTarget(m_pMuzzleOwnerUnit, pHitScanTarget, ExtractMuzzleFireDirection(vecDirectionOffset), eHitScanType);
    }

    public void DoMuzzleFireHitScanDirection(Vector3 vecFireDirection, int eHitScanType, float fLength, float fDuration)
    {
        PrivMuzzleEffect();
        transform.forward = vecFireDirection;
        OnMuzzleFireHitScanDirection(m_pMuzzleOwnerUnit, vecFireDirection, eHitScanType, fLength, fDuration);
    }

    //-------------------------------------------------------
    public virtual Vector3 GetMuzzlePosition()
    {
        return transform.position;
    }

    public virtual Vector3 GetMuzzleDirection()
    {
        return transform.rotation * Vector3.forward;
    }
    //--------------------------------------------------------
    private void PrivMuzzleEffect()
    {
        if(EffectMuzzleFire == null) return;
        EffectMuzzleFire.DoEffectStart();
    }

    private Vector3 ExtractMuzzleFireDirection(Vector3 vecDirectionOffset)
    {
        Vector3 vecResult = transform.forward;
        return vecResult;
    }

    //--------------------------------------------------------
    protected virtual void OnMuzzleFireProjectileDirection(CUnitBase pProjectileOwner, Vector3 vecFireDirection, float fLength, int iFireType) { }
    protected virtual void OnMuzzleFireProjectileTarget(CUnitBase pProjectileOwner, Vector3 vecFireDirection, float fMoveSpeed, int iFireType, CUnitBase pTarget) { }
    protected virtual void OnMuzzleFireHitScanTarget(CUnitBase pHitScanOwner, CUnitBase pHitScanTarget, Vector3 vecFireDirection, int iFireType) {  }
    protected virtual void OnMuzzleFireHitScanDirection(CUnitBase pHitScanOwner, Vector3 vecFireDirection, int iFireType, float fLength, float fDuration) { }
    protected virtual void OnMuzzleInitialize(CUnitBase pMuzzleOwnerUnit) { }
    protected virtual void OnMuzzleFireEnd() { }
    protected virtual void OnMuzzleUpdate() { }
}
