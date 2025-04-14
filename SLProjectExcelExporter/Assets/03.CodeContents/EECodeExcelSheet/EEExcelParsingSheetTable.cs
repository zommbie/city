using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class EEExcelParsingSheetTable : EEExcelParsingSheetBase
{
	private string m_strSheetTableName = string.Empty;						public string GetExcelParsingSheetTableName() { return m_strSheetTableName; }
	private List<STableData> m_listTableData = new List<STableData>();
	//----------------------------------------------------------------------------
	protected override void OnParsingSheetReset()
	{
		m_listTableData.Clear();
	}

	protected override SErrorInfo OnParsingSheetLoad(DataTable pDataTable, params object[] aParams)
	{
		SErrorInfo rError = new SErrorInfo();

		m_strSheetTableName = pDataTable.TableName;
		List<STableDataHeader> pListHeader = PrivSheetTableMakeHeaderInstance(pDataTable.Columns.Count);


		for (int i = 0; i < pDataTable.Rows.Count; i++)
		{
			DataRow pDataRow = pDataTable.Rows[i];

			if (pDataRow.ItemArray.Length == 0)
			{
				rError.ErrorType = EEEErrorCategory.TableField;
				rError.ErrorMessage = "[TableField] Column Length = 0";
			}
			else
			{
				rError = PrivSheetTableReadRow(pDataRow, pListHeader, m_listTableData);
			}

			if (rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}

		return rError;
	}

	protected override void OnParsingSheetCompile(SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
		for (int i = 0; i < m_listTableData.Count; i++)
		{
			STableData pTableData = m_listTableData[i];

			pErrorContainer.RowIndex = i;
			pErrorContainer.TableKey = pTableData.TableKey;

			PrivSheetTableVerifyKeyName(pTableData, pErrorContainer);
			PrivSheetTableArrayData(pTableData, pSheetReference, pErrorContainer);
		}
	}
	//-----------------------------------------------------------------------------------
	public List<STableData>.Enumerator ExtractParsingSheetTableData() { return m_listTableData.GetEnumerator(); }
	//--------------------------------------------------------------------------------------
	private List<STableDataHeader> PrivSheetTableMakeHeaderInstance(int iCount)
	{
		List<STableDataHeader> pListHeader = new List<STableDataHeader>();
		for (int i = 0; i < iCount; i++)
		{
			if (i == 0)
			{
				pListHeader.Add(null);      // 첫번째 컬럼은 식별 헤더이므로 로드하지 않음
			}
			else
			{
				pListHeader.Add(new STableDataHeader());
			}
		}

		return pListHeader;
	}
	private SErrorInfo PrivSheetTableReadRow(DataRow pDataRow, List<STableDataHeader> pListHeader, List<STableData> pListTableData)
	{
		SErrorInfo rError = new SErrorInfo();

		string strHeaderCell = EEStringUtility.ConvertObjectToString(pDataRow.ItemArray[0]);
		if (strHeaderCell == string.Empty) // 해더 입력을 안 할 경우 읽지 않음(메모 용도로 사용시)
		{
			return rError;
		}

		if (EEStringUtility.IsTagString(strHeaderCell))
		{
			strHeaderCell = EEStringUtility.RemoveTag(strHeaderCell);
			EColumnType eColumnType = Extension.StringToEnum<EColumnType>(strHeaderCell);
			if (eColumnType != EColumnType.None)
			{
				rError = PrivSheetTableReadHeader(eColumnType, pDataRow.ItemArray, pListHeader);
			}
			else
			{
				rError.ErrorType = EEEErrorCategory.TableField;
				rError.ErrorMessage = "Invalid Header Name : " + strHeaderCell;
			}
		}
		else
		{
			if (pDataRow.ItemArray.Length != pListHeader.Count)
			{
				rError.ErrorType = EEEErrorCategory.TableField;
				rError.ErrorMessage = "The number of columns and rows does not match";
			}
			else
			{
				STableData pTableData = new STableData();
				pListTableData.Add(pTableData);
				rError = PrivSheetTableReadData(pDataRow.ItemArray, pListHeader, pTableData);
			}
		}
		return rError;
	}
	private SErrorInfo PrivSheetTableReadHeader(EColumnType eColumnType, object[] aItemContents, List<STableDataHeader> pListHeader)
	{
		SErrorInfo rError = new SErrorInfo();

		for (int i = 1; i < aItemContents.Length; i++)
		{
			string strCell = EEStringUtility.ConvertObjectToString(aItemContents[i]);

			if (strCell == string.Empty)
			{
				if (eColumnType == EColumnType.KeyName)
				{
					pListHeader[i] = null;
				}
				continue;
			}

			STableDataHeader pTableHeader = pListHeader[i];
			if (pTableHeader == null)
			{
				continue;
			}

			if (eColumnType == EColumnType.Export)
			{
				EExportType eExportType = Extension.StringToEnum<EExportType>(strCell);
				if (pTableHeader.ExportType != EExportType.None)
				{
					pTableHeader.ExportType = eExportType;
				}
				else
				{
					rError.ErrorType = EEEErrorCategory.TableField;
					rError.ErrorMessage = "Invalid Export Type = " + strCell;
				}
			}
			else if (eColumnType == EColumnType.Required)
			{
				bool bBoolean = false;
				if (bool.TryParse(strCell, out bBoolean))
				{
					pTableHeader.Required = bBoolean;
				}
				else
				{
					rError.ErrorType = EEEErrorCategory.TableField;
					rError.ErrorMessage = "Invalid Require Type = " + strCell;
				}
			}
			else if (eColumnType == EColumnType.Min)
			{
				int iMin = 0;
				if (int.TryParse(strCell, out iMin))
				{
					pTableHeader.Min = iMin;
				}
				else
				{
					rError.ErrorType = EEEErrorCategory.TableField;
					rError.ErrorMessage = "Invalid Min Value = " + strCell;
				}
			}
			else if (eColumnType == EColumnType.Max)
			{
				int iMax = 0;
				if (int.TryParse(strCell, out iMax))
				{
					pTableHeader.Max = iMax;
				}
				else
				{
					rError.ErrorType = EEEErrorCategory.TableField;
					rError.ErrorMessage = "Invalid Max Value = " + strCell;
				}
			}
			else if (eColumnType == EColumnType.Type)
			{
				EValueType eValueType = Extension.StringToEnum<EValueType>(strCell);
				if (eValueType != EValueType.None)
				{
					pTableHeader.ValueType = eValueType;
				}
				else
				{
					rError.ErrorType = EEEErrorCategory.TableField;
					rError.ErrorMessage = "Invalid Value Type = " + strCell;
				}
			}
			else if (eColumnType == EColumnType.Link)
			{
				string[] aLink = EEStringUtility.ExtractSplitStringComma(strCell);
				for (int j = 0; j < aLink.Length; j++)
				{
					pTableHeader.Link.Add(aLink[j]);
				}
			}
			else if (eColumnType == EColumnType.KeyName)
			{
				pTableHeader.KeyName = strCell;
			}


			if (rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}

		return rError;
	}
	private SErrorInfo PrivSheetTableReadData(object[] aItemContents, List<STableDataHeader> pListHeader, STableData pTableData)
	{
		SErrorInfo rError = new SErrorInfo();

		for (int i = 0; i < aItemContents.Length; i++)
		{
			string strCell = EEStringUtility.ConvertObjectToString(aItemContents[i]);

			if (i == 0)
			{
				uint iKey = 0;
				if(uint.TryParse(strCell, out iKey))
				{
                    pTableData.TableKey = iKey;
                }
				else
				{
                    rError.ErrorType = EEEErrorCategory.TableField;
                    rError.ErrorMessage = "Invalid Key Field = " + strCell;
					break;
                }
			}
			else
			{
				if (pListHeader[i] == null) continue;

				STableDataHeader pCloneTableDataHeader = pListHeader[i].Clone();
				pCloneTableDataHeader.Contents = strCell;

				STableDataContents pTableDataContents = new STableDataContents();
				pTableDataContents.DataHeader = pCloneTableDataHeader;
				pTableData.TableItemList.Add(pTableDataContents);
			}
		}
		return rError;
	}
	//---------------------------------------------------------------------------------------
	private void PrivSheetTableVerifyKeyName(STableData pTableData, SErrorContainer pErrorContainer)
	{
		// 키 값의 유효성 검사 
		if (pTableData.TableKey == 0)
		{
			pErrorContainer.AddError(EEEErrorCategory.TableCompile, "Key Name Empty : 0");
		}
		else
		{
            STableData pTableDataDuplicated = m_listTableData.Find((x => x.TableKey == pTableData.TableKey && x != pTableData));
			if (pTableDataDuplicated != null)
			{
				pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Duplicate Key Detect : {pTableData.TableKey}");
			}
		}
	}
	private void PrivSheetTableArrayData(STableData pTableData, SSheetReference pSheetReference, SErrorContainer pErrorContainer) // 키 이름이 첨자 연산식인지
	{
		for (int i = 0; i < pTableData.TableItemList.Count; i++)
		{
			STableDataContents pTableDataOrigin = pTableData.TableItemList[i];
			pTableDataOrigin.DataArrayParents = pTableData.TableItemList;

			STableDataContents pChildData = FindSheetTableChildData(pTableDataOrigin);		
			PrivSheetTableArrayColumeMerge(pChildData, pTableDataOrigin.DataHeader, pSheetReference, pErrorContainer);
		}
	}

	private STableDataContents FindSheetTableChildData(STableDataContents pTableDataOrigin)
	{
		STableDataContents pFindTableChild = null;
		string[] aKeyName = EEStringUtility.ExtractSplitStringTag(pTableDataOrigin.DataHeader.KeyName);
		if (aKeyName.Length <= 1)
		{
			pFindTableChild = pTableDataOrigin;
			pFindTableChild.DataName = pTableDataOrigin.DataHeader.KeyName;			
		}
		else
		{
			pFindTableChild = RecursiveSheetTableSearchChildData(aKeyName, 0, pTableDataOrigin, pTableDataOrigin.DataHeader, pTableDataOrigin.DataArrayParents);
		}
		return pFindTableChild;
	}

	//----------------------------------------------------------------------------------------------
	private STableDataContents RecursiveSheetTableSearchChildData(string[] aKeyName, int iRecursiveCount, STableDataContents pOriginTable, STableDataHeader pOriginHeader,  List<STableDataContents> listTableDataContent)
	{
		if (iRecursiveCount >= aKeyName.Length) return null;

		STableDataContents pFindTableData = null;
		string strFindKeyName = aKeyName[iRecursiveCount];

		for (int i = 0; i < listTableDataContent.Count; i++) 
		{
			STableDataContents pTableDataContents = listTableDataContent[i];

			if (pTableDataContents == null) continue;

			if (pTableDataContents.DataName == strFindKeyName)
			{			
				pFindTableData = RecursiveSheetTableSearchChildData(aKeyName, iRecursiveCount + 1, pTableDataContents, pOriginHeader, pTableDataContents.DataArrayChild);	
				if (pFindTableData == null)
				{
					pFindTableData = pTableDataContents;
				}

				break;
			}
		}

		if (pFindTableData == null) // 
		{
			pFindTableData = RecursiveSheetTableMakeChildData(aKeyName, iRecursiveCount, pOriginTable, pOriginHeader, pOriginTable.DataArrayChild);			
		}
		else
		{
			// 머지가 되었다면 해당 컬럼을 초기화 해준다
			for (int i = 0; i < listTableDataContent.Count; i++)
			{

				if (pOriginTable == listTableDataContent[i])
				{
					listTableDataContent[i] = null;
					break;
				}
			}
		}

		return pFindTableData;
	}

	private STableDataContents RecursiveSheetTableMakeChildData(string[] aKeyName, int iRecursiveCount, STableDataContents pOriginTable, STableDataHeader pOriginHeader, List<STableDataContents> listTableDataContent)
	{
		if (iRecursiveCount >= aKeyName.Length) return null;

		string strFindKeyName = aKeyName[iRecursiveCount];
		STableDataContents pFindTableData = null;

		STableDataContents pMakeChildData = new STableDataContents();
		pMakeChildData.DataArrayParents = pOriginTable.DataArrayChild;	
		
		if (pOriginTable.DataName == string.Empty)
		{
			pOriginTable.DataName = strFindKeyName;
			pOriginTable.IsArrayNote = true;
		}
					
		pFindTableData = RecursiveSheetTableMakeChildData(aKeyName, iRecursiveCount + 1, pMakeChildData, pOriginHeader, pMakeChildData.DataArrayChild);

		if (pFindTableData == null)
		{
			pFindTableData = pMakeChildData;
			pFindTableData.DataHeader = pOriginHeader;
			pFindTableData.DataName = strFindKeyName;
			pFindTableData.IsArrayNote = true;
		}

		pOriginTable.DataArrayChild.Add(pFindTableData);

		return pFindTableData;
	}

	//---------------------------------------------------------------------------------
	private void PrivSheetTableArrayColumeMerge(STableDataContents pTableDataContents, STableDataHeader pOriginHeader, SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
		string strKeyName = pTableDataContents.DataName;

		STableDataContents pMergeDataContents = null;

		bool bDeleteItem = false;

		for (int i = 0; i < pTableDataContents.DataArrayParents.Count; i++)
		{
			STableDataContents pFindTableData = pTableDataContents.DataArrayParents[i];

			if (pFindTableData == null) continue;

			if (pFindTableData == pTableDataContents)
			{
				pMergeDataContents = pTableDataContents;
				break;
			}

			if (pFindTableData.DataName == strKeyName )
			{
				bDeleteItem = true;
				pTableDataContents.IsArrayNote = false;
				pMergeDataContents = pFindTableData;
				break;
			}
		}

		if (bDeleteItem)
		{
			for (int i = 0; i < pTableDataContents.DataArrayParents.Count; i++)
			{
				STableDataContents pFindTableData = pTableDataContents.DataArrayParents[i];
				if (pFindTableData == pTableDataContents)
				{
					pTableDataContents.DataArrayParents[i] = null;
					break;
				}
			}
		}
		if (pMergeDataContents == null)
		{
			pMergeDataContents = pTableDataContents;
		}
		PrivSheetTableArrayContents(pMergeDataContents, pOriginHeader, pSheetReference, pErrorContainer);
	}
	//------------------------------------------------------------------------------------
	private void PrivSheetTableArrayContents(STableDataContents pTableDataContents, STableDataHeader pOriginHeader,  SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
        string[] aSplitArray = null;
        if (pOriginHeader.ValueType == EValueType.String || pOriginHeader.ValueType == EValueType.Text)
        {
            aSplitArray = new string[1];
            aSplitArray[0] = pOriginHeader.Contents;
        }
        else
        {
            aSplitArray = EEStringUtility.ExtractSplitStringComma(pOriginHeader.Contents);
        }

        for (int i = 0; i < aSplitArray.Length; i++)
		{
			STableDataContents pTableDataContentsValue = new STableDataContents();
			pTableDataContentsValue.DataHeader = pOriginHeader;
			pTableDataContentsValue.DataArrayParents = pTableDataContents.DataArrayChild;
			pTableDataContentsValue.DataName = pTableDataContents.DataName;
			pTableDataContents.DataArrayChild.Add(pTableDataContentsValue);

			if (i != 0)
			{
				pTableDataContents.IsArrayNote = false;
			}

			PrivSheetTableVerifyType(aSplitArray[i], pTableDataContentsValue, pOriginHeader, pSheetReference, pErrorContainer);
		}
	}

	private void PrivSheetTableVerifyType(string strContents, STableDataContents pTableDataContents, STableDataHeader pOriginHeader, SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
		if (pOriginHeader.KeyName == null) return;

        if (pOriginHeader.Required)
        {
            if (strContents == string.Empty)
            {
                pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Column Type [Required] Must have Data {pTableDataContents.DataHeader.KeyName}");
                return;
            }
        }

		if (pOriginHeader.ValueType == EValueType.String)
		{
			PrivSheetTableVerifyString(strContents, pTableDataContents, pErrorContainer);
		}
		else if (pOriginHeader.ValueType == EValueType.Text)
		{
			PrivSheetTableVerifyText(strContents, pTableDataContents, pSheetReference, pErrorContainer);
		}
		else if (pOriginHeader.ValueType == EValueType.Number)
		{
			PrivSheetTableVerifyNumber(strContents, pTableDataContents, pErrorContainer);
		}
		else if (pOriginHeader.ValueType == EValueType.NumberBig)
        {
            PrivSheetTableVerifyNumberBig(strContents, pTableDataContents, pErrorContainer);
        }
        else if (pOriginHeader.ValueType == EValueType.Bool)
		{
			PrivSheetTableVerifyBoolean(strContents, pTableDataContents, pErrorContainer);
		}
		else if (pOriginHeader.ValueType == EValueType.Enum)
		{
			PrivSheetTableVerifyEnum(strContents, pTableDataContents, pSheetReference, pErrorContainer);
		}
		else if (pOriginHeader.ValueType == EValueType.Key)
		{
			PrivSheetTableVerifyKey(strContents, pTableDataContents, pSheetReference, pErrorContainer);
		}
		else if (pOriginHeader.ValueType == EValueType.Float)
        {
			PrivSheetTableVerifyFloat(strContents, pTableDataContents, pErrorContainer);
		}
	}

	//------------------------------------------------------------------------------------
	private void PrivSheetTableVerifyString(string strContents, STableDataContents pTableDataContents, SErrorContainer pErrorContainer)
	{
		pTableDataContents.DataValue.DataString = strContents;
	}

	private void PrivSheetTableVerifyBoolean(string strContents, STableDataContents pTableDataContents, SErrorContainer pErrorContainer)
	{
        if (strContents == string.Empty) strContents = "false";

        bool bBoolean = false;
		if (bool.TryParse(strContents, out bBoolean))
		{
			pTableDataContents.DataValue.DataBoolean = bBoolean;
		}
		else
		{
			pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Invalid Bool Field {strContents}");
		}
	}

	private void PrivSheetTableVerifyEnum(string strContents, STableDataContents pTableDataContents, SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
		if (strContents == string.Empty) strContents = "None";
		
		string [] aKeyName = EEStringUtility.ExtractSplitStringTag(pTableDataContents.DataHeader.KeyName);
		string strKeyName = aKeyName[aKeyName.Length - 1];
        SEnumData pEnumData = pSheetReference.SheetEnum.GetSheetEnumData(strKeyName);
		if (pEnumData != null)
		{
			pTableDataContents.DataValue.DataEnumData = pEnumData;
			pTableDataContents.DataValue.DataEnumString = strContents;
		}
		else
		{
			pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Invalid Enum Field {strContents}");
		}
	}

	private void PrivSheetTableVerifyText(string strContents, STableDataContents pTableDataContents, SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
		// 로컬라이징 테이블은 런타임에 참조되므로 여기서는 해당 데이터의 유효성만 검사하게 된다.
		pTableDataContents.DataValue.DataText = strContents;
	}

	private void PrivSheetTableVerifyNumber(string strContents, STableDataContents pTableDataContents, SErrorContainer pErrorContainer)
	{
        if (strContents == string.Empty) strContents = "0";

        int iNumber = 0;
		if (int.TryParse(strContents, out iNumber))
		{
			pTableDataContents.DataValue.DataNumber = iNumber;
		}
		else
		{
			pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Invalid Number Field {strContents}");
		}
	}

    private void PrivSheetTableVerifyFloat(string strContents, STableDataContents pTableDataContents, SErrorContainer pErrorContainer)
    {
        if (strContents == string.Empty) strContents = "0";

        float fFloat = 0;
        if (float.TryParse(strContents, out fFloat))
        {
            pTableDataContents.DataValue.DataFloat = fFloat;
        }
        else
        {
            pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Invalid Number Field {strContents}");
        }
    }

    private void PrivSheetTableVerifyNumberBig(string strContents, STableDataContents pTableDataContents, SErrorContainer pErrorContainer)
    {
        if (strContents == string.Empty) strContents = "0";

        long iNumber = 0;
        if (long.TryParse(strContents, out iNumber))
        {
            pTableDataContents.DataValue.DataNumberBig = iNumber;
        }
        else
        {
            pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Invalid NumberBig Field {strContents}");
        }
    }

	private void PrivSheetTableVerifyKey(string strContents, STableDataContents pTableDataContents, SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{
		if (strContents == string.Empty) strContents = "0";

        uint iKey = 0;
		if (uint.TryParse(strContents, out iKey))
		{
			pTableDataContents.DataValue.DataKey = iKey;
			//ToDo Link를 참조하여 해당 키가 있는지 확인 할 것 

		}
		else
		{
			pErrorContainer.AddError(EEEErrorCategory.TableCompile, $"Invalid Key Field {strContents}");
		}
	}
	//-------------------------------------------------------------------------
	
}
