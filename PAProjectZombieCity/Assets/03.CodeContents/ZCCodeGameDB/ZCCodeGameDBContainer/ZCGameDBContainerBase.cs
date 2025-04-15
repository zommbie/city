using UnityEngine;
using UnityEngine.Events;

public abstract class ZCGameDBContainerBase
{
    protected ZCGameDBInventory m_pDBInventory = new ZCGameDBInventory();                       public ZCGameDBInventory DBInventory { get { return m_pDBInventory; } }
    //------------------------------------------------------------------------------------
    public void DoGameDBContainerInitialize(UnityAction delFinish)
    {
        OnGameDBContainerInitialize(delFinish);
    }

    //------------------------------------------------------------------------------------
    protected virtual void OnGameDBContainerInitialize(UnityAction delFinish) { }

}
