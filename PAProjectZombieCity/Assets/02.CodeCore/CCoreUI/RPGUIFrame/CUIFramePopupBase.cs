using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CUIFramePopupBase : CUIFrameWidgetBase
{
	public enum EPopupType
	{
		Confirm,
		Notify,
		Question,
		System,
		Warning,
		Error,		
	}

	[System.Serializable]
	public class SPopupTitlePreset
	{
		public EPopupType TitleType = EPopupType.Confirm;
		public List<GameObject> TitleObject;
	}

	private class SPopupContents
	{
		public EPopupType ePopupType;
		public string     strMessage;
		public UnityAction delAnswerNO;
		public UnityAction delAnswerYes;
	}
	
	[SerializeField]
	private CText TextMessage;
	[SerializeField]
	private List<SPopupTitlePreset> TitlePreset;

	private SPopupContents		  m_pPopupContentsCurrent = null;
	private Stack<SPopupContents> m_stackPopupContents = new Stack<SPopupContents>();

    //-----------------------------------------------------------------------------------
    protected override void OnUIFrameHide()
    {
        base.OnUIFrameHide();
        m_pPopupContentsCurrent = null;
        m_stackPopupContents.Clear();
    }
    //-----------------------------------------------------------------------------------
    protected void ProtUIPopupMessageStack(EPopupType ePopupTitleType, string strMessage, UnityAction delAnswerNO, UnityAction delAnswerYES)
	{
		SPopupContents pPopupContents = new SPopupContents();
		pPopupContents.ePopupType = ePopupTitleType;
		pPopupContents.strMessage = strMessage;
		pPopupContents.delAnswerNO = delAnswerNO;
		pPopupContents.delAnswerYes = delAnswerYES;

		if (m_pPopupContentsCurrent == null)
		{
			PrivUIPopupMessagePrint(pPopupContents);
		}
		else
		{
			m_stackPopupContents.Push(m_pPopupContentsCurrent);
			PrivUIPopupMessagePrint(pPopupContents);
		}
	}

	//----------------------------------------------------------------------------------

	private void PrivUIPopupMessageTitleObject(EPopupType ePopupTitleType)
	{
		SPopupTitlePreset pSelectedTitlePreset = null;
		for (int i = 0; i < TitlePreset.Count; i++)
		{
			SPopupTitlePreset pTitlePreset = TitlePreset[i];
			if (TitlePreset[i].TitleType == ePopupTitleType)
			{
				pSelectedTitlePreset = pTitlePreset;
			}
			else
			{
				for (int j = 0; j < pTitlePreset.TitleObject.Count; j++)
				{
					pTitlePreset.TitleObject[j].SetActive(false);
				}
			}
		}

		if (pSelectedTitlePreset != null)
		{
			for (int i = 0; i < pSelectedTitlePreset.TitleObject.Count; i++)
			{
				pSelectedTitlePreset.TitleObject[i].SetActive(true);
			}
		}
	}

	private void PrivUIPopupMessageText(string strMessage)
	{
		TextMessage.text = strMessage;
	}

	private void PrivUIPopupMessagePrint(SPopupContents pPopupContents)
	{
		m_pPopupContentsCurrent = pPopupContents;
		PrivUIPopupMessageTitleObject(pPopupContents.ePopupType);
		PrivUIPopupMessageText(pPopupContents.strMessage);
	}

	private void PrivUIPopupMessageNext()
	{
		if (m_stackPopupContents.Count == 0)
		{           
			DoUIFrameSelfHide();
		}
		else
		{
			SPopupContents pPopupContents = m_stackPopupContents.Pop();
			PrivUIPopupMessagePrint(pPopupContents);
			OnUIPopupMessageRefresh();
		}
	}

	//-----------------------------------------------------------------------------------
	protected void ProtUIPopupMessageAnswer(bool bAnswerYes)
	{
		if (m_pPopupContentsCurrent == null) return;
		if (bAnswerYes)
		{
			m_pPopupContentsCurrent.delAnswerYes?.Invoke();
		}
		else
		{
			m_pPopupContentsCurrent.delAnswerNO?.Invoke();
		}
		PrivUIPopupMessageNext();
	}	

	//-------------------------------------------------------------
	protected virtual void OnUIPopupMessageRefresh() { }
}
