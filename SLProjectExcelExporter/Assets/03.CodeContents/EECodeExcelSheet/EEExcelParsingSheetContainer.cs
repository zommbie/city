using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;


public class EEExcelParsingSheetContainer
{
	public enum EExcelSheetType
	{
		None,
		Enumeration,
		LocalizeText,
		TableData,
	}

	private const string c_SheetEnumName			= "(Enum)";
	private const string c_SheetEnumNameSystem		= "(EnumSystem)";
	private const string c_SheetEnumNameContents	= "(EnumContents)";
	private const string c_SheetTextName			= "(Text)";

	private EEExcelParsingSheetEnum m_pSheetEnum = new EEExcelParsingSheetEnum();
	private EEExcelParsingSheetText m_pSheetText = new EEExcelParsingSheetText();  
	private Dictionary<string, EEExcelParsingSheetTable> m_mapExcelSheetTable = new Dictionary<string, EEExcelParsingSheetTable>();
    //--------------------------------------------------------------------------------------
    public void DoSheetContainerReset()
	{
		m_pSheetEnum.DoParsingSheetReset();
		m_pSheetText.DoParsingSheetReset();
        m_mapExcelSheetTable.Clear();
	}

	public void DoSheetContainerLoad(DataTableCollection pExcelSheetList, EExcelSheetType eExcelSheetType, ref SErrorInfo rError)
	{
		for (int i = 0; i < pExcelSheetList.Count; i++)
		{
			DataTable pSheetTable = pExcelSheetList[i];
			string strSheetName = pSheetTable.TableName;

			if (strSheetName.Contains(c_SheetEnumName))
			{
				if (eExcelSheetType == EExcelSheetType.Enumeration)
				{
					rError = m_pSheetEnum.DoParsingSheetLoad(pSheetTable);
				}
			}
			else if (strSheetName.Contains(c_SheetTextName))
			{
				if (eExcelSheetType == EExcelSheetType.LocalizeText)
				{
					rError = m_pSheetText.DoParsingSheetLoad(pSheetTable);
				}
			}
			else
			{
				if (eExcelSheetType == EExcelSheetType.TableData)
				{
					rError = PrivSheetContainerLoadSheetData(pSheetTable);
				}
			}

			if (rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}		
	}

	public void DoSheetContainerCompile(SErrorContainer pErrorContainer)
	{		
		SSheetReference pSheetReference = new SSheetReference();
		pSheetReference.SheetEnum  = m_pSheetEnum;
		pSheetReference.SheetText  = m_pSheetText;
		pSheetReference.SheetTable = m_mapExcelSheetTable;
		
		Dictionary<string, EEExcelParsingSheetTable>.Enumerator it = m_mapExcelSheetTable.GetEnumerator();
		while(it.MoveNext())
		{
			it.Current.Value.DoParsingSheetCompile(pSheetReference, pErrorContainer);
		}
		return;
	}

	public void ExtractContainerTableList(out List<EEExcelParsingSheetTable> pListOutTable)
	{
		pListOutTable = m_mapExcelSheetTable.Values.ToList();
	}

	public SEnumData GetSheetContainerEnumData(string strEnumName)
	{
		return m_pSheetEnum.GetSheetEnumData(strEnumName);
	}
	
	//------------------------------------------------------------------------
	private SErrorInfo PrivSheetContainerLoadSheetData(DataTable pSheetTable)
	{
		string strSheetName = pSheetTable.TableName;

		EEExcelParsingSheetTable pParsingData = null;
		if (m_mapExcelSheetTable.ContainsKey(strSheetName))
		{
			pParsingData = m_mapExcelSheetTable[strSheetName];
		}
		else
		{
			pParsingData = new EEExcelParsingSheetTable();
			m_mapExcelSheetTable[strSheetName] = pParsingData;
		}
		return pParsingData.DoParsingSheetLoad(pSheetTable);
	}
}
