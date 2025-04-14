using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// BGM 페이드 인아웃 
// 각종 효과음 출력 

public abstract class CManagerSoundBase : CManagerTemplateBase<CManagerSoundBase>
{
    public enum ESoundPlayType
    {
        Reset,          // 재생을 중지하고 다시 재생한다.
        PlayOneShot,    // 중복 재생을 한다.
        Exclusive,      // 재생중이면 재생하지 않는다
    }

    private class SSoundChannelInfo
    {
        public int SoundChannelID;
        public AudioSource AudioOutput;
        public Coroutine VolChangeHandle;
        public float DefaultVolume;
    }

    [SerializeField]
    private AnimationCurve FadeIn = AnimationCurve.Linear(0, 0, 1f, 1f);
    [SerializeField]
    private AnimationCurve FadeOut = AnimationCurve.Linear(0, 0, 1f, 1f);
    [SerializeField]
    private float FadeInTime = 2f;
    [SerializeField]
    private float FadeOutTime = 2f;

    private Dictionary<int, SSoundChannelInfo> m_mapSoundChannel = new Dictionary<int, SSoundChannelInfo>();

    //-----------------------------------------------------------------------------------------
    protected override void OnUnityFixedUpdate()
    {

    }

    //---------------------------------------------------------------------------------------
    protected void ProtMgrSoundChannelAdd(int eSoundChannelType, AudioSource pAudioSource)
    {
        if (m_mapSoundChannel.ContainsKey(eSoundChannelType) == false)
        {
            SSoundChannelInfo pSoundEffectGroup = new SSoundChannelInfo();
            pSoundEffectGroup.SoundChannelID = eSoundChannelType;
            pSoundEffectGroup.AudioOutput = pAudioSource;
            pSoundEffectGroup.VolChangeHandle = null;
            pSoundEffectGroup.DefaultVolume = pAudioSource.volume;
            m_mapSoundChannel.Add(eSoundChannelType, pSoundEffectGroup);
        }
    }

    //----------------------------------------------------------------------------------------
    protected void ProtMgrSoundPlay(int eSoundChannelType, AudioClip pPlayClip, ESoundPlayType ePlayType, float fVolume, bool bLoop)  // 효과음들은 녹음된 볼륨이 다른 것들이 많음
    {
        if (m_mapSoundChannel.ContainsKey(eSoundChannelType))
        {
            SSoundChannelInfo pSoundChannel = m_mapSoundChannel[eSoundChannelType];
            PrivMgrSoundPlay(pSoundChannel, pPlayClip, ePlayType, fVolume, bLoop);
        }
    }
    
    protected void ProtMgrSoundChange(int eSoundChannelType, AudioClip pPlayClip, bool bInstant)
    {
        if (m_mapSoundChannel.ContainsKey(eSoundChannelType))
        {
            SSoundChannelInfo pSoundChannel = m_mapSoundChannel[eSoundChannelType];

            PrivMgrSoundChange(pSoundChannel, pPlayClip, bInstant, pSoundChannel.DefaultVolume);
        }
    }
    protected void ProtMgrSoundFadeInOut(int eSoundChannelType, bool bFadeIn, float fVolumeEnd, Action<AudioSource> delFinish)
    {
        if (m_mapSoundChannel.ContainsKey(eSoundChannelType))
        {
            SSoundChannelInfo pSoundChannel = m_mapSoundChannel[eSoundChannelType];

            AnimationCurve pVolumeCurve = bFadeIn ? FadeIn : FadeOut;
            float fChangeTime = bFadeIn ? FadeInTime : FadeOutTime;

            PrivMgrSoundChangeVolume(pSoundChannel, pVolumeCurve, fChangeTime, pSoundChannel.AudioOutput.volume, fVolumeEnd, delFinish);
        }
    }
    protected void ProtMgrSoundResume(int eSoundChannelType)
    {
        if (m_mapSoundChannel.ContainsKey(eSoundChannelType))
        {
            SSoundChannelInfo pSoundChannel = m_mapSoundChannel[eSoundChannelType];
            PrivMgrSoundResume(pSoundChannel.AudioOutput);
        }
    }
    protected void ProtMgrSoundPause(int eSoundChannelType)
    {
        if (m_mapSoundChannel.ContainsKey(eSoundChannelType))
        {
            SSoundChannelInfo pSoundChannel = m_mapSoundChannel[eSoundChannelType];
            PrivMgrSoundPause(pSoundChannel.AudioOutput);
        }
    }

