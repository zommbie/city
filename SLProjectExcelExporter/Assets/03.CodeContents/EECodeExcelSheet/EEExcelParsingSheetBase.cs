using System.Collections;
using System.Collections.Generic;
using System.Data;
public abstract class EEExcelParsingSheetBase
{
	private string m_strSheetName = string.Empty;   public string GetParsingSheetName() { return m_strSheetName; }
	//-----------------------------------------------------------------------
	public void DoParsingSheetReset()
	{
		OnParsingSheetReset();
	}

	public SErrorInfo DoParsingSheetLoad(DataTable pDataTable, params object [] aParams)
	{
		m_strSheetName = pDataTable.TableName;
        return OnParsingSheetLoad(pDataTable);
	}

	public void DoParsingSheetCompile(SSheetReference pSheetReference, SErrorContainer pErrorContainer)
	{		
		OnParsingSheetCompile(pSheetReference, pErrorContainer);		
	}


	//------------------------------------------------------------------------
	protected virtual void OnParsingSheetReset() { }
	protected virtual SErrorInfo OnParsingSheetLoad(DataTable pDataTable, params object[] aParams) { return new SErrorInfo(); }
	protected virtual void OnParsingSheetCompile(SSheetReference pSheetReference, SErrorContainer pErrorContainer) { }
}
