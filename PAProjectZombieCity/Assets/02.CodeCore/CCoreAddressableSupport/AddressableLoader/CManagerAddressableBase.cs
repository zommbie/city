using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class CManagerAddressableBase<TEMPLATE, INSTANCE> : CManagerTemplateBase<CManagerAddressableBase<TEMPLATE, INSTANCE>> where TEMPLATE : CManagerAddressableBase<TEMPLATE, INSTANCE> where INSTANCE : CAddressableProviderBase<INSTANCE>, new()
{
	/// <summary> 동시에 로드가능한 개수 </summary>
	public int ConcurrentCount
	{
		get => m_iConCurrentCount;
		set => m_iConCurrentCount = Mathf.Max(1, value);
	}
    private int m_iConCurrentCount = 1;
    private long m_iTotalLoadByte = 0;
    private LinkedList<INSTANCE>                m_listCurrentProvider = new LinkedList<INSTANCE>();    
    private Queue<INSTANCE>                     m_queStandByProvider = new Queue<INSTANCE>(); 
    //----------------------------------------------------------------------
    public bool HasLoadingWork()
	{
        bool work = true;

        if (m_listCurrentProvider.Count == 0 && m_queStandByProvider.Count == 0)
		{
            work = false;
		}

        return work;
	}
	//----------------------------------------------------------------------
	protected override void OnUnityUpdate()
	{
        if (m_listCurrentProvider.Count > 0)
        {
            foreach(INSTANCE CurrentProvider in m_listCurrentProvider)
			{
                CurrentProvider.UpdateLoadWork();
			}
        }
       
        NextProvider();        
    }

    //-----------------------------------------------------------------------
    protected void RequestLoad(string strAddressableName, UnityAction<string, AsyncOperationHandle> delFinish, UnityAction<string, float> deltProgress = null)
	{
        PrivAddressableEnque(strAddressableName, delFinish, deltProgress);
    }

    //-----------------------------------------------------------------------
    private void NextProvider()
    {
        if (m_queStandByProvider.Count == 0) return;

        int EmptyCount = m_iConCurrentCount - m_listCurrentProvider.Count;

        for (int i = 0; i < EmptyCount; i++)
		{
            INSTANCE Provider = m_queStandByProvider.Dequeue();
            m_listCurrentProvider.AddLast(Provider);
            Provider.DoProviderLoadStart();
		}
    }

    private void DeleteProvider(INSTANCE pProvider)
    {
        CAddressableProviderBase<INSTANCE>.InstancePoolReturn(pProvider);
        m_listCurrentProvider.Remove(pProvider);
    }
   
    private INSTANCE FindProvider(string strAddressableName)
	{
        INSTANCE Find = null;
        LinkedList<INSTANCE>.Enumerator itCurrent = m_listCurrentProvider.GetEnumerator();
        while(itCurrent.MoveNext())
		{
            if (itCurrent.Current.GetAddresableName() == strAddressableName)
            {
                Find = itCurrent.Current;
                break;
            }
		}

        if (Find == null)
		{
            IEnumerator<INSTANCE> itStandBy = m_queStandByProvider.GetEnumerator();
            while(itStandBy.MoveNext())
			{
                if (itStandBy.Current.GetAddresableName() == strAddressableName)
				{
                    Find = itStandBy.Current;
                    break;
				}
			}
        }

        return Find;
	}


    private void PrivAddressableEnque(string strAddressableName, UnityAction<string, AsyncOperationHandle> delFinish, UnityAction<string, float> deltProgress)
    {
        INSTANCE provider = CAddressableProviderBase<INSTANCE>.InstancePoolMake<INSTANCE>();
        provider.SetProviderLoadPrepare(strAddressableName, HandleLoadResult, HandleLoadError);
        provider.SetProviderLoadEventAdd(deltProgress, delFinish);
        m_queStandByProvider.Enqueue(provider);
    }
    //-----------------------------------------------------------------------
    private void HandleLoadResult(INSTANCE _provider, CAddressableProviderBase<INSTANCE>.SLoadResult rLoadResult)
    {
        // 참고 : 추출된 GameObject는 프리팹 인스턴스로 GC되지 않으며 Transform을 사용할 수 없다. 사용을 위해서는 instantiate 해야 한다
        DeleteProvider(_provider);
       
		m_iTotalLoadByte -= rLoadResult.LoadedHandle.GetDownloadStatus().TotalBytes;

        if (rLoadResult.LoadedHandle.Result as GameObject)
		{
            OnAddressableLoadGameObject(rLoadResult.AddressableName, rLoadResult.LoadedHandle, rLoadResult.LoadedHandle.Result as GameObject);
        }
        else
		{
            if (rLoadResult.LoadedHandle.Result.GetType() == typeof(SceneInstance))
			{
                SceneInstance TypeCastScene = (SceneInstance)rLoadResult.LoadedHandle.Result;
                OnAddressableLoadScene(rLoadResult.AddressableName, rLoadResult.LoadedHandle, TypeCastScene);
            }
            else 
			{
                OnAddressableLoadObject(rLoadResult.AddressableName, rLoadResult.LoadedHandle, rLoadResult.LoadedHandle.Result as Object);
            }
		}
    }

    private void HandleLoadError(INSTANCE _provider, string _addressableName, string _error)
	{
        DeleteProvider(_provider);
        OnAddressableError(_addressableName, _error);
    }
    //----------------------------------------------------------------------
    protected virtual void OnAddressableLoadGameObject(string strAddressableName, AsyncOperationHandle pLoadedHandle, GameObject pLoadedGameObject) { }
    protected virtual void OnAddressableLoadObject(string strAddressableName, AsyncOperationHandle pLoadedHandle, Object _loadObject) { }
    protected virtual void OnAddressableLoadScene(string strAddressableName, AsyncOperationHandle pLoadedHandle, SceneInstance _SceneInstance) { }  
    protected virtual void OnAddressableError(string strAddressableName,  string strErrorMessage) { }
    
}
