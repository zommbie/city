using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public abstract class CStageBase : CMonoBase
{
    public enum EStageStatus
    {
        None,
        Loading,        // 에셋을 로딩하고 있다.
        Ready,          // 로딩이 끝나서 스타트 가능 상태이다
        Start,          // 스테이지가 시작되었다.
        End,            // 스테이지가 끝난 상태이다. Reset등으로 재시작이 가능하다.
    }

    [SerializeField]
    private CStageCameraBase StageCamera;

    private uint m_hStageID = 0;   public uint GetStageID() { return m_hStageID; }
    private EStageStatus m_eStageStatus = EStageStatus.None;
    //--------------------------------------------------------------------
    internal void InterStageInitialize()
    {
        StageCamera?.InterStageCameraInitialize();
        OnStageInitialize();
    }

    internal void InterStageLoad(uint hStageID, UnityAction delFinish, params object [] aParams)
    {
        if (m_eStageStatus == EStageStatus.None)
        {
            m_hStageID = hStageID;
            m_eStageStatus = EStageStatus.Loading;
            OnStageLoad(hStageID, ()=> {
                m_eStageStatus = EStageStatus.Ready;
                delFinish?.Invoke();
            }, aParams);
        }
        else
        {
            //Error! 이미 로딩 되어 있다.
        }
    }

    internal void InterStageStart(int iOption)
    {
        if (m_eStageStatus == EStageStatus.Ready)
        {
            m_eStageStatus = EStageStatus.Start;
            OnStageStart(iOption);
        }
        else
        {
            //Error!
        }
    }

    internal void InterStageEnd(int iResult)
    {
        if (m_eStageStatus == EStageStatus.Start)
        {
            m_eStageStatus = EStageStatus.End;
            OnStageEnd(iResult);
        }
        else
        {
            //Error!
        }
    }

    internal void InterStageReset(int iOption)
    {
        if (m_eStageStatus == EStageStatus.End)
        {
            m_eStageStatus = EStageStatus.Ready;
            OnStageReset(iOption);
        }
        else
        {
            //Error!
        }
    }

    internal void InterStageExit()  // 해당 스테이지를 종료하고 언로드 하기 직전 호출
    {
        OnStageExit();
    }

    //----------------------------------------------------------------------
    protected virtual void OnStageInitialize() { }
    protected virtual void OnStageLoad(uint hStageID, UnityAction delFinish, params object[] aParams) { }
    protected virtual void OnStageStart(int iOption) { }
    protected virtual void OnStageEnd(int iResult) { }
    protected virtual void OnStageReset(int iOption) { }
    protected virtual void OnStageExit() { }
}
