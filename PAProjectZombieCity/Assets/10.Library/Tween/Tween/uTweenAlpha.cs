
////////////////////////////////////////////////////////////////////////////////
//  
// @module  uTween for UGUI
// @author  Flamesky Dexive
// @support flamesky@live.com
//
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace uTools {
    [AddComponentMenu("uTools/Tween/Tween Alpha(uTools)")]
    public class uTweenAlpha : uTweenValue
    {

        public bool includeChilds = false;
        public Transform TweenTransform;
        public Transform cachedTransform { get { if (TweenTransform == null) TweenTransform = GetComponent<Transform>(); return TweenTransform; } }

        float mAlpha = 0f;

        public float alpha
        {
            get
            {
                return mAlpha;
            }
            set
            {
                SetAlpha(value);
                mAlpha = value;
            }
        }

        private List<Graphic> m_listChildGraphic = new List<Graphic>();
        //-----------------------------------------------------------------------------------
        private void Awake()
        {
            if (includeChilds)
            {
                cachedTransform.gameObject.GetComponentsInChildren(true, m_listChildGraphic);
            }
            else
            {
                Graphic pGraphic = cachedTransform.gameObject.GetComponent<Graphic>();
                if (pGraphic != null)
                {
                    m_listChildGraphic.Add(pGraphic);
                }
            }
        }

        protected override void ValueUpdate(float value, bool isFinished)
        {
            alpha = value;
        }

        void SetAlpha(float fAlpha)
        {
            for (int i = 0; i < m_listChildGraphic.Count; i++)
            {
                Graphic pGraphic = m_listChildGraphic[i];
                Color rColor = pGraphic.color;
                rColor.a = fAlpha;
                pGraphic.color = rColor;
            }
        }

      
     
    }
}