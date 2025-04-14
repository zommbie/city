using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// [주의!] AudioClip이나 Texture등에 사용해야 하며 로드 이후 핸들을 해재 하기 때문에 각 객체가를 Destroy하면 비디오 매모리 해재가 된다.
// 그러나 같은 오브젝트를 중복 호출했을 경우 각자 메모리가 할당되어 버린다. AssetBundle.unload(false) 참조

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
