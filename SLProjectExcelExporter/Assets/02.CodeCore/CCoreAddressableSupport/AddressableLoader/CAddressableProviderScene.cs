using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class CAddressableProviderScene : CAddressableProviderBase<CAddressableProviderScene>
{
    private AsyncOperationHandle<SceneInstance> m_pAsyncHandle;
  
	//--------------------------------------------------------------------------
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
                float fDownloadPercent = m_pAsyncHandle.GetDownloadStatus().Percent;
                if (fDownloadPercent == 0)
				{
                    PrivLoadSceneLocalLoad(m_pAsyncHandle);
                }
                else
				{
					LoadProgress(fDownloadPercent);
				}
			}
        }
    } 

    protected override void OnLoadStart()
    {
        m_pAsyncHandle = Addressables.LoadSceneAsync(m_strAssetName, UnityEngine.SceneManagement.LoadSceneMode.Additive, true);
        m_pAsyncHandle.Completed += HandleLoadSceneComplete;
    }
    //-------------------------------------------------------------------------
    private void PrivLoadSceneLocalLoad(AsyncOperationHandle<SceneInstance> pAsyncHandle)
	{
        LoadProgress(ExtractAddressableLoadProgress(pAsyncHandle));
    }

    //-------------------------------------------------------------------------
    private void HandleLoadSceneComplete(AsyncOperationHandle<SceneInstance> pLoadedSceneHandle)
    {
        // [주의!] 씬 인스턴스는 로드 되었으나 씬 내부의 게임 오브젝트가 Awake되었는지는 불분명하다.
        //         디바이스 상태에 따라 Finish 이벤트 발생시 Awake가 모두 호출되지 않을 수도 있다.
        if (pLoadedSceneHandle.IsValid())
        {
            if (pLoadedSceneHandle.Status == AsyncOperationStatus.Failed)
            {
                LoadError(pLoadedSceneHandle);
            }
            else
            {
                SLoadResult LoadResult = new SLoadResult(m_strAssetName, pLoadedSceneHandle);
				LoadFinish(ref LoadResult);
			}
		}
    }

}
