using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
using System;


abstract public class CScriptDataBase : CMonoBase
{
    private const byte c_UTFBom1 = 0xEF;
    private const byte c_UTFBom2 = 0xBB;
    private const byte c_UTFBom3 = 0xBF;

	[SerializeField]
	private List<TextAsset> TextScript = new List<TextAsset>();
    [SerializeField]
    private bool Kor949ToUTF8 = false; // 코드 페이지 949(멀티바이트 한글 페이지) 사용할 경우 l18N.CJK.dll / l18N.dll 을 플러그인 해야한다 (실행파일 1MB 증가 페널티)
	//------------------------------------------------------------

	public void DoScriptDataInitialize(byte[] aTextData)
	{
        string strTextData = PrivScriptDataConverUTF8(aTextData);
        OnScriptDataLoad(strTextData);
        OnScriptDataLoadFinish();
    }

    public void DoScriptDataInitialize(string strTextData)
    {
        OnScriptDataLoad(strTextData);
        OnScriptDataLoadFinish();
    }

	//------------------------------------------------------------
	internal void InterScriptDataInitialize()
	{
		for (int i = 0; i < TextScript.Count; i++)
		{
            string strTextData = PrivScriptDataConverUTF8(TextScript[i].bytes);
            OnScriptDataLoad(strTextData);
		}
        OnScriptDataLoadFinish();
        TextScript.Clear();
	}
    //-------------------------------------------------------------------
    private string PrivScriptDataConverUTF8(byte [] aTextData)
    {
        string strText = string.Empty;
        if (Kor949ToUTF8)  // 멀티 바이트일 경우
        {
            aTextData = Encoding.Convert(Encoding.GetEncoding("euc-kr"), Encoding.UTF8, aTextData);
            strText = Encoding.UTF8.GetString(aTextData);
        }
        else if (aTextData.Length > 3 && aTextData[0] == c_UTFBom1 && aTextData[1] == c_UTFBom2 && aTextData[2] == c_UTFBom3)
        {   // 유니코드 UTF - 8 Bom일경우 (엑셀 익스포트 기본 포멧)
            strText = Encoding.UTF8.GetString(aTextData, 3, aTextData.Length - 3);
        }
        else
        {
            // 이외의 표준 UTF - 8
            strText = Encoding.UTF8.GetString(aTextData);
        }
        return strText;
    }

    //--------------------------------------------------------------------
	public static void GlobalScriptDataReadField<INSTANCE>(INSTANCE pInstance, string strValueName, string strValue) where INSTANCE : class
	{
		Type pClassType = pInstance.GetType();

		FieldInfo pFieldInfo = pClassType.GetField(strValueName);
		if (pFieldInfo == null) { return;}
        if (strValue == null) return;
        if (strValue == string.Empty)
        {
            if (pFieldInfo.FieldType == typeof(string))
            {
                pFieldInfo.SetValue(pInstance, string.Empty);
            }
            return;
        }

        if (pFieldInfo.FieldType == typeof(int)) 
		{
			pFieldInfo.SetValue(pInstance, int.Parse(strValue));
		}
		else if (pFieldInfo.FieldType == typeof(uint))
		{
			pFieldInfo.SetValue(pInstance, uint.Parse(strValue));
		}
		else if (pFieldInfo.FieldType == typeof(string))
		{
            pFieldInfo.SetValue(pInstance, strValue.Trim());
        }
		else if (pFieldInfo.FieldType == typeof(float))
		{
			pFieldInfo.SetValue(pInstance, float.Parse(strValue));
		}
		else if (pFieldInfo.FieldType == typeof(bool))
		{
			pFieldInfo.SetValue(pInstance, bool.Parse(strValue));
		}
		else if (pFieldInfo.FieldType.BaseType == typeof(Enum))
		{
			pFieldInfo.SetValue(pInstance, Enum.Parse(pFieldInfo.FieldType, strValue));
		}
		else if (pFieldInfo.FieldType == typeof(List<int>))
		{
			List<string> pListSeparate = PrivScriptDataSeparateComma(strValue);
			List<int> pListValue = pFieldInfo.GetValue(pInstance) as List<int>;
			for (int i = 0; i < pListSeparate.Count; i++)
			{				
				pListValue.Add(int.Parse(pListSeparate[i]));
			}
		}
		else if (pFieldInfo.FieldType == typeof(List<uint>))
		{
			List<string> pListSeparate = PrivScriptDataSeparateComma(strValue);
			List<uint> pListValue = pFieldInfo.GetValue(pInstance) as List<uint>;
			for (int i = 0; i < pListSeparate.Count; i++)
			{
				pListValue.Add(uint.Parse(pListSeparate[i]));
			}
		}
		else if (pFieldInfo.FieldType == typeof(List<string>))
		{
			List<string> pListSeparate = PrivScriptDataSeparateComma(strValue);
			List<string> pListValue = pFieldInfo.GetValue(pInstance) as List<string>;
			for (int i = 0; i < pListSeparate.Count; i++)
			{
				pListValue.Add(pListSeparate[i].Trim());
			}
		}
		else if (pFieldInfo.FieldType == typeof(List<float>))
		{
			List<string> pListSeparate = PrivScriptDataSeparateComma(strValue);
			List<float> pListValue = pFieldInfo.GetValue(pInstance) as List<float>;
			for (int i = 0; i < pListSeparate.Count; i++)
			{
				pListValue.Add(float.Parse(pListSeparate[i]));
			}
		}
	}
	
	//------------------------------------------------------------
	private static List<string> PrivScriptDataSeparateComma(string strData)
	{
		List<string> pListResult = new List<string>();
		StringBuilder Note = new StringBuilder();
		for (int i = 0; i < strData.Length; i++)
		{
			char C = strData[i];
			if (C == ',')
			{
				pListResult.Add(Note.ToString().Trim());
				Note.Clear();
			}
			else
			{
				Note.Append(C);
			}
		}

		if (Note.Length > 0)
		{
			pListResult.Add(Note.ToString().Trim());
		}

		return pListResult;
	}

	//-------------------------------------------------------------
	protected virtual void OnScriptDataLoad(string strTextData) { }
    protected virtual void OnScriptDataLoadFinish() { }
}
