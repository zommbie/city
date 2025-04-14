using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Newtonsoft.Json;
using NWebPacket;

public abstract class CSessionWebRequestBase : CSessionBase
{
	public enum EWebRequestSyncType
	{
		Blocking,           // 순서가 중요한 작업. 모든 작업은 큐잉을 하며 순서대로 처리한다. Send는 다음 프레임에 처리된다.
		NoneBlocking,       // 즉시 발송된다. 순서가 중요하지 않는 작업
	}

	public enum EWebRequestMethodType
	{
		GET,
		POST,
		PUT,
	}
     
	private class SWebPacketWork : CObjectInstancePoolBase<SWebPacketWork>
	{
		public string URL;
		public string ContentsType;
		public UnityWebRequest WebRequest = null;
		public EWebRequestSyncType RequestSyncType = EWebRequestSyncType.NoneBlocking;
		public EWebRequestMethodType RequestMethodType = EWebRequestMethodType.GET;
		public IJsonPacketRequest PacketRequest = null;
		public IJsonPacketResponse PacketResponse = null;
		public UnityAction<IJsonPacketResponse> ResponseDelegate = null;
 
		protected override void OnObjectPoolActivate()
		{
			WebRequest = null;
			PacketRequest = null;
            RequestSyncType = EWebRequestSyncType.NoneBlocking;
            RequestMethodType = EWebRequestMethodType.GET;
            URL = null;
			ResponseDelegate = null;
			ContentsType = string.Empty;
        }
	}

	protected struct SRequestHeader
	{
		public string strHeaderName;
		public string strHeaderValue;
	}

	//------------------------------------------------------------------------
	[SerializeField]
	private int TimeOutSecond = 5;


	private string m_strDestSessionURL = string.Empty;
	private SWebPacketWork m_pRequestCurrent = null;
	private Queue<SWebPacketWork> m_queRequestContents = new Queue<SWebPacketWork>();
	private List<SRequestHeader> m_listRequestHeader = new List<SRequestHeader>();
	//-------------------------------------------------------------
	protected override void OnSessionUpdate(float fDeltaTime)
	{
		UpdateWebRequestQueue();
	}

    protected override void OnSessionFocusRemove()
    {		
		if (m_pRequestCurrent != null)
		{
			m_pRequestCurrent.WebRequest.Abort();
			m_pRequestCurrent.WebRequest.Dispose();
		}

		m_pRequestCurrent = null;
		m_queRequestContents.Clear();
		m_listRequestHeader.Clear();
	}

	protected override void OnSessionDestURL(string strDestURL)
	{
		m_strDestSessionURL = strDestURL;
	}

	//--------------------------------------------------------------
	protected void ProtWebRequestIniailizeHeader(List<SRequestHeader> listRequestHeader)
	{		
		m_listRequestHeader = listRequestHeader;
	}

	protected void ProtWebRequestSendPacket(IJsonPacketRequest pPacketRequest, IJsonPacketResponse pPacketResponse, EWebRequestSyncType eWebRequestSyncType, EWebRequestMethodType eWebRequestMethodType,  UnityAction<IJsonPacketResponse> delResponse)
	{
		if (m_strDestSessionURL == string.Empty)
		{
			Debug.LogFormat("[WebRequest] Dest Session URL has null"); 
			return;
		}

		SWebPacketWork pContents = SWebPacketWork.InstancePoolMake<SWebPacketWork>();
		pContents.ResponseDelegate = delResponse;       
		pContents.RequestSyncType = eWebRequestSyncType;
		pContents.RequestMethodType = eWebRequestMethodType;
		pContents.PacketRequest = pPacketRequest;
		pContents.PacketResponse = pPacketResponse;
		pContents.URL = m_strDestSessionURL;
        m_queRequestContents.Enqueue(pContents);
    }

    //------------------------------------------------------------
    private void UpdateWebRequestQueue()
	{
		if (m_pRequestCurrent == null)
		{
			if (m_queRequestContents.Count > 0)
			{
				m_pRequestCurrent = m_queRequestContents.Dequeue();
				PrivWebRequestPacketSend(m_pRequestCurrent); 
			}
		}
	}

