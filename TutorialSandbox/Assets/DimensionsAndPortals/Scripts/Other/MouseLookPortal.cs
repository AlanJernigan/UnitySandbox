using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look Portal")]
public class MouseLookPortal : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	public float rotationY = 0F;
	
	public float zEase = 1f;

	void Update ()
	{
		Vector3 newLocalE = transform.localEulerAngles;

		if (newLocalE.z != 0) {
			newLocalE.z = Mathf.LerpAngle(newLocalE.z,0,(Time.deltaTime*zEase/360));
		}
		if (newLocalE.y != 0) {
			newLocalE.y = 0;
		}
		
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			newLocalE.x = -rotationY;
			
			transform.localEulerAngles = newLocalE;
		}
	}
	
	public void updatePortalRotation(Vector3 rotation) {
		rotationY = rotation.x;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
		transform.localEulerAngles = rotation;
	}
	
	public void modifyRotation() {
		if (axes == RotationAxes.MouseXAndY || axes == RotationAxes.MouseY) {
			rotationY = -transform.localEulerAngles.x;
			if (rotationY > maximumY) {
				rotationY -= 360;
			}

			if (rotationY < minimumY) {
				rotationY += 360;
			}
		}
	}

	public void SetRotationY(float yValue) {
		yValue *= -1;
		if (yValue > 90) {
			yValue -= 360;
		}
		if (yValue < -90) {
			yValue += 360;
		}

		rotationY = yValue;
	}



	void Awake ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
}