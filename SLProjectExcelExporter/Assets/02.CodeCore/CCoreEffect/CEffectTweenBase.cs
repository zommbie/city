using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CEffectTweenBase : CEffectBase
{
	[SerializeField]
	private Jun_TweenRuntime TweenDecision;

	//--------------------------------------------------------------
	protected override void OnEffectInitialize()
	{
		TweenDecision.AddOnFinished(HandleEffectTweenEnd);
	}

	protected override void OnEffectActivate()
	{
		if (TweenDecision.awakePlay == false)
		{
			TweenDecision.Play();
		}
	}
	//-----------------------------------------------------------------
	public void HandleEffectTweenEnd()
	{
		DoEffectEnd();
	}
	//----------------------------------------------------------------
	protected void ProtEffectTweenTargetTransform(Transform pTargetTransform)
	{
		TweenDecision.SetTweenTargetTransform(pTargetTransform);
	}


}
