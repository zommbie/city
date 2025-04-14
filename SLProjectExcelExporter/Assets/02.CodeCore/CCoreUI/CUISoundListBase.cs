using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI 전용 사운드 리스트, BGM 은 메모리 관리등으로 인해 별도의 메니저로 분리되어 있으며
// 본 레이어는 버튼 사운드와 같은 공통 객체의 추상화를 위한것 

[RequireComponent(typeof(AudioSource))]
public abstract class CUISoundListBase : CMonoBase
{
    private Dictionary<int, AudioClip> m_mapAudioClip = new Dictionary<int, AudioClip>();
    //-----------------------------------------------------------
    internal AudioClip InterUISoundInstance(int eSoundID)
	{
        AudioClip pAudio = null;
        if (m_mapAudioClip.ContainsKey(eSoundID))
		{
            pAudio = m_mapAudioClip[eSoundID];
		}

        return pAudio;
	}

    //-----------------------------------------------------------
    protected void ProtUISoundListAdd(int eSoundID, AudioClip pAudioClip)
	{
        if (m_mapAudioClip.ContainsKey(eSoundID))
		{
            Debug.LogWarning($"[UI Sound] Duplicated Sound ID : {eSoundID}");
		}
        else
		{
            pAudioClip.LoadAudioData();
            m_mapAudioClip[eSoundID] = pAudioClip;
		}
	}

} 
