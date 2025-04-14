using UnityEditor;

[CustomPropertyDrawer(typeof(CDictionaryStringString))]
[CustomPropertyDrawer(typeof(CDictionaryIntColor))]
[CustomPropertyDrawer(typeof(CDictionaryStringGameObject))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

