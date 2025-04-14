using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class CManagerLocalizingTableBase : CManagerTemplateBase<CManagerLocalizingTableBase>
{
    protected class SLocalizingTable
    {
        public string                   TextKey;                                     // 해당 텍스트 키로 대부분 한글 문자열을 사용한다. 한글이 길 경우 태그 텍스트를 사용하자
        public Dictionary<int, string>  LanguageMap = new Dictionary<int, string>(); // 각 언어별 문자열. 언어가 많거나 전체 텍스트가 많을 경우 로딩 타임에 해당 언어만 로드하자. 
    }

    private HashSet<KeyValuePair<string, string>> m_setLocalizingText = new HashSet<KeyValuePair<string, string>>();
    //----------------------------------------------------------
   
}
