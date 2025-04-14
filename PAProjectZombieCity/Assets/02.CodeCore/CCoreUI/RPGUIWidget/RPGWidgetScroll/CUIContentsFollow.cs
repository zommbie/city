using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 스크롤바 - 컨텐츠 패널을 따라 다닌다.
// 컨텐츠에 종속적인 객체의 레이어 문제를 해결하기 위하 고안되었다.

public class CUIContentsFollow : CUIWidgetBase
{
	[SerializeField]
	private RectTransform Follow;

	//------------------------------------------------------
	protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
	}

	protected virtual void Update()
	{
		UpdateFollowCopy();
	}
#if UNITY_EDITOR
	private void OnValidate()
	{
		UpdateFollowEditor();	
	}
#endif

	//--------------------------------------------------------
	public void DoUIContentsFollowUpdate()
	{
		UpdateFollowCopy();
	}

	//--------------------------------------------------------
	private void UpdateFollowCopy()
	{
		if (Follow == null) return;
		if (Follow.gameObject.activeInHierarchy == false) return;
		
		RectTransform pTransform = transform as RectTransform;
		pTransform.sizeDelta = Follow.sizeDelta;
		pTransform.offsetMin = Follow.offsetMin;
		pTransform.offsetMax = Follow.offsetMax;


		pTransform.anchorMin = Follow.anchorMin;
		pTransform.anchorMax = Follow.anchorMax;
		pTransform.anchoredPosition = Follow.anchoredPosition;
		pTransform.pivot = Follow.pivot;
	}

	private void UpdateFollowEditor()
	{
		if (Follow == null) return;
		if (Follow.gameObject.activeInHierarchy == false) return;

		gameObject.SetActive(true);
		StartCoroutine(CoroutineFollowRefresh());
	}

	private IEnumerator CoroutineFollowRefresh()
	{
		yield return new WaitForEndOfFrame();
		LayoutRebuilder.ForceRebuildLayoutImmediate(Follow);
		UpdateFollowCopy();
		yield break;
	}


}
