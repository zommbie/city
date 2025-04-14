using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CEffectParticleBase : CEffectBase
{
    private List<ParticleSystem> m_listParticleSystem = new List<ParticleSystem>();
	private List<ParticleSystemRenderer> m_listParticleSystemRender = new List<ParticleSystemRenderer>();
	//---------------------------------------------------------------
	protected override void OnEffectUpdate(float fDelta)
	{
		base.OnEffectUpdate(fDelta);
        if (p_Duration == 0)
        {
            UpdeteEffectParticleExpire();
        }
    }

	protected override void OnEffectInitialize()
	{
		base.OnEffectInitialize();
		GetComponentsInChildren(true, m_listParticleSystem);
        GetComponentsInChildren(true, m_listParticleSystemRender);

		for (int i = 0; i < m_listParticleSystemRender.Count; i++)
		{
			m_listParticleSystemRender[i].sortingLayerID = SortingLayer.NameToID(LayerMask.LayerToName(gameObject.layer));
		}
	}

    protected override void OnEffectStartSinglePosition(params object[] aParams)
    {
		PrivEffectParticleStart();
	}

	protected override void OnEffectStart(float fDuration, params object[] aParams)
	{
		PrivEffectParticleStart();
	}
     

	protected override void OnEffectEnd(bool bForce)
	{
		base.OnEffectEnd(bForce);		
	}

	//-----------------------------------------------------------------
	private void UpdeteEffectParticleExpire()
	{
		bool bExpire = true;
		for (int i = 0; i < m_listParticleSystem.Count; i++)
		{
			if (m_listParticleSystem[i].isPlaying)
			{
				bExpire = false;
				break;
			}
		}

		if (bExpire)
		{
			DoEffectEnd();
		}
	}

	private void PrivEffectParticleStart()
	{
		for (int i = 0; i < m_listParticleSystem.Count; i++)
		{
			m_listParticleSystem[i].Clear();
			m_listParticleSystem[i].Play();
		}
	}

	//-------------------------------------------------------------------
	protected void ProtEffctParticleRendererOnOff(bool bOn)
    {
		for (int i = 0; i < m_listParticleSystemRender.Count; i++)
        {
			m_listParticleSystemRender[i].enabled = bOn;
        }
    }

    protected void ProtEffctParticleRendererOnOff(int iIndex, bool bOn)
    {
        if (iIndex < 0 && iIndex >= m_listParticleSystemRender.Count) return;
        m_listParticleSystemRender[iIndex].enabled = bOn;
    }

    protected void ProtEffectParticleLength(float fLength)
    {
         
        for(int i = 0; i < m_listParticleSystemRender.Count; i++)
        {
            ParticleSystem pParticleSystem = m_listParticleSystem[i];
            ParticleSystemRenderer pParticleRenderer = m_listParticleSystemRender[i];
            if (pParticleRenderer.renderMode == ParticleSystemRenderMode.Stretch)
            {
                float fSize = pParticleSystem.main.startSizeYMultiplier;
                pParticleRenderer.lengthScale =  fLength * fSize;      
            }            
        }
    }
}
 