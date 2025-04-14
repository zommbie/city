using System.Collections;
using System.Collections.Generic;
using System.Text;
using NExcelExporter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

public class EEJsonLoadCodeGeneratorCSharp : EEJsonLoadCodeGeneratorBase
{
    private const string c_ServerClassNameSuffix = "Server";
    private const string c_TableItemName = "STableItem";
    private const string c_BlankText = "    ";

    private int m_iAreaDepth = 0;
    private List<string> m_listEnum = new List<string>();   //해당 데이터에서 사용되는 Enum 데이터의 이름 (pSheetContainer 검색 용도)
    private Dictionary<string, Dictionary<string, string>> m_mapTableItemText = new Dictionary<string, Dictionary<string, string>>(); //클래스 단위로 변수명과 이에 해당하는 변수선언 텍스트를 보유 
    //------------------------------------------------------------
    protected override string OnJsonLoadCodeGenerator(JObject pRootJson, EEExcelParsingSheetContainer pSheetContainer, EExportType eExportType, StringBuilder pNote)
	{
        PrivJsonLoadCodeGenerator(pRootJson, pSheetContainer, eExportType, pNote);

        return pNote.ToString();
	}
    //-----------------------------------------------------------------
    private void PrivJsonLoadCodeGenerator(JObject pRootJson, EEExcelParsingSheetContainer pSheetContainer, EExportType eExportType, StringBuilder pNote)
    {
        PrivAppendCSharpTextRow(pNote,"using System.Collections.Generic;");
        PrivAppendCSharpTextRow(pNote,"namespace NExcelExporter");
        PrivAreaOpen(pNote);
        foreach (JContainer pJContainer in pRootJson.Children())
        {
            m_listEnum.Clear();
            m_mapTableItemText.Clear();

            PrivAppendCSharpTextRow(pNote,"[System.Serializable]");

            string strClassNameSuffix = eExportType == EExportType.Server ? c_ServerClassNameSuffix : string.Empty;
            PrivAppendCSharpTextRow(pNote,string.Format("public class STableLoader{0}", pJContainer.Path + strClassNameSuffix)); //엑셀 시트명

            PrivAreaOpen(pNote);
            foreach (JToken pJToken in pJContainer.Children())
            {
                foreach (JToken pJTokenTableItem in pJToken.Children())
                {
                    PrivJsonLoadCodeGeneratorValueObject(pJTokenTableItem, pSheetContainer, c_TableItemName);
                }

                PrivJsonLoadCodeGeneratorAppendEnum(pSheetContainer, pNote);
                PrivJsonLoadCodeGeneratorAppendContents(pNote);
                PrivAppendCSharpTextRow(pNote,string.Format("public List<{0}> {1} = new List<{0}>();", c_TableItemName, pJContainer.Path));
            }
            PrivAreaClose(pNote);
        }
        PrivAreaClose(pNote);
    }

    private void PrivJsonLoadCodeGeneratorValueObject(JToken pJToken, EEExcelParsingSheetContainer pSheetContainer, string strClassName)
    {
        Dictionary<string, string> mapClassContents = null;

        if (m_mapTableItemText.ContainsKey(strClassName))
        {
            mapClassContents = m_mapTableItemText[strClassName];
        }
        else
        {
            mapClassContents = new Dictionary<string, string>();
            m_mapTableItemText.Add(strClassName, mapClassContents);
        }

        foreach (JProperty pJProperty in pJToken.Children())
        {
            string pContentsTextRow = PrivJsonLoadCodeGeneratorValueProterty(pJProperty, pSheetContainer);

            if (mapClassContents.ContainsKey(pJProperty.Name))
            {
                //"List"를 기존의 값이 포함하지 않고, 새로운 값이 포함한 경우 갱신처리
                if (!mapClassContents[pJProperty.Name].Contains("List") && pContentsTextRow.Contains("List"))
                {
                    mapClassContents[pJProperty.Name] = pContentsTextRow;
                }
            }
            else
            {
                mapClassContents.Add(pJProperty.Name, pContentsTextRow);
            }
        }
    }

