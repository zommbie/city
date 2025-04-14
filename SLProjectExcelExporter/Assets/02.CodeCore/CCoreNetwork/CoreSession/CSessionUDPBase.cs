using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Unity Job은 멀티 스레드 기반이 아니므로 별도의 동기화 오브젝트 로직은 필요가 없다.
public abstract class CSessionUDPBase : CSessionBase
{   
    private UdpClient m_pUDPClient = new UdpClient();
    private IPEndPoint m_pUDPEndPoint = null;
    private Queue<byte []> m_queMainThreadDispatch = new Queue<byte []>();
    private Thread m_pWorkThread = null;
    //------------------------------------------------------
    protected override void OnSessionInitialize()
    {
         
    }

    protected override void OnSessionFocusRemove()
    {    
        m_pUDPClient.Close();
        m_pWorkThread.Abort();
    }

    protected override void OnSessionUpdate(float fDeltaTime)
    {
        while(m_queMainThreadDispatch.Count > 0)
        {
            lock(m_queMainThreadDispatch)
            {
                OnSessionUDPReceivePacket(m_queMainThreadDispatch.Dequeue());
            }
        }
    }

    //----------------------------------------------------------
    protected void ProtSessionUDPOpen(string strDestIP, int iDestPort)
    {
        m_pUDPEndPoint = new IPEndPoint(IPAddress.Parse(strDestIP), iDestPort);
        m_pUDPClient.Connect(m_pUDPEndPoint);
        m_pWorkThread = new Thread(HandleSessionUDPReadWork);
        m_pWorkThread.Start();
    }

    protected void ProtSessionUDPSend(byte [] aSendBuffer, int iSendCount) // 재활용 버퍼이므로 어디까지 사용할지 알려줘야 한다.
    {
        try
        {
            m_pUDPClient.Send(aSendBuffer, iSendCount);
        }
        catch(Exception e)
        {
            Debug.LogErrorFormat("[UDP Send] Error : {0}", e);
        }
    }

    //-----------------------------------------------------------
    private void HandleSessionUDPReadWork()
    {
        while(true)
        {
            byte[] aReadBuffer = m_pUDPClient.Receive(ref m_pUDPEndPoint); // 블로킹 함수라 리시브를 해야 다음 코드로 진행 
            lock (m_queMainThreadDispatch)
            {
                m_queMainThreadDispatch.Enqueue(aReadBuffer);
            }           
        }
    }

    //-----------------------------------------------------------
    protected virtual void OnSessionUDPReceivePacket(byte [] aBuffer) { }
}
