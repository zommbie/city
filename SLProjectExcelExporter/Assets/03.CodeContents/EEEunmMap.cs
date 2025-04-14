using System.Collections;
using System.Collections.Generic;


//-------------------------------------------------
public enum EColumnType
{
	None,
	Export,				// 출력모드 지정 : 서버 / 클라 / 둘다 
	Required,           // 값을 반드시 입력 해야 하는지 아닌지
	Min,				// 입력 최소값
	Max,				// 입력 최대값
	Type,				// 필드 값 형태 ( 문자열, 숫자등 )
	Link,				// 참조 Sheet 이름. Type:Text의 경우 (Text) 시트내에 TextCategory를 필터링하는 것으로 쓰인다.
	KeyName,			// 해당 변수의 이름 EWorldNation처럼 접두에 'E'를 붙일경우 Enum에서 해당 값을 검색하여 출력한다.
}

public enum EValueType
{	
	None,
	String,
	Number,
	Text,
	Bool,
	Key,
	Enum,
	NumberBig,
	Float,
}

public enum EBoolType
{
	None,
	TRUE,
	FALSE,
}

public enum EExportType
{
	None,
	Both,
	Client,
	Server,
}

public enum EEnumLoadType
{
	None,
	Vertical,
	Horizontal,
}

//---------------------------------------------------------------
public class STableDataHeader : object
{
	public EExportType	ExportType	= EExportType.Both;
	public EValueType	ValueType	= EValueType.String;

	public List<string> Link = new List<string>();
	public string KeyName = null;
	public string Contents = null;
	public bool   Required = false;

	public int Min;
	public int Max;

	public STableDataHeader Clone() { return this.MemberwiseClone() as STableDataHeader; }
}

public class STableDataValue
{
	public SEnumData	DataEnumData = new SEnumData();
	public string       DataEnumString = string.Empty;	
	public string		DataString = string.Empty;
	public string		DataText = string.Empty;
	public int			DataNumber;
	public float        DataFloat;
	public long			DataNumberBig;
	public uint			DataKey;
	public bool			DataBoolean;

}

public class STableData
{
	public uint TableKey;
	public List<STableDataContents> TableItemList = new List<STableDataContents>();
}

public class STableDataContents
{
	public STableDataHeader DataHeader = new STableDataHeader();
	public STableDataValue  DataValue = new STableDataValue();
	public string			DataName = string.Empty;
	public bool				IsArrayNote = false;
	public bool				IsExport = true;

	public List<STableDataContents> DataArrayChild = new List<STableDataContents>();	
	public List<STableDataContents> DataArrayParents = null;	
}

public class SEnumData
{
	public string EnumName;
	public List<string> EnumContents = new List<string>();
}

public class SJsonExportData
{
	public string DataName;
	public string DataJsonContentsClient;
	public string DataJsonContentsServer;
}
