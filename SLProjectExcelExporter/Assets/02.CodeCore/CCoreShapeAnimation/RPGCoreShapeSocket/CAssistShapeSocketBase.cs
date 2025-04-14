using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistShapeSocketBase : CAssistUnitBase
{
    private Dictionary<string, CShapeSocketBase> m_mapShapeSocketInstance = new Dictionary<string, CShapeSocketBase>();
    //-------------------------------------------------------------------
    public CShapeSocketBase GetAssistShapeSocket(string strSocketName)
    {
        return FindAssistShapeSocket(strSocketName);
    }

    //------------------------------------------------------------------
    protected void ProtAssistShapeSocketAdd(string strSocketName, CShapeSocketBase pShapeSocket)
    {
        if (m_mapShapeSocketInstance.ContainsKey(strSocketName))
        {
            //Error!
        }
        else
        {
            pShapeSocket.InterShapeSocketInitialize(m_pAssistOwner);
            m_mapShapeSocketInstance[strSocketName] = pShapeSocket;
        }
    }

    protected CShapeSocketBase FindAssistShapeSocket(string strSocketName)
    {
        CShapeSocketBase pFindSocket = null;
        m_mapShapeSocketInstance.TryGetValue(strSocketName, out pFindSocket);
        return pFindSocket;
    }

    protected Transform FindAssistShapeSocketTransform(string strSocketName)
    {
        Transform pFindTransform = null;
        CShapeSocketBase pSocket = FindAssistShapeSocket(strSocketName);
        if (pSocket != null)
        {
            pFindTransform = pSocket.gameObject.transform;
        }

        return pFindTransform;
    }
}
