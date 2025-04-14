using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 백터 방향으로 이동하는 로직으로 미사일등의 발사체등에 적용 
public abstract class CAssistMovementVectorBaseOld : CAssistMovementBaseOld
{
    [SerializeField]
    private AnimationCurve AccelationCurve;
    [SerializeField]
    private float MaxRoationAngle = 180f;
    
    private float m_fCurrentMoveTime = 0;
    //------------------------------------------------------
    protected override void OnAssistMovementUpdateToObject(float fDelta, float fMoveValue, Transform pTargetObject)
    {
        m_fCurrentMoveTime += fDelta;
        Vector3 vecDest = pTargetObject.position;
        Vector3 vecOrigin = GetAssistMovementPosition();       
        if (vecOrigin != vecDest)
        {                      
  //          PrivAssistMovementVectorMoveDirection(vecOrigin, vecDest, fMoveValue * fDelta, m_fCurrentMoveTime);
        }        
    }

    protected override void OnAssistMovementUpdateToPosition(float fDelta, float fMoveValue, Vector3 vecDestPosition)
    {
        m_fCurrentMoveTime += fDelta;
        Vector3 vecDest = vecDestPosition;
        Vector3 vecOrigin = GetAssistMovementPosition();
        if (vecOrigin != vecDest)
        {
   //         PrivAssistMovementVectorMoveDirection(vecOrigin, vecDest, fMoveValue * fDelta, m_fCurrentMoveTime);
        }
        else
        {
            ProtAssistMovementStop();
        }
    }

    protected override void OnAssistMovementStart()
    {
        base.OnAssistMovementStart();
        m_fCurrentMoveTime = 0;
    }
    //-----------------------------------------------------
    //public override Vector3 GetAssistMovementPosition()
    //{
    //    return m_pAssistOwner.transform.position;
    //}

    //public override Quaternion GetAssistMovementRotation()
    //{
    //    return m_pAssistOwner.transform.rotation;
    //}

    //public override Vector3 GetAssistMovementDirection()
    //{
    //    return m_pAssistOwner.transform.forward;
    //}

    //public override void SetAssistMovementDirection(Vector3 vecDirection)
    //{
    //    m_pAssistOwner.transform.LookAt(vecDirection);
    //}
    //-------------------------------------------------------

    //private void PrivAssistMovementVectorMoveDirection(Vector3 vecHere, Vector3 vecDest, float fMoveValue, float fMoveTime)
    //{
    //    float fDistance = Vector3.Distance(vecHere, vecDest);
    //    float fMoveLength = AccelationCurve.Evaluate(fMoveTime) * fMoveValue;
    //    if (fMoveLength > fDistance)
    //    {
    //        fMoveLength = fDistance;
    //    }

    //    Vector3 vecDirection = vecDest - vecHere;
    //    vecDirection.Normalize();
    //    Vector3 vecForward = vecDirection * fMoveLength;
    //    Vector3 vecLookPosition = m_pAssistOwner.transform.position + vecForward;
    //    m_pAssistOwner.transform.LookAt(vecLookPosition);
    //    m_pAssistOwner.transform.localPosition = vecLookPosition;
    //}
    
   
}

