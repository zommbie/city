using UnityEngine;
using UnityEngine.Events;

public class ZCManagerEffect : CManagerEffectBase
{   public static new ZCManagerEffect Instance { get { return CManagerEffectBase.Instance as ZCManagerEffect; } }



    //--------------------------------------------------------
    protected override void OnMgrEffectPreLoadWork(UnityAction delFinish)
    {
        delFinish?.Invoke();
    }

    //----------------------------------------------------------
}
