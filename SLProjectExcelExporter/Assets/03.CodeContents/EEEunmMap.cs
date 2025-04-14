using System.Collections;
using System.Collections.Generic;


//-------------------------------------------------
public enum EColumnType
{
	None,
	Export,				// ��¸�� ���� : ���� / Ŭ�� / �Ѵ� 
	Required,           // ���� �ݵ�� �Է� �ؾ� �ϴ��� �ƴ���
	Min,				// �Է� �ּҰ�
	Max,				// �Է� �ִ밪
	Type,				// �ʵ� �� ���� ( ���ڿ�, ���ڵ� )
	Link,				// ���� Sheet �̸�. Type:Text�� ��� (Text) ��Ʈ���� TextCategory�� ���͸��ϴ� ������ ���δ�.
	KeyName,			// �ش� ������ �̸� EWorldNationó�� ���ο� 'E'�� ���ϰ�� Enum���� �ش� ���� �˻��Ͽ� ����Ѵ�.
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