    private string PrivJsonLoadCodeGeneratorValueProterty(JProperty pJProperty, EEExcelParsingSheetContainer pSheetContainer)
    {
        string pContentsTextRow = string.Empty;

        if (pJProperty.Value.Type == JTokenType.Integer)
        {
            long lValue = pJProperty.Value.Value<long>();

            if (lValue >= int.MinValue && lValue <= int.MaxValue)
            {
                pContentsTextRow = string.Format("public int {0};", pJProperty.Name);
            }
            else
            {
                pContentsTextRow = string.Format("public long {0};", pJProperty.Name);
            }
        }
        else if (pJProperty.Value.Type == JTokenType.Boolean)
        {
            pContentsTextRow = string.Format("public bool {0};", pJProperty.Name);
        }
        else if (pJProperty.Value.Type == JTokenType.String)
        {
            if (pSheetContainer.GetSheetContainerEnumData(pJProperty.Name) != null)
            {
                string strReplaceName = pJProperty.Name.Replace('[', '_').Replace("]", string.Empty);

                pContentsTextRow = string.Format("public {0} {0};", strReplaceName);

                if (!m_listEnum.Contains(pJProperty.Name))
                {
                    m_listEnum.Add(pJProperty.Name);
                }
            }
            else
            {
                pContentsTextRow = string.Format("public string {0};", pJProperty.Name);
            }
        }
        else if (pJProperty.Value.Type == JTokenType.Float)
        {
            pContentsTextRow = string.Format("public float {0};", pJProperty.Name);
        }
        else if (pJProperty.Value.Type == JTokenType.Array)
        {
            foreach (JToken pJToken in pJProperty.Values())
            {
                if (pJToken.Type == JTokenType.Object)
                {
                    string pClassName = string.Format("S{0}", pJProperty.Name.FirstCharacterToUpper());
                    pContentsTextRow = string.Format("public List<{0}> {1} = new List<{0}>();", pClassName, pJProperty.Name);

                    PrivJsonLoadCodeGeneratorValueObject(pJToken, pSheetContainer, pClassName);
                }
                else if (pJToken.Type == JTokenType.Integer)
                {
                    pContentsTextRow = string.Format("public List<int> {0} = new List<int>();", pJProperty.Name);
                }
                else if (pJToken.Type == JTokenType.Boolean)
                {
                    pContentsTextRow = string.Format("public List<bool> {0} = new List<bool>();", pJProperty.Name);
                }
                else if (pJToken.Type == JTokenType.String)
                {
                    pContentsTextRow = string.Format("public List<string> {0} = new List<string>();", pJProperty.Name);
                }
                else if (pJToken.Type == JTokenType.Float)
                {
                    pContentsTextRow = string.Format("public List<float> {0} = new List<string>();", pJProperty.Name);
                }
            }
        }

        return pContentsTextRow;
    }

    private void PrivJsonLoadCodeGeneratorAppendContents(StringBuilder pNote)
    {
        Dictionary<string, Dictionary<string,string>>.Enumerator enumeratorClassName = m_mapTableItemText.GetEnumerator();
        while (enumeratorClassName.MoveNext())
        {
            PrivAppendCSharpTextRow(pNote,"[System.Serializable]");
            PrivAppendCSharpTextRow(pNote,string.Format("public class {0}", enumeratorClassName.Current.Key));
            PrivAreaOpen(pNote);
            Dictionary<string, string>.Enumerator enumeratorContents = enumeratorClassName.Current.Value.GetEnumerator();
            while(enumeratorContents.MoveNext())
            {
                PrivAppendCSharpTextRow(pNote,enumeratorContents.Current.Value);
            }
            PrivAreaClose(pNote);
        }
    }

    private void PrivJsonLoadCodeGeneratorAppendEnum(EEExcelParsingSheetContainer pSheetContainer, StringBuilder pNote)
    {
        if (m_listEnum.Count > 0)
        {
            for (int i = 0; i < m_listEnum.Count; i++)
            {
                SEnumData pEnumData = pSheetContainer.GetSheetContainerEnumData(m_listEnum[i]);
                PrivAppendCSharpTextRow(pNote, string.Format("public enum {0}", pEnumData.EnumName.Replace('[', '_').Replace("]", string.Empty)));
                PrivAreaOpen(pNote);
                PrivAppendCSharpTextRow(pNote, "None = 0,");
                for (int j = 0; j < pEnumData.EnumContents.Count; j++)
                {
                    PrivAppendCSharpTextRow(pNote, string.Format("{0} = {1}{2}", pEnumData.EnumContents[j].Replace(' ', '_'), j + 1, ','));
                }
                PrivAreaClose(pNote);
            }
        }
    }

    private void PrivAppendCSharpTextRow(StringBuilder pNote, string strCSharpTextRow)
    {
        pNote.Append(PrivAppendBlankText());
        pNote.AppendLine(strCSharpTextRow);
    }

    private void PrivAreaOpen(StringBuilder pNote)
    {
        pNote.Append(PrivAppendBlankText());
        pNote.AppendLine("{");
        m_iAreaDepth++;
    }

    private void PrivAreaClose(StringBuilder pNote)
    {
        m_iAreaDepth--;
        pNote.Append(PrivAppendBlankText());
        pNote.AppendLine("}");
        pNote.AppendLine();
    }

    private string PrivAppendBlankText()
    {
        string strBlankText = string.Empty;

        for (int i = 0; i < m_iAreaDepth; i++)
        {
            strBlankText += c_BlankText;
        }

        return strBlankText;
    }
}
