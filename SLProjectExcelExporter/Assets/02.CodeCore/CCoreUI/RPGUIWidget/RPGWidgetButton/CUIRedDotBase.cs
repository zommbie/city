using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIRedDotBase : CUIWidgetBase
{
	private class SUIRedDotGroup
	{
		public string		 RedDotName;
		public CUIRedDotBase RedDotInstance;
	}
	private static Dictionary<string, SUIRedDotGroup> g_RedDotInfo = new Dictionary<string, SUIRedDotGroup>();
	//--------------------------------------------------------------------------------

	[SerializeField]
	private string RedDotName = string.Empty;
	[SerializeField]
	private string LinkedName = string.Empty;


	private bool m_bShowRedDot = false;               public bool IsShowRedDot { get { return m_bShowRedDot; } }
	private CUIRedDotBase m_pLinkedRedDot = null;  
	//---------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);

		if (g_RedDotInfo.ContainsKey(RedDotName))
		{
			Debug.LogErrorFormat("[UIRedDot] RedDotName must be unique : %s in %s", RedDotName, gameObject.name);
		}
		else
		{
			SUIRedDotGroup pNewRedDot = new SUIRedDotGroup();
			pNewRedDot.RedDotName = RedDotName;
			pNewRedDot.RedDotInstance = this;
			g_RedDotInfo[RedDotName] = pNewRedDot;
		}
	}

	protected override void OnUIEntryInitializePost(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitializePost(pParentFrame);

		if (g_RedDotInfo.ContainsKey(LinkedName))
		{
			m_pLinkedRedDot = g_RedDotInfo[LinkedName].RedDotInstance;
		}
	}

	//------------------------------------------------------------
	public void DoRedDotRefresh()
	{

	}



	//---------------------------------------------------------------
	protected virtual void OnUIRedDotRefresh() { }

}
