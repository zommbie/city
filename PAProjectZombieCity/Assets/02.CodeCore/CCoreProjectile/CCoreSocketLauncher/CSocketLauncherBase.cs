using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSocketLauncherBase  : CMonoBase
{
    private CShapeSocketBase m_pOwnerSocket = null;                 public CUnitActorBase GetSocketLauncherOwnerActor() { return m_pOwnerSocket.GetShapeSocketOwner();}
    //---------------------------------------------------------------------------
    private void FixedUpdate()
    {
        float fFixedTime = Time.fixedDeltaTime * CTimeScale.TimeScale;
        OnSocketLauncherUpdate(fFixedTime);
    }

    //-----------------------------------------------------------------------------
    public void DoSocketLauncherInitialize(CShapeSocketBase pOwnerSocket)
    {
        m_pOwnerSocket = pOwnerSocket;
        OnSocketLauncherInitialize();
    }

    public void DoSocketLauncherFireTarget(CUnitActorBase pTargetUnitActor, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, params object[] aParams)  //  타겟의 월드 포지션에서 이 값만큼 더해서 타겟한다.
    {
        OnSocketLauncherFireTarget(pTargetUnitActor, eMovementType, pTargetSocket, vecTargetOffset, aParams);
    }

    public void DoSocketLauncherFireDirection(Vector3 vecFirDirection, int eDirectionType, params object[] aParams)
    {
        OnSocketLauncherFireDirection(vecFirDirection, eDirectionType, aParams);
    }

    //--------------------------------------------------------
    protected virtual void OnSocketLauncherInitialize() { }
    protected virtual void OnSocketLauncherUpdate(float fDelta) { }
    protected virtual void OnSocketLauncherFireTarget(CUnitActorBase pTargetUnitActor, int eMovementType, CShapeSocketBase pTargetSocket, Vector3 vecTargetOffset, params object[] aParams) { }
    protected virtual void OnSocketLauncherFireDirection(Vector3 vecFirDirection, int eDirectionType, params object[] aParams) { }
}
