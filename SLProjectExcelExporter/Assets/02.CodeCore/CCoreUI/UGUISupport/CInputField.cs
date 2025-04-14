using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 인풋 가이드 텍스트등 다양한 기능 추가 예정 
public class CInputField : InputField
{
    [SerializeField]
    private bool HideKeyBoard = false;  // 플렛폼별 입력 키보드 출력을 숨긴다 
										//------------------------------------------------------------
	protected override void Start()
	{
		if (HideKeyBoard)
		{
			keyboardType = (TouchScreenKeyboardType)(-1);
		}
		base.Start();
	}
}
