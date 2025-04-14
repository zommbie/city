using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitActorEffectBase : CAssistUnitActorBase
{
    [System.Serializable]
    public class SActorEffectRegistInfo
    {
        public int CloneCount = 1;
        public CEffectBase EffectOrigin;
    }
    [SerializeField]
    private List<SActorEffectRegistInfo> ActorEffectRegist = new List<SActorEffectRegistInfo>();
    //----------------------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitActorBase pOwner)
    {
        base.OnAssistInitialize(pOwner);       
        PrivUnitActorEffectToManagerRegist();
    }

    protected override void OnAssistUnitActorRemove()
    {
        base.OnAssistUnitActorRemove();
        PrivUnitActorEffectToManagerUnRegist();
    }

    //---------------------------------------------------------------------------------


    private void PrivUnitActorEffectToManagerRegist()
    {
        if (CManagerEffectBase.Instance == null) return;

        for (int i = 0; i < ActorEffectRegist.Count; i++)
        {
            CManagerEffectBase.Instance.InterMgrEffectLocalRegist(ActorEffectRegist[i].EffectOrigin, ActorEffectRegist[i].CloneCount);
        }
    }

    private void PrivUnitActorEffectToManagerUnRegist()
    {
        if (CManagerEffectBase.Instance == null) return;

        for (int i = 0; i < ActorEffectRegist.Count; i++)
        {
            CManagerEffectBase.Instance.InterMgrEffectLocalUnRegist(ActorEffectRegist[i].EffectOrigin);
        }
    }

}
