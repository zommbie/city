using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 케릭터 귀속 이펙트가 아닌 공용 이펙트 관리 로직

public abstract class CManagerEffectBase : CManagerTemplateBase<CManagerEffectBase>, CDLCLoader.IDLCLoadAction
{
    [SerializeField]
    private Transform RootGlobal = null;
    [SerializeField]
    private Transform RootLocalEffect = null;

    private class SEffectRegistInfo
    {
        public string               EffectName;
        public CEffectBase          EffectOrigin;
        public int                  ReferenceCount;
        public bool                 LocalEffect;       // 로컬 이펙트 (액터에 소속된 이펙트)는 해재해야된다. 글로벌 공용 이펙트는 해재 하지 않는다      
        public List<CEffectBase>    EffectCloneList = new List<CEffectBase>();
    }

    private float m_fTimeScale = 1f;  public float GetMgrEffectTimeScale() { return m_fTimeScale; }
    private Dictionary<string, SEffectRegistInfo> m_mapEffectOrigin   = new Dictionary<string, SEffectRegistInfo>(); 
	//-------------------------------------------------------------
	protected override void OnUnityUpdate()
	{
		base.OnUnityUpdate();        
    }

    public void IPreviousLoadContents(UnityAction delFinish)
    {
        OnMgrEffectPreLoadWork(delFinish);
    }

    //---------------------------------------------------------------
    internal void InterMgrEffectLocalRegist(CEffectBase pEffectInstance, int iCloneCount)
    {
        PrivMgrEffectCloneInsertLocal(pEffectInstance, iCloneCount);
    }

    internal void InterMgrEffectLocalUnRegist(CEffectBase pEffectInstance)
    {
        PrivMgrEffectCloneRemoveLocal(pEffectInstance);
    }

    internal void InterMgrEffectClearLocal()
    {
        ProtMgrEffectClearLocal();
    }

    //----------------------------------------------------------------
    public void DoMgrEffectTimeScale(float fTimeScale)
    {      
        PrivMgrEffectTimeScale(fTimeScale);
    }

    //----------------------------------------------------------------  
    protected void ProtMgrEffectRegisterGlobal(string strAddresableEffectName, int iCloneCount, UnityAction delFinish)
    {
        PrivMgrEffectCloneInsertGlobal(strAddresableEffectName, iCloneCount, delFinish);
    }

    protected void ProtMgrEffectClearLocal()
    {
        Dictionary<string, SEffectRegistInfo>.Enumerator it = m_mapEffectOrigin.GetEnumerator();
        List<string> pListRemove = new List<string>();
        while(it.MoveNext())
        {
            SEffectRegistInfo pEffectRegistInfo = it.Current.Value;
            if (pEffectRegistInfo.LocalEffect)
            {
                PrivMgrEffectLocalInstanceRemove(pEffectRegistInfo, true);
                pListRemove.Add(pEffectRegistInfo.EffectName);
            }
        }

        for (int i = 0; i < pListRemove.Count; i++)
        {
            m_mapEffectOrigin.Remove(pListRemove[i]); 
        }
    }

    protected CEffectBase ProtMgrEffectRequest(string strEffectName)
    {
        CEffectBase pNewEffect = null;
        SEffectRegistInfo pEffectRegistInfo = FindMgrEffectRegistInfo(strEffectName);
        if (pEffectRegistInfo != null)
        {
            pNewEffect = FindMgrEffectInActiveInstance(pEffectRegistInfo);
            pNewEffect.DoEffectTimeScale(m_fTimeScale);
        }

        return pNewEffect;
    }

    protected void ProtMgrEffectReturn(CEffectBase pReturnEffect)
    {
        SEffectRegistInfo pEffectRegistInfo = FindMgrEffectRegistInfo(pReturnEffect.GetEffectName());
        if (pEffectRegistInfo != null)
        {
            if (pEffectRegistInfo.LocalEffect)
            {
                pReturnEffect.transform.SetParent(RootLocalEffect);
            }
            else
            {
                pReturnEffect.transform.SetParent(RootGlobal);
            }

            pReturnEffect.DoEffectEnd(true);
        }
    }

    protected void ProtMgrEffectHideAll()
    {
        Dictionary<string, SEffectRegistInfo>.Enumerator it = m_mapEffectOrigin.GetEnumerator();
        while(it.MoveNext())
        {
            List<CEffectBase> pListEffect = it.Current.Value.EffectCloneList;
            for (int i = 0; i < pListEffect.Count; i++)
            {
                CEffectBase pEffect = pListEffect[i];
                if (pEffect.IsActive)
                {
                    pEffect.DoEffectEnd();
                }
            }
        }
    }

    //---------------------------------------------------------------
    private void PrivMgrEffectCloneInsertLocal(CEffectBase pEffectOrigin, int iCloneCount)
    {
        string strEffectName = pEffectOrigin.GetEffectName();

        SEffectRegistInfo pEffectRegistInfo = FindMgrEffectRegistInfo(strEffectName);
        if (pEffectRegistInfo == null)
        {
            pEffectRegistInfo = MakeMgrEffectRegistEffect(strEffectName, true);
            pEffectRegistInfo.EffectOrigin = pEffectOrigin;
            for (int i = 0; i < iCloneCount; i++)
            {
                CEffectBase pEffectNew = Instantiate(pEffectRegistInfo.EffectOrigin);
                PrivMgrEffectAddInstance(pEffectRegistInfo, pEffectNew);
            }
        }

        if (pEffectRegistInfo.EffectOrigin == null)
        {
            pEffectRegistInfo.EffectOrigin = pEffectOrigin;  // 동일한 이름을 사용하는 객체가 삭제되면서 이펙트도 삭제 되었을 경우          
        }
        pEffectRegistInfo.ReferenceCount++;
    }

