using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CManagerPatchWebRequestBase : CManagerTemplateBase<CManagerPatchWebRequestBase>
{
    
    private CPatcherWebRequest m_pPatcherWebRequest = new CPatcherWebRequest();
  
    //----------------------------------------------------------------------------
    protected virtual void Update()
    {
        m_pPatcherWebRequest.InterPatcherUpdate(Time.deltaTime);       
    }

    //---------------------------------------------------------------------------
    protected CPatcherBase.SPatchEvent ProtMgrPatchWebRequestInitialize(string strDownloadURL, string strSavePath)
    {
        CPatcherBase.SPatchEvent pEventHandler = m_pPatcherWebRequest.InterPatcherInitialize(this, strDownloadURL, strSavePath, true, null);
        pEventHandler.EventPatchError += HandlePatchWebRequestError;
        return pEventHandler;
    }

    protected void ProtMgrPatchWebRequestReadFromFile(string strFileName, Action<byte[]> delFinish, bool bDecryption = false)
    {
        m_pPatcherWebRequest.InterPatcherReadFromFile(strFileName, bDecryption, delFinish);
    }

    protected void ProtMgrPatchWebRequestReadFromURL()
    {

    }

    protected void ProtMgrPatchWebRequestWriteToFile(string strFileName, string strContents, Action delFinish, bool bEncryption = false)
    {
        m_pPatcherWebRequest.InterPatcherFileSave(strFileName, strContents, bEncryption, delFinish);
    }   

    //-----------------------------------------------------------------------------
    protected void ProtMgrPatchFileDelete(string strFileName)
    {
        m_pPatcherWebRequest.InterPatcherFileDelete(strFileName);
    }


    //------------------------------------------------------------------------------
    private void HandlePatchWebRequestError(CPatcherBase.EPatchError eError, string strMessage)
    {
        OnPatchWebRequestError(eError, strMessage);
    }

    //--------------------------------------------------
    protected virtual void OnPatchWebRequestError(CPatcherBase.EPatchError eError, string strMessage) { }
}
