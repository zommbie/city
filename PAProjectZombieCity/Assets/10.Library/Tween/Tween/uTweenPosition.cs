
////////////////////////////////////////////////////////////////////////////////
//  
// @module  uTween for UGUI
// @author  Flamesky Dexive
// @support flamesky@live.com
//
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTools {
	[AddComponentMenu("uTools/Tween/Tween Position(uTools)")]
	public class uTweenPosition : uTweener {

        public bool AnchorPosition = false;
		public Vector3 from;
		public Vector3 to;
		
		public RectTransform TweenTransform;
		
		public RectTransform cachedRectTransform { get { if (TweenTransform == null) TweenTransform = GetComponent<RectTransform>(); return TweenTransform;}}
		public Vector3 value {
			get { return cachedRectTransform.localPosition;}
			set { 
                if (AnchorPosition)
                {
                    cachedRectTransform.anchoredPosition = value;
                }
                else
                {
                    cachedRectTransform.localPosition = value;
                }
            }
		}
		
		protected override void OnUpdate (float factor, bool isFinished)
		{
			value = from + factor * (to - from);
		}
		
		public static uTweenPosition Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) {
			uTweenPosition comp = uTweener.Begin<uTweenPosition>(go, duration);
			comp.from = from;
			comp.to = to;
			comp.duration = duration;
			comp.delay = delay;
			if (duration <= 0) {
				comp.Sample(1, true);
				comp.enabled = false;
			}
			return comp;
		}
         
		[ContextMenu("Set 'From' to current value")]
		public override void SetStartToCurrentValue () { from = value; }
		
		[ContextMenu("Set 'To' to current value")]
		public override void SetEndToCurrentValue () { to = value; }
		
		[ContextMenu("Assume value of 'From'")]
		public override void SetCurrentValueToStart () { value = from; }
		
		[ContextMenu("Assume value of 'To'")]
		public override void SetCurrentValueToEnd () { value = to; }

	}
}
