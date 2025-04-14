using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오픈 이후 계속해서 열려 있음
public abstract class CStageSpawnerOpenAllBase : CStageSpawnerBase
{

    //-------------------------------------------------------------
    protected override void OnSpawnerOpen()
    {
        base.OnSpawnerOpen();
        PrivSpawnerOpenAll();
    }

    protected override void OnSpanwerUpdate(float fDelta)
    {
        base.OnSpanwerUpdate(fDelta);
        UpdateSpawner(fDelta);
    }


    //------------------------------------------------------------
    private void UpdateSpawner(float fDelta)
    {
        List<CStageSpawnWorkBase>.Enumerator it = IterStageSpawnerWorkList();
        while(it.MoveNext())
        {
            it.Current.InterSpawnWorkUpdate(fDelta);
        }
    }

    private void PrivSpawnerOpenAll()
    {
        List<CStageSpawnWorkBase>.Enumerator it = IterStageSpawnerWorkList();
        while (it.MoveNext())
        {
            it.Current.InterSpawnWorkStart(this);
        }
    }
}
