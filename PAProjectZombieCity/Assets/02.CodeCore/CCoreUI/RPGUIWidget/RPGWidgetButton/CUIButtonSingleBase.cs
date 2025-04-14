using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(CButton))]
public abstract class CUIButtonSingleBase : CUIWidgetBase
{
	private CButton m_pUGUIButton = null; protected CButton pButton { get { return m_pUGUIButton; } }
	private UnityEvent m_delButtonOnClick = null;
	//----------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		m_pUGUIButton = GetComponent<CButton>();
		m_delButtonOnClick = m_pUGUIButton.onClick; // 인스펙터에 입력된 델리케이터는 별도 보관
		m_pUGUIButton.onClick = new UnityEngine.UI.Button.ButtonClickedEvent(); // 델리게이터 리셋및 커스텀 연결
		m_pUGUIButton.onClick.AddListener(HandleButtonClick);        
	}

	//------------------------------------------------
	public void SetUIButtonInteraction(bool bEnable)
	{
		m_pUGUIButton.interactable = bEnable;
	}

	//-----------------------------------------------
	protected void ProtButtonActionPress()
	{
		m_pUGUIButton.DoButtonClick();
	} 

	protected void ProtButtonActionEvent()
	{
		m_delButtonOnClick.Invoke();
	}

	//----------------------------------------------
	private void HandleButtonClick()
	{
		OnButtonClick();
	}

	//------------------------------------------------
	protected virtual void OnButtonClick() { m_delButtonOnClick.Invoke(); }
	protected virtual void OnButtonPointUp() { }
}
