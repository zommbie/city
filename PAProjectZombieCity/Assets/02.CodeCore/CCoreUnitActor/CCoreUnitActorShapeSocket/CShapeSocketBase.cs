using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class CShapeSocketBase : CMonoBase
{
    public enum EBaseDirection
    {
        None,
        Forward,
        Backward,
        Right,
        Left,
    }
   
    [SerializeField]
    private EBaseDirection BaseDirection = EBaseDirection.Forward;
    [SerializeField]
    private Vector3 UpAxis = new Vector3(0f, 1f, 0f);

    private Vector3 m_vecOriginPosition = Vector3.zero;
    private Vector3 m_vecBaseDirection = Vector3.zero;
    private float m_fLength = 0;
    private float m_fAngleOffset = 0;

    private CAssistUnitActorShapeSocketBase m_pSocketAisstOwner = null;
    //---------------------------------------------------
    internal void InterShapeSocketInitialize(CAssistUnitActorShapeSocketBase pAssistOwner)
    {
        PrivShapeSocketBaseDirection();
        m_pSocketAisstOwner = pAssistOwner;
        m_vecOriginPosition = transform.localPosition;
        m_vecOriginPosition.x *= transform.lossyScale.x;
        m_vecOriginPosition.y *= transform.lossyScale.y;
        m_vecOriginPosition.z *= transform.lossyScale.z;
        
        m_fLength = Vector3.Distance(Vector3.zero, m_vecOriginPosition);        
        m_vecOriginPosition.Normalize();

        m_fAngleOffset = Vector3.SignedAngle(m_vecBaseDirection, m_vecOriginPosition, UpAxis);
        OnShapeSocketInitialize(pAssistOwner);
    }

    internal void InterShapeSocketUpdate(float fDelta)
    {
        OnShapeSocketUpdate(fDelta);
    }
    
    //---------------------------------------------------
    

    public virtual Vector3 GetShapeSocketPosition()
    {
        return transform.position;
    }

    public virtual Vector3 GetShapeSocketDirection()
    {
        Vector3 vecDirection = transform.rotation * Vector3.forward;
        return vecDirection;
    }

    public virtual STransform GetShpaeSocketTransformInfo()
    {
        STransform rTransform = new STransform();
        rTransform.Position  = transform.position;
        rTransform.Scale     = transform.localScale;
        rTransform.Direction = transform.rotation * Vector3.forward;
        return rTransform;
    }

    public virtual Transform GetShapeSocketTransform()
    {
        return transform;
    }

    public CUnitActorBase GetShapeSocketOwner()
    {
        return m_pSocketAisstOwner.GetAssistOwnerActor();
    }
    //---------------------------------------------------------
    public void DoShapeSocketSetDirectionBaseCenter(Vector3 vecDirection)
    {
        if (vecDirection == Vector3.zero || m_fLength == 0) return;
        vecDirection.Normalize();

        Vector3 vecRotateDirection = Quaternion.AngleAxis(m_fAngleOffset, Vector3.up) * vecDirection;
        transform.localRotation = Quaternion.LookRotation(vecRotateDirection);
        vecRotateDirection *= m_fLength;
        transform.localPosition = vecRotateDirection;
    }
 
    //-------------------------------------------------
    private void PrivShapeSocketBaseDirection()
    {
        if (BaseDirection == EBaseDirection.Forward)
        {
            m_vecBaseDirection = Vector3.forward;
        }
        else if (BaseDirection == EBaseDirection.Backward)
        {
            m_vecBaseDirection = Vector3.back;
        }
        else if (BaseDirection == EBaseDirection.Left)
        {
            m_vecBaseDirection = Vector3.left;
        }
        else if (BaseDirection == EBaseDirection.Right)
        {
            m_vecBaseDirection = Vector3.right;
        }
    }

    //-----------------------------------------------------
    protected virtual void OnShapeSocketInitialize(CAssistUnitActorShapeSocketBase pAssistOwner) { }
    protected virtual void OnShapeSocketUpdate(float fDelta) { }
}
