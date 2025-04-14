using System;
using System.Text;

public static class EEStringUtility
{
	private const string c_TagOpen = "[";
	private const string c_TagClose = "]";
	private const char	 c_SplitSeperator = ',';
	private readonly static char[] c_SplitTag = { '[', ']' };
	//-----------------------------------------------------------
	public static bool IsTagString(string strString)
	{
		bool bTag = false;
		if (strString.StartsWith(c_TagOpen) && strString.EndsWith(c_TagClose))
		{
			bTag = true;
		}
		return bTag;
	}

	public static string RemoveTag(string strString)
	{
		strString = strString.Replace(c_TagOpen, "");
		strString = strString.Replace(c_TagClose, "");
		return strString;
	}

	public static string ConvertObjectToString(object pObject)
	{
		string strResult = string.Empty;
		if (pObject.GetType() == typeof(double))
		{
			double fDouble = (double)pObject;
			strResult = fDouble.ToString();
		}
		else if (pObject.GetType() == typeof(bool))
		{
			strResult = ((bool)pObject).ToString();
		}
		else if (pObject.GetType() == typeof(string))
		{
			strResult = pObject.ToString();
		}
		return strResult;
	}

	public static string[] ExtractListString(string strString, char cSeperator = ',')
	{
		return strString.Split(cSeperator, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string ExtractHashByteToString(byte[] aHashByte)
	{		
		StringBuilder pNote = new StringBuilder();
		for (int i = 0; i < aHashByte.Length; i++)
		{
			pNote.AppendFormat("{0:X2}", aHashByte[i]);
		}		
		return pNote.ToString();
	}

	public static string [] ExtractSplitStringComma(string strString)
	{
		string [] aSplitString = strString.Split(c_SplitSeperator, StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < aSplitString.Length; i++)
		{
			aSplitString[i] = aSplitString[i].Trim();
		}

		if (aSplitString.Length == 0)
		{
			aSplitString = new string[1];
			aSplitString[0] = string.Empty;
		}

		return aSplitString;
	}

	public static string [] ExtractSplitStringTag(string strString)
	{		
		string [] aSplitString = strString.Split(c_SplitTag, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < aSplitString.Length; i++)
		{
			aSplitString[i] = aSplitString[i].Trim();
		}
		return aSplitString;
	}

    public static bool HasDecimal(double number)
    {
        return number != Math.Truncate(number);
    }
}
