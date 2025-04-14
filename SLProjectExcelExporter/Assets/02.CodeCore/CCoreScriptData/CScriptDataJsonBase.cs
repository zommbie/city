using System.Collections.Generic;
using Newtonsoft.Json.Linq;


abstract public class CScriptDataJsonBase : CScriptDataBase
{
   
	//---------------------------------------------------------
    protected List<TEMPLATE> ProtDataJsonRead<TEMPLATE>(string strTextData) where TEMPLATE : class
    {
        List<TEMPLATE> pListJsonData = new List<TEMPLATE>();

        JArray pJsonArray = JArray.Parse(strTextData);
        for (int i = 0; i < pJsonArray.Count; i++)
        {
            TEMPLATE pInstance = pJsonArray[i].ToObject<TEMPLATE>();
            pListJsonData.Add(pInstance);
        }

        return pListJsonData;
    }
}