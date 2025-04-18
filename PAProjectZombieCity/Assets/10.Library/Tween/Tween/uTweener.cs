
////////////////////////////////////////////////////////////////////////////////
//  
// @module  uTween for UGUI
// @author  Flamesky Dexive
// @support flamesky@live.com
//
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
 
/// <summary>
/// Base of tween
/// </summary>
namespace uTools {
	public abstract class uTweener : MonoBehaviour {

		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
		public EaseType easeType = EaseType.none;
		public LoopStyle loopStyle = LoopStyle.Once;
		public float delay = 0f;
		public float duration = 1f;
		public bool ignoreTimeScale = true;
		public bool EnableStart = false;
		public UnityEvent onFinished = null;
		

		float mAmountPerDelta = 1000f;
		float mDuration = 0f;
		float mStartTime = -1f;
		float mFactor = 0f;
        PlayDirection mDirection = PlayDirection.Forward;

		bool mActive = false; public bool IsAcive { get { return mActive; } }
		/// <summary>
		/// Gets or sets the factor.
		/// </summary>
		/// <value>The factor.</value>
		public float factor {
			get { return mFactor;}
			set { mFactor = Mathf.Clamp01(value);}
		}

		/// <summary>
		/// Gets the amount per delta.
		/// </summary>
		/// <value>The amount per delta.</value>
		public float amountPerDelta {
			get {
				if (mDuration != duration) {
					mDuration = duration;
					mAmountPerDelta = duration > 0 ? 1f/duration: 1000f;
				}
				return mAmountPerDelta;
			}
		}

		// Use this for initialization
		void Start () {
            if (EnableStart == true)
            {
                mActive = true;
                Update();
            }
        }
		
		// Update is called once per frame
		void Update () {
			if (mActive == false) return; 

			float delta = ignoreTimeScale?Time.unscaledDeltaTime : Time.deltaTime;
			float time = ignoreTimeScale?Time.unscaledTime : Time.time;
			if (mStartTime < 0) {
				mStartTime = time + delay;
			}
			if (time < mStartTime) return;
			mFactor += amountPerDelta * delta;
			if (loopStyle == LoopStyle.Loop) {
				if (mFactor > 1f) {
					mFactor -= Mathf.Floor(mFactor);
				}
			}
			else if (loopStyle == LoopStyle.PingPong) {
				if (mFactor > 1f) {
					mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
					mAmountPerDelta = - mAmountPerDelta;
				}
				else if (mFactor < 0f) {
					mFactor = -mFactor;
					mFactor -= Mathf.Floor(mFactor);
					mAmountPerDelta = - mAmountPerDelta;
				}
			}
			if ((loopStyle == LoopStyle.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f)) {
				mFactor = Mathf.Clamp01(mFactor);
				Sample(mFactor, true);
				//	enabled = false;//finished.set script enable
				mActive = false;
				if (onFinished != null) {
					onFinished.Invoke();
				}
//				if (eventRecevier != null && !string.IsNullOrEmpty(onFinished)) {
//					eventRecevier.Invoke(onFinished, 0);
//				}
			}
			else {
				Sample(mFactor, false);
			}
		}

		/// <summary>
		/// Sample the specified _factor and _isFinished.
		/// </summary>
		/// <param name="_factor">_factor.</param>
		/// <param name="_isFinished">If set to <c>true</c> _is finished.</param>
		public void Sample(float _factor, bool _isFinished) {
			float val = Mathf.Clamp01(_factor);
			val = (easeType == EaseType.none) ? animationCurve.Evaluate(val): EaseManager.EasingFromType(0, 1, val, easeType);
			OnUpdate(val, _isFinished);
		}

        public void SampleFromTime(float fTime, bool bFinished)
        {
            Sample(fTime * amountPerDelta, bFinished);
        }

		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="_factor">_factor.</param>
		/// <param name="_isFinished">If set to <c>true</c> _is finished.</param>
		protected virtual void OnUpdate(float _factor, bool _isFinished) {}


		private void OnEnable()
		{
			if (EnableStart) ResetPlay();
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset() {
			mActive = true;
			enabled = true;
			easeType = EaseType.none;
			loopStyle = LoopStyle.Once;
			delay = 0f;
			duration = 1f;
			ignoreTimeScale = true;
			//eventRecevier = null;
			onFinished = null;
			
			mAmountPerDelta = 1000f;
			mDuration = 0f;
			mStartTime = -1f;
			mFactor = 0f;
		}

		public void Stop()
		{
			gameObject.SetActive(false);
			ResetPlay();
			mActive = false;
			enabled = false;
		}

		public void ResetPlay()
		{
			gameObject.SetActive(true);
			mActive = true;
			enabled = true;

			mAmountPerDelta = 1000f;
			mDuration = 0f;
			mStartTime = -1f;
			mFactor = 0f;
			Sample(mFactor, false);
		}

		/// <summary>
		/// Begin the specified _go and _duration.
		/// </summary>
		/// <param name="_go">_go.</param>
		/// <param name="_duration">_duration.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Begin<T>(GameObject _go, float _duration) where T : uTweener {
			T comp = _go.GetComponent<T>();
			if (comp == null) {
				comp = _go.AddComponent<T>();
			}
			comp.Reset();
			comp.duration = _duration;
			comp.enabled = true;
			return comp;
		}

		public void Play(PlayDirection dir = PlayDirection.Forward) {
            //-----------------------------------
			if (dir == PlayDirection.Forward)
			{
				mDuration = 0f;
				mFactor = 0f;
			}
			else if (dir == PlayDirection.Reverse)
			{
				mDuration = 0f;
				mFactor = 1f;				
			}
            mDirection = dir;
            Sample(mFactor, false);
            //-------------------------------------


            mActive = true;
			gameObject.SetActive(true);
			mAmountPerDelta = (dir == PlayDirection.Reverse)? - Mathf.Abs(amountPerDelta): Mathf.Abs(amountPerDelta);
			enabled = true;
		//	Update();
		}

		public void Toggle() {
			mAmountPerDelta *= -1;
			enabled = true;
		}


		[ContextMenu("Set 'From' to current value")]
		public virtual void SetStartToCurrentValue () {}
		
		[ContextMenu("Set 'To' to current value")]
		public virtual void SetEndToCurrentValue () {}
		
		[ContextMenu("Assume value of 'From'")]
		public virtual void SetCurrentValueToStart () {}
		
		[ContextMenu("Assume value of 'To'")]
		public virtual void SetCurrentValueToEnd () {}

	}
}