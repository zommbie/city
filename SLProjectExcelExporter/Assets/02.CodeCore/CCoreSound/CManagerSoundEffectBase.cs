using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// BGM 페이드 인아웃 
// 각종 효과음 출력 

public abstract class CManagerSoundEffectBase : CManagerTemplateBase<CManagerSoundEffectBase>
{
	public enum ESoundPlayType
	{	
		Reset,			// 재생을 중지하고 다시 재생한다.
		PlayOneShot,    // 중복 재생을 한다.
		Exclusive,      // 재생중이면 재생하지 않는다
	}

	private class SSoundChannelInfo
	{
		public int				SoundChannelID;
		public AudioSource		AudioOutput;		
	}

	private class SSoundBGMInfo
	{
		public int ChannelID;
		public AnimationCurve FadeCurveIn = null;
		public AnimationCurve FadeCurveOut = null;
		public AudioSource    AudioOutput;
	}

	private Dictionary<int, SSoundChannelInfo>	m_mapSoundChannel = new Dictionary<int, SSoundChannelInfo>();
	private Dictionary<int, SSoundBGMInfo>		m_mapBGM = new Dictionary<int, SSoundBGMInfo>();
	//-----------------------------------------------------------------------------------------
	protected void ProtMgrSoundEffectChannelAdd(int hSoundChannelID,AudioSource pAudioSource)
	{
		if (m_mapSoundChannel.ContainsKey(hSoundChannelID) == false)
		{
			SSoundChannelInfo pSoundEffectGroup = new SSoundChannelInfo();
			pSoundEffectGroup.SoundChannelID = hSoundChannelID;
			pSoundEffectGroup.AudioOutput = pAudioSource;		
			m_mapSoundChannel.Add(hSoundChannelID, pSoundEffectGroup);
		}
	}

	//----------------------------------------------------------------------------------------
	protected void ProtMgrSoundEffecPlay(int SoundChannlID, AudioClip pPlayClip, ESoundPlayType ePlayType, float fVolume)
	{
		if (m_mapSoundChannel.ContainsKey(SoundChannlID))
		{
			SSoundChannelInfo pSoundChannel = m_mapSoundChannel[SoundChannlID];
			PrivMgrSoundEffectPlay(pSoundChannel, pPlayClip, ePlayType, fVolume);	
		}
	}
	//---------------------------------------------------------------------------------------
	private void PrivMgrSoundEffectPlay(SSoundChannelInfo pSoundChannelInfo, AudioClip pPlayClip, ESoundPlayType ePlayType, float fVolume)
	{      
        if (ePlayType == ESoundPlayType.Exclusive)
		{
			if (pSoundChannelInfo.AudioOutput.clip == pPlayClip)
			{
				if (pSoundChannelInfo.AudioOutput.isPlaying == false)
                {
					PrivMgrSoundEffectPlayClip(pSoundChannelInfo.AudioOutput, pPlayClip, fVolume);
                }
            }
			else
            {
				PrivMgrSoundEffectPlayClip(pSoundChannelInfo.AudioOutput, pPlayClip, fVolume);
			}
        }
		else if (ePlayType == ESoundPlayType.PlayOneShot)
		{
			pSoundChannelInfo.AudioOutput.PlayOneShot(pPlayClip);
		}
		else if (ePlayType == ESoundPlayType.Reset)
		{
			PrivMgrSoundEffectPlayClip(pSoundChannelInfo.AudioOutput, pPlayClip, fVolume);
		}
	}

	private void PrivMgrSoundEffectPlayClip(AudioSource pAudioSource, AudioClip pPlayClip, float fVolume)
    {
		pAudioSource.Stop();
		pAudioSource.clip = pPlayClip;
		pAudioSource.volume = fVolume;
		pAudioSource.Play();
    }
}
