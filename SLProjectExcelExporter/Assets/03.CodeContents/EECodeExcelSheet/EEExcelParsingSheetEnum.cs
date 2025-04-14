using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EEExcelParsingSheetEnum : EEExcelParsingSheetBase
{
	
	private Dictionary<string, SEnumData> m_mapParsingEnum = new Dictionary<string, SEnumData>();
	//--------------------------------------------------------
	protected override void OnParsingSheetReset()
	{
		m_mapParsingEnum.Clear();
	}

	protected override SErrorInfo OnParsingSheetLoad(DataTable pDataTable, params object[] aParams)
	{
		SErrorInfo rError = new SErrorInfo();
		DataRowCollection pCollectionRow = pDataTable.Rows;

		for (int i = 0; i < pCollectionRow.Count; i++)
		{
			DataRow pDataRow = pCollectionRow[i];

            rError = PrivSheetEnumLoad(pDataRow);

            if (rError.ErrorType != EEEErrorCategory.None)
			{
				break;
			}
		}
		return rError;
	}

	//------------------------------------------------------------------
	public SEnumData GetSheetEnumData(string strEnumName)
	{
		SEnumData pEnumData = null;
		if (m_mapParsingEnum.ContainsKey(strEnumName))
		{
			pEnumData = m_mapParsingEnum[strEnumName];
		}
		return pEnumData;
	}

	//--------------------------------------------------------------------
	private SErrorInfo PrivSheetEnumLoad(DataRow pDataRow)
	{
        SErrorInfo rError = new SErrorInfo();
        SEnumData pEnumData = null;

        for (int i = 0; i < pDataRow.ItemArray.Length; i++)
		{
			if(i == 0)
			{
                string strEnumName = EEStringUtility.ConvertObjectToString(pDataRow.ItemArray[i]);

                if (strEnumName == string.Empty)
                {
					return rError;   
				}

				if (m_mapParsingEnum.ContainsKey(strEnumName))
				{
					pEnumData = m_mapParsingEnum[strEnumName];
                    continue;
				}
				else
				{
					pEnumData = new SEnumData();
                }

				pEnumData.EnumName = strEnumName;
                m_mapParsingEnum.Add(strEnumName, pEnumData);
            }
			else
			{
                string strEnumContents = EEStringUtility.ConvertObjectToString(pDataRow.ItemArray[i]);

				if(pEnumData.EnumContents.Contains(strEnumContents))
				{
                    rError.ErrorType = EEEErrorCategory.EnumField;
                    rError.RowIndex = i;
                    rError.ErrorMessage = "Invalid Column Overlap Enum. Check Excel Sheet.";
                    return rError;
                }

                if (strEnumContents != string.Empty)
					pEnumData.EnumContents.Add(strEnumContents);
            }
		}

		if (pEnumData.EnumContents.Count == 0)
			m_mapParsingEnum.Remove(pEnumData.EnumName);

        return rError;
    }
}
