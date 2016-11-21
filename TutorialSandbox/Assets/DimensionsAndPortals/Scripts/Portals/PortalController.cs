using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalController : MonoBehaviour {
	
	public bool isEnabled = false;
	public PortalController otherController;
	
	public Transform portalCenter;
	[HideInInspector]
	public Transform playerCamera;
	public Transform portalMask;
	public Transform gateFx;
	public Transform portalGate;
	public Transform pgCalc;
	
	public Collider portalBoxCollider;
	public Collider frontCollider;
	
	public Transform attachedToObject;
	
	[HideInInspector]
	public Camera ownCamera;
	[HideInInspector]
	public Transform ownCameraTrans;
	[HideInInspector]
	public PortalCamera portalCamController;
	[HideInInspector]
	public float renderDepth = -1;
	
	private DimensionsAndPortalsController DAPController;
	[HideInInspector]
	private Matrix4x4 m;
	//[HideInInspector]
	//private Matrix4x4 mO;
	
	[HideInInspector]
	public float m_ClipPlaneOffset = 0.07f;
	[HideInInspector]
	public Transform dummy;

	public void Init () {
		GameObject dummyGO =GameObject.Instantiate( new GameObject()) as GameObject;
		dummy = dummyGO.transform;
		dummy.parent = transform;
		dummy.name = "TransDummy";

		GameObject ownCameraGO = (GameObject) GameObject.Instantiate(playerCamera.gameObject);
		ownCameraTrans = ownCameraGO.transform;
		ownCameraTrans.parent = this.transform;
		ownCamera = ownCameraTrans.GetComponent<Camera>();
		ownCamera.layerCullSpherical = true;
		Destroy(ownCameraTrans.GetComponent<AudioListener>());
		portalCamController = ownCameraTrans.gameObject.AddComponent<PortalCamera>();
		portalCamController.portalController = this;
		ownCamera.layerCullSpherical = false;
		ownCamera.depth = renderDepth;
		ownCamera.name = name + " (Camera)";
		ownCamera.transform.localScale = Vector3.one;
		
		portalBoxCollider.gameObject.GetComponent<Renderer>().enabled = false;


		
		getDAPController();
	}
	
	/**
	 * Detect if a certain renderen is visible from a camera
	 */
	public bool IsVisibleFrom(Renderer renderer, Camera camera) {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}

	
	// Update is called once per frame
	void LateUpdate () {
		if (otherController && otherController.ownCameraTrans) {
			UpdateCameraPosAndRot();
			UpdateCameraMatrix();
			UpdateCameraOrder();

		} else {
			portalBoxCollider.gameObject.SetActive(false);
		}
	}

	
	/**
	 * Get the DimensionsAndPortalsController in a lazy way
	 */
	private DimensionsAndPortalsController getDAPController() {
		if (!DAPController) {
			DAPController = GameObject.FindObjectOfType(typeof(DimensionsAndPortalsController)) as DimensionsAndPortalsController;
			if (DAPController == null) {
				Debug.LogError("No DimensionsAndPortalsController found!");
			}
		}
		
		return DAPController;
	}
	
	/**
	 * Update the camera render order
	 */
	void UpdateCameraOrder() {
		if (otherController.ownCamera.nearClipPlane > ownCamera.nearClipPlane) {
			ownCamera.depth = -1;
			otherController.ownCamera.depth = -2;
		} else {
			ownCamera.depth = -2;
			otherController.ownCamera.depth = -1;
		}
	}

	/**
	 * Update the camera position and rotation
	 */
	public void UpdateCameraPosAndRot() {
		dummy.position = playerCamera.position;
		dummy.rotation = playerCamera.rotation;
		dummy.RotateAround(transform.position,transform.transform.up,180);
		
		otherController.ownCameraTrans.localRotation = dummy.localRotation;
		otherController.ownCameraTrans.localPosition = dummy.localPosition;
	}
	
	/**
	 * Update the camera matrix so it nicely aligns with the portal
	 */
	void UpdateCameraMatrix() {
		float clipPlaneDistance = 0.001f;
		
		otherController.ownCamera.nearClipPlane = Mathf.Max (clipPlaneDistance,Vector3.Distance(otherController.ownCameraTrans.position,otherController.portalCenter.position)-0.1f);
		otherController.ownCamera.enabled = true;
		
		if (IsVisibleFrom(gateFx.GetComponent<Renderer>(),playerCamera.GetComponent<Camera>())) {	// Can the main camera see the gate?
			otherController.ownCamera.ResetProjectionMatrix();
			otherController.ownCamera.enabled = true;
			
			
			m = Matrix4x4.Perspective(playerCamera.GetComponent<Camera>().fieldOfView,(float)Screen.width/(float)Screen.height,clipPlaneDistance,1000);
			if (portalBoxCollider.bounds.Contains(playerCamera.position) == false) {	// Are we in the portal?
	        	Vector4 clipPlane = CameraSpacePlane( otherController.ownCamera,clipPlaneDistance, otherController.gateFx.position, otherController.gateFx.forward, 1.0f );
	        	CalculateObliqueMatrix (ref m, clipPlane);
			}
			otherController.ownCamera.projectionMatrix = m;
			//mO = playerCamera.GetComponent<Camera>().projectionMatrix;
		} else {
			otherController.ownCamera.enabled = false;
		}
	}
	
	
	/**
	 * Mirroring and matrix manipulation functions below
	 */
    private Vector4 CameraSpacePlane (Camera cam,float clipPlaneDistance, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * clipPlaneDistance;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint( offsetPos );
        Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
        return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );

    }
	
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }
	
    private void CalculateObliqueMatrix (ref Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            sgn(clipPlane.x),
            sgn(clipPlane.y),
            1.0f,
            1.0f
        );

        Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));
		
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
	}
}