    //---------------------------------------------------------------------------------------
    private IEnumerator CoroutineChangeVolume(SSoundChannelInfo pChannelInfo, AnimationCurve pAnimCurve, float fTime, float fVolStart, float fVolEnd, Action<AudioSource> delFinish)
    {
        AudioSource pAudioSource = pChannelInfo.AudioOutput;
        float m_fChangeCurTime = 0;
        float fPercent = 0;
        float fCurveValue = 0;

        while (m_fChangeCurTime < fTime)
        {
            fPercent = m_fChangeCurTime / fTime;
            fCurveValue = pAnimCurve.Evaluate(fPercent);

            pAudioSource.volume = Mathf.Lerp(fVolStart, fVolEnd, fCurveValue);

            m_fChangeCurTime += Time.deltaTime;

            yield return null;
        }
        pAudioSource.volume = fVolEnd;
        pChannelInfo.VolChangeHandle = null;
        delFinish?.Invoke(pAudioSource);
    }

    private void PrivMgrSoundChangeVolume(SSoundChannelInfo pChannelInfo, AnimationCurve pAnimCurve, float fTime, float fVolStart, float fVolEnd, Action<AudioSource> delFinish)
    {
        if (pChannelInfo.VolChangeHandle != null)
            StopCoroutine(pChannelInfo.VolChangeHandle);

        pChannelInfo.VolChangeHandle = StartCoroutine(CoroutineChangeVolume(pChannelInfo, pAnimCurve, fTime, fVolStart, fVolEnd, delFinish));
    }

    private void PrivMgrSoundChange(SSoundChannelInfo pChannelInfo, AudioClip pPlayClip, bool bInstant, float fVolume)
    {
        AudioSource pAudioSource = pChannelInfo.AudioOutput;

        if (bInstant == true)
        {
            PrivMgrSoundPlayClip(pAudioSource, pPlayClip, fVolume, true);
            return;
        }

        if (pAudioSource.isPlaying)
        {
            PrivMgrSoundChangeVolume(pChannelInfo, FadeOut, FadeOutTime, pAudioSource.volume, 0, (AudioSource pAudioSource) =>
            {
                PrivMgrSoundPlayClip(pAudioSource, pPlayClip, pAudioSource.volume, true);
                PrivMgrSoundChangeVolume(pChannelInfo, FadeIn, FadeInTime, pAudioSource.volume, fVolume, null);
            });
        }
        else
        {
            PrivMgrSoundPlayClip(pAudioSource, pPlayClip, pAudioSource.volume, true);
            PrivMgrSoundChangeVolume(pChannelInfo, FadeIn, FadeInTime, pAudioSource.volume, fVolume, null);
        }
    }

    private void PrivMgrSoundPause(AudioSource pAudioSource)
    {
        pAudioSource.Pause();
    }

    private void PrivMgrSoundResume(AudioSource pAudioSource)
    {
        pAudioSource.UnPause();
    }

    private void PrivMgrSoundPlay(SSoundChannelInfo pSoundChannelInfo, AudioClip pPlayClip, ESoundPlayType ePlayType, float fVolume, bool bLoop)
    {
        if (ePlayType == ESoundPlayType.Exclusive)
        {
            if (pSoundChannelInfo.AudioOutput.clip == pPlayClip)
            {
                if (pSoundChannelInfo.AudioOutput.isPlaying == false)
                {
                    PrivMgrSoundPlayClip(pSoundChannelInfo.AudioOutput, pPlayClip, fVolume, bLoop);
                }
            }
            else
            {
                PrivMgrSoundPlayClip(pSoundChannelInfo.AudioOutput, pPlayClip, fVolume, bLoop);
            }
        }
        else if (ePlayType == ESoundPlayType.PlayOneShot)
        {
            pSoundChannelInfo.AudioOutput.PlayOneShot(pPlayClip);
        }
        else if (ePlayType == ESoundPlayType.Reset)
        {
            PrivMgrSoundPlayClip(pSoundChannelInfo.AudioOutput, pPlayClip, fVolume, bLoop);
        }
    }

    private void PrivMgrSoundPlayClip(AudioSource pAudioSource, AudioClip pPlayClip, float fVolume, bool bLoop)
    {
        pAudioSource.Stop();
        pAudioSource.clip = pPlayClip;
        pAudioSource.volume = fVolume;
        pAudioSource.loop = bLoop;
        pAudioSource.Play();
    }

    //-------------------------------------------------------------------------------------------
}