    private void PrivMgrEffectCloneRemoveLocal(CEffectBase pEffectOrigin)
    {
        string strEffectName = pEffectOrigin.GetEffectName();
        SEffectRegistInfo pEffectRegistInfo = FindMgrEffectRegistInfo(strEffectName);
        if (pEffectRegistInfo != null)
        {
            PrivMgrEffectLocalInstanceRemove(pEffectRegistInfo, false);
            m_mapEffectOrigin.Remove(pEffectRegistInfo.EffectName);
        }
    }

    private void PrivMgrEffectLocalInstanceRemove(SEffectRegistInfo pEffectRegistInfo, bool bForce)
    {
        if (pEffectRegistInfo.LocalEffect == false) return;

        if (bForce == false)
        {
            pEffectRegistInfo.ReferenceCount--;
            if (pEffectRegistInfo.ReferenceCount > 0)
            {
                return;
            }
        }

        for (int i = 0; i < pEffectRegistInfo.EffectCloneList.Count; i++)
        {
            if (pEffectRegistInfo.EffectCloneList[i] != null)
            {
                Destroy(pEffectRegistInfo.EffectCloneList[i].gameObject);
            }
        }       
    }

    private void PrivMgrEffectCloneInsertGlobal(string strAddresableEffectName, int iCloneCount, UnityAction delFinish)
    {
        if (CManagerPrefabPoolUsageBase.Instance == null) return;

        SEffectRegistInfo pEffectRegistInfo = FindMgrEffectRegistInfo(strAddresableEffectName);
        if (pEffectRegistInfo == null)
        {
            pEffectRegistInfo = MakeMgrEffectRegistEffect(strAddresableEffectName, false);
        }

        CManagerPrefabPoolUsageBase.Instance.LoadComponent(EAssetPoolType.Effect, strAddresableEffectName, (CEffectBase pCloneEffect) => { 
            if (pCloneEffect != null)
            {
                for (int i = 0; i< iCloneCount; i++)
                {
                    CEffectBase pEffectNew = CManagerPrefabPoolUsageBase.Instance.FindClone<CEffectBase>(EAssetPoolType.Effect, strAddresableEffectName);
                    PrivMgrEffectAddInstance(pEffectRegistInfo, pEffectNew);
                }
            }
            delFinish?.Invoke();
        }, 1);
    }

    private CEffectBase FindMgrEffectInActiveInstance(SEffectRegistInfo pEffectRegistInfo)
    {
        CEffectBase pEffectInstance = null;
        for (int i = 0; i < pEffectRegistInfo.EffectCloneList.Count; i++)
        {
            if (pEffectRegistInfo.EffectCloneList[i].IsActive == false)
            {
                pEffectInstance = pEffectRegistInfo.EffectCloneList[i];
                break;
            }
        }

        if (pEffectInstance == null)
        {
            pEffectInstance = PrivMgrEffectAllocateInstance(pEffectRegistInfo);
        }

        return pEffectInstance;
    }

    private CEffectBase PrivMgrEffectAllocateInstance(SEffectRegistInfo pEffectRegistInfo)
    {
        CEffectBase pEffectNew = null;
        if (pEffectRegistInfo.LocalEffect)
        {
            pEffectNew = Instantiate(pEffectRegistInfo.EffectOrigin);
        }
        else
        {
            pEffectNew = CManagerPrefabPoolUsageBase.Instance.FindClone<CEffectBase>(EAssetPoolType.Effect, pEffectRegistInfo.EffectName);
        }
        PrivMgrEffectAddInstance(pEffectRegistInfo, pEffectNew);
        return pEffectNew;
    }

    private void PrivMgrEffectAddInstance(SEffectRegistInfo pEffectRegistInfo, CEffectBase pEffectClone)
    {
        if (pEffectRegistInfo.LocalEffect)
        {
            pEffectClone.transform.SetParent(RootLocalEffect, true);
        }
        else
        {
            pEffectClone.transform.SetParent(RootGlobal, true);
        }
        pEffectClone.InterEffectInitialize();
        pEffectClone.SetMonoActive(false);
        pEffectRegistInfo.EffectCloneList.Add(pEffectClone);
    }

    private void PrivMgrEffectTimeScale(float fTimeScale)
    {
        m_fTimeScale = fTimeScale;
        Dictionary<string, SEffectRegistInfo>.Enumerator it = m_mapEffectOrigin.GetEnumerator();
        while(it.MoveNext())
        {
            for (int i = 0; i < it.Current.Value.EffectCloneList.Count; i++)
            {
                it.Current.Value.EffectCloneList[i].DoEffectTimeScale(fTimeScale);
            }            
        }
    }

    //----------------------------------------------------------------------------------
    private SEffectRegistInfo MakeMgrEffectRegistEffect(string strEffectName, bool bLocalEffect)
    {
        SEffectRegistInfo pEffectRegistInfo = new SEffectRegistInfo();
        pEffectRegistInfo.EffectName = strEffectName;
        pEffectRegistInfo.LocalEffect = bLocalEffect;
        m_mapEffectOrigin[strEffectName] = pEffectRegistInfo;
        return pEffectRegistInfo;
    }

    private SEffectRegistInfo FindMgrEffectRegistInfo(string strEffectName)
    {
        SEffectRegistInfo pFindEffectRegist = null;
        m_mapEffectOrigin.TryGetValue(strEffectName, out pFindEffectRegist);
        return pFindEffectRegist;
    }

    //----------------------------------------------------------------
    protected virtual void OnMgrEffectPreLoadWork(UnityAction delFinish) { }
}
