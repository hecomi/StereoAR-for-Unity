using UnityEngine;
using System.Collections;

public class MikuAppearMotion : MonoBehaviour {
	public GameObject AnotherMiku = null;
	
	public Vector3 AppearPositionFrom = Vector3.zero;
	public Vector3 AppearPositionTo   = Vector3.zero;
	public Vector3 AppearScaleFrom    = Vector3.zero;
	public Vector3 AppearScaleTo      = Vector3.zero;
	
	private Vector3 originalPosition_;
	private Vector3 originalScale_;
	
	public float   Velocity   = 0.02f;
	public float   Diff       = 30.0f;
	
	private bool endFlag_     = false;
	private bool startFlag_   = false;
	
	public void Appear() {
		if (startFlag_ == true) return;
		startFlag_ = true;
		
		// appear motion start
		transform.animation.CrossFade("Appear");
		transform.animation["Appear"].time = 0;
		AnotherMiku.GetComponent<MikuAppearMotion>().Appear();
	}
	
	public void Reset() {
		transform.localPosition = originalPosition_;
		transform.localScale    = originalScale_;
		startFlag_ = false;
		endFlag_   = false;
	}
	
	void Awake() {
		if (AnotherMiku == null) {
			Debug.LogError("Another Miku is not set!");	
		}
		
		AppearPositionTo += transform.localPosition;
		transform.localPosition += AppearPositionFrom;
		originalPosition_ = transform.localPosition;
		
		AppearScaleTo += transform.localScale;
		transform.localScale += AppearScaleFrom;
		originalScale_ = transform.localScale;
	}
	
	void Update() {
		if (startFlag_ == false || endFlag_ == true) return;
		
		transform.localPosition = Vector3.Slerp(transform.localPosition, AppearPositionTo, Velocity);
		transform.localScale    = Vector3.Slerp(transform.localScale,    AppearScaleTo,    Velocity);
		float diff = (AppearPositionTo - transform.localPosition).magnitude;
		if (diff < Diff) {
			animation.CrossFade("Idle");
			endFlag_ = true;
		}
	}
}
