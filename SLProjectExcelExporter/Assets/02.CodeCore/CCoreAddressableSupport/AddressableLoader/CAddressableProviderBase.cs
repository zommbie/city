using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

// 실제 로드작업을 수행하는 객체 

public abstract class CAddressableProviderBase<TEMPLATE> : CObjectInstancePoolBase<TEMPLATE> where TEMPLATE : CAddressableProviderBase<TEMPLATE>, new()
{
    private event UnityAction<string, float>                        m_delEventProgress = null;
    private event UnityAction<string, AsyncOperationHandle>         m_delEventFinish = null;
    //-----------------------------------------------------------
    private UnityAction<TEMPLATE, SLoadResult>            m_delLoadResult = null;
    private UnityAction<TEMPLATE, string, string>         m_delError = null;
    //----------------------------------------------------------
    protected string m_strAssetName;                        
    private bool m_bUpdateWork = false;   
    private bool m_bLoadingEnd = true; public bool p_LoadingEnd { get { return m_bLoadingEnd; } }
    //---------------------------------------------------------------------------------------
    public void UpdateLoadWork()
    {
        if (m_bUpdateWork)
        {
            OnLoadUpdate();
        }
    } 

    //-----------------------------------------------------------------------------------------
    public void SetProviderLoadPrepare(string strAssetName, UnityAction<TEMPLATE, SLoadResult> delLoadResult, UnityAction<TEMPLATE, string, string> delError)
    {
        m_strAssetName = strAssetName;
        m_delLoadResult = delLoadResult;
        m_delError = delError;
        m_bLoadingEnd = false;
    }

    public void SetProviderLoadEventAdd(UnityAction<string, float> delEventProgress, UnityAction<string, AsyncOperationHandle> delFinish)
    {
        if (delEventProgress != null)
		{
            m_delEventProgress += delEventProgress;
        }
        if (delFinish != null)
		{
            m_delEventFinish += delFinish;
        }
    }

    public string GetAddresableName() 
    { 
        return m_strAssetName; 
    }

    public void DoProviderLoadStart()
    {
        m_bUpdateWork = true;
        OnLoadStart();
    }

    //-------------------------------------------------------------------------------------
    protected void LoadProgress(float fProgress)
    {
        m_delEventProgress?.Invoke(m_strAssetName, fProgress);
    }

    protected void LoadError(AsyncOperationHandle pErrorHandle)
    {
        m_bUpdateWork = false;
        m_delEventFinish?.Invoke(m_strAssetName, pErrorHandle);
        m_delError?.Invoke(this as TEMPLATE, m_strAssetName, pErrorHandle.OperationException.ToString());
    }

    protected void LoadFinish(ref SLoadResult rLoadResult)
    {
        m_bUpdateWork = false;
        m_bLoadingEnd = true;
        m_delEventFinish?.Invoke(m_strAssetName, rLoadResult.LoadedHandle);
        m_delLoadResult?.Invoke(this as TEMPLATE, rLoadResult);
    }

	protected float ExtractAddressableLoadProgress(AsyncOperationHandle pHandle)
	{
		List<AsyncOperationHandle> listDependencies = new List<AsyncOperationHandle>();
        pHandle.GetDependencies(listDependencies);

        if (listDependencies.Count > 0)
		{
			AsyncOperationHandle defHandle = listDependencies[0];
			listDependencies.Clear();
			defHandle.GetDependencies(listDependencies);
		}
        else
		{
            return pHandle.PercentComplete;
		}

        float fProgress = 0;
		uint iCount = 0;
		for (int i = 0; i < listDependencies.Count; i++)
		{
			if (listDependencies[i].Status == AsyncOperationStatus.Succeeded)
			{
				iCount++;
			}
		}

        if (iCount != 0 && listDependencies.Count != 0)
        {
            fProgress = iCount / listDependencies.Count;
		}
        else
        {
            fProgress = 1f;
        }

		return fProgress;
	}


	//--------------------------------------------------------------------------------------
	protected sealed override void OnObjectPoolActivate() { }
    protected sealed override void OnObjectPoolDeactivate()
    {
        m_delLoadResult = null;
        m_delEventProgress = null;
        m_delEventFinish = null;
        m_delError = null;
        m_strAssetName = null;       
        m_bUpdateWork = false;
    }
    //---------------------------------------------------------------------------------------
    protected virtual void OnLoadUpdate() { }
    protected virtual void OnLoadStart() { }
    //-----------------------------------------------------------------------------------------
    public struct SBundleInfo
    {
        public string BundleFileName;
        public string BundleName;
        public long   BundleDiskSize;
    }

    public struct SLoadResult
    {
        public string               AddressableName;
        public AsyncOperationHandle LoadedHandle;       
          
        public SLoadResult(string strAddressableName, AsyncOperationHandle pLoadedHandle)
        {
            AddressableName = strAddressableName;
			LoadedHandle = pLoadedHandle;           
        }
    }
}
