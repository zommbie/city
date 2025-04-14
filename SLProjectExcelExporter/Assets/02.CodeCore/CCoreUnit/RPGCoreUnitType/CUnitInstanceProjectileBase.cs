using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitInstanceProjectileBase : CUnitAuthorityBase
{
    [SerializeField] [Header("[UnitAssist]")]
    private CAssistUnitAIBase                ProjectileAI;
    [SerializeField]
    private CAssistMovementBase              ProjectileMovement;
    [SerializeField]
    private CFiniteStateMachineUnitSkillBase ProjectileFSM;
   
    private CMuzzleProjectileBase m_pProjectileOwnerMuzzle = null;
    private CUnitBase m_pProjectileOwnerUnit = null;                    public CUnitBase GetUnitProjectileOwnerUnit() { return m_pProjectileOwnerUnit; }
    //----------------------------------------------------------------------------
    protected override void OnUnitInitialize()
    {
        base.OnUnitInitialize();
        PrivUnitProjectileInstance();
    }
    protected override sealed void ProtUnitAssistAdd(CAssistUnitBase pAssistUnit)
    {
        base.ProtUnitAssistAdd(pAssistUnit);
    }

    //---------------------------------------------------------------------------
    internal void InterUnitProjectileMuzzleMount(CMuzzleProjectileBase pMuzzleOwner)
    {
        m_pProjectileOwnerMuzzle = pMuzzleOwner;
        m_pProjectileOwnerUnit = pMuzzleOwner.GetMuzzleOwnerUnit();
        OnUnitProjectileMuzzleMount(pMuzzleOwner);
    }

    //---------------------------------------------------------------------------
    protected void ProtUnitProjectileDismount()
    {
        m_pProjectileOwnerMuzzle?.InterMuzzleProjectileDismount(this);
    }

    //---------------------------------------------------------------------------
    private void PrivUnitProjectileInstance()
    {
        InstanceUnitAI(ProjectileAI);
        InstanceUnitMovement(ProjectileMovement);
        InstanceUnitFSM(ProjectileFSM);       

        ProtUnitAssistAdd(ProjectileAI);
        ProtUnitAssistAdd(ProjectileMovement);
   //     ProtUnitAssistAdd(ProjectileFSM);       
    }

    //-----------------------------------------------------------------------------
    protected virtual void OnUnitProjectileMuzzleMount(CMuzzleBase pMuzzle) { }
}
