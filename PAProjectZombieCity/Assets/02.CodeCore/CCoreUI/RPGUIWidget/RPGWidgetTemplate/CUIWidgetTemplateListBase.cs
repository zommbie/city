using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// CSerializableDictionary 이용하여 코어에서 컨텐츠 레이어의 인스턴스를 인스펙터에 맵핑하도록 정렬
// 해당 딕셔너리는 에디터 클레스에서 인스펙터 드로우를 설정해 줘야한다.

public abstract class CUIWidgetTemplateListBase<TEMPLATE, ENUMTYPE> : CUIEntryBase where TEMPLATE : CSerializableDictionary<ENUMTYPE, CUIWidgetTemplateInstance>, new() where ENUMTYPE : Enum 
{
    [SerializeField]
    private TEMPLATE TemplateList = new TEMPLATE();
    //---------------------------------------------------
    protected CUIWidgetTemplateItemBase ProtTemplateListCloneInstance(ENUMTYPE eEnumType, Transform pParent = null)
    {
        CUIWidgetTemplateItemBase pCloneInstance = null;
        if (TemplateList.ContainsKey(eEnumType))
        {
            CUIWidgetTemplateInstance pTemplate = TemplateList[eEnumType];
            pCloneInstance = pTemplate.DoUITemplateRequestItem(pParent);
        }

        return pCloneInstance;
    }

    protected void ProtTemplateListCloneReturnAll()
    {
        CSerializableDictionary<ENUMTYPE, CUIWidgetTemplateInstance>.Enumerator it = TemplateList.GetEnumerator();
        while (it.MoveNext())
        {
            CUIWidgetTemplateBase pTemplate = it.Current.Value;
            pTemplate.DoUITemplateReturnAll();
        }
    }
    //---------------------------------------------------

}
