using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

abstract public class CScriptDataXMLBase : CScriptDataBase
{
	//-------------------------------------------------------------
	protected override sealed void OnScriptDataLoad(string strTextData)
	{
		XmlDocument pXmlDocument = new XmlDocument();
		pXmlDocument.LoadXml(strTextData);
		OnScriptXMLRoot(pXmlDocument.DocumentElement);
	}

	//--------------------------------------------------------------
	protected virtual void OnScriptXMLRoot(XmlElement pRootElem) { }
}
