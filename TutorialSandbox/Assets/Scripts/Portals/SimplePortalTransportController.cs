using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* -----------------------------------------------------------------------------------------------------
	-- SimplePortalTransportController --
	Requires -- SimplePortalGateDetector -- 
 
	This an EXAMPLE scipt that shows a setup for a simple portal Transport Controller that works for 
	portals in the most	basic setups.
	
	To achieve real physic effect you will have to use a physics based character controller (e.g. rigidbodies
	and custom control scripts). Since most of the 1st person projects are based on the unity3d character 
	controller, a script has been made to create portal like effects around this controller. Do note that this
	is not a 1:1 implementation from the portals used by Valve!
	
	So again: This has a basic setup for you to experiment with. This is NOT a full portal Implementation!
	This means you CAN trigger unwanted behaviour e.g. by shooting other portals while standing in one.
   ------------------------------------------------------------------------------------------------------ */

public class SimplePortalTransportController : MonoBehaviour {
	
	public PortalController portalController;
	[HideInInspector]
	public SimplePortalTransportController otherSPTController;
	private DimensionsAndPortalsController DAPController;
	public float portalExitVelocity = 1f;

	public float earlyDetectionTime = 0.1f; // Time in seconds until you will hit the portal to enable the disable trigger;
	
	// Use this for initialization
	void Awake () {
		if (portalController == null) {
			portalController = GetComponent<PortalController>();
			if (portalController == null) {
				Debug.Log("No PortalController Present in this GameObject: " + gameObject.name);
			}
		}
		
		getDAPController();
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
	 * Get the SimplePortalTransportController in a lazy way
	 */
	private SimplePortalTransportController getOtherSPTController() {
		if (!otherSPTController) {
			getDAPController();
			if(portalController != null) {
				otherSPTController = portalController.otherController.GetComponent<SimplePortalTransportController>();
				if (otherSPTController == null) {
					Debug.Log("The other portal does not have a SimplePortalTransportController!");
				}
			}
		}
		
		return otherSPTController;
	}
	
	/**
	 * When we enter the portal collider
	 */
	public void ManualTriggerEnter(Collider other, bool earlyDetector = false) {

		// Is the collider portable?
		if (other.tag == DAPController.portableTag) {
			// Is the collider not within the bounds of the portal? If it is, this is a second trigger (after transport)
			if (!portalController.portalBoxCollider.bounds.Contains(other.transform.position) ) {
				if (earlyDetector == false) {
					// Disable the attached objects for the object
					portalController.attachedToObject.gameObject.layer = DAPController.portalDisabledLayerInt;
					portalController.otherController.attachedToObject.gameObject.layer = DAPController.portalDisabledLayerInt;
					
					// If the collider is a player, enable the bounding box.
					if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
						portalController.portalBoxCollider.gameObject.SetActive(true);
						portalController.otherController.portalBoxCollider.gameObject.SetActive(true);
						portalController.portalBoxCollider.GetComponent<Renderer>().enabled = true;
						portalController.otherController.portalBoxCollider.GetComponent<Renderer>().enabled = true;
						
					}
				} else {

					if (other.GetComponents<PlayerPortalController>() != null) {
						CharacterController cc = other.GetComponent<CharacterController>();

						RaycastHit info;

						if (Physics.Raycast(other.transform.position,cc.velocity, out info, cc.velocity.magnitude*earlyDetectionTime,1<<DAPController.portalLayer)) {
							portalController.portalBoxCollider.gameObject.SetActive(true);
							portalController.portalBoxCollider.GetComponent<Renderer>().enabled = true;
							portalController.attachedToObject.gameObject.layer = DAPController.portalDisabledLayerInt;
							portalController.otherController.attachedToObject.gameObject.layer = DAPController.portalDisabledLayerInt;
						}
					} else {
						if (other.GetComponent<Rigidbody>() != null) {
							Debug.Log(other.GetComponent<Rigidbody>().velocity);
						}
					}
				}
			}	
		}
	}
	
	
	/**
	 * When we exit the portal collider
	 */
	public void ManualTriggerExit(Collider other) {
		// Is the object portable?
		if (other.tag == DAPController.portableTag) {
			if (portalController.portalBoxCollider.bounds.Contains(other.transform.position)) {					// We are leaving the portal
				
				Vector3 currentForward = other.transform.forward;
				
				// Do we have a player?
				if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
					HandlePlayerThroughPortal(other);
				// We have an object without a player
				} else {
					HandleOtherThroughPortal(other);
				}
				
				// In case of a rigidbody maintain velocity
				if (other.GetComponent<Rigidbody>()) {
					other.GetComponent<Rigidbody>().velocity = Quaternion.FromToRotation(currentForward,other.transform.forward) * other.GetComponent<Rigidbody>().velocity;
				}

			}
			
			if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
				portalController.portalBoxCollider.gameObject.GetComponent<Renderer>().enabled = false;
			} else if (other.gameObject.layer == LayerMask.NameToLayer("PortableObject")) {
				other.gameObject.layer = LayerMask.NameToLayer("Default");
			}

