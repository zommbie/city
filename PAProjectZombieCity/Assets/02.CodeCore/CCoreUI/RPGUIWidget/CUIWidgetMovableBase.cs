using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uTools;

public abstract class CUIWidgetMovableBase : CUIWidgetBase
{
	[SerializeField]
	private uTweenPosition TweenPosition = null;
	[SerializeField]
	private uTweenScale TweenScale = null;
	[SerializeField]
	private uTweenAlpha TweenAlpha = null;

	private bool m_bMoveStart = false;
	//----------------------------------------------------------------
	protected override void OnUIWidgetShowHide(bool bShow)
	{
		base.OnUIWidgetShowHide(bShow);

		if (bShow)
		{
			PrivMovableTweenerPlay();
		}
	}

	private void Update()
	{
		if (m_bMoveStart)
		{
			if (CheckMovableTweenerFinish())
			{
				PrivMovableTweenerFinish();
			}
		}
	}

	//----------------------------------------------------------------
	private void PrivMovableTweenerPlay()
	{
		TweenPosition?.Play();
		TweenScale?.Play();
		TweenAlpha?.Play();
		m_bMoveStart = true;
		GetUIEntryParentsUIFrame().SetUIFrameInputEnable(false);
	}

	private void PrivMovableTweenerFinish()
	{
		m_bMoveStart = false;
		GetUIEntryParentsUIFrame().SetUIFrameInputEnable(true);
		OnWidgetMovableFinish();
	}

	private bool CheckMovableTweenerFinish()
	{
		if (TweenAlpha != null)
		{
			if (TweenAlpha.IsAcive)
			{
				return false;
			}
		}

		if (TweenPosition != null)
		{
			if (TweenPosition.IsAcive)
			{
				return false;
			}
		}

		if (TweenScale != null)
		{
			if (TweenScale.IsAcive)
			{
				return false;
			}
		}

		return true;
	}
	//------------------------------------------------------------------------------
	protected virtual void OnWidgetMovableFinish() { }
}
