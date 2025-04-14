using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;


public class CPatcherWebRequest : CPatcherBase
{
    private StringBuilder m_pNote = new StringBuilder(256);
    private string m_strDownLoadURL;
    private string m_strSavePath;
    private string m_strCryptionKey;
    private bool m_bInitialize = false;
  
    //------------------------------------------------------------------------------
    protected override void OnPatcherInitialize(string strDownloadURL, string strSavePath)
    {
        m_bInitialize = true;
        m_strDownLoadURL = strDownloadURL;
        m_strSavePath = Path.Combine(Application.persistentDataPath, strSavePath);
        PrivPatcherFindOrMakeSavePath(m_strSavePath);
    }

    //-------------------------------------------------------------------------------
    internal void InterPatcherCryptionInfo(string strCryptionKey)
    {

    }

    internal void InterPatcherFileDownload(string strFileName, bool bSavePath, bool bEncryption, Action<byte[]> delFinish) // 다운로드 한 다음 교체한다.
    {
        if (CheckPatcherWorkReady() == false) return;

        string strFilePath = ExtractPatcherDownloadURL(strFileName);       
        StartCoroutine(CoroutineWebRequestDownload(strFilePath, bEncryption, true, delFinish));
    }

    internal void InterPatcherReadFromFile(string strFileName, bool bDecription, Action<byte[]> delFinish)
    {
        if (CheckPatcherWorkReady() == false) return;

        string strLocalFilePath = ExtractPatcherSaveFilePath(strFileName);

        if (File.Exists(strLocalFilePath))
        {           
            StartCoroutine(CoroutineWebRequestDownload(strLocalFilePath, bDecription, false, delFinish));
        }
        else
        {
            delFinish?.Invoke(null);
        }
    }

    internal void InterPatcherFileDeleteAllDirectory(string strSavePath)
    {

    }

    internal void InterPatcherFileDelete(string strFileName)
    {
        string strLocalFilePath = ExtractPatcherSaveFilePath(strFileName);
        File.Delete(strLocalFilePath);
    }

    internal void InterPatcherFileSave(string strFileName, string strContents, bool bEncryption, Action delFinish)
    {
        //ToDo 암호화
        PrivPatcherWebRequestSaveFile(strFileName, strContents, delFinish);
    }

    //---------------------------------------------------------------------------------
    private IEnumerator CoroutineWebRequestDownload(string strFilePath, bool bDecription, bool bSaveFile,  Action<byte[]> delFinish)
    {
        UnityWebRequest pWebRequest = UnityWebRequest.Get(strFilePath);
        UnityWebRequestAsyncOperation pHandle = pWebRequest.SendWebRequest();
        string strFileName = Path.GetFileName(strFilePath);
        while(true)
        {
            if (pHandle.isDone)
            {
                EventPatchProgress(strFileName, 0, 0, 1f, 0, 0);
                if (pHandle.webRequest.error == null)
                {
                    PrivPatcherWebRequestFinish(strFileName, pHandle.webRequest.downloadHandler.data, bDecription, bSaveFile, delFinish);
                }
                else
                {
                    EventPatchError(EPatchError.HTTPError, pHandle.webRequest.error);
                }

                break;
            }
            else
            {
                EventPatchProgress(strFileName, 0, 0, pHandle.progress, 0, 0);
                yield return null;
            }
        }
    }


    //----------------------------------------------------------------------------------

    private void PrivPatcherFindOrMakeSavePath(string strSavePath)
    {
        if (Directory.Exists(strSavePath) == false)
        {
            Directory.CreateDirectory(strSavePath);
        }
    }

    private void PrivPatcherWebRequestFinish(string strFileName, byte [] aBuffer, bool bDecription, bool bSaveFile, Action<byte[]> delFinish)
    {       
        if (aBuffer == null)
        {
            delFinish?.Invoke(null);
            return;
        }

        // ToDo 암호화 작업 

        if (bSaveFile)
        {
            PrivPatcherWebRequestSaveFile(strFileName, aBuffer, () =>
            {
                delFinish?.Invoke(aBuffer);
            });
        }
        else
        {
            delFinish?.Invoke(aBuffer);
        }
    }

    private async void PrivPatcherWebRequestSaveFile(string strFileName, byte [] aBuffer, Action delFinish)
    {
        string strFilePath = ExtractPatcherSaveFilePath(strFileName);
        await File.WriteAllBytesAsync(strFilePath, aBuffer);
        delFinish?.Invoke();
    }

    private async void PrivPatcherWebRequestSaveFile(string strFileName, string strContents, Action delFinish)
    {
        string strFilePath = ExtractPatcherSaveFilePath(strFileName);
        await File.WriteAllTextAsync(strFilePath, strContents);
        delFinish?.Invoke();
    }


    //-------------------------------------------------------------
    private string ExtractPatcherSaveFilePath(string strFileName)
    {
        m_pNote.Clear();
        m_pNote.Append(m_strSavePath);
        m_pNote.Append(Path.DirectorySeparatorChar);
        m_pNote.Append(strFileName);
        return m_pNote.ToString();
    }

    private string ExtractPatcherDownloadURL(string strFileName)
    {
        m_pNote.Clear();
        m_pNote.Append(m_strDownLoadURL);
        m_pNote.Append(Path.DirectorySeparatorChar);
        m_pNote.Append(strFileName);
        return m_pNote.ToString();
    }

    private bool CheckPatcherWorkReady()
    {
        if (m_bInitialize == false)
        {
            EventPatchError(EPatchError.NotInitialized);
            return false;
        }

        return true;
    }
}
