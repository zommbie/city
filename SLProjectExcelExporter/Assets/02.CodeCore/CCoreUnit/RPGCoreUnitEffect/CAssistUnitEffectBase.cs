using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitEffectBase : CAssistUnitBase
{

    private Dictionary<string, CEffectBase> m_mapUnitEffectInstance = new Dictionary<string, CEffectBase>();
    //---------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitBase pOwner)
    {
        base.OnAssistInitialize(pOwner);

        List<CEffectBase> pList = new List<CEffectBase>();
        GetComponentsInChildren(true, pList);

        for (int i = 0; i < pList.Count; i++)
        {
            m_mapUnitEffectInstance[pList[i].gameObject.name] = pList[i];
        }
    }


    //----------------------------------------------------------------
    public CEffectBase GetAssistUnitEffect(string strEffectName)
    {
        CEffectBase pFind = null;
        m_mapUnitEffectInstance.TryGetValue(strEffectName, out pFind);
        return pFind;
    }

    public void SetAssistUnitEffectDisable(string strEffectName)
    {
        if (m_mapUnitEffectInstance.ContainsKey(strEffectName))
        {
            CEffectBase pEffect = m_mapUnitEffectInstance[strEffectName];
            pEffect.DoEffectEnd();
            pEffect.transform.SetParent(transform);
        }
    }

}
