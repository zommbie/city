using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 로드되는 아이템으로 로드 피니시등의 이벤트를 받는다.
public abstract class CDynamicSlotItemBase : CMonoBase
{
	[SerializeField]
	private Vector3 AttachOffset = Vector3.zero;   internal Vector3 p_AttachOffset { get { return AttachOffset; } }
	//--------------------------------------------------------------------
	internal void InterDynamicLoadItemLoadFinish()
	{
		RemoveCloneObjectName(gameObject);
		OnDynamicLoadFinish();
	}

	internal void InterDynamicLoadItemRemove()
	{
		OnDynamicRemove();
	}

	internal void InterDynamicLoadItemShow()
	{
		if (gameObject.activeSelf == false)
		{
			SetMonoActive(true);
			OnDynamicShow();
		}
	}

	internal void InterDynamicLoadItemHide()
	{
		if (gameObject.activeSelf == true)
		{
			SetMonoActive(false);
			OnDynamicHide();
		}
	}
	//------------------------------------------------------------------
	public string GetDynamicLoadedItemName()
	{
		return gameObject.name;
	}

	//--------------------------------------------------------------------
	protected virtual void OnDynamicLoadFinish() { }
	protected virtual void OnDynamicRemove() { }
	protected virtual void OnDynamicShow() { }
	protected virtual void OnDynamicHide() { }
}
