using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

// 소규모 프리팹 풀로 정적 로드된 객체를 관리 하는 기능을 가지고 있다.
public abstract class CPrefabTemplateBase : CMonoBase
{
    [SerializeField]
    private int TemplateCount = 1;

    [SerializeField]
    private CPrefabTemplateItemBase TemplateItem;

    private bool m_bInitialize = false;
    private Transform m_pRootTemplateItem = null;
    private LinkedList<CPrefabTemplateItemBase> m_listTemplateItemActiveInstance = new LinkedList<CPrefabTemplateItemBase>();
    private List<CPrefabTemplateItemBase> m_listTemplateItemInstance = new List<CPrefabTemplateItemBase>();
    //---------------------------------------------------------------------------------
    internal void InterPrefabTemplateInitialize()
    {
        if (m_bInitialize) return;
        m_bInitialize = true;

        TemplateItem?.SetMonoActive(false);
        m_pRootTemplateItem = TemplateItem?.transform.parent;
        PrivPrefabTemplateItem(TemplateCount);
        OnPrefabTemplateInitialize();
    }

    internal void InterPrefabTemplateItemReturn(CPrefabTemplateItemBase pReturnItem)
    {
        pReturnItem.SetMonoActive(false);
        m_listTemplateItemActiveInstance.Remove(pReturnItem);
        OnPrefabTemplateReturn(pReturnItem);
    }

    //----------------------------------------------------------------------------
    protected CPrefabTemplateItemBase ProtPrefabTemplateRequestInstance()
    {
        CPrefabTemplateItemBase pRequestItem = FindAndAllocPrefabTemplateRequestInstance();
        if (pRequestItem != null)
        {
            pRequestItem.InterPrefabTemplateEnable();
            m_listTemplateItemActiveInstance.AddLast(pRequestItem);
        }
        return pRequestItem;
    }

    //------------------------------------------------------
    private CPrefabTemplateItemBase FindAndAllocPrefabTemplateRequestInstance()
    {
        CPrefabTemplateItemBase pFindInstance = null;
        for (int i = 0; i < m_listTemplateItemInstance.Count; i++)
        {
            if (m_listTemplateItemInstance[i].IsTemplateItemActivated == false)
            {
                pFindInstance = m_listTemplateItemInstance[i];
                break;
            }
        }

        if (pFindInstance == null)
        {
            pFindInstance = MakePrefabTemplateItem();
        }

        return pFindInstance;
    }

    private void PrivPrefabTemplateItem(int iCount)
    {
        for (int i = 0; i < iCount; i++)
        {
            MakePrefabTemplateItem();
        }
    }

    private CPrefabTemplateItemBase MakePrefabTemplateItem()
    {
        if (TemplateItem == null) return null;

        CPrefabTemplateItemBase pNewInstance = Instantiate(TemplateItem, m_pRootTemplateItem);
        pNewInstance.InterPrefabTemplateItemAllocated(this);
        m_listTemplateItemInstance.Add(pNewInstance);
        return pNewInstance;
    }
    //-----------------------------------------------------------------------------
    protected virtual void OnPrefabTemplateInitialize() { }
    protected virtual void OnPrefabTemplateReturn(CPrefabTemplateItemBase pReturnItem) { }

}
