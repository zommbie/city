using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
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
        // 이미 로드된 프리팹의 경우 로드 직후 1프레임 랜더링에 걸리는 것을 막을 수 없어서 임의좌표로 이동
        InstantiationParameters rInitParam = new InstantiationParameters(new Vector3(0f, 10000f, 0), Quaternion.identity, null);
        m_pAsyncHandle = Addressables.InstantiateAsync(m_strAssetName, rInitParam);
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
