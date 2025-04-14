using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CMovementBase : CMonoBase
{
    
    protected Vector3 m_vecMoveOrigin = Vector3.zero;
    protected Vector3 m_vecMoveDest = Vector3.zero;
    protected Vector3 m_vecMoveDirection = Vector3.zero;
    protected Transform m_pMoveTransform = null;
    protected Transform m_pMoveTargetTransform = null;

    protected IEventMoveHandler m_pEventMoveHandler = null; 
    //--------------------------------------------------------------------------------------------
    internal void InterMovementInitialize(IEventMoveHandler pEventHandler, Transform pMoveTransform)
    {
        m_pMoveTransform = pMoveTransform;
        m_pEventMoveHandler = pEventHandler;
        OnAssistMovementInitialize(pMoveTransform);
    }

    internal void InterMovementUpdate(float fDelta, float fMoveDelta)
    {
        OnAssistMovementUpdate(fDelta, fMoveDelta);
    }

    internal void InterMovementStartToPosition(Vector3 vecDestPosition)
    {
        PrivMovementReset();
        m_vecMoveDest = vecDestPosition;
        OnAssistMovementStartToPosition(vecDestPosition);
    }

    internal void InterMovementStartToDirection(Vector3 vecDestDirection)
    {
        PrivMovementReset();
        m_vecMoveDirection = vecDestDirection;
        OnAssistMovementStartToDirection(vecDestDirection);
    }

    internal void InterMovementStartToObject(Transform pTargetTransform)
    {
        PrivMovementReset();
        m_pMoveTargetTransform = pTargetTransform;
        OnAssistMovementStartToObject(pTargetTransform);
    }
    //-----------------------------------------------------------------------
    private void PrivMovementReset()
    {
        m_vecMoveOrigin = m_pMoveTransform.position;
        m_vecMoveDest = Vector3.zero;
        m_vecMoveDirection = Vector3.zero;      
        m_pMoveTargetTransform = null;
    }

    //-------------------------------------------------------------------------------------------
    protected virtual void OnAssistMovementInitialize(Transform pMoveTransform) { }
    protected virtual void OnAssistMovementUpdate(float fDelta, float fMoveDelta) { }
    protected virtual void OnAssistMovementStartToPosition(Vector3 vecDestPosition) { }
    protected virtual void OnAssistMovementStartToDirection(Vector3 vecDestDirection) { }
    protected virtual void OnAssistMovementStartToObject(Transform pTargetTransform) { }
  
}
