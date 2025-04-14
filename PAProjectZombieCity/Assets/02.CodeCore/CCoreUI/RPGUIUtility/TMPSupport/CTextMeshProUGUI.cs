using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class CTextMeshProUGUI : TextMeshProUGUI
{   private static StringBuilder g_TMPNote = new StringBuilder(1024);

    private event UnityAction OnTMPTextChanged = null;
    public override string text { get { return base.text; } set { base.text = value;} }
    //-------------------------------------------------------    
    public override void SetAllDirty()
    {
        base.SetAllDirty();

        if (OnTMPTextChanged != null)
        {
            OnTMPTextChanged.Invoke();
        }
    }

    public void SetTMPSubScriptTextEvent(UnityAction delEvent) { OnTMPTextChanged += delEvent; }
    public void SetTMPUnSubScriptTextEvent(UnityAction delEvent) { OnTMPTextChanged -= delEvent; }
}
