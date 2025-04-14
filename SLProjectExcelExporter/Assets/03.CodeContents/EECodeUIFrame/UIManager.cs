using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

public class UIManager : CManagerUIFrameUsageBase
{
   

    //--------------------------------------------------------------------------------------------
    protected override void OnUnityAwake()
    {
        base.OnUnityAwake();

    }
    protected override void OnUnityStart()
    {
        base.OnUnityStart();

        UIManager.Instance.UIShow<UIFrameMain>();
    }

    //-------------------------------------------------------------
  
}