			if (portalController.attachedToObject != portalController.otherController.attachedToObject) {
				if (!portalController.portalBoxCollider.bounds.Contains(other.transform.position)) {
					portalController.portalBoxCollider.gameObject.SetActive(false);
					portalController.attachedToObject.gameObject.layer = DAPController.defaultLayerInt;
				}
				
				if (!portalController.otherController.portalBoxCollider.bounds.Contains(other.transform.position) ) {
					portalController.otherController.attachedToObject.gameObject.layer = DAPController.defaultLayerInt;
					portalController.otherController.portalBoxCollider.gameObject.SetActive(false);
				}
			} else {
				if (!portalController.portalBoxCollider.bounds.Contains(other.transform.position) && !portalController.otherController.portalBoxCollider.bounds.Contains(other.transform.position)) {
					portalController.otherController.attachedToObject.gameObject.layer = DAPController.defaultLayerInt;
					portalController.otherController.portalBoxCollider.gameObject.SetActive(false);
					portalController.portalBoxCollider.gameObject.SetActive(false);
				}
			}
				
		} 
	}
	
	/**
	 * Normal movement through portal (e.g. bullets)
	 */
	public void HandleOtherThroughPortal(Collider other) {
		Vector3 relativePos = transform.InverseTransformPoint(other.transform.position);
		Vector3 newPos = portalController.otherController.transform.TransformPoint(relativePos);
		
		Vector3 relativeDir = transform.InverseTransformDirection(other.transform.forward);
		Vector3 newDir = portalController.otherController.transform.TransformDirection(-relativeDir);
		
		other.transform.position = newPos;
		other.transform.forward = newDir;
	}
	
	/**
	 * Player movement through portal considering the camera and charactermotor
	 */
	public void HandlePlayerThroughPortal(Collider other) {
		PlayerPortalController pController = other.GetComponent<PlayerPortalController>();
		if (pController) {
			Vector3 currentForward = other.transform.forward;
			// Change position of the player (accounting for the camera)
			other.transform.position = portalController.otherController.ownCameraTrans.position - pController.playerCameraHolder.localPosition;

			MouseLookPortal pmLook = pController.playerCameraHolder.GetComponent<MouseLookPortal>();
			pmLook.transform.rotation = portalController.otherController.ownCameraTrans.rotation;
			pmLook.modifyRotation();

			// Update the player rotation (Yaw)
			Vector3 eulerDir = Quaternion.LookRotation(portalController.otherController.ownCameraTrans.forward).eulerAngles;
			Vector3 eulerY = eulerDir;
			eulerY.x = 0;
			eulerY.z = 0;
			other.transform.localEulerAngles = eulerY;
			
			// In case of a character controller (CharacterMotor) maintain velocity
			CharacterMotor cController = other.GetComponent<CharacterMotor>();
			if (cController) {
				float exitAngle = Vector3.Angle(portalController.otherController.transform.up,Vector3.up);

				Vector3 velocity = cController.movement.velocity;
				velocity = portalController.pgCalc.transform.InverseTransformDirection(velocity);
				Vector3 newVelocity = portalController.otherController.transform.TransformDirection(velocity);

				float angle = Vector3.Angle(Vector3.up,portalController.otherController.transform.forward);
				float boost = Mathf.Cos(Mathf.Abs(angle)*Mathf.Deg2Rad);
				if (angle > 90) {
					boost = 0f;
				}

				cController.movement.velocity = newVelocity;
				cController.movement.velocity += Vector3.up*boost*portalExitVelocity; 
			}
			
		} else {
			Debug.Log("Trying to portal a player without PlayerPortalController!");
		}	
	}
	
}
