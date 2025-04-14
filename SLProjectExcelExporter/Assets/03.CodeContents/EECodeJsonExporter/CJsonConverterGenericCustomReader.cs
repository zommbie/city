using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Reflection;
namespace NExcelExporter
{
    public class CJsonConverterGenericCustomReader : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject pJsonObject = JObject.Load(reader);
            object pNewObject = Activator.CreateInstance(objectType);

            foreach (JContainer pJContainer in pJsonObject.Children())
            {
                foreach (JToken pJToken in pJContainer.Children())
                {
                    RecursiveJsonConverter(pJToken, pNewObject);
                }
            }

            return pNewObject;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

        //-------------------------------------------------------------------------------------
        private void RecursiveJsonConverter(JToken pJToken, object pInstance)
        {
            if (pJToken is JObject)
            {
                PrivJsonConverter(pJToken as JObject, pInstance);
            }
            else if (pJToken is JArray)
            {
                PrivJsonConverter(pJToken as JArray, pInstance);
            }
        }

        private void PrivJsonConverter(JArray pJArray, object pInstance)
        {
            Type pInstanceType = pInstance.GetType();
            JProperty pParentContainer = pJArray.Parent as JProperty;
            FieldInfo pFieldInfo = pInstanceType.GetField(pParentContainer.Name);
            if (pFieldInfo == null) return;

            Type pFieldType = pFieldInfo.FieldType;
            if (pFieldType.IsGenericType == false || pFieldType.GetGenericTypeDefinition() != typeof(List<>)) return;

            IList pListType = pFieldInfo.GetValue(pInstance) as IList;
            Type[] aGenericType = pFieldType.GetGenericArguments();

            for (int i = 0; i < pJArray.Count; i++)
            {
                object pArrayItemInstance = Activator.CreateInstance(aGenericType[0]);
                pListType.Add(pArrayItemInstance);
                RecursiveJsonConverter(pJArray[i], pArrayItemInstance);
            }
        }

        private void PrivJsonConverter(JObject pJObject, object pInstance)
        {
            foreach (JToken pJToken in pJObject.Children())
            {
                if (pJToken.Type == JTokenType.Property)
                {
                    PrivJsonConverterReadValueProperty(pJToken as JProperty, pInstance);
                }
            }
        }

        private void PrivJsonConverterReadValueProperty(JProperty pProperty, object pInstance)
        {
            Type pIntanceType = pInstance.GetType();
            FieldInfo pFieldInfo = pIntanceType.GetField(pProperty.Name.Replace('[','_').Replace("]",string.Empty));

            if (pFieldInfo == null)
            {
                //Error! �ش� �̸��� �ɹ� ������ ���� (�Ľ� ����)
                return;
            }

            foreach (JToken pJToken in pProperty.Values())
            {
                if (pJToken.Type == JTokenType.Integer)
                {
                    long lValue = pJToken.Value<long>();

                    if (lValue >= int.MinValue && lValue <= int.MaxValue)
                    {
                        PrivJsonConverterReadValueInteger(pJToken, pFieldInfo, pInstance);
                    }
                    else
                    {
                        PrivJsonConverterReadValueLong(pJToken, pFieldInfo, pInstance);
                    }
                }
                else if (pJToken.Type == JTokenType.Boolean)
                {
                    PrivJsonConverterReadValueBoolean(pJToken, pFieldInfo, pInstance);
                }
                else if (pJToken.Type == JTokenType.String)
                {
                    PrivJsonConverterReadValueString(pJToken, pFieldInfo, pInstance);
                }
                else if (pJToken.Type == JTokenType.Float)
                {
                    PrivJsonConverterReadValueFloat(pJToken, pFieldInfo, pInstance);
                }
                else if (pJToken.Type == JTokenType.Object)
                {
                    PrivJsonConverterReadValueObject(pJToken as JObject, pFieldInfo, pInstance);
                }
            }
        }

