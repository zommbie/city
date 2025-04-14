using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSocketLauncherSingleMuzzleBase : CSocketLauncherBase
{
    [SerializeField]
    private float FireDelay = 0;
    [SerializeField]
    private CMuzzleBase SingleMuzzle = null;

    private float m_fCurrentDelay = 0;
    private bool m_bFireReady = true;
    //------------------------------------------------------------------------------
    protected override void OnSocketLauncherInitialize()
    {      
        SingleMuzzle?.InterPrefabTemplateInitialize();
        SingleMuzzle?.InterMuzzleInitialize(GetSocketLauncherOwnerActor());
    }

    protected override void OnSocketLauncherUpdate(float fDelta)
    {
        if (m_bFireReady == false)
        {
            m_fCurrentDelay += fDelta;
            if (m_fCurrentDelay >= FireDelay)
            {
                m_bFireReady = true;
            }
        }

        if (SingleMuzzle != null)
        {
            SingleMuzzle.InterMuzzleUpdate(fDelta);
        }
    }

    protected override void OnSocketLauncherFireTarget(CUnitActorBase pTargetUnitActor, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, params object[] aParams)
    {
        if (m_bFireReady && SingleMuzzle != null)
        {
            m_bFireReady = false;
            SingleMuzzle.DoMuzzleFireTarget(pTargetUnitActor, eMovementType, pTargetSocket, vecTargetOffset, aParams);
            OnSocketLauncherSingle(SingleMuzzle);
        }
    }

    //--------------------------------------------------------------------------------
    protected virtual void OnSocketLauncherSingle(CMuzzleBase pFireMuzzle) { }
}
