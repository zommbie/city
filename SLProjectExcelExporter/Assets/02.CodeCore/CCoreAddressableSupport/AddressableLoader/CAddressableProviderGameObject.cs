using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CAddressableProviderGameObject : CAddressableProviderBase<CAddressableProviderGameObject>
{
    private AsyncOperationHandle<GameObject> m_pAsyncHandle;
    //-------------------------------------------------------------------
    protected override void OnLoadUpdate()
    {
        if (m_pAsyncHandle.IsValid())
        {
            if (m_pAsyncHandle.Status == AsyncOperationStatus.Failed)
            {
                LoadError(m_pAsyncHandle);
                return;
            }
            else
            {
                LoadProgress(m_pAsyncHandle.GetDownloadStatus().Percent);
            }
        }
    }

    protected override void OnLoadStart()
    {
        m_pAsyncHandle = Addressables.InstantiateAsync(m_strAssetName);
        m_pAsyncHandle.Completed += HandleLoadComplete;
    }
     
    //---------------------------------------------------------------------
    private void HandleLoadComplete(AsyncOperationHandle<GameObject> pLoadedObjectHandle)
    {
        if (pLoadedObjectHandle.IsValid())
        {
            if (pLoadedObjectHandle.Status == AsyncOperationStatus.Failed)
            {
                LoadError(pLoadedObjectHandle);
            }
            else 
            {
                pLoadedObjectHandle.Result.SetActive(false);
                SLoadResult LoadResult = new SLoadResult(m_strAssetName, pLoadedObjectHandle);
                LoadFinish(ref LoadResult);     
            }
        }
        else
        {
            LoadError(pLoadedObjectHandle);
        }
    }
}
