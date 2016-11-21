using UnityEngine;
using System.Collections;

/* -----------------------------------------------------------------------------------------------------
	-- PlayerPortalController -- 
 
	This an EXAMPLE scipt that shows a setup for a simple portal shooting scipt.
   ------------------------------------------------------------------------------------------------------ */


public class PlayerPortalController : MonoBehaviour {
	
	public DimensionsAndPortalsController DAPController;
	
	public bool usePortalShootFx = true;
	
	public Transform playerCameraHolder;
	public Camera playerCamera;
	
	public GameObject lmbPortal;
	public GameObject rmbPortal;
	
	[HideInInspector]
	public PortalController lmbPortalController;
	[HideInInspector]
	public PortalController rmbPortalController;
	[HideInInspector]
	public Transform lmbPortalTransform;
	[HideInInspector]
	public Transform rmbPortalTransform;
	
	public ParticleSystem poralGunParticles;
	
	[HideInInspector]
	public int portalShootMask;
	
	// Use this for initialization
	void Awake () {
		getDAPController();
		portalShootMask = (1 << DAPController.defaultLayerInt);
	}
	
	// Update is called once per frame
	void Update () {	
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		PortalShooter();
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
	 * Simple shoot FX that is Portal Like
	 */
	void ShootFx(Color color) {
		if (usePortalShootFx == true) {
			for (int i = 0; i < 720; i++)
			{
				int offset = Random.Range(0,360);
				Vector3 position = poralGunParticles.transform.TransformPoint(new Vector3(1f * Mathf.Cos(Mathf.Deg2Rad * i + offset), 4f*(i/720f), 1f * Mathf.Sin(Mathf.Deg2Rad * i + offset)));
				poralGunParticles.Emit(position, poralGunParticles.transform.up*100f, Random.Range(0.01f,0.05f), 1f, color);
			}
		}
	}

	/**
	 * Simple portal shooter
	 */
	void PortalShooter() {
		if (Input.GetMouseButtonDown(0)) {
			ShootFx(new Color(0f,0.5f,1f));

			RaycastHit hit;
			if(Physics.Raycast (playerCamera.transform.position, playerCamera.transform.forward , out hit, 500,portalShootMask)) {
				if (hit.collider.tag == "CanHavePortal") {
					
					if (!lmbPortalTransform) {
						GameObject newLmbPortal = (GameObject) GameObject.Instantiate(lmbPortal,hit.point,Quaternion.LookRotation(hit.normal));
						lmbPortalTransform = newLmbPortal.transform;
						lmbPortalController = lmbPortalTransform.GetComponent<PortalController>();
						lmbPortalController.playerCamera = playerCamera.transform;
						lmbPortalController.Init();
					}
					
					lmbPortalTransform.forward = hit.normal;
					lmbPortalTransform.position = hit.point;

					if (lmbPortalTransform.gameObject.layer != DAPController.portalDisabledLayerInt) {
						lmbPortalTransform.gameObject.SetActive(true);

						if (lmbPortalController != null && lmbPortalController.attachedToObject != null) {
							lmbPortalController.attachedToObject.gameObject.layer = DAPController.defaultLayerInt;
						}

						if (canPlacePortal(lmbPortalController,hit.collider.transform,true) && (rmbPortalController == false || Vector3.Distance(lmbPortalTransform.position,rmbPortalTransform.position) > 4f)) {
							lmbPortalController.attachedToObject = hit.collider.transform;
							lmbPortalController.isEnabled = true;
							lmbPortalTransform.gameObject.SetActive(true);
						} else {
							lmbPortalController.isEnabled = false;
							lmbPortalTransform.gameObject.SetActive(false);
							lmbPortalTransform.position = new Vector3(0,-1000,0);
						}
					}
				}
				
			}
		} 
		if (Input.GetMouseButtonDown(1)) {
			ShootFx(new Color(1f,0.5f,0f));
			
			RaycastHit hit;
			if(Physics.Raycast (playerCamera.transform.position, playerCamera.transform.forward , out hit, 500,portalShootMask)) {
				if (hit.collider.tag == "CanHavePortal") {
					if (!rmbPortalTransform) {
						GameObject newRmbPortal = (GameObject) GameObject.Instantiate(rmbPortal,hit.point,Quaternion.LookRotation(hit.normal));
						rmbPortalTransform = newRmbPortal.transform;
						rmbPortalController = rmbPortalTransform.GetComponent<PortalController>();
						rmbPortalController.playerCamera = playerCamera.transform;
						rmbPortalController.Init();
					} 
					
					rmbPortalTransform.forward = hit.normal;
					rmbPortalTransform.position = hit.point;

					if (rmbPortalTransform.gameObject.layer != DAPController.portalDisabledLayerInt) {
						//Detect if face is in bounds
						rmbPortalTransform.gameObject.SetActive(true);

						if (rmbPortalController != null && rmbPortalController.attachedToObject != null) {
							rmbPortalController.attachedToObject.gameObject.layer = DAPController.defaultLayerInt;
						}

						if (canPlacePortal(rmbPortalController,hit.collider.transform,true) && (lmbPortalController == false || Vector3.Distance(lmbPortalTransform.position,rmbPortalTransform.position) > 3.5f)) {
							rmbPortalController.attachedToObject = hit.collider.transform;
							rmbPortalController.isEnabled = true;
							rmbPortalTransform.gameObject.SetActive(true);
						} else {
							rmbPortalController.isEnabled = false;
							rmbPortalTransform.gameObject.SetActive(false);
							rmbPortalTransform.position = new Vector3(0,-1000,0);
						}
					}
				}
			}
		}
		
		if (rmbPortalController != null && lmbPortalController != null) {
			rmbPortalController.otherController = lmbPortalController;
			lmbPortalController.otherController = rmbPortalController;
		}
	}
	
	/**
	 * Simple detection script if a portal can be placed
	 */
	bool canPlacePortal(PortalController portal,Transform targetTransform,bool useOffset) {
		Bounds frontBounds = portal.frontCollider.bounds;

		Vector3 min = frontBounds.min ;
		Vector3 max = frontBounds.max - (portal.frontCollider.transform.forward * 0.1f);
		
		if (useOffset == true) {
			min -= (portal.frontCollider.transform.forward * 0.1f);
			max -= (portal.frontCollider.transform.forward * 0.1f);
		}
			
		Vector3[] allBounds = new Vector3[8];
		
		allBounds[0] = min;
		allBounds[1] = max;
		allBounds[2] = new Vector3(min.x, min.y, max.z);
		allBounds[3] = new Vector3(min.x, max.y, min.z);
		allBounds[4] = new Vector3(max.x, min.y, min.z);
		allBounds[5] = new Vector3(min.x, max.y, max.z);
		allBounds[6] = new Vector3(max.x, min.y, max.z);
		allBounds[7] = new Vector3(max.x, max.y, min.z);
		
		foreach(Vector3 point in allBounds) {
			if (!targetTransform.GetComponent<Collider>().bounds.Contains(point)) {
				return false;	
			}
		}
		
		return true;
	}
}
