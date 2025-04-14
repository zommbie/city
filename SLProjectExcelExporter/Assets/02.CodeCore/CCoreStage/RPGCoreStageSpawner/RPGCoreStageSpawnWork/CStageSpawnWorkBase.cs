using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// 어떤 방식으로 스폰할지 결정

public abstract class CStageSpawnWorkBase : CMonoBase
{
    [SerializeField]
    private List<CSpawnSpotBase> SpawnSpot = null;

    private bool m_bSpawnWorkUpdate = false;
    private bool m_bSpawnWorkFinish = false;   public bool IsWorkFinish { get { return m_bSpawnWorkFinish; } }   // 모든 작업을 마첬을 경우
    //----------------------------------------------------------------
    internal void InterSpawnWorkInitilize(UnityAction delFinish)
    {
        int iSpotCount = 0;
        for (int i = 0; i < SpawnSpot.Count; i++)
        {
            SpawnSpot[i].InterSpawnSpotInitialize(() => {
                iSpotCount++;
                if (iSpotCount >= SpawnSpot.Count)
                {
                    delFinish?.Invoke();
                    OnSpawnWorkEnd();
                }
            });
        }
        OnSpawnWorkInitialize();
    }

    internal void InterSpawnWorkUpdate(float fDelta)
    {
        if (m_bSpawnWorkUpdate == false) return;
        if (gameObject.activeSelf == false) return;

        OnSpawnWorkUpdate(fDelta);
    }

    internal void InterSpawnWorkStart(CStageSpawnerBase pOwnSpawner)
    {
        m_bSpawnWorkUpdate = true;
        OnSpawnWorkStart(pOwnSpawner);
    }

    internal void InterSpawnWorkStop()
    {
        m_bSpawnWorkUpdate = false;
        OnSpawnWorkStop();
    }

    internal void InterSpawnWorkReset()
    {
        OnSpawnWorkReset();
    }

    //--------------------------------------------------------------
    protected List<CSpawnSpotBase>.Enumerator IterSpawnSpot()
    {
        return SpawnSpot.GetEnumerator();
    }

    protected void ProtStageSpawnWorkFinish()
    {
        m_bSpawnWorkFinish = true;
    }

    protected CSpawnSpotBase GetStageSpawnSpot(int iIndex)
    {
        CSpawnSpotBase pSpot = null;
        if (iIndex < SpawnSpot.Count)
        {
            pSpot = SpawnSpot[iIndex];
        }
        return pSpot;
    }

    //----------------------------------------------------------------
    protected virtual void OnSpawnWorkInitialize() { }
    protected virtual void OnSpawnWorkUpdate(float fDelta) { }
    protected virtual void OnSpawnWorkStart(CStageSpawnerBase pOwnSpawner) { }
    protected virtual void OnSpawnWorkEnd() { }
    protected virtual void OnSpawnWorkStop() { }
    protected virtual void OnSpawnWorkReset() { }
    
}
