using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인스펙터 입력이 되지 않는 읽기 전용 . 런타임 데이터를 모니터 할때 사용

[System.Serializable]
public class CDictionaryInspectorShow<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey>   ItemKey = new List<TKey>();
    [SerializeField]
    private List<TValue> ItemValue = new List<TValue>();

    public void OnBeforeSerialize()
    {
        ItemKey.Clear();
        ItemValue.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            ItemKey.Add(pair.Key);
            ItemValue.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (ItemKey.Count != Values.Count)
        {
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
        }

        for (int i = 0; i < ItemKey.Count; i++)
        {
            this.Add(ItemKey[i], ItemValue[i]);
        }
       
    }
     

}
