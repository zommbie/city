using UnityEngine;
using System.Collections.Generic;
using System;
#pragma warning disable CS0649
#pragma warning disable 649

// CObjectMonoBase 
// [개요] 유니티용 Mono 클레스 
// 1) 유니티 예약 함수를  Protected 가상 함수로 맵핑
// 2) 주로 컴포넌트에서 자주 쓰는 함수의 가상화로 유틸성을 증대하기 위한 목적이다.
// 3) SetActive는 브레이크 포인트가 걸리지 않아서 디버깅할때 불편함을 해소하기 위한 목적이다. 
 
abstract public class CMonoBase : MonoBehaviour
{
    private void Awake()        { OnUnityAwake();                           }
    private void Start()        { OnUnityStart();                           }
    private void OnEnable()     { OnUnityEnable();                          }
    private void OnDisable()    { OnUnityDisable();                         }
    private void OnDestroy()    { OnUnityDestroy();                         }
     
    public void SetMonoActive(bool Activate)
    {   
        gameObject.SetActive(Activate);  // 여기에 브레이크 포인트를 걸기 위해서이다.
    }

	public static void RemoveCloneObjectName(UnityEngine.Object pObject)
	{
        pObject.name = pObject.name.Replace("(Clone)", "").Trim();
	}

    public static string RemoveCloneObjectName(string strName)
	{
        return strName.Replace("(Clone)", "").Trim();
    }

    public TEMPATE GetComponentInChildOneDepth<TEMPATE>() where TEMPATE : Component
    {
        TEMPATE pComponent = null;
        int iTotal = transform.childCount;
        for (int i = 0; i < iTotal; i++)
        {
            pComponent = transform.GetChild(i).GetComponent<TEMPATE>();

            if (pComponent != null)
            {
                break;
            }
        }
        return pComponent;
    }

    public void GetComponentsInChildOneDepth<TEMPATE>(List<TEMPATE> listInstance, bool bActiveOnly = false) where TEMPATE : Component
    {
        int iTotal = transform.childCount;
        for (int i = 0; i < iTotal; i++)
        {
            TEMPATE pComponent = transform.GetChild(i).GetComponent<TEMPATE>();
            if (pComponent != null)
            {
                if (bActiveOnly)
				{
                    if (pComponent.gameObject.activeSelf == true)
					{
						listInstance.Add(pComponent);
					}
				}
                else
				{
					listInstance.Add(pComponent);
				}
			}
        }
    }

    //----------------------------------------------------------
    protected Transform FindTransformRoot(Transform pTarget)
    {
		Transform pResult = pTarget;
		if (pResult.parent != null)
        {
			pResult = FindTransformRoot(pResult.parent);
        }
		return pResult;
    }

    //--------------------------------------------------------
    protected virtual void OnUnityStart() { }
    protected virtual void OnUnityAwake() { }   
    protected virtual void OnUnityEnable() { }
    protected virtual void OnUnityDisable() { }
    protected virtual void OnUnityDestroy() { }
}
