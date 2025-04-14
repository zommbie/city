using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CMuzzleProjectileBase : CMuzzleBase
{
    private CLinkedList<CUnitInstanceProjectileBase> m_listFiredProjectile = new CLinkedList<CUnitInstanceProjectileBase>();
    //-------------------------------------------------------
    internal void InterMuzzleProjectileDismount(CUnitInstanceProjectileBase pDisableProjectile)
    {
        m_listFiredProjectile.Remove(pDisableProjectile);
        OnMuzzleProjectileDisable(pDisableProjectile);
    }

    //--------------------------------------------------------
    protected void ProtMuzzleProjectileMount(CUnitInstanceProjectileBase pProjectileInstance)
    {
        m_listFiredProjectile.AddLast(pProjectileInstance);
        pProjectileInstance.InterUnitProjectileMuzzleMount(this);
    }
    

    //-------------------------------------------------------
    protected virtual void OnMuzzleProjectileDisable(CUnitInstanceProjectileBase pDisableProjectile){ }
    
}
