using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class CShapeSocketBase : CMonoBase
{
    public enum EBaseDirection
    {
        Forward,
        Backward,
        Right,
        Left,
    }

    [ReadOnlyAttribute]
    public EBaseDirection BaseDirection = EBaseDirection.Right;


    private Vector3 m_vecOriginDirection = Vector3.zero;
    private Vector3 m_vecBaseDirection = Vector3.zero;
    private float m_fLength = 0;
    private float m_fAngleOffset = 0;
    private List<CMuzzleBase> m_listSocketMuzzle = new List<CMuzzleBase>();
    //---------------------------------------------------
    internal void InterShapeSocketInitialize(CUnitBase pSocketOwner)
    {
        PrivShapeSocketBaseDirection();
        GetComponentsInChildren(true, m_listSocketMuzzle);

        for (int i = 0; i < m_listSocketMuzzle.Count; i++)
        {
            m_listSocketMuzzle[i].InterMuzzleInitialize(pSocketOwner);
        }

        m_vecOriginDirection = transform.localPosition;
        m_vecOriginDirection.x *= transform.lossyScale.x;
        m_vecOriginDirection.y *= transform.lossyScale.y;
        m_vecOriginDirection.z *= transform.lossyScale.z;
        m_fLength = Vector3.Distance(Vector3.zero, m_vecOriginDirection);
        m_vecOriginDirection.Normalize();
        m_fAngleOffset = Vector3.SignedAngle(m_vecBaseDirection, m_vecOriginDirection, Vector3.up);
        OnShapeSocketInitialize(pSocketOwner);
    }
    
    //---------------------------------------------------
    public void DoShapeSocketAttach(GameObject pGameObject, Vector3 vecOffset)
    {
        pGameObject.transform.SetParent(transform, false);
        pGameObject.transform.localPosition = vecOffset;
        pGameObject.transform.localRotation = Quaternion.identity;

    }

    public Vector3 GetShapeSocketPosition()
    {
        return transform.position;
    }

    public int GetShapeSocektMuzzleCount()
    {
        return m_listSocketMuzzle.Count;
    }

    public CMuzzleBase GetShapeSocektMuzzle(int iIndex)
    {
        CMuzzleBase pMuzzle = null;
        if (iIndex < m_listSocketMuzzle.Count)
        {
            pMuzzle = m_listSocketMuzzle[iIndex];
        }
        return pMuzzle;
    }

    public void DoShapeSocketDirectionWithOffset(Vector3 vecDirection)
    {
        if (vecDirection == Vector3.zero || m_fLength == 0) return;
        vecDirection.Normalize();
        Vector3 vecRotateDirection = Quaternion.AngleAxis(m_fAngleOffset, Vector3.up) * vecDirection;
        transform.localRotation = Quaternion.LookRotation(vecRotateDirection);
        vecRotateDirection *= m_fLength;
        transform.localPosition = vecRotateDirection;
    }

    public void DoShapeSocketPosition(Vector3 vecPosition)
    {
        transform.position = vecPosition;
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
    protected virtual void OnShapeSocketInitialize(CUnitBase pSocketOwner) { }
}
