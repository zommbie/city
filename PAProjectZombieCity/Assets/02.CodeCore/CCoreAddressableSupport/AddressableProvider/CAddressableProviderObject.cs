using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class CAddressableProviderObject : CAddressableProviderBase<CAddressableProviderObject>
{
    private AsyncOperationHandle<object> mAsyncHandle;
    //------------------------------------------------------------------------------

    protected override void OnLoadUpdate()
    {
        if (mAsyncHandle.IsValid())
        {
            if (mAsyncHandle.Status == AsyncOperationStatus.Failed)
            {
                LoadError(mAsyncHandle);
                return;
            }
            else
            {
                LoadProgress(mAsyncHandle.GetDownloadStatus().Percent);
            }
        }
    }

    protected override void OnLoadStart()
    {
        mAsyncHandle = Addressables.LoadAssetAsync<object>(m_strAssetName);
        mAsyncHandle.Completed += HandleLoadComplete;
    }
     
    //---------------------------------------------------------------------
    private void HandleLoadComplete(AsyncOperationHandle<object> pHandleLoadedObject)
    {
        if (pHandleLoadedObject.IsValid())
        {
            if (pHandleLoadedObject.Status == AsyncOperationStatus.Failed)
            {
                LoadError(pHandleLoadedObject);
            }
            else
            {
                SLoadResult LoadResult = new SLoadResult(m_strAssetName, pHandleLoadedObject);
                LoadFinish(ref LoadResult);
            }
        }
        else
        {
            LoadError(pHandleLoadedObject);
        }       
    }
}