    private void PrivWebRequestPacketSend(SWebPacketWork pRequestContents)
    {
		string strContents = JsonConvert.SerializeObject(pRequestContents.PacketRequest);
		
		pRequestContents.WebRequest = PrivWebRequestPacketUploadSetting(pRequestContents.URL, pRequestContents.RequestMethodType, strContents, pRequestContents.ContentsType);

		if (pRequestContents.RequestSyncType == EWebRequestSyncType.Blocking)
		{
			OnMgrWebRequestBlocking(true);
		}

		ProtSessionCoroutineStart(CoroutineRequestPacketSend(pRequestContents));
	}

	private void PrivWebRequestPacketRemove(SWebPacketWork pRequestContents)
	{
		if (m_pRequestCurrent == pRequestContents)
		{
			m_pRequestCurrent = null;
		}
		pRequestContents.PacketRequest.ReturnInstance();
		pRequestContents.PacketRequest = null;
		pRequestContents.PacketResponse.ReturnInstance();
		pRequestContents.PacketResponse = null;
		pRequestContents.WebRequest.Dispose();
		pRequestContents.WebRequest = null;

		SWebPacketWork.InstancePoolReturn(pRequestContents);
	}

	private void PrivWebRequestPacketResponse(SWebPacketWork pRequestContents)
	{
		if (pRequestContents.RequestSyncType == EWebRequestSyncType.Blocking)
		{
			OnMgrWebRequestBlocking(false);
		}

		if (pRequestContents.WebRequest.result != UnityWebRequest.Result.Success)
		{          
			ProtSessionError((int)pRequestContents.WebRequest.responseCode, pRequestContents.WebRequest.error);
            pRequestContents.ResponseDelegate?.Invoke(null);
		}
        else
        {
			PrivWebRequestPacketResponseRead(pRequestContents);
        }

        PrivWebRequestPacketRemove(pRequestContents);
	}

	private void PrivWebRequestPacketResponseRead(SWebPacketWork pRequestContents)
	{
		string strContents = pRequestContents.WebRequest.downloadHandler.text;
		JsonConvert.PopulateObject(strContents, pRequestContents.PacketResponse);
		pRequestContents.ResponseDelegate?.Invoke(pRequestContents.PacketResponse);
	}

	private UnityWebRequest PrivWebRequestPacketUploadSetting(string strURL, EWebRequestMethodType eMethodType, string strContents, string strContentsType)
	{
		UnityWebRequest pRequest = null;

		if (eMethodType == EWebRequestMethodType.POST)
		{
			pRequest = UnityWebRequest.Post(strURL, strContents, strContentsType);
		}
		else if (eMethodType == EWebRequestMethodType.GET)
		{
			pRequest = UnityWebRequest.Get(strURL);
		}
		else if (eMethodType == EWebRequestMethodType.PUT)
		{
			pRequest = UnityWebRequest.Put(strURL, strContents);
		}

		for (int i = 0; i < m_listRequestHeader.Count; i++)
		{
			pRequest.SetRequestHeader(m_listRequestHeader[i].strHeaderName, m_listRequestHeader[i].strHeaderValue);
		}
		pRequest.timeout = TimeOutSecond;
		return pRequest;
	}

	//--------------------------------------------------------------------------
	IEnumerator CoroutineRequestPacketSend(SWebPacketWork pRequestContents)
	{
		yield return pRequestContents.WebRequest.SendWebRequest();
		PrivWebRequestPacketResponse(pRequestContents);
		yield break;
	}

	//---------------------------------------------------------------------------
	protected virtual void OnMgrWebRequestBlocking(bool bGameBlockStart) { }
	protected virtual void OnMgrWebResponse(UnityWebRequest pWebResponse) { }  // 각종 로그 출력용 
	protected virtual void OnMgrWebRequestError(long iReponseCode, string strErrorDescription) { }
}
