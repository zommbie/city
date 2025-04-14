using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIButtonRadioBase : CUIButtonToggleBase
{
	private static CMultiDictionary<string, CUIButtonRadioBase> g_mapRadioGroup = new CMultiDictionary<string, CUIButtonRadioBase>();

	[SerializeField][Header("[Button Radio]")]
	private string RadioGroup;
	//--------------------------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
	}

	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		PrivButtonRadioRegist();
		base.OnUIEntryInitialize(pParentFrame);
	}

	protected override void OnButtonClick()
	{
		if (m_bToggleOn) return;

		base.OnButtonClick();
	}
     
	protected override void OnButtonToggleOn()
	{
		base.OnButtonToggleOn();
	}

	protected override void OnButtonToggleEventFire(bool bOnEvent)
	{
		base.OnButtonToggleEventFire(bOnEvent);
		if (bOnEvent)
		{
			PrivButtonRadioFocus();
		}
	}

	//-------------------------------------------------------------------
	internal void InterButtonRadioFocusOff()
	{
		DoButtonToggleOff();
	}

	//------------------------------------------------------------------
	private void PrivButtonRadioRegist()
	{
		if (RadioGroup == string.Empty) return;

		if (g_mapRadioGroup.ContainsKey(RadioGroup))
		{
			List<CUIButtonRadioBase> pList = g_mapRadioGroup[RadioGroup];
			if (pList.Contains(this) == false)
			{
				pList.Add(this);
			}
		}
		else
		{
			g_mapRadioGroup.Add(RadioGroup, this);
		}
	}

	private void PrivButtonRadioFocus()
	{
		if (RadioGroup == string.Empty) return;
		List<CUIButtonRadioBase> pListRadio = g_mapRadioGroup[RadioGroup];

		for (int i = 0; i < pListRadio.Count; i++)
		{
			if (pListRadio[i] != this)
			{
				pListRadio[i].InterButtonRadioFocusOff();
			}
		}
	}
}
