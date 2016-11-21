using UnityEngine;
using System.Collections;

public class RaycastTest : MonoBehaviour {

	public bool outputInConsole = false;
	public Collider targetCollider;
	public Transform testSphere;
	public Vector3 hitPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo;
		if (DAPPhysics.Raycast(transform.position,transform.forward,out hitInfo)) {
			hitPoint = hitInfo.point;
			targetCollider = hitInfo.collider;
			testSphere.position = hitInfo.point;

			if (outputInConsole == true) {
				Debug.Log(targetCollider);
			}
		}
	}
}
