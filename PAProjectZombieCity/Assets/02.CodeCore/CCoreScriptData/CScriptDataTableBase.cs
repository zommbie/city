using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;
// 엑셀 CSV - UTF8 포멧 기반
abstract public class CScriptDataTableBase : CScriptDataBase
{	
	private List<Dictionary<string, List<string>>> m_listTableData = new List<Dictionary<string, List<string>>>();
	//---------------------------------------------------------
	protected override sealed void OnScriptDataLoad(string strTextData)
	{
		m_listTableData.AddRange(CSVReader.Read(strTextData));
	}

    protected sealed override void OnScriptDataLoadFinish()
    {
        OnScriptDataTableCSVParseComplete();
    }

    //---------------------------------------------------------
    protected List<INSTANCE> ProtDataTableRead<INSTANCE>() where INSTANCE : class, new()
	{
		List<INSTANCE> pListInstance = new List<INSTANCE>();

		for (int i = 0; i < m_listTableData.Count; i++)
		{
			INSTANCE pInstance = new INSTANCE();
			Dictionary<string, List<string>>.Enumerator it = m_listTableData[i].GetEnumerator();
			while(it.MoveNext())
			{
				List<string> pListValue = it.Current.Value;
				for (int j = 0; j < pListValue.Count; j++)
				{
					GlobalScriptDataReadField(pInstance, it.Current.Key, pListValue[j]);
				}
			}
			pListInstance.Add(pInstance);
		}

		return pListInstance;
	}

	//---------------------------------------------------------------
	protected virtual void OnScriptDataTableCSVParseComplete() { }
}

//static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";


public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r";
	static char[] TRIM_CHARS = { '\"' };

	public static List<Dictionary<string, List<string>>> Read(string strTextAsset)
	{
		var list = new List<Dictionary<string, List<string>>>();

		var lines = Regex.Split(strTextAsset, LINE_SPLIT_RE);

		if (lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++)
		{
			var values = Regex.Split(lines[i], SPLIT_RE);

			var entry = new Dictionary<string, List<string>>();
			for (var j = 0; j < header.Length && j < values.Length; j++)
			{
				string strValue = values[j];
				string strHeaderName = header[j];
				strValue = strValue.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);

				List<string> pListValue = null;
				if (entry.ContainsKey(strHeaderName))
				{
					pListValue = entry[strHeaderName];
				}
				else
				{
					pListValue = new List<string>();
					entry[strHeaderName] = pListValue;
				}

				pListValue.Add(strValue);
			}
			list.Add(entry);
		}
		return list;
	}
}