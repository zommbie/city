using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitActorShapeSocketBase : CAssistUnitActorBase
{
    private Dictionary<int, CShapeSocketBase> m_mapShapeSocketInstance = new Dictionary<int, CShapeSocketBase>();
    //-------------------------------------------------------------------
    protected override void OnAssistFixedUpdate(float fDelta)
    {
        UpdateAssistSocket(fDelta);
    }

    //-------------------------------------------------------------------
    protected CShapeSocketBase GetAssistShapeSocket(int eSocketType)
    {
        return FindAssistShapeSocket(eSocketType);
    }

    protected STransform GetAssistShapeSocketTransform(int eSocketType)
    {
        STransform rResult = new STransform();
        CShapeSocketBase pSocket = FindAssistShapeSocket(eSocketType);
        if (pSocket != null)
        {
            rResult = pSocket.GetShpaeSocketTransformInfo();
        }
        return rResult;
    }

    protected Vector3 GetAssistShapeSocketPosition(int eSocketType)
    {
        Vector3 vecPosition = Vector3.zero;
        CShapeSocketBase pSocket = FindAssistShapeSocket(eSocketType);
        if (pSocket != null)
        {
            vecPosition = pSocket.GetShapeSocketPosition();
        }
        return vecPosition;
    }

    protected Vector3 GetAssistShapeSocketDirection(int eSocketType)
    {
        Vector3 vecDirection = Vector3.zero;
        CShapeSocketBase pSocket = FindAssistShapeSocket(eSocketType);
        if (pSocket != null)
        {
            vecDirection = pSocket.GetShapeSocketDirection();
        }
        return vecDirection;
    }

    //------------------------------------------------------------------
    protected void ProtAssistShapeSocketAdd(int eSocketType, CShapeSocketBase pShapeSocket)
    {
        if (m_mapShapeSocketInstance.ContainsKey(eSocketType))
        {
            //Error!
        }
        else
        {
            m_mapShapeSocketInstance[eSocketType] = pShapeSocket;
            pShapeSocket.InterShapeSocketInitialize(this);
        }
    }

    protected CShapeSocketBase FindAssistShapeSocket(int eSocketType)
    {
        CShapeSocketBase pFindSocket = null;
        m_mapShapeSocketInstance.TryGetValue(eSocketType, out pFindSocket);
        return pFindSocket;
    }

    //-------------------------------------------------------------------
    private void UpdateAssistSocket(float fDelta)
    {
        Dictionary<int, CShapeSocketBase>.Enumerator it = m_mapShapeSocketInstance.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.Value.InterShapeSocketUpdate(fDelta);
        }
    }

}
