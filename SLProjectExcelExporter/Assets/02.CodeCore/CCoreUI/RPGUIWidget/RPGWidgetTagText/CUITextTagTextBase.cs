using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
[RequireComponent(typeof(CText))]
public abstract class CUITextTagTextBase : CUIWidgetBase
{
	[SerializeField]
	private char TagOpen = '[';
	[SerializeField]
	private char TagClose = ']';

	private CText m_pUGUIText = null;
	private StringBuilder m_pNoteExport = new StringBuilder(256);
    private StringBuilder m_pNoteOrigin = new StringBuilder(1024);
	
	public string text { set { m_pUGUIText.text = value; } get { return m_pUGUIText.text; } }
	//----------------------------------------------------------------
	protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
	{
		base.OnUIEntryInitialize(pParentFrame);
		m_pUGUIText = GetComponent<CText>();
		m_pUGUIText.SetTextReference(HandleTagText);
	}

	//-----------------------------------------------------------------
	public string HandleTagText(string strText)
	{
		return PrivTagTextParse(strText);
	}
	//--------------------------------------------------------------------
	private string PrivTagTextParse(string strText)
	{
		m_pNoteExport.Clear();
        m_pNoteOrigin.Clear();

        m_pNoteOrigin.Append(strText);
		
		bool bTagOpen = false;
		int iOpneStart = 0;
		int iOpenEnd = 0;
		int iIndex = 0;
		for (int i = 0; i < m_pNoteOrigin.Length; i++)
		{
			if (bTagOpen)
			{
				if (m_pNoteOrigin[i] == TagClose)
				{
					if (i + 1 < m_pNoteOrigin.Length)
					{
						if (m_pNoteOrigin[i+1] == TagClose)
						{
							bTagOpen = false;
							iOpenEnd = i + 2;
							string strReplace = OnUITagText(m_pNoteExport.ToString(), iIndex);
							m_pNoteExport.Clear();
                            m_pNoteOrigin = m_pNoteOrigin.Remove(iOpneStart, iOpenEnd - iOpneStart);
                            m_pNoteOrigin = m_pNoteOrigin.Insert(iOpneStart, strReplace);             
							i = iOpneStart;
							iIndex++;
						}
						else
						{
							m_pNoteExport.Append(m_pNoteOrigin[i]);
						}
					}
					else
					{
						m_pNoteExport.Append(m_pNoteOrigin[i]);
					}
				}
				else
				{
					m_pNoteExport.Append(m_pNoteOrigin[i]);
				}				
			}
			else
			{
				if (m_pNoteOrigin[i] == TagOpen)
				{
					if (i + 1 < m_pNoteOrigin.Length)
					{
						if (m_pNoteOrigin[i + 1] == TagOpen)
						{
							bTagOpen = true;
							iOpneStart = i;
							i++;
						}
					}
				}
			}
		}
		return m_pNoteOrigin.ToString();
	}

	protected virtual string OnUITagText(string strContents, int iIndex) { return string.Empty; }
}
