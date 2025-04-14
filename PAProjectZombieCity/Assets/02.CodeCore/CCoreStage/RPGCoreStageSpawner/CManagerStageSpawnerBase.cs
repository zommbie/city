using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
public abstract class CManagerStageSpawnerBase : CManagerTemplateBase<CManagerStageSpawnerBase>
{
    private CStageSpawnerBase m_pCurrentSpawner = null; protected CStageSpawnerBase GetMgrStageSpawnerCurrent() { return m_pCurrentSpawner; } 
    private SortedDictionary<int, CStageSpawnerBase> m_mapStageSpawnerInstance = new SortedDictionary<int, CStageSpawnerBase>();
    //----------------------------------------------------------------------------
    protected override void OnUnityAwake()
    {
        base.OnUnityAwake();
        PrivStageSpawnerCollectInstance();
    }

    //------------------------------------------------------------------------------
    protected void ProtMgrStageSpawnerStart(bool bReset = false)
    {
        if (bReset)
        {
            PrivStageSpawnerReset();
        }
        PrivStageSpawnerFirst();
        OnMgrStageSpawnerStart();
    }

    protected void ProtMgrStageSpawnerPause()
    {
    }

    protected void ProtMgrStageSpawnerResume()
    {
    }

    protected void ProtMgrStageSpawnerClear()
    {
        PrivMgrStageSpawnerClear();
    }

    protected void ProtMgrStageSpawnerPrepare(UnityAction delFinish)
    {
        PrivMgrStageSpawnerInitilize(delFinish);
    }

   

	//---------------------------------------------------------
	internal void InterMgrStageSpawnerRegist(CStageSpawnerBase pSpawner)
	{
		PrivStageSpawnerRegist(pSpawner);
	}

    internal void InterMgrStageSpawnerClose(int hSpawnerID)
    {
        if (m_mapStageSpawnerInstance.ContainsKey(hSpawnerID))
        {
            CStageSpawnerBase pStageSpawner = m_mapStageSpawnerInstance[hSpawnerID];
            PrivStageSpawnerClose(pStageSpawner);
        }
        else
        {
            //Error
        }
    }
    //----------------------------------------------------------------------------
    private void PrivStageSpawnerCollectInstance()
	{
		CStageSpawnerBase[] aSpawner = FindObjectsByType<CStageSpawnerBase>(FindObjectsSortMode.None);
		for (int i = 0; i < aSpawner.Length; i++)
		{
			PrivStageSpawnerRegist(aSpawner[i]);
		}
	}

	private void PrivStageSpawnerRegist(CStageSpawnerBase pSpawner)
	{
		int Order = pSpawner.GetStageSpawnerID();
		if (m_mapStageSpawnerInstance.ContainsKey(Order))
		{
            // Error!
			return;
		}

		m_mapStageSpawnerInstance.Add(Order, pSpawner);
	}

    private void PrivStageSpawnerOpen(CStageSpawnerBase pStageSpawner)
    {
        if (pStageSpawner.IsExpire) return;
        m_pCurrentSpawner = pStageSpawner;       
        m_pCurrentSpawner.InterSpawnerOpen();
        OnMgrStageSpawnerActivate(pStageSpawner);
    }

    private void PrivStageSpawnerOpen(int hSpawnerID)
    {
        if (m_mapStageSpawnerInstance.ContainsKey(hSpawnerID))
        {
            CStageSpawnerBase pStageSpawner = m_mapStageSpawnerInstance[hSpawnerID];
            PrivStageSpawnerOpen(pStageSpawner);
        }
        else
        {
            //Error
        }
    }

    private void PrivStageSpawnerClose(CStageSpawnerBase pStageSpawner)
    {
        pStageSpawner.InterSpawnerClose();
        int iNextSpawnID = pStageSpawner.GetStageSpawnerNextID();
        if (iNextSpawnID != 0)
        {
            PrivStageSpawnerOpen(iNextSpawnID);
        }
        else
        {
            PrivStageSpawnerEnd();
        }
    }

    private void PrivStageSpawnerEnd()
    {
        m_pCurrentSpawner = null;
        OnMgrStageSpawnerEnd();
    }

    private void PrivStageSpawnerReset()
	{		
		SortedDictionary<int, CStageSpawnerBase>.Enumerator it = m_mapStageSpawnerInstance.GetEnumerator();
		while(it.MoveNext())
		{
			it.Current.Value.InterSpawnerReset();
		}
		OnMgrStageSpawnerReset();
	}

	private void PrivStageSpawnerFirst()
	{
		SortedDictionary<int, CStageSpawnerBase>.Enumerator it = m_mapStageSpawnerInstance.GetEnumerator();
		if (it.MoveNext())
		{
            PrivStageSpawnerOpen(it.Current.Value);
		}  
	}

    private void PrivMgrStageSpawnerClear()
    {        
        m_mapStageSpawnerInstance.Clear();
    }

    private void PrivMgrStageSpawnerInitilize(UnityAction delFinish)
    {
        int iCountMax = m_mapStageSpawnerInstance.Count;
        int iCountCurrent = 0;
        SortedDictionary<int, CStageSpawnerBase>.Enumerator it = m_mapStageSpawnerInstance.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.InterSpawnerInitialize(() => {
                iCountCurrent++;
                if (iCountCurrent >= iCountMax)
                {
                    delFinish?.Invoke();
                }
            });
        }
    }

	//--------------------------------------------------------------------------------
	protected List<CStageSpawnerBase> ExtractStageSpawner()
	{
		return m_mapStageSpawnerInstance.Values.ToList();
	}

	//--------------------------------------------------------------------------------
	protected virtual void OnMgrStageSpawnerStart() { }
	protected virtual void OnMgrStageSpawnerReset() { }
	protected virtual void OnMgrStageSpawnerActivate(CStageSpawnerBase pSpawner) { }
	protected virtual void OnMgrStageSpawnerPause(CStageSpawnerBase pSpawner) { }
	protected virtual void OnMgrStageSpawnerResume(CStageSpawnerBase pSpawner) { }
    protected virtual void OnMgrStageSpawnerEnd() { }
}
