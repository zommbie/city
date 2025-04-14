using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;
using UnityEngine.U2D;

// [개요] Texture와 같은 일반 리소스 객체를 로드하기 위한 레이어
// [주의] 로드 즉시 해당 에셋번들을 해재하므로 같은 에셋을 여러번 호출하면 각자 메모리가 할당된다 (참조로 로딩하고 싶으면 Pool을 사용할것)
public abstract class CManagerResourceBase : CManagerAddressableBase<CManagerResourceBase, CAddressableProviderObject>
{   public static new CManagerResourceBase Instance { get { return CManagerAddressableBase<CManagerResourceBase, CAddressableProviderObject>.Instance as CManagerResourceBase; } }
    //-----------------------------------------------------------------------------------
    protected override void OnAddressableError(string _addressableName, string _error)
    {
        Debug.LogError(string.Format("[Addressable] {0} Error : {1}", _addressableName, _error));
    }
	//----------------------------------------------------------------------
	public void DoMgrResourceVideoClip(string strVideoClipName, UnityAction<VideoClip> delFinish)
	{
		PrivResourceVideoClip(strVideoClipName, delFinish);
	}

	public void DoMgrResourceAtlas(string strAtlasName, UnityAction<SpriteAtlas> delFinish)
	{
		PrivResourceAtlas(strAtlasName, delFinish);
	}

	public void DoMgrResourceTexture(string strTextureName, UnityAction<Texture> delFinish)
	{
		PrivResourceTexture(strTextureName, delFinish);
	}

	//----------------------------------------------------------------------
	private void PrivResourceVideoClip(string strAssetName, UnityAction<VideoClip> delFinish)
	{
        if (delFinish == null) return;

        RequestLoad(strAssetName, (string strLoadedNAme, AsyncOperationHandle pLoadedHandle) => {

			if (pLoadedHandle.Result != null)
			{
				delFinish.Invoke(pLoadedHandle.Result as VideoClip);
			}
			else
			{
				delFinish.Invoke(null);
			}

		});
	}

    private void PrivResourceTextAsset(string strAssetName, UnityAction<TextAsset> delFinish)
    {
        RequestLoad(strAssetName, (string strLoadedName, AsyncOperationHandle pLoadedHandle) => 
        { 
            if (pLoadedHandle.Result != null)
            {
				delFinish.Invoke(pLoadedHandle.Result as TextAsset);
			}
			else
			{
				delFinish.Invoke(null);
			}

		});
	}

	private void PrivResourceAtlas(string strAssetName, UnityAction<SpriteAtlas> delFinish)
	{
		RequestLoad(strAssetName, (string strLoadedName, AsyncOperationHandle pLoadedHandle) =>
		{
			if (pLoadedHandle.Result != null)
			{
				delFinish.Invoke(pLoadedHandle.Result as SpriteAtlas);
			}
			else
			{
				delFinish.Invoke(null);
			}
		});
	}

	private void PrivResourceTexture(string strAssetName, UnityAction<Texture> delFinish)
	{
		RequestLoad(strAssetName, (string strLoadedName, AsyncOperationHandle pLoadedHandle) =>
		{
			if (pLoadedHandle.Result != null)
			{
				delFinish.Invoke(pLoadedHandle.Result as Texture);
			}
			else
			{
				delFinish.Invoke(null);
			}
		});
	}

	public void ReleaseAsset(UnityEngine.Object pAsset) // 해당 에셋을 참조하는 모든 레퍼를 제거 해야 한다.
    {
        Addressables.Release(pAsset);		
    }
    //----------------------------------------------------------------------------------------


}
