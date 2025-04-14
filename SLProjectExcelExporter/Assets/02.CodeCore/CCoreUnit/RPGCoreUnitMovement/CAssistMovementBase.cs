using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Video;

public interface IUnitEventMove
{
}

public abstract class CAssistMovementBase : CAssistUnitBase
{
    public const float c_MoveThreshold = 0.00001f;

    public enum EMoveType
    {
        Stop,
        ToPosition,
        ToObject,
        ToDirection,
    }

    private float m_fMoveScale = 1f;        // 외부에서 주어지는 스케일 값 (테스트 모드등)
    private float m_fMoveSpeed = 0f;        // 객체 자체의 이동 속도 값
  
    private Vector3 m_vecDestPosition = Vector3.zero;
    private Vector3 m_vecDestDirection = Vector3.zero;
    private Transform m_pTargetObject = null;
    private IUnitEventMove m_pEventMoveOwner = null;
    private UnityAction m_delFinishPosition = null;
    private EMoveType m_eMoveStatus = EMoveType.Stop; public EMoveType GetAssistMoveStatus() { return m_eMoveStatus; }
    //------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        m_pEventMoveOwner = pOwner as IUnitEventMove;
    }

    protected override void OnAssistUnitState(CUnitBase.EUnitState eState)
    {
        base.OnAssistUnitState(eState);
        if (eState == CUnitBase.EUnitState.Spawning || eState == CUnitBase.EUnitState.PhaseIn)
        {
            SetAssistUpdateEnable(true);
        }
        else if (eState == CUnitBase.EUnitState.DeathStart || eState == CUnitBase.EUnitState.PhaseOut)
        {
            SetAssistUpdateEnable(false);
        }     
    }


    protected sealed override void OnAssistFixedUpdate(float fDelta)
    {
        base.OnAssistFixedUpdate(fDelta);

        float fMoveSpeed = m_fMoveScale * m_fMoveSpeed;
        if (m_eMoveStatus == EMoveType.ToPosition)
        {
            OnAssistMovementUpdateToPosition(fDelta, fMoveSpeed, m_vecDestPosition);
        }
        else if (m_eMoveStatus == EMoveType.ToDirection)
        {
            OnAssistMovementUpdateToDirection(fDelta, fMoveSpeed, m_vecDestDirection);
        }
        else if (m_eMoveStatus == EMoveType.ToObject)
        {
            OnAssistMovementUpdateToObject(fDelta, fMoveSpeed, m_pTargetObject);
        }

        OnAssistMovementUpdate(fDelta, fMoveSpeed, m_eMoveStatus);
    }
    //------------------------------------------------------------------
    protected void ProAssistMovementUpdateToPosition(Vector3 vecPosition, float fMoveSpeed, UnityAction delFinish)
    {
        PrivAssistMovementReset();
        m_eMoveStatus = EMoveType.ToPosition;
        m_delFinishPosition = delFinish;
        m_fMoveSpeed = 0;
        m_vecDestPosition = vecPosition;
        OnAssistMovementStart();
        OnAssistFixedUpdate(0);
    }

    protected void ProtAssistMovementUpdateToDirection(Vector3 vecDirection, float fMoveSpeed)
    {
        PrivAssistMovementReset();
        m_eMoveStatus = EMoveType.ToPosition;
        m_vecDestDirection = vecDirection;
        m_fMoveSpeed = 0;
        OnAssistMovementStart();
        OnAssistFixedUpdate(0);
    }

    protected void ProtAssistMovementUpdateToObject(Transform pTargetObject, float fMoveSpeed)
    {
        PrivAssistMovementReset();
        m_eMoveStatus = EMoveType.ToObject;
        m_pTargetObject = pTargetObject;
        m_fMoveSpeed = fMoveSpeed;
        OnAssistMovementStart();
        OnAssistFixedUpdate(0);
    }

    protected void ProtAssistMovementChangeToPosition(Vector3 vecPosition, UnityAction delFinish)
    {
        m_eMoveStatus = EMoveType.ToPosition;
        m_delFinishPosition = delFinish;
        m_vecDestPosition = vecPosition;
    }

    protected void ProtAssistMovementStop()
    {
        m_eMoveStatus = EMoveType.Stop;
        m_delFinishPosition?.Invoke();
    }

    //------------------------------------------------------------------
    public IUnitEventMove GetAssistEventOwner()
    {
        return m_pEventMoveOwner;
    }

    public void SetAssistMoveScale(float fMoveScale)
    {
        m_fMoveScale = fMoveScale;
    }

    public void SetAssistMoveSpeed(float fMoveSpeed)
    {
        m_fMoveSpeed = fMoveSpeed;
    }

    public float GetAssistMoveScale()
    {
        return m_fMoveScale;
    }
    //------------------------------------------------------------------
    private void PrivAssistMovementReset()
    {
        m_vecDestPosition = Vector3.zero;
        m_vecDestDirection = Vector3.zero;
        m_delFinishPosition = null;
        m_pTargetObject = null;
    }
    //------------------------------------------------------------------
    public virtual Vector3      GetAssistMovementPosition() { return Vector3.zero; }
    public virtual Quaternion   GetAssistMovementRotation() { return Quaternion.identity; }
    public virtual Vector3      GetAssistMovementDirection() { return Vector3.zero; }
    public virtual void         SetAssistMovementDirection(Vector3 vecDirection) { }
    //------------------------------------------------------------------
    protected virtual void OnAssistMovementUpdate(float fDelta, float fMoveValue, EMoveType eMoveStatus) { }
    protected virtual void OnAssistMovementUpdateToPosition(float fDelta, float fMoveValue, Vector3 vecDestPosition) { }
    protected virtual void OnAssistMovementUpdateToDirection(float fDelta, float fMoveValue, Vector3 vecDestDirection) { }
    protected virtual void OnAssistMovementUpdateToObject(float fDelta, float fMoveValue, Transform pTargetObject) { }
    protected virtual void OnAssistMovementStart() { }
}
