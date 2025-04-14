using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class CMuzzlePrjectileBase : CMuzzleBase
{
    private List<CProjectileBase> m_listProjectileUpdateNote = new List<CProjectileBase>();
    private LinkedList<CProjectileBase> m_listProjectileInstance = new LinkedList<CProjectileBase>();
    //---------------------------------------------------------------
    protected override sealed void OnPrefabTemplateReturn(CPrefabTemplateItemBase pReturnItem)
    {
        CProjectileBase pProjectile = pReturnItem as CProjectileBase;
        if (pProjectile != null)
        {
            pProjectile.transform.SetParent(transform);
            pProjectile.transform.localPosition = Vector3.zero;
            m_listProjectileInstance.Remove(pProjectile);
        }
    }

    protected override void OnMuzzleUpdate(float fDelta)
    {
        m_listProjectileUpdateNote.Clear();

        LinkedList<CProjectileBase>.Enumerator it = m_listProjectileInstance.GetEnumerator();
        while (it.MoveNext())
        {
            m_listProjectileUpdateNote.Add(it.Current);
        }

        for (int i = 0; i < m_listProjectileUpdateNote.Count; i++)
        {
            m_listProjectileUpdateNote[i].InterProjectileUpdate(fDelta);
        }
    }

    protected override sealed void OnMuzzleFireTarget(CUnitActorBase pProjectileOwner, CUnitActorBase pProjectileTarget, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, params object[] aParams)
    {
        CProjectileBase pFireProjectile = MakeMuzzleProjectileInstance();
        if (pFireProjectile != null)
        {
            PrivMuzzleProjectileFireTarget(pFireProjectile, pProjectileOwner, pProjectileTarget, eMovementType, pTargetSocket, vecTargetOffset, ()=> { PrivMuzzleProjectileMoveFinish(pFireProjectile); }, aParams);
        }
    }

    //-----------------------------------------------------------------------------
    private CProjectileBase MakeMuzzleProjectileInstance()
    {
        return ProtPrefabTemplateRequestInstance() as CProjectileBase;
    }

    private void PrivMuzzleProjectileFireTarget(CProjectileBase pFireProjectile, CUnitActorBase pProjectileOwner, CUnitActorBase pProjectileTarget,  int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, UnityAction delFinish, params object[] aParams)
    {
        pFireProjectile.transform.SetParent(null);
        pFireProjectile.transform.position = transform.position;
        pFireProjectile.transform.rotation = transform.rotation;
        pFireProjectile.transform.localScale = transform.localScale;

        m_listProjectileInstance.AddLast(pFireProjectile);
        pFireProjectile.DoProjectileFireTarget(pProjectileOwner, pProjectileTarget, eMovementType, this, pTargetSocket, vecTargetOffset, delFinish, aParams);
        OnMuzzleProjectileFire(pFireProjectile);
    }

    private void PrivMuzzleProjectileMoveFinish(CProjectileBase pProjectile)
    {
        m_listProjectileInstance.Remove(pProjectile);
        InterPrefabTemplateItemReturn(pProjectile);
    }

    //------------------------------------------------------------------------------------
    

    //-----------------------------------------------------------------------------------------
    protected virtual void OnMuzzleProjectileFire(CProjectileBase pFireProjectile) { }
}
