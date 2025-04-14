using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 케릭터 귀속 이펙트가 아닌 공용 이펙트 관리 로직

public abstract class CManagerEffectBase : CManagerTemplateBase<CManagerEffectBase>
{
	
	private CLinkedList<CEffectBase> m_listActiveEffectInstance = new CLinkedList<CEffectBase>();
	//-------------------------------------------------------------
	protected override void OnUnityUpdate()
	{
		base.OnUnityUpdate();
        UpdateMgrEffectExpire();
    }
    //---------------------------------------------------------------   
    protected void ProtMgrEffectRegist(CEffectBase pEffect)
	{
        m_listActiveEffectInstance.AddLast(pEffect);
        pEffect.InterEffectInitialize();
	}

    //---------------------------------------------------------------
    private void UpdateMgrEffectExpire()
    {
        CLinkedList<CEffectBase>.Enumerator it = m_listActiveEffectInstance.GetEnumerator();
        while(it.MoveNext())
        {
            if (it.Current.IsActive == false)
            {
                PrivMgrEffectExpire(it.Current);
                it.Remove();
            }
        }
    }
     
    private void PrivMgrEffectExpire(CEffectBase pEffect)
    {       
        OnEffectExpire(pEffect);
    }


    //----------------------------------------------------------------
    protected virtual void OnEffectExpire(CEffectBase pEffect) {}
}
