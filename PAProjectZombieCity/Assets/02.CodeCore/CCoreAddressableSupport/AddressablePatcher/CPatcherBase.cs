using System;
using UnityEngine;
using UnityEngine.Events;
abstract public class CPatcherBase : CObjectBase
{
    public enum EPatchError
    {
        NotInitialized,         // 초기화가 되어 있지 않다.
        AlreadyPatchProcess,    // 패치 진행중이다.
        NotEnoughDiskSpace,     // 저장공간이 부족하다
        NetworkDisable,         // 인터넷이 연결되지 않았다.
        CatalogUpdateFail,      // 내부적으로 카탈로그 업데이트가 실패 했다.
        PatchFail,              // 패치가 실패 했다.
        HTTPError,              // 404 등 프로토콜 에러가 발생했다.
        WebRequestError,        // WebRequestDownload 에서 에러가 발생했다.
    }

    public class SPatchEvent
    {
        public event Action EventPatchInitComplete = null;
        public event Action<string, long> EventPatchDownloadSize = null;
        public event Action<string, long, long, float, uint, uint> EventPatchProgress = null; // 패치이름 , 다운로드된 바이트 , 전체 바이트, 전체 다운로드 비율, 로드된 에셋번들 헤더 (패치 없을시), 로드할 전체 에셋(패치 없을시)
        public event Action<string>       EventDownloadFinish = null;
        public event Action<EPatchError, string> EventPatchError = null;
        public event Action EventPatchFinish = null;
        public event Action<string> EventPatchLabelStart = null;

        protected void ProtReset() { EventPatchInitComplete = null; EventPatchDownloadSize = null; EventPatchProgress = null; EventPatchError = null; EventDownloadFinish = null; }
        protected void ProtInitComplete() { EventPatchInitComplete?.Invoke(); }
        protected void ProtDownloadSize(string Name, long Size) { EventPatchDownloadSize?.Invoke(Name, Size); }
        protected void ProtPatchFinish() { EventPatchFinish?.Invoke(); }
        protected void ProtProgress(string Name, long _downloadedByte, long _totalByte, float Progress, uint _loadCurrent, uint _loadMax) { EventPatchProgress?.Invoke(Name, _downloadedByte, _totalByte, Progress, _loadCurrent, _loadMax); }
        protected void ProtDownloadFinish(string Name) { EventDownloadFinish?.Invoke(Name); }
        protected void ProtError(EPatchError _errorType, string _message) { EventPatchError?.Invoke(_errorType, _message); }
        protected void ProtLabelStart(string Name) { EventPatchLabelStart?.Invoke(Name); }
    }

    private class SPatchEventHandle : SPatchEvent
    {
        public void DoReset() { ProtReset(); }
        public void DoInitComplete() { ProtInitComplete(); }
        public void DoDownloadSize(string Name, long Size) { ProtDownloadSize(Name, Size); }
        public void DoProgress(string Name, long _downloadedByte, long _totalByte, float Progress, uint _loadCurrent, uint _loadMax) { ProtProgress(Name, _downloadedByte, _totalByte, Progress, _loadCurrent, _loadMax); }
        public void DoDownloadFinish(string Name) { ProtDownloadFinish(Name); }
        public void DoError(EPatchError _errorType, string _message) { ProtError(_errorType, _message); }

        public void DoPatchFinish() { ProtPatchFinish(); }
        public void DoLabelStart(string Name) { ProtLabelStart(Name); }
    };

    private SPatchEventHandle m_pEventHandler = new SPatchEventHandle();
    private MonoBehaviour m_pCoroutine = null;
    //---------------------------------------------------------
    internal SPatchEvent InterPatcherInitialize(MonoBehaviour pCoroutine, string strDownloadURL, string strDownloadSavePath,  bool bResetHandler, UnityAction delFinish)
    {
        if (bResetHandler)
        {
            m_pEventHandler.DoReset();
        }
        m_pCoroutine = pCoroutine;
        OnPatcherInitialize(strDownloadURL, strDownloadSavePath, delFinish);
        return m_pEventHandler;
    }

    internal void InterPatcherUpdate(float DeltaTime)
    {
        OnPatcherUpdate(DeltaTime);
    }

    //-----------------------------------------------------------
    protected void EventPatchInitComplete()
    {
        m_pEventHandler.DoInitComplete();
    }

    protected void EventPatchDownloadSize(string PatchName, long Size)
    {
        m_pEventHandler.DoDownloadSize(PatchName, Size);
    }

    protected void EventPatchProgress(string _patchName, long _downloadedByte, long _totalByte, float _percent, uint _loadCurrent, uint _loadMax)
    {
        m_pEventHandler.DoProgress(_patchName, _downloadedByte, _totalByte, _percent, _loadCurrent, _loadMax);
    }

    protected void EventPatchError(EPatchError _errorType, string _message = null)
    {
        m_pEventHandler.DoError(_errorType, _message);
    }

    protected void EventDownloadFinish(string PatchName)
    {
        m_pEventHandler.DoDownloadFinish(PatchName);
    }

    protected void EventPatchFinish()
	{
        m_pEventHandler.DoPatchFinish();
	}

    protected void EventPatchLabelStart(string _labelName)
	{
        m_pEventHandler.DoLabelStart(_labelName);
	}

    protected Coroutine StartCoroutine(System.Collections.IEnumerator pFunction)
    {
        return m_pCoroutine.StartCoroutine(pFunction);
    }

    protected void StopCoroutine(System.Collections.IEnumerator pFunction)
    {
        m_pCoroutine.StopCoroutine(pFunction);
    }

    //----------------------------------------------------------
    protected virtual void OnPatcherInitialize(string strDownloadURL, string strDownloadSavePath, UnityAction delFinish) { }
    protected virtual void OnPatcherUpdate(float DeltaTime) { }
    //----------------------------------------------------------
    public static string ConvertToFittedSize(long lByteSize)
    {
        string[] readonly_SIZE_UNIT_ARRAY = new string[5] { "B", "KB", "MB", "GB", "TB" };

        int iQuotient = 0;
        double dDividend = lByteSize;
        while (true)
        {
            bool bDivideFinish = dDividend <= 1024;
            bool bArrayOverflow = iQuotient >= readonly_SIZE_UNIT_ARRAY.Length - 1;
            if (bDivideFinish || bArrayOverflow) break;
            iQuotient++;
            dDividend = dDividend / 1024;
        }
        return string.Format("{0:0.00}{1}", dDividend, readonly_SIZE_UNIT_ARRAY[iQuotient]);
    }
}
