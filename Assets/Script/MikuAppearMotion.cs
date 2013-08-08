using UnityEngine;
using System.Collections;

public class MikuAppearMotion : MonoBehaviour {
	public Vector3 AppearPositionFrom = Vector3.zero;
	public Vector3 AppearPositionTo   = Vector3.zero;
	public Vector3 AppearScaleFrom    = Vector3.zero;
	public Vector3 AppearScaleTo      = Vector3.zero;
	public float   Velocity   = 0.02f;
	public float   Diff       = 30.0f;
	private bool endFlag_     = false;
	
	void Awake() {
		AppearPositionTo += transform.localPosition;
		transform.localPosition += AppearPositionFrom;
		
		AppearScaleTo += transform.localScale;
		transform.localScale += AppearScaleFrom;
		
		animation.CrossFade("Appear");
	}
	
	void Update() {
		if (endFlag_ == true) return;
		
		transform.localPosition = Vector3.Slerp(transform.localPosition, AppearPositionTo, Velocity);
		transform.localScale    = Vector3.Slerp(transform.localScale,    AppearScaleTo,    Velocity);
		float diff = (AppearPositionTo - transform.localPosition).magnitude;
		if (diff < Diff) {
			animation.CrossFade("Idle");
			endFlag_ = true;
		}
	}
}
