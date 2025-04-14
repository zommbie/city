using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


abstract public class CScriptDataJsonBase : CScriptDataBase
{   
	//---------------------------------------------------------
    protected TEMPLATE ProtDataJsonRead<TEMPLATE>(string strTextData) where TEMPLATE : class
    {
        return JsonConvert.DeserializeObject<TEMPLATE>(strTextData, new CJsonConverterGenericCustomReader());
    }
}