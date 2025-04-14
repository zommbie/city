using System.Collections.Generic;
using System.Xml;
using System.Reflection;

public abstract class CXmlElementParserBase 
{
	private List<CXmlElementParserBase> ChildXmlElementParser = new List<CXmlElementParserBase>();
	//------------------------------------------------------------
	public void DoScriptDataLoad(XmlElement pElem)
	{
		XmlAttributeCollection pAttList = pElem.Attributes;
		for (int i = 0; i < pAttList.Count; i++)
		{
			PrivXmlAttribute(pAttList[i]);
		}

		XmlNodeList pNodeList = pElem.ChildNodes;
		for (int i = 0; i < pNodeList.Count; i++)
		{
			XmlNode pNode = pNodeList[i];
			if (pNode.NodeType == XmlNodeType.Element)
			{
				PrivXmlElement(pNode as XmlElement, GetType().Namespace, Assembly.GetCallingAssembly());
			}
		}
	}

	//--------------------------------------------------------------
	private void PrivXmlAttribute(XmlAttribute pXmlAttribute)
	{
		CScriptDataBase.GlobalScriptDataReadField(this, pXmlAttribute.Name, pXmlAttribute.Value);
	}

	private void PrivXmlElement(XmlElement pElement, string strNameSpace, Assembly pAssembly)
	{
		string strClassName = string.Format("{0}.{1}", strNameSpace, pElement.Name);
		CXmlElementParserBase pChildLoader = pAssembly.CreateInstance(strClassName) as CXmlElementParserBase;
		if (pChildLoader == null)
		{			
			return;
		}

		ChildXmlElementParser.Add(pChildLoader);
		pChildLoader.DoScriptDataLoad(pElement);
	}

	//------------------------------------------------------------------------------------
	

}
