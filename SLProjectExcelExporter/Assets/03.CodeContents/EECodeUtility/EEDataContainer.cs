using System.Collections;
using System.Collections.Generic;


public enum EEEErrorCategory
{
	None,
	Directory,
	FileOpen,

	EnumField,
	TextField,
	TableField,

	TableCompile,
}

public struct SErrorInfo
{
	public EEEErrorCategory ErrorType;
	public string ErrorMessage;
	public int RowIndex;
	public uint TableKey;
	public SErrorInfo(EEEErrorCategory eErrorType, int iRowIndex, uint iTableKey, string strErrorMessage) {  ErrorType = eErrorType; RowIndex = iRowIndex; TableKey = iTableKey; ErrorMessage = strErrorMessage; }
}

public class SErrorContainer
{
	public List<SErrorInfo> listError = new List<SErrorInfo>();
	public int RowIndex;
	public uint TableKey;
	public void AddError(EEEErrorCategory eErrorCategory, string strMessage) { listError.Add(new SErrorInfo(eErrorCategory, RowIndex, TableKey, strMessage));}
}

public class SSheetReference
{
	public EEExcelParsingSheetEnum SheetEnum = null;
	public EEExcelParsingSheetText SheetText = null;
	public Dictionary<string, EEExcelParsingSheetTable> SheetTable = null;
}
public class SExportData
{
	public string ExportName;
	public string ExportJson;
}