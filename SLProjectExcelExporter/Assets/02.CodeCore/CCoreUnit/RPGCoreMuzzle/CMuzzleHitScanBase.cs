using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CMuzzleHitScanBase : CMuzzleBase
{

    // ToDo 레이케스트로 HitTo 이벤트 생성
    //--------------------------------------------------------------------


    //---------------------------------------------------------------------
    protected void ProMuzzleHitScanMount(CEffectBase pEffectBody)
    {
        pEffectBody.transform.SetParent(transform);
        pEffectBody.transform.localPosition = Vector3.zero;
    }
}
