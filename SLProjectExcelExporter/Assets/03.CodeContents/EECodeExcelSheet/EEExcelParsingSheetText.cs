using System.Collections;
using System.Collections.Generic;
using System.Data;

public class EEExcelParsingSheetText : EEExcelParsingSheetBase
{
	private const string c_TextCategoryColumnName		= "TextCategory";
	private const string c_TextKeyColumnName			= "TextKey";
	private const string c_TextCategoryDefault			= "DefaultCategory";

	private class STextContents
	{
		public string TextKey;
		public string TextString;
	}

	private class STextLanguage
	{
		public string							 Language;
		public Dictionary<string, STextContents> TexContents = new Dictionary<string, STextContents>();
	}

	private class STextCategory
	{
		public string CategoryName;
		public Dictionary<string, STextLanguage> TextLanguage = new Dictionary<string, STextLanguage>();
	}

	private STextCategory m_pTextCategory = new STextCategory();
	private Dictionary<string, STextCategory> m_mapTextCategory = new Dictionary<string, STextCategory>();
	//--------------------------------------------------------------------------
	protected override void OnParsingSheetReset()
	{
		m_mapTextCategory.Clear();
		m_pTextCategory = new STextCategory();
		m_pTextCategory.CategoryName = c_TextCategoryDefault;
		m_mapTextCategory[c_TextCategoryDefault] = m_pTextCategory;
	}

	protected override SErrorInfo OnParsingSheetLoad(DataTable pDataTable, params object[] aParams)
	{
		SErrorInfo rError = new SErrorInfo();

		List<string> listHeader = new List<string>();
		for(int i = 0; i < pDataTable.Rows.Count; i++)
		{
			DataRow pDataRow = pDataTable.Rows[i];
			if (i == 0)
			{
				rError = PrivParsingSheetTextLoadHeader(pDataRow, listHeader);
			}
			else
			{
				rError = PrivParsingSheetTextLoadContents(pDataRow, listHeader);
			}

			if (rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}

		return rError;
	}
	//-------------------------------------------------------------------------------------
	public string GetParsingSheetText(string strKeyName, string strCategory = c_TextCategoryDefault) 
	{
		string strResult = string.Empty;


		return strResult;
	}

	//------------------------------------------------------------------------------------
	private SErrorInfo PrivParsingSheetTextLoadHeader(DataRow pDataRow, List<string> listHeader)
	{
		SErrorInfo rError = new SErrorInfo();
		for (int i = 0; i < pDataRow.ItemArray.Length; i++)
		{
			string strCell = EEStringUtility.ConvertObjectToString(pDataRow.ItemArray[i]);
			if (strCell == string.Empty)
			{
				rError.ErrorType = EEEErrorCategory.EnumField;
				rError.ErrorMessage = "[Enum Field] Header contents has null";
				break;
			}

			listHeader.Add(strCell);			
		}

		return rError;
	}

	private SErrorInfo PrivParsingSheetTextLoadContents(DataRow pDataRow, List<string> listHeader)
	{
		SErrorInfo rError = new SErrorInfo();

		int iTotalItem = pDataRow.ItemArray.Length;

		if (iTotalItem != listHeader.Count)
		{
			rError.ErrorType = EEEErrorCategory.TextField;
			rError.ErrorMessage = "[TextField] Header Count miss match";
			return rError;
		}

		string strTextKey = string.Empty;

		for (int i = 0; i < iTotalItem; i++)
		{
			string strCell = EEStringUtility.ConvertObjectToString(pDataRow.ItemArray[i]);			
			
			if (listHeader[i] == c_TextCategoryColumnName)
			{
				if (strCell == string.Empty)
				{
					continue;
				}
				else
				{
					PrivParsingSheetTextCategory(strCell);
				}
			}
			else if (listHeader[i] == c_TextKeyColumnName)
			{
				strTextKey = strCell;
			}
			else
			{
				if (strTextKey == string.Empty)
				{
					rError.ErrorType = EEEErrorCategory.TextField;
					rError.ErrorMessage = "[TextField] Invalid Text Key";
					return rError;
				}

				rError = PrivParsingSheetTextContents(strTextKey, strCell, listHeader[i]);
			}		

			if (rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}

		return rError;
	}

	private void PrivParsingSheetTextCategory(string strCategoryName)
	{
		if (m_mapTextCategory.ContainsKey(strCategoryName))
		{
			m_pTextCategory = m_mapTextCategory[strCategoryName];
		}
		else
		{
			m_pTextCategory = new STextCategory();
			m_pTextCategory.CategoryName = strCategoryName;
			m_mapTextCategory[strCategoryName] = m_pTextCategory;
		}
	}

	private SErrorInfo PrivParsingSheetTextContents(string strKey, string strContents, string strLanguage)
	{
		SErrorInfo rError = new SErrorInfo();
		STextLanguage pTextLanguage = null;
		if (m_pTextCategory.TextLanguage.ContainsKey(strLanguage))
		{
			pTextLanguage = m_pTextCategory.TextLanguage[strLanguage];
		}
		else
		{
			pTextLanguage = new STextLanguage();
			pTextLanguage.Language = strLanguage;
			m_pTextCategory.TextLanguage[strLanguage] = pTextLanguage;
		}

		if (pTextLanguage.TexContents.ContainsKey(strKey))
		{
			rError.ErrorType = EEEErrorCategory.TextField;
			rError.ErrorMessage = $"[TextField] Duplicated Text Key : {strKey}";
		}
		else
		{
			STextContents pTextContents = new STextContents();
			pTextContents.TextKey = strKey;
			pTextContents.TextString = strContents;
			pTextLanguage.TexContents[strKey] = pTextContents;
		}

		return rError;
	}

}
