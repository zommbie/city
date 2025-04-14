using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CDictionaryStringString : CSerializableDictionary<string, string> {}
[Serializable]
public class CDictionaryStringGameObject : CSerializableDictionary<string, GameObject> { }
[Serializable]
public class CDictionaryIntColor : CSerializableDictionary<int, Color> {}


 
