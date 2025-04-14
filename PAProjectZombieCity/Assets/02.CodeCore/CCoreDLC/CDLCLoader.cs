using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CDLCLoader : CMonoBase
{
    public interface IDLCLoadAction
    {
        public void IPreviousLoadContents(UnityAction delFinish);
    }

    private bool m_bLoaded = false;
    //----------------------------------------------------
    public void DoDLCLoaderActivate(UnityAction delFinish)
    {
        PrivDLCLoaderActivate(delFinish);
    }

    //----------------------------------------------------------------
    private void PrivDLCLoaderActivate(UnityAction delFinish)
    {
        if (m_bLoaded) return;
        m_bLoaded = true;

        int iLoadCount = 0;

        IDLCLoadAction[] aDLCAction = GetComponentsInChildren<IDLCLoadAction>(true);
        for (int i = 0; i < aDLCAction.Length; i++)
        {
            aDLCAction[i].IPreviousLoadContents(()=> {
                iLoadCount++;
                if (iLoadCount >= aDLCAction.Length)
                {
                    delFinish.Invoke();
                }
            });
        }
    }
}
