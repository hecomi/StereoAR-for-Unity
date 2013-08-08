using UnityEngine;
using System.Collections;

public class MikuFaceDirection : MonoBehaviour {

	public GameObject Head   = null;
	public GameObject Target = null;
	
	public Vector3 RotationOffset = Vector3.zero;
	public GameObject SyncHead    = null;

	void Update () {
		if (Head == null || Target == null) return;
		
		if (SyncHead != null) {
			Head.transform.localRotation = SyncHead.transform.localRotation;
		} else {
			Head.transform.Rotate(-RotationOffset);
			Quaternion headRotFrom = Head.transform.rotation;
			Quaternion headRotTo   = Quaternion.LookRotation(Head.transform.position - Target.transform.position);
			Head.transform.rotation = Quaternion.Slerp(headRotFrom, headRotTo, 0.05f);
			Head.transform.Rotate(RotationOffset);
			
			// head angle limitation
			Vector3 localRot = Head.transform.localRotation.eulerAngles;
			float rotX = (localRot.x + 360) % 360;
			float rotY = (localRot.y + 360) % 360;
			float rotZ = (localRot.z + 360) % 360;
			const float rotLimit = 45;
			if (rotX > rotLimit && rotX <= 180) {
				rotX = rotLimit;
			} else if (rotX >= 180 && rotX < 360 - rotLimit) {
				rotX = 360 - rotLimit;
			}
			if (rotY > rotLimit && rotY <= 180) {
				rotY = rotLimit;
			} else if (rotY >= 180 && rotY < 360 - rotLimit) {
				rotY = 360 - rotLimit;
			}
			if (rotZ > rotLimit && rotZ <= 180) {
				rotZ = rotLimit;
			} else if (rotZ >= 180 && rotZ < 360 - rotLimit) {
				rotZ = 360 - rotLimit;
			}
			Head.transform.localEulerAngles = new Vector3(rotX, rotY, rotZ);
		}
		
	}
}
