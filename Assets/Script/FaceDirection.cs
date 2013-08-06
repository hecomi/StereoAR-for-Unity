using UnityEngine;
using System.Collections;

public class FaceDirection : MonoBehaviour {

	private GameObject head_   = null;
	public string headTagName  = "MikuHead";
	public Camera targetCamera = null;
	
	public Vector3 rotationOffset = Vector3.zero;

	void Awake () {
		head_ = GameObject.FindWithTag(headTagName).gameObject;
		Debug.Log (head_);
	}
	
	void Update () {
		if (head_ == null || targetCamera == null) return;
		head_.transform.Rotate(-rotationOffset);
		Quaternion headRotFrom = head_.transform.rotation;
		Quaternion headRotTo   = Quaternion.LookRotation(head_.transform.position - targetCamera.transform.position);
		head_.transform.rotation = Quaternion.Slerp(headRotFrom, headRotTo, 0.05f);
		head_.transform.Rotate(rotationOffset);
	}
}
