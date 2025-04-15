using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 스테이지는 독립된 게임 플레이 단위이다. AdditiveScene으로 복수의 스테이지가 하나의 씬에 병합 될 수 있다
// 화면을 분할하여 별도의 카메라를 가진다든가 하는 다양한 실행 환경에 대응하기 위한 레이어이다.

abstract public class CManagerStageBaseOld : CManagerTemplateBase<CManagerStageBaseOld>
{
    private bool m_bStageStart = false; public bool IsStageStart { get { return m_bStageStart; } }
    private uint m_hStageID = 0; public uint p_StageID { get { return m_hStageID; } }
    private List<CStageBaseOld> m_listStageInstance = new List<CStageBaseOld>();
    //--------------------------------------------------------------
    protected override void OnUnityAwake()
    {
        base.OnUnityAwake();
        PrivStageFindGlobalStage();
    }

    //---------------------------------------------------------------
    internal void InterStageRegist(CStageBaseOld pStage)
    {
        PrivStageRegist(pStage);
    }

    internal void InterStageUnRegist(CStageBaseOld pStage)
    {
        PrivStageUnRegist(pStage);
    }
    //-----------------------------------------------------------------

    protected void ProtMgrStageStart(int iIndex = 0, params object[] aParams)  // 스테이지 시작 기능 진입점
    {
        if (m_bStageStart) return;
        PrivStageStart(iIndex, aParams);
    }

    protected void ProtMgrStageEnd(int iIndex = 0)  // 스테이지 플레이 종료. 메모리가 유지되므로  스테이지가 재 시작 될 수 있다.
    {
        m_bStageStart = false;
        PrivStageEnd(iIndex);
    }

    protected void ProtMgrStageReset(int iIndex, params object[] aParams) // 스테이지의 모든 내용이 초기화 
    {
        PrivStageReset(iIndex, aParams);
    }

    protected void ProtMgrStageLoad(uint hLoadID, UnityAction delFinish, int iIndex = 0, params object[] aParams)
    {
        PrivStageLoad(hLoadID, delFinish, iIndex, aParams);
    }

    protected void ProtMgrStageReload(UnityAction delFinish, int iIndex = 0, params object[] aParams)
    { 
        PrivStageReLoad(delFinish, iIndex, aParams);
    }

    protected void ProtMgrStagePrepare(UnityAction delFinish, int iIndex = 0, params object[] aParams)
    {
        PrivStagePrepare(delFinish, iIndex, aParams);
    }

    protected void ProtMgrStagePauseResume(bool bPause, int iIndex = 0)
    {
        CStageBaseOld pStage = FindMgrStage(iIndex);
        if (pStage != null)
        {
            pStage.InterStagePauseResume(bPause);
            OnMgrStagePause(pStage);
        }
    }

    protected void ProtMgrStageExit()
    {
        PrivStageExit();
    }

    //---------------------------------------------------------------
    private void PrivStageRegist(CStageBaseOld pStage)
    {
        if (m_listStageInstance.Contains(pStage) == false)
        {
            m_listStageInstance.Add(pStage);
            pStage.InterStageRegister();
            OnMgrStageRegister(pStage);
        }
    }

    private void PrivStageUnRegist(CStageBaseOld pStage)
    {
        m_listStageInstance.Remove(pStage);
        pStage.InterStageUnRegister();
        OnMgrStageUnRegister(pStage);
    }

    private void PrivStageReset(int iIndex, params object[] aParams)
    {
		CStageBaseOld pStage = FindMgrStage(iIndex);
		if (pStage != null)
		{
			pStage.InterStageReset(aParams);
			OnMgrStageReset(pStage);
		}		
    }

    private void PrivStageStart(int iIndex = 0, params object[] aParams)
    {
		CStageBaseOld pStage = FindMgrStage(iIndex);
        if (pStage.p_StageLoaded == false)
		{
			Debug.LogError(string.Format("[Stage] Stage Does not Loaded : {0}", aParams));
		}
        else
		{
			pStage.InterStageStart(aParams);
			OnMgrStageStart(pStage);
		}
    }

    private void PrivStageEnd(int iIndex)
    {
        CStageBaseOld pStage = FindMgrStage(iIndex);
        if (pStage != null)
		{
            pStage.InterStageEnd();
            OnMgrStageEnd(pStage);
		}
    }

    private void PrivStageLoad(uint hLoadID, UnityAction delFinish, int iIndex, params object[] aParams)
    {
        CStageBaseOld pStage = FindMgrStage(iIndex);
        if (pStage != null)
        {
            pStage.InterStageLoad(hLoadID, (CStageBaseOld pLoadedStage) => {               
                OnMgrStageLoaded(pLoadedStage);
                delFinish?.Invoke();
            }, aParams);
            OnMgrStageLoad(pStage);
        }
    }

    private void PrivStageReLoad(UnityAction delFinish, int iIndex, params object[] aParams)
    {
        CStageBaseOld pStage = FindMgrStage(iIndex);
        if (pStage != null)
        {
            pStage.InterStageReLoad((CStageBaseOld pLoadedStage) =>
            {
                delFinish?.Invoke();
                OnMgrStageReLoaded(pLoadedStage);
            }, aParams);
            OnMgrStageReLoad(pStage);
        }
    }

    private void PrivStagePrepare(UnityAction delFinish, int iIndex, params object[] aParams)
    {
        CStageBaseOld pStage = FindMgrStage(iIndex);
        if (pStage != null)
        {
            pStage.InterStagePrepare(() => {
                OnMgrStagePrepare(pStage);
                delFinish?.Invoke();
            }, aParams);
        }
    }

    private void PrivStageExit()
    {
        for (int i = 0; i < m_listStageInstance.Count; i++)
        {
            m_listStageInstance[i].InterStageExit();
        }
    }

    //------------------------------------------------------------------
    private void PrivStageFindGlobalStage()
    {
        CStageBaseOld[] aFindStage = FindObjectsByType<CStageBaseOld>(FindObjectsSortMode.None);
        for (int i = 0; i < aFindStage.Length; i++)
        {
            PrivStageRegist(aFindStage[i]);
        }
    }

    //-----------------------------------------------------------------
    protected CStageBaseOld FindMgrStage(int iIndex)
    {
        CStageBaseOld pFindStage = null;
        if (iIndex < m_listStageInstance.Count)
        {
            pFindStage = m_listStageInstance[iIndex];
        }

        return pFindStage;
    }

    protected List<CStageBaseOld>.Enumerator GetMgrStageInstanceIterator()
    {
        return m_listStageInstance.GetEnumerator();
    }

    //-------------------------------------------------------------------
    protected virtual void OnMgrStageStart(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageEnd(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageReset(CStageBaseOld pStage) { }
    protected virtual void OnMgrStagePrepare(CStageBaseOld pStage) { }   
    protected virtual void OnMgrStagePause(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageLoad(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageLoaded(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageReLoad(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageReLoaded(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageRegister(CStageBaseOld pStage) { }
    protected virtual void OnMgrStageUnRegister(CStageBaseOld pStage) { }
}
