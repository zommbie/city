using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public abstract class CTextMeshProBase : TextMeshPro
{
	private static StringBuilder g_TMPNote = new StringBuilder(1024);

	private const char c_TagOpen = '[';
	private const char c_TagClose = ']';

	public override string text { get { return base.text; } set { PrivTMPProcessingTag(value); } }


	
	//----------------------------------------------------
	private string PrivTMPProcessingTag(string strString)
	{
		return strString;
	}

	//-------------------------------------------------------
	
}
