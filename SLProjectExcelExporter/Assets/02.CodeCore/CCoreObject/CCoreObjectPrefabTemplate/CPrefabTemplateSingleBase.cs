using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CPrefabTemplateSingleBase : CPrefabTemplateBase
{
	[SerializeField]
	private CPrefabTemplateItemBase TemplateItem = null;
	[SerializeField]
	private int CloneCount = 3;

	//-------------------------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
		if (TemplateItem != null )
		{
			ProtPrefabTemplateAdd(0, TemplateItem, CloneCount);
		} 
	}

	//-------------------------------------------------------------------
	protected TEMPLATE ProtPrefabTemplateCloneInstanceSingle<TEMPLATE>(Transform pParent) where TEMPLATE : CPrefabTemplateItemBase
	{
		return ProtPrefabTemplateCloneInstance<TEMPLATE>(0, pParent);
	}

}
