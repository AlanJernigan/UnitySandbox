using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DimensionsAndPortalsController : MonoBehaviour {

	public enum RenderMethod {Personal = 0, Pro = 1};
	public RenderMethod renderMethod = RenderMethod.Personal;

	public bool useLegacy = true;

	public string portableTag = "Portable";
	public string portalTag = "Portal";

	public int portalDisabledLayerInt;

	public int defaultLayerInt;
	public int alternateLayerInt;
	public int defaultAlternateLayerInt;

	public int portalLayer;
	public int portalMaskLayer;
	public int defaultLayers;
	public int defaultLayersReal;
	public int alternateLayers;
	public int alternateLayersReal;
	
	public int realCamLayer;
	public int alternateCamLayer;
	
	public Camera rCam;
	public Camera aCam;

	public bool handleCamLayerSettings = true;
	public bool useOculurRift = false;
	
	public List<DimensionalGateController> activeGates = new List<DimensionalGateController>();

	public RenderSettingHandler renderSettingsHandler;

	public DimensionConfig[] dimensions;

	public Material dimensionPersonal;
	public Material dimensionPro;

	// Use this for initialization
	void Awake () {
		if (useLegacy == true) {
			AwakeLegacy();
		} else {
			if (dimensions.Length > 2 && renderMethod == RenderMethod.Personal) {
				Debug.LogError("Can't use more than 2 dimensions when using Unity Personal");
			}

			if (renderMethod == RenderMethod.Personal) {
				AwakePersonal();
			} else if (renderMethod == RenderMethod.Pro) {
				AwakePro();
			}
		}
	}

	void AwakeLegacy() {
		renderSettingsHandler = gameObject.GetComponent<RenderSettingHandler>();
		
		if (handleCamLayerSettings == true) {
			GameObject[] portables = GameObject.FindGameObjectsWithTag(portableTag);
			foreach(GameObject portable in portables) {
				Camera[] cameras = portable.GetComponentsInChildren<Camera>();
				foreach(Camera cam in cameras) {
					if (useOculurRift == true) {
						setupCamOVR(cam,portable.transform);
					} else {
						setupCamLegacy(cam,portable.transform);
					}
				}
			}
		}
		
		GameObject[] portals = GameObject.FindGameObjectsWithTag(portalTag);
		foreach(GameObject portal in portals) {
			DimensionalGateController dimGateController = portal.GetComponent<DimensionalGateController>();
			if (dimGateController) {
				AddGateToList(dimGateController);
			}
		}
	}

	void AwakePersonal() {
		renderSettingsHandler = gameObject.GetComponent<RenderSettingHandler>();
		
		if (handleCamLayerSettings == true) {
			GameObject[] portables = GameObject.FindGameObjectsWithTag(portableTag);
			foreach(GameObject portable in portables) {
				Camera[] cameras = portable.GetComponentsInChildren<Camera>();
				foreach(Camera cam in cameras) {
					setupCamPersonal(cam,portable.transform);
				}
			}
		}
		
		GameObject[] portals = GameObject.FindGameObjectsWithTag(portalTag);
		foreach(GameObject portal in portals) {
			DimensionalGateController dimGateController = portal.GetComponent<DimensionalGateController>();
			if (dimGateController) {
				AddGateToList(dimGateController);
			}
		}
	}

	void AwakePro() {
		if (handleCamLayerSettings == true) {
			rCam = Camera.main;
			rCam.depthTextureMode = DepthTextureMode.Depth;

			CamSwitcher switcher = rCam.gameObject.AddComponent<CamSwitcher>();
			switcher.SetDimensionSettings(dimensions[0],true);

		}
	}

	void Start() {

	}

	public Camera GetRealCam() {
		if (Application.isPlaying) {
			if (rCam != null) {
				return rCam;
			}
			
			return Camera.main;
		} else {
			return Camera.current;
		}
	}

	private void setupCamLegacy(Camera cam, Transform portable) {
		if (cam.depth == 0) {
			aCam = cam;
			cam.cullingMask = alternateLayersReal;
		}else if (cam.depth == 1) {
			rCam = cam;
			cam.cullingMask = defaultLayersReal;
		}

		CamSwitcher switcher = cam.gameObject.AddComponent<CamSwitcher>();
		switcher.listSwitcher();
		switcher.playerTransform = portable;

		// Pro feature support
		cam.depthTextureMode = DepthTextureMode.Depth;
	}

	private void setupCamPersonal(Camera cam, Transform portable) {
		CamSwitcher switcher = cam.gameObject.AddComponent<CamSwitcher>();
		if (cam.depth == 0) {
			aCam = cam;
			switcher.SetDimensionSettings(dimensions[1],false);
			cam.clearFlags = CameraClearFlags.Skybox;
		}else if (cam.depth == 1) {
			rCam = cam;
			switcher.SetDimensionSettings(dimensions[0],true);
			cam.clearFlags = CameraClearFlags.Depth;
		}

		switcher.playerTransform = portable;
		
		// Pro feature support
		cam.depthTextureMode = DepthTextureMode.Depth;
	}

	private void setupCamOVR(Camera cam, Transform portable) {
		if (cam.tag == "CamNormal") {
			cam.depth = 3;
			rCam = cam;
			CamSwitcher camSwitcher = cam.gameObject.AddComponent<CamSwitcher>();
			camSwitcher.playerTransform = portable;
			Camera[] cameras = cam.GetComponentsInChildren<Camera>();
			Debug.Log(cam);
			foreach(Camera subCam in cameras) {
				Debug.Log(subCam);
				if (subCam.name == "CameraLeft" || subCam.name == "LeftEyeAnchor") {
					subCam.cullingMask = defaultLayersReal;
					subCam.depth = 2;
					subCam.clearFlags = CameraClearFlags.Depth;
					subCam.renderingPath = RenderingPath.DeferredLighting;
					CamSwitcher switcher = subCam.gameObject.AddComponent<CamSwitcher>();
					switcher.playerTransform = portable;
				} else if (subCam.name == "CameraRight" || subCam.name == "RightEyeAnchor") {
					subCam.cullingMask = defaultLayersReal;
					subCam.depth = 1;
					subCam.clearFlags = CameraClearFlags.Depth;
					subCam.renderingPath = RenderingPath.DeferredLighting;
					CamSwitcher switcher = subCam.gameObject.AddComponent<CamSwitcher>();
					switcher.listSwitcher();
					switcher.playerTransform = portable;
					AudioListener listener = subCam.GetComponent<AudioListener>();
					if (listener != null) {
						listener.enabled = true;
					}
				}
				if (subCam.name == "CameraLeft" || subCam.name == "CameraRight") {
					subCam.depthTextureMode = DepthTextureMode.Depth;
				}
			}
		} else if (cam.tag == "CamAlternate") {
			cam.depth = 0;
			aCam = cam;
			CamSwitcher camSwitcher = cam.gameObject.AddComponent<CamSwitcher>();
			camSwitcher.playerTransform = portable;
			Camera[] cameras = cam.GetComponentsInChildren<Camera>();
			foreach(Camera subCam in cameras) {
				if (subCam.name == "CameraLeft") {
					subCam.cullingMask = alternateLayersReal;
					subCam.depth = -1;
					subCam.clearFlags = CameraClearFlags.Skybox;
					subCam.renderingPath = RenderingPath.DeferredLighting;
					CamSwitcher switcher = subCam.gameObject.AddComponent<CamSwitcher>();
					switcher.playerTransform = portable;
				} else if (subCam.name == "CameraRight") {
					subCam.cullingMask = alternateLayersReal;
					subCam.depth = -2;
					subCam.clearFlags = CameraClearFlags.Skybox;
					subCam.renderingPath = RenderingPath.DeferredLighting;
					CamSwitcher switcher = subCam.gameObject.AddComponent<CamSwitcher>();
					switcher.listSwitcher();
					switcher.playerTransform = portable;
					AudioListener listener = subCam.GetComponent<AudioListener>();
					if (listener != null) {
						listener.enabled = false;
					}
				}
				if (subCam.name == "CameraLeft" || subCam.name == "CameraRight") {
					subCam.depthTextureMode = DepthTextureMode.Depth;
				}
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (renderMethod == RenderMethod.Personal) {
			handleOtherDimensionalGatesPersonal();
		}
	}
	
	/**
	 * Add a gate to the list.
	 */
	public void AddGateToList(DimensionalGateController dimGateController) {
		if (!activeGates.Contains(dimGateController)) {
			activeGates.Add(dimGateController);	
		}
	}
	
	/**
	 * Remove a gate from the list.
	 */
	public void RemoveGateFromList(DimensionalGateController dimGateController) {
		if (activeGates.Contains(dimGateController)) {
			activeGates.Remove(dimGateController);	
		}
	}
	
	/**
	 * Update an object if it passes a portal that allows travel.
	 */
	public bool PortableSwitchesRealm(Transform portable,DimensionalGateController gateController) {
		// Check if the dimension direction is right
		DimensionalGateController.DimensionalGateDirection direction = gateController.travelDirection;
		
		if (direction == DimensionalGateController.DimensionalGateDirection.Both || direction == DimensionalGateController.DimensionalGateDirection.OtherBoth ||
			(portable.gameObject.layer == defaultLayerInt && (direction == DimensionalGateController.DimensionalGateDirection.Real2Alternate || direction == DimensionalGateController.DimensionalGateDirection.OtherReal2Alternate)) ||
			(portable.gameObject.layer != alternateLayerInt && (direction == DimensionalGateController.DimensionalGateDirection.Alternate2Real || direction == DimensionalGateController.DimensionalGateDirection.OtherAlternate2Real))) {
				
			// Move the object
			if(direction == DimensionalGateController.DimensionalGateDirection.OtherAlternate2Real |direction == DimensionalGateController.DimensionalGateDirection.OtherReal2Alternate || direction == DimensionalGateController.DimensionalGateDirection.OtherBoth) {
				Vector3 relativePos = gateController.transform.InverseTransformPoint(portable.position);
				Vector3 newPos = gateController.otherGate.transform.TransformPoint(relativePos);
				
				Vector3 relativeDir = gateController.transform.InverseTransformDirection(portable.forward);
				Vector3 newDir = gateController.otherGate.transform.TransformDirection(relativeDir);
				
				portable.position = newPos;
				portable.forward = newDir;
				gateController.otherGate.addIgnoreObject(portable);
				
				// In case of a rigidbody maintain velocity
				if (portable.GetComponent<Rigidbody>()) {
					Vector3 relativeVelocity = gateController.transform.InverseTransformDirection(portable.GetComponent<Rigidbody>().velocity);
					Vector3 newVelocity = gateController.otherGate.transform.TransformDirection(relativeVelocity);
					portable.GetComponent<Rigidbody>().velocity = newVelocity;
				}
				
				// In case of a character controller (CharacterMotor) maintain velocity
				CharacterMotor cController = portable.GetComponent<CharacterMotor>();
				if (cController) {
					Vector3 relativeVelocity = gateController.transform.InverseTransformDirection(cController.movement.velocity);
					Vector3 newVelocity = gateController.otherGate.transform.TransformDirection(relativeVelocity);
					cController.movement.velocity = newVelocity;
				}
			}

			if (portable.GetComponentInChildren<Camera>() != null) {
				if (useLegacy == true) {
					ChangeCamerasLegacy(portable);
				} else {
					if (renderMethod == RenderMethod.Personal) {
						ChangeCamerasPersonal(portable);
					} else if (renderMethod == RenderMethod.Pro) {
						ChangeCamerasPro(portable, gateController);
					}
				}
			}
			

			
			return true;
		}
		return false;
	}

	public void ChangeCamerasLegacy(Transform portable) {
		// Fix the layers of the object
		if (portable.gameObject.layer == defaultLayerInt) {
			portable.gameObject.layer = alternateLayerInt;
			if (aCam && rCam) {
				aCam.transform.position = rCam.transform.position;
				aCam.transform.rotation = rCam.transform.rotation;
			}
		} else {
			portable.gameObject.layer = defaultLayerInt;
			if (aCam && rCam) {
				rCam.transform.position = aCam.transform.position;
				rCam.transform.rotation = aCam.transform.rotation;
			}
		}
		
		// Fix the camera settings if the object has them
		Camera[] cameras = portable.GetComponentsInChildren<Camera>();
		if (cameras != null) {
			if (useOculurRift == true) {
				SwitchCamerasOVR(cameras);
			} else {
				SwitchCamerasNormal(cameras);
			}
		}
	}

	public void ChangeCamerasPersonal(Transform portable) {
		// Fix the layers of the object
		if (portable.gameObject.layer == defaultLayerInt) {
			portable.gameObject.layer = alternateLayerInt;
			if (aCam && rCam) {
				aCam.transform.position = rCam.transform.position;
				aCam.transform.rotation = rCam.transform.rotation;
			}
		} else {
			portable.gameObject.layer = defaultLayerInt;
			if (aCam && rCam) {
				rCam.transform.position = aCam.transform.position;
				rCam.transform.rotation = aCam.transform.rotation;
			}
		}
		
		// Fix the camera settings if the object has them
		Camera[] cameras = portable.GetComponentsInChildren<Camera>();
		if (cameras != null) {
			SwitchCamerasPersonal(cameras);
		}
	}

	public void ChangeCamerasPro(Transform portable, DimensionalGateController gateController) {
		DimensionConfig gateConfig = gateController.pointsTowardsDimension;

		if (gateController.otherGate == null) { // TODO: Check for direction and so!
			gateController.SetPointsTowardsDimension(GetRealCam().GetComponent<CamSwitcher>().dimensionConfig);

		}

		GetRealCam().GetComponent<CamSwitcher>().SetDimensionSettings(gateConfig,true);


	}

	/**
	 * Personal Cam Switching
	 */
	private void SwitchCamerasPersonal(Camera[] cameras) {
		foreach(Camera cam in cameras) {
			if (cam.depth == 0) {
				cam.cullingMask = (1 << portalLayer) | cam.cullingMask;
				cam.clearFlags = CameraClearFlags.Depth;
				cam.depth = 1;
				
				AudioListener listener = cam.GetComponent<AudioListener>();
				if (listener != null) {
					listener.enabled = true;	
				}
			}else if (cam.depth == 1) {
				cam.cullingMask = ~(1 << portalLayer) & cam.cullingMask;
				cam.clearFlags = CameraClearFlags.Skybox;
				cam.depth = 0;
				AudioListener listener = cam.GetComponent<AudioListener>();
				if (listener != null) {
					listener.enabled = false;	
				}
			}
		}
	}

	/**
	 * Legacy Cam Switching
	 */
	private void SwitchCamerasNormal(Camera[] cameras) {
		foreach(Camera cam in cameras) {
			if (cam.depth == 0) {
				cam.cullingMask = (cam.cullingMask | (1<<portalLayer));
				cam.clearFlags = CameraClearFlags.Depth;
				cam.depth = 1;
				
				AudioListener listener = cam.GetComponent<AudioListener>();
				if (listener != null) {
					listener.enabled = true;	
				}
			}else if (cam.depth == 1) {
				cam.cullingMask = ~(~cam.cullingMask | (1<<portalLayer));
				cam.clearFlags = CameraClearFlags.Skybox;
				cam.depth = 0;
				AudioListener listener = cam.GetComponent<AudioListener>();
				if (listener != null) {
					listener.enabled = false;	
				}
			}
		}
	}

	/**
	 * Oculus Rift Cam Switching
	 */
	private void SwitchCamerasOVR(Camera[] cameras) {
		foreach(Camera cam in cameras) {
			if (cam.tag == "CamNormal") {
				bool wasPrimary = false;
				if (cam.depth == 0) {
					cam.depth = 3;
				} else if (cam.depth == 3) {
					cam.depth = 0;
					wasPrimary = true;
				}
				Camera[] subCameras = cam.GetComponentsInChildren<Camera>();
				if (subCameras != null) {
					handleSwitchSubCams(subCameras,wasPrimary);
				}
			} else if (cam.tag == "CamAlternate") {
				bool wasPrimary = false;
				if (cam.depth == 0) {
					cam.depth = 3;
				} else if (cam.depth == 3) {
					cam.depth = 0;
					wasPrimary = true;
				}
				Camera[] subCameras = cam.GetComponentsInChildren<Camera>();
				if (subCameras != null) {
					handleSwitchSubCams(subCameras,wasPrimary);
				}
			}
		}
	}

	/**
	 * Oculus Rift Subcam Switching
	 */
	private void handleSwitchSubCams(Camera[] subCameras, bool wasPrimary) {
		foreach(Camera subCam in subCameras) {
			if (wasPrimary == true) {
				if (subCam.depth == 2) {
					subCam.depth = -1;
				} else if (subCam.depth == 1) {
					subCam.depth = -2;
				}
				if (subCam.depth == -1 || subCam.depth == -2) {
					subCam.cullingMask = ~(~subCam.cullingMask | (1<<portalLayer));
					subCam.clearFlags = CameraClearFlags.Skybox;
					AudioListener listener = subCam.GetComponent<AudioListener>();
					if (listener != null) {
						listener.enabled = false;	
					}
				}
			} else {
				if (subCam.depth == -1) {
					subCam.depth = 2;
				} else if (subCam.depth == -2) {
					subCam.depth = 1;
				}
				if (subCam.depth == 1 || subCam.depth == 2) {
					subCam.cullingMask = (subCam.cullingMask | (1<<portalLayer));
					subCam.clearFlags = CameraClearFlags.Depth;
					AudioListener listener = subCam.GetComponent<AudioListener>();
					if (listener != null) {
						listener.enabled = true;	
					}
				}
			}
		}
	}
	
	/**
	 * Detect if a certain renderen is visible from a camera
	 */
	public bool IsVisibleFrom(Renderer renderer, Camera camera) {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}
	
	/**
	 * PERSONAL VERSION: Update the camera position in regard to the nearest position of a gate of the right type
	 */
	void handleOtherDimensionalGatesPersonal() {
		if (rCam && aCam) {
			Transform playerTransform = rCam.GetComponent<CamSwitcher>().playerTransform;
			LayerMask playerMask = playerTransform.gameObject.layer;
			int intPlayerMask = 1<<playerMask.value;
			int intDefaultLayerMask = 1<<defaultLayerInt;
			int intAlternateLayerMask = 1<<alternateLayerInt;
			DimensionalGateController closestGate = null;
			Camera closestCam = null;
			float closest = rCam.farClipPlane;
			foreach(DimensionalGateController dimGateC in activeGates) {
				if (dimGateC.isConnectedToOther() || dimGateC.displaced == false) {
					if (dimGateC.otherReflectsLayer.value == 0 || dimGateC.otherReflectsLayer.value == intPlayerMask) { // Is the portal reflection the same as the player (or general)
						if (intPlayerMask == intDefaultLayerMask) {						// Is the player in the default world?
							if (IsVisibleFrom(dimGateC.GetComponent<Renderer>(),rCam)) { 									// In line of sight of the default camera
								float dist = Vector3.Distance(rCam.transform.position,dimGateC.transform.position);
								if (closest > dist) {
									closest = dist;
									closestGate = dimGateC;
									closestCam = rCam;
								}
							}
						} else if (intPlayerMask == intAlternateLayerMask) {				// Is the player in the alternate world?
							if (IsVisibleFrom(dimGateC.GetComponent<Renderer>(),aCam)) { 									// In line of sight of the alternate camera
								float dist = Vector3.Distance(aCam.transform.position,dimGateC.transform.position);
								if (closest > dist) {
									closest = dist;
									closestGate = dimGateC;
									closestCam = aCam;
								}
							}
						}
					}
				}
			}
			
			if (closestGate) {
				if (closestCam == aCam) {
					UpdateCamPosition(closestGate,aCam,rCam);
				} else if (closestCam == rCam) {
					UpdateCamPosition(closestGate,rCam,aCam);
				}
			}
		}
	}

	/**
	 * PRO VERSION: Update the camera position in regard to the nearest position of a gate of the right type
	 */
	void handleOtherDimensionalGatesPro() {
		if (rCam && aCam) {
			Transform playerTransform = rCam.GetComponent<CamSwitcher>().playerTransform;
			LayerMask playerMask = playerTransform.gameObject.layer;
			int intPlayerMask = 1<<playerMask.value;
			int intDefaultLayerMask = 1<<defaultLayerInt;
			int intAlternateLayerMask = 1<<alternateLayerInt;
			DimensionalGateController closestGate = null;
			Camera closestCam = null;
			float closest = rCam.farClipPlane;
			foreach(DimensionalGateController dimGateC in activeGates) {
				if (dimGateC.isConnectedToOther() || dimGateC.displaced == false) {
					if (dimGateC.otherReflectsLayer.value == 0 || dimGateC.otherReflectsLayer.value == intPlayerMask) { // Is the portal reflection the same as the player (or general)
						if (intPlayerMask == intDefaultLayerMask) {						// Is the player in the default world?
							if (IsVisibleFrom(dimGateC.GetComponent<Renderer>(),rCam)) { 									// In line of sight of the default camera
								float dist = Vector3.Distance(rCam.transform.position,dimGateC.transform.position);
								if (closest > dist) {
									closest = dist;
									closestGate = dimGateC;
									closestCam = rCam;
								}
							}
						} else if (intPlayerMask == intAlternateLayerMask) {				// Is the player in the alternate world?
							if (IsVisibleFrom(dimGateC.GetComponent<Renderer>(),aCam)) { 									// In line of sight of the alternate camera
								float dist = Vector3.Distance(aCam.transform.position,dimGateC.transform.position);
								if (closest > dist) {
									closest = dist;
									closestGate = dimGateC;
									closestCam = aCam;
								}
							}
						}
					}
				}
			}
			
			if (closestGate) {
				if (closestCam == aCam) {
					UpdateCamPosition(closestGate,aCam,rCam);
				} else if (closestCam == rCam) {
					UpdateCamPosition(closestGate,rCam,aCam);
				}
			}
		}
	}
	
	/**
	 * Update the camera posistion of the camera on the other side of the gate (only used with linked gates)
	 */
	void UpdateCamPosition(DimensionalGateController dimGate, Camera sourceCam,Camera targetCam) {
		if (sourceCam && targetCam) {
			
			Transform sourceGate = dimGate.transform;
			Transform targetGate  = sourceGate;

			DimensionalGateController otherGate = dimGate.otherGate;
			if (otherGate != null) {
				targetGate  = otherGate.transform;
				if(dimGate.matchOtherGateRotationAndSize == true) {
					// Gates must be equal in rotation and location!
					targetGate.rotation = sourceGate.rotation;
					targetGate.localScale = sourceGate.localScale;
				}
				
				Vector3 LocalPos = sourceGate.InverseTransformPoint(sourceCam.transform.position);
				Vector3 directionPos = sourceGate.InverseTransformDirection(sourceCam.transform.forward);
				
				targetCam.transform.position = targetGate.TransformPoint(LocalPos);
				targetCam.transform.forward = targetGate.TransformDirection(directionPos);
			}

			if (useOculurRift == true) {
				Camera[] subCameras = targetCam.GetComponentsInChildren<Camera>();
				foreach(Camera subCam in subCameras) {
					Debug.Log(subCam);
					if (subCam.tag != "CamNormal" && subCam.tag != "CamAlternate") {

						subCam.nearClipPlane = Mathf.Max(0.001f,Vector3.Distance(subCam.transform.position,targetGate.position)-dimGate.renderOffset);
					}
				}
			} else {
				targetCam.nearClipPlane = Mathf.Max(0.001f,Vector3.Distance(targetCam.transform.position,targetGate.position)-dimGate.renderOffset);
			}
		}
	}

}
