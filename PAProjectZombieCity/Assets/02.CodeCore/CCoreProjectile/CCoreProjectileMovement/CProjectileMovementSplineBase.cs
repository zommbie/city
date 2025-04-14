using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// 포워드 방향으로 진행되며 점 두개 스플라인을 사용하여 계산 

public abstract class CProjectileMovementSplineBase : CProjectileMovementBase
{
    public enum EMoveType
    {
        MoveBaseSpeed,
        MoveBaseTime,
    }
    [SerializeField]
    private Vector3           WorldForward = Vector3.right;

    [SerializeField]
    protected SplineContainer SplineTrack;
    [SerializeField]
    protected AnimationCurve  MoveCurve = AnimationCurve.Linear(0, 0, 1f, 1f);
    [SerializeField]
    private   float           MoveSpeed = 1f; // 초당 이동량 
    [SerializeField]
    private EMoveType         MoveType = EMoveType.MoveBaseSpeed;
   
    [SerializeField]
    private int SplineIndex = 0;

    private float m_fCurrentTime = 0;
    private Spline m_pMoveSpline = null;
    //------------------------------------------------------------------------------------------------
    protected override sealed void OnProjectileMovementInitialize()
    {
        m_pMoveSpline = FindProjectileSpline(SplineIndex);
        OnProjectileMovementSplineInitialize(m_pMoveSpline);
    }

    protected sealed override void OnProjectileMovementToTransform(Vector3 vecFirePosition, Vector3 vecTargetOffset, Quaternion quaFireRotation, Transform pTargetTransform)
    {
        PrivProjectileMovementReset();
        PrivProjectileMovementSplineToTransform(vecFirePosition, pTargetTransform, vecTargetOffset);
        OnProjectileMovementUpdate(0f);
        OnProjectileMovementSplineStart();
    }

    protected override sealed void OnProjectileMovementUpdate(float fDelta)
    {
        if (m_pMoveSpline == null) return;

        if (MoveType == EMoveType.MoveBaseSpeed)
        {
            UpdateProjectileMovementBaseSpeed(fDelta);
        }
        else if (MoveType == EMoveType.MoveBaseTime)
        {
            UpdateProjectileMovementBaseTime(fDelta);
        }
    }
    //-----------------------------------------------------------------------------------------------
   

    protected Spline FindProjectileSpline(int iIndex)
    {
        Spline pFindSpline = null;
        if (iIndex < SplineTrack.Spline.Count)
        {
            pFindSpline = SplineTrack.Splines[iIndex];   
        }
        return pFindSpline;
    }

    protected Vector3 ExtractProjectileTargetInversePosition(Vector3 vecTargetLocalPosition)
    {
        Vector3 vecInversePosition = Vector3.zero;
        if (m_pTargetTransform != null)
        {
            vecInversePosition = m_pTargetTransform.InverseTransformDirection(vecTargetLocalPosition);
        }

        return vecInversePosition;
    }

    protected Quaternion ExtractProjectileTargetInverseRotation(Quaternion quaTargetLocalRotation)
    {
        Quaternion quaInverseRotation = Quaternion.identity;
        if (m_pTargetTransform != null)
        {
            quaInverseRotation = Quaternion.Inverse(m_pTargetTransform.rotation) * quaInverseRotation;
        }

        return quaInverseRotation;
    }

    //-------------------------------------------------------------------------------------------------
    private void PrivProjectileMovementSplineToTransform(Vector3 vecFirePosition, Transform pTargetTransform, Vector3 vecTargetOffset)
    {
        m_vecTargetPosition = pTargetTransform.position;
        // ToDo 포워드 방향 기준으로 오프셋 좌표를 산출할 것
        OnProjectileMovementSplineToLocation(m_vecFirePosition, m_vecTargetPosition);
    }

    private void PrivProjectileMovementReset()
    {
        m_fCurrentTime = 0;
    }

    private void UpdateProjectileMovementBaseSpeed(float fDelta)
    {
        float fMoveDelta = fDelta * MoveSpeed;
        float fSplineLength = m_pMoveSpline.GetLength();
        m_fCurrentTime += fMoveDelta;

        float fSplineCurve = m_fCurrentTime / fSplineLength;
        fSplineCurve = Mathf.Clamp01(fSplineCurve);

        float fSplineRate = MoveCurve.Evaluate(fSplineCurve);
        Vector3 vecSplinePosition = m_pMoveSpline.EvaluatePosition(fSplineRate);
        m_pMoveTransform.localPosition = vecSplinePosition;

        if (fSplineCurve >= 1f)
        {
            ProtProjectileMovementArrive();
        }
        else
        {
            m_pMoveTransform.rotation = ExtractSplineRotation(fSplineRate, m_pMoveSpline, WorldForward);
            OnProjectileMovementSplineUpdatePosition(vecSplinePosition, m_pMoveTransform.localRotation);
        }
    }

    private void UpdateProjectileMovementBaseTime(float fDelta)
    {

    }

    public static Quaternion ExtractSplineRotation(float fSplineRate, Spline pSpline, Vector3 vecWorldForward)
    {           
        Vector3 vecSplineForward = Vector3.Normalize(pSpline.EvaluateTangent(fSplineRate));
        if (vecSplineForward == Vector3.zero)
        {
            return Quaternion.identity;
        }

        Vector3 vecBaseUp = Vector3.zero;

        if (vecSplineForward.x < 0)
        {
            vecBaseUp = Vector3.down;
        }
        else
        {
            vecBaseUp = Vector3.up;
        }

        Vector3 vecUp = pSpline.EvaluateUpVector(fSplineRate);
        Quaternion quaAxisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(vecWorldForward, vecBaseUp));
        return Quaternion.LookRotation(vecSplineForward, vecUp) * quaAxisRemapRotation;
    }
    
    
    //-----------------------------------------------------------------------------------------------------
    protected virtual void OnProjectileMovementSplineInitialize(Spline pSpline) { }
    protected virtual void OnProjectileMovementSplineStart() { }
    protected virtual void OnProjectileMovementSplineToLocation(Vector3 vecStartPosition, Vector3 vecDestPosition) { }
    protected virtual void OnProjectileMovementSplineUpdatePosition(Vector3 vecSplineLocalPosition, Quaternion quaSplineLocalRotation) { }
}
