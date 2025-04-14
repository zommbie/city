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

public abstract class CManagerResourceBase : CManagerAddressableBase<CManagerResourceBase, CAddressableProviderObject>
{   public static new CManagerResourceBase Instance { get { return CManagerAddressableBase<CManagerResourceBase, CAddressableProviderObject>.Instance as CManagerResourceBase; } }
    //-----------------------------------------------------------------------------------
    protected override void OnAddressableError(string _addressableName, string _error)
    {
        Debug.LogError(string.Format("[Addressable] {0} Error : {1}", _addressableName, _error));
    }
    //----------------------------------------------------------------------
    public void DoMgrResourceAudioClip(string strAudioClipName, UnityAction<AudioClip> delFinish)
    {
        PrivResourceAudioClip(strAudioClipName, delFinish);
    }

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

    public void DoMgrResourceText(string strTextName, UnityAction<TextAsset> delFinish)
    {
        PrivResourceTextAsset(strTextName, delFinish);
    }

    //----------------------------------------------------------------------
    private void PrivResourceAudioClip(string strAssetName, UnityAction<AudioClip> delFinish)
    {
        if (delFinish == null) return;

        RequestLoad(strAssetName, (string strLoadedNAme, AsyncOperationHandle pLoadedHandle) => {

            if (pLoadedHandle.Result != null)
            {
                delFinish.Invoke(pLoadedHandle.Result as AudioClip);
            }
            else
            {
                delFinish.Invoke(null);
            }
        });
    }

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
