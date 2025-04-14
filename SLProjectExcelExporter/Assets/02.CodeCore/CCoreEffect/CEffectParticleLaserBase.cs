using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CEffectParticleLaserBase : CEffectParticleBase
{
    [SerializeField]
    private CEffectBase HitEffect;   protected CEffectBase GetEffectParticleLaserHitEffect() { return HitEffect; }

    private Vector3 m_vecDestPosition = Vector3.zero;
    //--------------------------------------------------------------------
    protected override void OnEffectStartDest(Vector3 vecDest, float fDuration, params object[] aParams)
    {
        base.OnEffectStartDest(vecDest, fDuration, aParams);        
        if (HitEffect != null)
        {
            HitEffect.DoEffectStartPosition(vecDest, Vector3.zero);
        }
        PrivEffectParticleLaserUpdate(vecDest);
    }

    protected override void OnEffectUpdate(float fDelta)
    {
        base.OnEffectUpdate(fDelta);
        if (m_vecDestPosition != Vector3.zero)
		{
			PrivEffectParticleLaserUpdate(m_vecDestPosition);
		}
	}

    protected override void OnEffectStartDirection(Vector3 vecDirection, float fLength, float fDuration, params object[] aParams)
    {
        base.OnEffectStartDirection(vecDirection, fLength, fDuration, aParams);
        Vector3 vecDest = transform.position + (vecDirection * fLength);

        if (HitEffect != null)
        {
            HitEffect.DoEffectStartPosition(vecDest, Vector3.zero);
        }
        PrivEffectParticleLaserUpdate(vecDest);
    }

    //-----------------------------------------------------------------
    public void SetEffectLaserTargetPosition(Vector3 vecDestPosition)
    {
        m_vecDestPosition = vecDestPosition;
    }

	public void SetEffectLaserLength(float fLength)
	{
        ProtEffectParticleLength(fLength);
    }

	//---------------------------------------------------------------------
	private void PrivEffectParticleLaserUpdate(Vector3 vecDest)    
    {
        m_vecDestPosition = vecDest;
        Vector3 vecDirection = m_vecDestPosition - transform.position;
        float fLength = Vector3.Distance(Vector3.zero, vecDirection);
        transform.LookAt(transform.position + vecDirection);
        if (HitEffect != null)
        {
            HitEffect.transform.position = m_vecDestPosition;
        }
        ProtEffectParticleLength(fLength);
    }
}
