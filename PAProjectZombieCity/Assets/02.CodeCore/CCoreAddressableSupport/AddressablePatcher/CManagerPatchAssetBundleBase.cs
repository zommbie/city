using System;
using System.Collections.Generic;
using UnityEngine.Events;
// CManagerPatchAssetBundleBase 
// [개요] 패치를 위한 다운로드 기능 추상화 클래스 
// [주의!] AddressableSetting에 MaxConcurrentWebRequest를 500에서 1~10정도로 변경한 후 에셋번들 빌드를 해야한다. 한꺼번에 리퀘스트가 몰리면 딜레이가 상당히 걸리게 된다.
// 동적으로 패치 경로를 바꿀때. 프로파일에 추가된 경로를 {CManagerPatchAssetBundleBase.DownloadURL} 로 삽입하면 된다.

abstract public class CManagerPatchAssetBundleBase : CManagerTemplateBase<CManagerPatchAssetBundleBase>
{
    public static string DownloadURL = "";  
	private CPatcherAddressable  m_pPatcherAddressable = new CPatcherAddressable();

    //-----------------------------------------------------
    protected virtual void Update()
    {       
        m_pPatcherAddressable.InterPatcherUpdate(0);
    }

	//-----------------------------------------------------
	protected CPatcherBase.SPatchEvent ProtPatchAssetBundleInitialize(bool bResetEventHandler, string strDownloadURL, UnityAction delFinish)
    {
        DownloadURL = strDownloadURL;
        CPatcherBase.SPatchEvent Handler = m_pPatcherAddressable.InterPatcherInitialize(this, strDownloadURL, string.Empty, true, delFinish);

        if (bResetEventHandler == false)
		{
            Handler.EventPatchInitComplete += HandlePatcherInitComplete;
            Handler.EventPatchDownloadSize += HandlePatcherDownloadSize;
            Handler.EventPatchFinish += HandlePatcherPatchEnd;
            Handler.EventPatchProgress += HandlePatcherProgress;
            Handler.EventPatchError += HandlePatcherError;           
            Handler.EventPatchLabelStart += HandlePatcherStartLabel;
        }

        return Handler;
    }


    protected void ProtPatchAddressableStart(List<string> pListAssetBundleLable)
    {
        m_pPatcherAddressable.DoPatcherAddressableStart(pListAssetBundleLable);
    }

    protected void ProtPatchAddressableTotalDowloadSize(List<string> pListAssetBundleLable, UnityEngine.Events.UnityAction<long> delFinish)
	{
        m_pPatcherAddressable.DoPatcherAddressableDowloadSize(pListAssetBundleLable, delFinish);
	}

    //----------------------------------------------------
    private void HandlePatcherInitComplete()
    {       
        OnPatcherInitComplete();
    }

    private void HandlePatcherDownloadSize(string strPatchName, long iSize)
    {
        OnPatcherDownloadSize(strPatchName, iSize);
    }

    private void HandlePatcherProgress(string strPatchName, long iDownloadedByte, long iTotalByte, float fProgressPercent, uint iLoadCurrent, uint iLoadMax)
    {
        OnPatcherProgress(strPatchName, iDownloadedByte, iTotalByte,  fProgressPercent, iLoadCurrent, iLoadMax);
    }

    private void HandlePatcherPatchEnd()
    {  
        OnPatcherEnd();
    }

    private void HandlePatcherError(CPatcherBase.EPatchError eErrorType, string strMessage)
    {
        OnPatcherError(eErrorType, strMessage);
    }

    private void HandlePatcherStartLabel(string strLabelhName)
	{
        OnPatcherStartLabel(strLabelhName);
    }
    //---------------------------------------------------------------------
    public string ExtractByteToMegaByte(long iByteCount)
    {
        return string.Format("{0:#.#}", iByteCount / 1048576); // 1024 * 1024
    }

    //---------------------------------------------------------------------------
    protected virtual void OnPatcherInitComplete() { }
    protected virtual void OnPatcherDownloadSize(string strLabelhName, long iSize) { }
    protected virtual void OnPatcherProgress(string strLabelhName, long iDownloadedByte, long iTotalByte, float fProgressPercent, uint iLoadCurrent, uint iLoadMax) { }
    protected virtual void OnPatcherEnd() { }
    protected virtual void OnPatcherStartLabel(string strLabelName) { }
    protected virtual void OnPatcherError(CPatcherBase.EPatchError eErrorType, string strMessage) { }

}
