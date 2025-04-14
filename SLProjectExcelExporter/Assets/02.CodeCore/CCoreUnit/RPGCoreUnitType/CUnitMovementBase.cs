using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitMovementBase : CUnitShapeAnimationBase, IUnitEventMove
{
	private CAssistMovementBase m_pAssistMovement = null;
   
    //-----------------------------------------------------
    public Vector3 GetUnitPosition()
    {
        return m_pAssistMovement.GetAssistMovementPosition();
    }

    public Quaternion GetUnitRotation()
    {
        return m_pAssistMovement.GetAssistMovementRotation();
    }

    public Vector3 GetUnitDirection()
    {
        return m_pAssistMovement.GetAssistMovementDirection();
    }

    public float GetUnitMoveScale() { return m_pAssistMovement.GetAssistMoveScale(); }
    public void SetUnitMoveScale(float fMoveScale) { m_pAssistMovement.SetAssistMoveScale(fMoveScale); }

    //---------------------------------------------------------------------
    protected virtual void InstanceUnitMovement(CAssistMovementBase pInstanceMovement) { m_pAssistMovement = pInstanceMovement; }
}
 