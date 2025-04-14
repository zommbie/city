using UnityEngine;
using UnityEngine.Events;

// 무엇을 스폰할지 결정
// 테이블 등에 의해서 스폰 클레스와 레벨등이 변경될 수 있다. 
public abstract class CSpawnSpotBase : CMonoBase
{
    [SerializeField]
    private Vector3 SpawnDirection = Vector3.right;

    [SerializeField]
    private int SpawnSpotID = 0;            public int GetSpawnSpotID() { return SpawnSpotID; }
    //---------------------------------------------
    internal void InterSpawnSpotInitialize(UnityAction delFinish)
	{
		OnSpawnSpotInitialize(delFinish);
	}

    //----------------------------------------------
    public void DoSpawnSpotWork(CStageSpawnWorkBase pOwnSpawnWorker, int iSpawnIndex)
    {
        OnSpawnSpotWork(pOwnSpawnWorker, SpawnDirection, iSpawnIndex);
    }
    //------------------------------------------------
	protected virtual void OnSpawnSpotInitialize(UnityAction delFinish) { }
	protected virtual void OnSpawnSpotWork(CStageSpawnWorkBase pOwnSpawnWorker, Vector3 vecSpawnDirection, int iSpawnIndex) { }
}
