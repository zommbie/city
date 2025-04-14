using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class EEJsonExporter 
{
	private const string c_ColumnKeyName = "Key";
	private const string c_RootTableName = "RootTable";
	//--------------------------------------------------------------------------------
	public string DoJsonExport(EEExcelParsingSheetTable pSheetTable, EExportType eExportType)
	{
		JObject pRootSheet = new JObject();
		JArray pRootTableArray = new JArray();

		pRootSheet.Add(pSheetTable.GetParsingSheetName(), pRootTableArray);

		List<STableData>.Enumerator it = pSheetTable.ExtractParsingSheetTableData();

		while(it.MoveNext())
		{
			JObject pRootTable = new JObject();
			pRootTableArray.Add(pRootTable);
			STableData pTableData = it.Current;

			PrivJsonExporterFilterExportType(pTableData, eExportType);
			PrivJsonExporterWork(pTableData, pRootTable);
		}
		return pRootSheet.ToString();
	}

	//--------------------------------------------------------------------------------
	private void PrivJsonExporterFilterExportType(STableData pTableData, EExportType eExportType)
	{
		for (int i = 0; i < pTableData.TableItemList.Count; i++)
		{
			STableDataContents pContents = pTableData.TableItemList[i];
			if (pContents == null) continue;

			EExportType eCompare = pContents.DataHeader.ExportType;

			if (eCompare != EExportType.Both && eCompare != eExportType)
			{
				pContents.IsExport = false;
			}
			else
			{
				pContents.IsExport = true;
			}
		}
	}

	private void PrivJsonExporterWork(STableData pTableData, JObject pNote)
	{
		pNote.Add(c_ColumnKeyName, pTableData.TableKey);
		RecursiveJsonExportWork(pTableData.TableItemList, pNote, "");
	}

	private void RecursiveJsonExportWork(List<STableDataContents> pListTableItem, JObject pNoteObject, string strParentDataName)
	{
		for (int i = 0; i < pListTableItem.Count; i++)
		{
			STableDataContents pContents = pListTableItem[i];
			if (pContents == null) continue;
			if (pContents.IsExport == false) continue;

			if (pContents.DataArrayChild.Count == 0)
			{
				PrivJsonExportReadData(pContents, pNoteObject);
			}
			else if (pContents.DataArrayChild.Count == 1)
			{
				RecursiveJsonExportWork(pContents.DataArrayChild, pNoteObject, pContents.DataName);
			}
			else
			{
				JArray pValueArray = new JArray();
				pNoteObject.Add(pContents.DataName, pValueArray);
				RecursiveJsonExportWork(pContents.DataArrayChild, pValueArray, pContents.DataName);
			}
		}
	}

	private void RecursiveJsonExportWork(List<STableDataContents> pListTableItem, JArray pNoteArray, string strParentDataName)
	{
		for (int i = 0; i < pListTableItem.Count; i++)
		{
			STableDataContents pContents = pListTableItem[i];
			if (pContents == null) continue;
			if (pContents.IsExport == false) continue;

			if (pContents.DataArrayChild.Count == 0)
			{
				if (strParentDataName != pContents.DataName)
				{
					JObject pValueObject = new JObject();
					pNoteArray.Add(pValueObject);
					PrivJsonExportReadData(pContents, pValueObject);
				}
				else
				{
					PrivJsonExportReadData(pContents, pNoteArray);
				}
			}
			else if (pContents.DataArrayChild.Count == 1)
			{
				if (pContents.IsArrayNote)
				{
					PrivJsonExportWorkArrayRead(pContents.DataArrayChild, pNoteArray, pContents.DataName);
				}
				else
				{
					RecursiveJsonExportWork(pContents.DataArrayChild, pNoteArray, pContents.DataName);
				}
			}
			else
			{
				if (pContents.IsArrayNote)
				{
					PrivJsonExportWorkArrayRead(pContents.DataArrayChild, pNoteArray, pContents.DataName);
				}
				else
				{
					JObject pValueObject = new JObject();
					JArray pValueArray = new JArray();
					pValueObject.Add(pContents.DataName, pValueArray);
					pNoteArray.Add(pValueObject);
					RecursiveJsonExportWork(pContents.DataArrayChild, pValueArray, pContents.DataName);
				}
			}
		}
	}

	private void PrivJsonExportWorkArrayRead(List<STableDataContents> pListTableItem, JArray pNoteArray, string strParentDataName)
	{		
		for (int i = 0; i < pListTableItem.Count; i++)
		{
			STableDataContents pContents = pListTableItem[i];
			if (pContents == null) continue;
			if (pContents.IsExport == false) continue;

			if (pNoteArray.Count <= i)
			{
				pNoteArray.Add(new JObject());
			}
			JObject pArrayItem = pNoteArray[i] as JObject;
			PrivJsonExportReadDataKeyValue(pContents, pArrayItem);
		}
	}


	private void PrivJsonExportReadData(STableDataContents pContents, JContainer pJContainer)
	{
		if (pJContainer is JObject)
		{
			PrivJsonExportReadDataKeyValue(pContents, pJContainer as JObject);
		}
		else if (pJContainer is JArray)
		{
			PrivJsonExportReadDataArrayValue(pContents, pJContainer as JArray);
		}
	}


	private void PrivJsonExportReadDataKeyValue(STableDataContents pContents, JObject pNote)
	{
		switch(pContents.DataHeader.ValueType)
		{
			case EValueType.Text:				
				pNote.Add(pContents.DataName, pContents.DataValue.DataText);
				break;
			case EValueType.String:					
				pNote.Add(pContents.DataName, pContents.DataValue.DataString);
				break;
			case EValueType.Key:
				pNote.Add(pContents.DataName, pContents.DataValue.DataKey);
				break;
			case EValueType.Enum:				
				pNote.Add(pContents.DataValue.DataEnumData.EnumName, pContents.DataValue.DataEnumString);
				break;
			case EValueType.Bool:
				pNote.Add(pContents.DataName, pContents.DataValue.DataBoolean);
				break;
			case EValueType.Number:
				pNote.Add(pContents.DataName, pContents.DataValue.DataNumber);
				break;
			case EValueType.NumberBig:
                pNote.Add(pContents.DataName, pContents.DataValue.DataNumberBig);
				break;
			case EValueType.Float:
				pNote.Add(pContents.DataName, pContents.DataValue.DataFloat);
				break;
        }
    }

	private void PrivJsonExportReadDataArrayValue(STableDataContents pContents, JArray pArrayNote)
	{
		switch (pContents.DataHeader.ValueType)
		{
			case EValueType.Text:
				pArrayNote.Add(pContents.DataValue.DataText);
				break;
			case EValueType.String:
				pArrayNote.Add(pContents.DataValue.DataString);
				break;
			case EValueType.Key:
				pArrayNote.Add(pContents.DataValue.DataKey);
				break;
			case EValueType.Enum:
				pArrayNote.Add(pContents.DataValue.DataEnumString);
				break;
			case EValueType.Bool:
				pArrayNote.Add(pContents.DataValue.DataBoolean.ToString());
				break;
			case EValueType.Number:
				pArrayNote.Add(pContents.DataValue.DataNumber);
				break;
			case EValueType.NumberBig:
				pArrayNote.Add(pContents.DataValue.DataNumberBig);
				break;
			case EValueType.Float:
				pArrayNote.Add(pContents.DataValue.DataFloat);
				break;
		}
	}

}