        private void PrivJsonConverterReadValueGeneric<VALUE>(VALUE pValue, FieldInfo pFieldInfo, object pInstance)
        {
            if (pFieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type[] aGenericType = pFieldInfo.FieldType.GetGenericArguments();
                if (aGenericType.Length > 0 && aGenericType[0] == typeof(VALUE))
                {
                    IList pListType = pFieldInfo.GetValue(pInstance) as IList;
                    if (pListType == null)
                    {
                        //Error!  �ش� ������ �������� ���� (�Ľ� ����)
                        return;
                    }
                    pListType.Add(pValue);
                }
            }
            else
            {
                //Error! List�̿��� �ڷᱸ���� ������� ����
            }
        }

        private void PrivJsonConverterReadValueInteger(JToken pReadToken, FieldInfo pFieldInfo, object pInstance)
        {
            int iValue = pReadToken.Value<int>();

            if (pFieldInfo.FieldType.IsGenericType)
            {
                PrivJsonConverterReadValueGeneric(iValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType == typeof(int))
            {
                pFieldInfo.SetValue(pInstance, iValue);
            }
        }

        private void PrivJsonConverterReadValueLong(JToken pReadToken, FieldInfo pFieldInfo, object pInstance)
        {
            long lValue = pReadToken.Value<long>();

            if (pFieldInfo.FieldType.IsGenericType)
            {
                PrivJsonConverterReadValueGeneric(lValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType == typeof(long))
            {
                pFieldInfo.SetValue(pInstance, lValue);
            }
        }

        private void PrivJsonConverterReadValueFloat(JToken pReadToken, FieldInfo pFieldInfo, object pInstance)
        {
            float fValue = pReadToken.Value<float>();

            if (pFieldInfo.FieldType.IsGenericType)
            {
                PrivJsonConverterReadValueGeneric(fValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType == typeof(float))
            {
                pFieldInfo.SetValue(pInstance, fValue);
            }
        }

        private void PrivJsonConverterReadValueBoolean(JToken pReadToken, FieldInfo pFieldInfo, object pInstance)
        {
            bool bValue = pReadToken.Value<bool>();

            if (pFieldInfo.FieldType.IsGenericType)
            {
                PrivJsonConverterReadValueGeneric(bValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType == typeof(bool))
            {
                pFieldInfo.SetValue(pInstance, bValue);
            }
        }

        private void PrivJsonConverterReadValueString(JToken pReadToken, FieldInfo pFieldInfo, object pInstance) // pReadToken�� "�״�����"
        {
            string strValue = pReadToken.Value<string>(); // strValue�� �״�����
            strValue = strValue.Replace(' ', '_');
            if (pFieldInfo.FieldType.IsGenericType)
            {
                PrivJsonConverterReadValueGeneric(strValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType == typeof(string))
            {
                pFieldInfo.SetValue(pInstance, strValue);
            }
            else if (pFieldInfo.FieldType.IsEnum) // pFieldInfo�� EWorldNation field�̰�, FieldType�� EEWorldNation�� enumŸ���̴�.
            {
                Type pFieldType = pFieldInfo.FieldType; // pFieldType�� EEWorldNation
                object pEnumObject = Enum.Parse(pFieldType, strValue); // strValue�� Enum���� ��ȯ
                                                                       // ��ȿ���� ���� strValue�� ������ ���ܸ� ��������, �츮�� �������� �޺��ڽ��� ó����..
                pFieldInfo.SetValue(pInstance, pEnumObject); // pInstance(StableItem) ��ü�� EWorldNation �ʵ尪�� �����ϰڴ�. EnumŸ������ ��ȯ�� strValue��
            }
        }

        private void PrivJsonConverterReadValueObject(JObject pReadToken, FieldInfo pFieldInfo, object pInstance)
        {
            if (pFieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                IList pListType = pFieldInfo.GetValue(pInstance) as IList;
                Type[] aGenericType = pFieldInfo.FieldType.GetGenericArguments();
                if (aGenericType.Length > 0)
                {
                    object pNewChild = Activator.CreateInstance(aGenericType[0]);
                    pListType.Add(pNewChild);
                    foreach (JProperty pJProperty in pReadToken.Children())
                    {
                        PrivJsonConverterReadValueProperty(pJProperty, pNewChild);
                    }
                }
            }
        }
    }
}