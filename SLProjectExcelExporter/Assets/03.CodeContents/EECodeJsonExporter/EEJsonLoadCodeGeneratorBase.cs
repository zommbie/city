using System.Text;
using Newtonsoft.Json.Linq;




public abstract class EEJsonLoadCodeGeneratorBase 
{

    private StringBuilder m_pNote = new StringBuilder();
    //---------------------------------------------------------------------
    public string DoJsonLoadCodeGenerate(string strJson, EEExcelParsingSheetContainer pSheetContainer, EExportType eExportType)
	{
        m_pNote.Clear();
        JObject pRootJson = JObject.Parse(strJson);
        return OnJsonLoadCodeGenerator(pRootJson, pSheetContainer, eExportType, m_pNote);
	}
    

    //--------------------------------------------------------------------
    protected virtual string OnJsonLoadCodeGenerator(JObject pRootJson, EEExcelParsingSheetContainer pSheetContainer, EExportType eExportType, StringBuilder pNote) { return string.Empty; }
}
