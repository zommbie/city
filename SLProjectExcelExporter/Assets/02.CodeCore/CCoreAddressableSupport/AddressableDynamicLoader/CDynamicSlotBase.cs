using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
// Addressable 기반의 동적 로드 시스템으로 메모리 풀을 거치지 않고 직접 로드하고 해재한다.
// 슬롯 개념으로 지속적으로 동적 객체가 로드되거나 해재된다.
public abstract class CDynamicSlotBase : CMonoBase
{
	private CDynamicSlotItemBase m_pItemLoaded = null;
	private CDynamicSlotItemBase m_pItemDefault = null;

	//------------------------------------------------------------------------
	protected override void OnUnityAwake()
	{
		m_pItemDefault = GetComponentInChildren<CDynamicSlotItemBase>();
		if (m_pItemDefault != null)
		{
			m_pItemLoaded = m_pItemDefault;
			OnDynamicSlotItemLoad(m_pItemLoaded);
		}
	}

	//------------------------------------------------------------------------
	protected void ProtDynamicSlotLoadItem(string strAddressableName, UnityAction delFinish)
	{
		if (m_pItemLoaded != null && m_pItemLoaded.GetDynamicLoadedItemName() == strAddressableName)
		{
			return;
		}

		if (m_pItemLoaded != null && m_pItemLoaded != m_pItemDefault)
		{
			PrivDynamicSlotRemoveItem(m_pItemLoaded);
			m_pItemLoaded = null;
		}

		Addressables.LoadAssetAsync<GameObject>(strAddressableName).Completed +=
			(AsyncOperationHandle<GameObject> pLoadedObjcet) => {
				CDynamicSlotItemBase pDynamicItem = pLoadedObjcet.Result.GetComponent<CDynamicSlotItemBase>();
				if (pDynamicItem != null)
				{
					PrivDynamicSlotAddItem(pDynamicItem);					
				}
				else
				{
					Addressables.ReleaseInstance(pLoadedObjcet);
					Destroy(pLoadedObjcet.Result);
				}
			};
	}

	protected void ProtDynamicSlotItemShow(CDynamicSlotItemBase pShowItem)
	{
		if (pShowItem == null)
		{
			PrivDynamicSlotRemoveItem(m_pItemLoaded);
			return;
		}

		if (pShowItem == m_pItemLoaded || pShowItem == m_pItemDefault)
		{
			pShowItem.InterDynamicLoadItemShow();
		}
		else 
		{			
			PrivDynamicSlotItemShow(pShowItem);
		}
	}

	//------------------------------------------------------------------------
	private void PrivDynamicSlotRemoveItem(CDynamicSlotItemBase pItem)
	{
		if (pItem == null) return;
		if (pItem == m_pItemDefault) return;

		pItem.gameObject.transform.parent = null;
		pItem.InterDynamicLoadItemRemove();
		Addressables.ReleaseInstance(pItem.gameObject);
		Destroy(pItem);
	}

	private void PrivDynamicSlotAddItem(CDynamicSlotItemBase pItem)
	{
		pItem.SetMonoActive(false);
		pItem.InterDynamicLoadItemLoadFinish();
		m_pItemLoaded = pItem;
		OnDynamicSlotItemLoad(pItem);
	}

	private void PrivDynamicSlotItemShow(CDynamicSlotItemBase pItem)
	{
		PrivDynamicSlotDefaultItemOnOff(false);
		PrivDynamicSlotRemoveItem(m_pItemLoaded);

		m_pItemLoaded = pItem;
		m_pItemLoaded.transform.SetParent(transform, false);
		m_pItemLoaded.transform.localPosition = Vector3.zero;
		m_pItemLoaded.InterDynamicLoadItemShow();
	}

	private void PrivDynamicSlotDefaultItemOnOff(bool bOn)
	{
		if (m_pItemDefault != null)
		{
			if (bOn)
			{
				m_pItemDefault.InterDynamicLoadItemShow();
			}
			else
			{
				m_pItemDefault.InterDynamicLoadItemHide();
			}
		}
	}

	//--------------------------------------------------------------------------
	protected virtual void OnDynamicSlotItemLoad(CDynamicSlotItemBase pItem) { }
	protected virtual void OnDynamicSlotItemRemove(CDynamicSlotItemBase pItem) { }


}
