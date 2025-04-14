using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 한번에 스폰하고 다음 스포너 호출
public abstract class CStageSpawnerOpenAndNextBase : CStageSpawnerBase
{   
    //-------------------------------------------------------
    protected override void OnSpawnerOpen()
    {
        List<CStageSpawnWorkBase>.Enumerator it = IterStageSpawnerWorkList();
        while(it.MoveNext())
        {
            it.Current.InterSpawnWorkStart(this);
            OnSpawnerWork(it.Current);
        }

        ProtSpawnerCloseSelf();               
    }


}
