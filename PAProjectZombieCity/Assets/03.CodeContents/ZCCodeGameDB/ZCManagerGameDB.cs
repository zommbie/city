using UnityEngine;
using UnityEngine.Events;

public class ZCManagerGameDB : CManagerGameDBBase
{ public static new ZCManagerGameDB Instance { get { return CManagerGameDBBase.Instance as ZCManagerGameDB; } }

    public enum EGameDBType
    {
        Virtual,
        GuestLocal,
    }

    private ZCGameDBContainerBase m_pDBContainer = null;
    public ZCGameDBContainerBase DB { get { return m_pDBContainer; } }
    //------------------------------------------------------------------
    public void DoMgrGameDBInitilize(EGameDBType eGameDBType, UnityAction delFinish)
    {
        switch(eGameDBType)
        {
            case EGameDBType.Virtual:
                m_pDBContainer = new ZCGameDBContainerVirtual();
                break;
            case EGameDBType.GuestLocal:
                m_pDBContainer = new ZCGameDBContainerGuestLocal();
                break;
        }

        if (m_pDBContainer != null)
        {
            m_pDBContainer.DoGameDBContainerInitialize(delFinish);
        }
    }
}
