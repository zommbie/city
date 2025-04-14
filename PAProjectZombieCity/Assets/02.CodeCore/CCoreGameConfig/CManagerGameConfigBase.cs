using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PlayerPrefs와 같이 클라 내부에 저장해야 하는 정보 입출력.
//PlayerPrefs중에 암호화 코드로 구성 
//키와 값이 무분별하게 확장되는 것을 막기 위해서 값을 JSON으로 한정하여 키를 영구적으로 고정하는 방법 
//서버가 아닌 클라 자체적으로 저장해야 하는 것들을 주로 처리 
public abstract class CManagerGameConfigBase : CManagerTemplateBase<CManagerGameConfigBase>
{
    //---------------------------------------------------------------------------------------
    protected void ProtMgrGameConfigClearAll()
    {
        CSecurityPlayerPrefs.DeleteAll();
    }

    protected void ProtMgrGameConfigClearCategory(string strCategoryName)
    {

    }

    //---------------------------------------------------------------------------------------


}
