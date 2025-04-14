using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public abstract class CUIButtonToggleBase : CUIButtonSingleBase
{
	[SerializeField][Header("[Button Toggle]")]
	private GameObject ToggleImageOn = null;
	[SerializeField]
	private GameObject ToggleImageOff = null;

	[SerializeField]
	private bool	FirstOn = true;

	[SerializeField]
	private UnityEvent ToggleEventOn = null;
	[SerializeField]
	private UnityEvent ToggleEventOff = null;

	protected bool m_bToggleOn = false;				public bool IsToggleOn() { return m_bToggleOn; }
	protected List<MaskableGraphic> m_listToggleImageOn = new List<MaskableGraphic>();
	protected List<MaskableGraphic> m_listToggleImageOff = new List<MaskableGraphic>();
	//----------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{ 
		if (ToggleImageOn != null) 
		{
			ToggleImageOn.GetComponentsInChildren(true, m_listToggleImageOn);
		}

		if (ToggleImageOff != null)
		{
			ToggleImageOff.GetComponentsInChildren(true, m_listToggleImageOff);
		}

		if (FirstOn)
		{
			PrivButtonToggleOn();
		}
		else
		{
			PrivButtonToggleOff();
		}

		base.OnUIEntryInitialize(pParentFrame);
	}

	protected override void OnUIEntryInitializePost(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitializePost(pParentFrame);
		
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();

		if (m_bToggleOn)
		{
			DoButtonToggleOff();
		}
		else
		{
			DoButtonToggleOn();
		}
	}

	//---------------------------------------------------------
	public void DoButtonToggleOn(bool bEventFire = true) // 관련 로직 만들때 무한 루프가 걸리는 경우가 많으니 조심할것
	{
		PrivButtonToggleOn();
		OnButtonToggleEventFire(true);
		if (bEventFire)
		{
			ToggleEventOn?.Invoke();
		}
        OnButtonToggleOn();
	}

	public void DoButtonToggleOff(bool bEventFire = true)
	{
		PrivButtonToggleOff();
		OnButtonToggleEventFire(false);
		if (bEventFire)
		{
			ToggleEventOff?.Invoke();
		}
        OnButtonToggleOff();
	}

	//----------------------------------------------------------
	private void PrivButtonToggleOn()
	{
		m_bToggleOn = true;
		PrivButtonToggleShowHide(m_listToggleImageOff, false);
		PrivButtonToggleShowHide(m_listToggleImageOn, true);
	}

	private void PrivButtonToggleOff()
	{
		m_bToggleOn = false;
		PrivButtonToggleShowHide(m_listToggleImageOn, false);
		PrivButtonToggleShowHide(m_listToggleImageOff, true);
	}

	private void PrivButtonToggleShowHide(List<MaskableGraphic> pListGraphic, bool bShow)
	{
		for (int i = 0; i < pListGraphic.Count; i++)
		{
			pListGraphic[i].enabled = bShow;           
		}
	}

	//---------------------------------------------------------

	protected virtual void OnButtonToggleOn() { }
	protected virtual void OnButtonToggleOff() { }
	protected virtual void OnButtonToggleEventFire(bool bOnEvent) { }
}
