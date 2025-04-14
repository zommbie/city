using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발사된 발사체의 인스턴스 관리 및 업데이트등 

public abstract class CMuzzleBase : CPrefabTemplateBase
{
    [SerializeField]
    private CEffectBase EffectMuzzleFlash = null;
    private CUnitActorBase m_pMuzzleOwnerActor = null;
    //-----------------------------------------------------------------
    internal void InterMuzzleUpdate(float fDelta)
    {   
        OnMuzzleUpdate(fDelta);
    }

    internal void InterMuzzleInitialize(CUnitActorBase pMuzzleOwnerActor)
    {
        m_pMuzzleOwnerActor = pMuzzleOwnerActor;
    }
   
    //---------------------------------------------------------------
    public void DoMuzzleFireTarget(CUnitActorBase pTargetActor, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, params object [] aParams)
    {
        PrivMuzzleProjectileEffectFlash();
        OnMuzzleFireTarget(m_pMuzzleOwnerActor, pTargetActor, eMovementType, pTargetSocket, vecTargetOffset, aParams);
    }

    public void DoMuzzleFireDirection(Vector3 vecDirection, int eDirectionType, params object[] aParams)
    {
        PrivMuzzleProjectileEffectFlash();
        OnMuzzleFireDirection(vecDirection, eDirectionType, aParams);
    }

   
    //--------------------------------------------------------------
   

    private void PrivMuzzleProjectileEffectFlash()
    {
        EffectMuzzleFlash?.DoEffectStart();
    }
    //---------------------------------------------------------------
    protected virtual void OnMuzzleUpdate(float fDelta) { }
    protected virtual void OnMuzzleFireTarget(CUnitActorBase pOwnerActor, CUnitActorBase pTargetActor, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, params object[] aParams) { }
    protected virtual void OnMuzzleFireDirection(Vector3 vecDirection, int eDirectionType, params object[] aParams) { }
}   
