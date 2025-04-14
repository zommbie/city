using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// 스폰 시작과 종료 처리 
// 스폰 스폿의 작동 관리

abstract public class CStageSpawnerBase : CMonoBase
{
	[SerializeField]
	private int SpawnerID = 0;		            public int GetStageSpawnerID() { return SpawnerID; }
    [SerializeField]
    private int NextSpawnerID = 0;              public int GetStageSpawnerNextID() { return NextSpawnerID; }
    [SerializeField]
    private List<CStageSpawnWorkBase> SpawnWork = null;

	private bool m_bActivate = false;	public bool IsActivate { get { return m_bActivate; } }
    private bool m_bExpire = false;     public bool IsExpire { get { return m_bExpire; } }
	//-------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake(); 
        if (CManagerStageSpawnerBase.Instance != null)
        {
            CManagerStageSpawnerBase.Instance.InterMgrStageSpawnerRegist(this);
        }
    }

	private void Update()
	{
		if (m_bActivate)
		{
            OnSpanwerUpdate(Time.deltaTime);
        }
	}
    //--------------------------------------------------
    internal void InterSpawnerInitialize(UnityAction delFinish)
	{
        int SpawnerCount = 0;

        if (SpawnWork.Count == 0)
        {
            delFinish?.Invoke();
        }

        for (int i = 0; i < SpawnWork.Count; i++)
		{
            SpawnWork[i].InterSpawnWorkInitilize(() => {
                SpawnerCount++;
                if (SpawnerCount >= SpawnWork.Count)
                {
                    delFinish?.Invoke();
                }
            });
		}
		

		OnSpawnerInitialize();
	}

	internal void InterSpawnerOpen()
	{
		m_bActivate = true;
        OnSpawnerOpen();
    }

    internal void InterSpawnerClose()
    {
        m_bActivate = false;
        m_bExpire = true;
        OnSpawnerClose();
    }

	internal void InterSpawnerReset()
	{
		m_bActivate = false;
        m_bExpire = false;
        PrivSpwnerResetWork();
        OnSpawnerReset();
	}

	internal void InterSpawnerPause()
	{
		if (m_bActivate)
		{
			m_bActivate = false;
			OnSpawnerPause();
		}
	}

	internal void InterSpawnerResume()
	{
		if (m_bActivate == false)
		{			
			m_bActivate = true;
			OnSpawnerResume();
		}
	}

	//---------------------------------------------------
    protected List<CStageSpawnWorkBase>.Enumerator IterStageSpawnerWorkList()
    {
        return SpawnWork.GetEnumerator();
    }

    protected void ProtSpawnerCloseSelf()
    {
        CManagerStageSpawnerBase.Instance.InterMgrStageSpawnerClose(GetStageSpawnerID());
    }
    //-----------------------------------------------------
    private void PrivSpwnerResetWork()
    {
        for (int i = 0; i < SpawnWork.Count; i++)
        {
            SpawnWork[i].InterSpawnWorkReset();
        }
    }

    //---------------------------------------------------
    protected virtual void OnSpawnerInitialize() { }
	protected virtual void OnSpawnerReset() { }
	protected virtual void OnSpawnerOpen() { }
    protected virtual void OnSpawnerClose() { }
	protected virtual void OnSpawnerPause() { }
	protected virtual void OnSpawnerResume() { }
	protected virtual void OnSpanwerUpdate(float fDelta) { }

    protected virtual void OnSpawnerWork(CStageSpawnWorkBase pSpawnedSpot) { }
}
