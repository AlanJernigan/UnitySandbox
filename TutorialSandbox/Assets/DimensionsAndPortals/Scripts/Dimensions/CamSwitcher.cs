using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamSwitcher : MonoBehaviour {
	
	public DimensionsAndPortalsController DAPController;
	public Transform playerTransform;
	public List<GameObject> GameObjectsToSwitchOn = new List<GameObject>();
	public List<GameObject> GameObjectsToSwitchOff = new List<GameObject>();

	public DimensionConfig dimensionConfig;

	private Camera cam;
	
	void Awake () {
		getDAPController();
		cam = GetComponent<Camera>();
	}

	public void SetDimensionSettings(DimensionConfig dimConfig, bool seePortals = false) {
		dimensionConfig = dimConfig;
		cam.cullingMask = dimensionConfig.layersIntReal;

		if (seePortals == false) {
			cam.cullingMask = ~(1 << getDAPController().portalLayer) & cam.cullingMask; // Prevent self rendering
		} else {
			cam.cullingMask = (1 << getDAPController().portalLayer) | cam.cullingMask; // Force self rendering
		}

		if (GetComponent<Skybox>() != null) {
			GetComponent<Skybox>().material = dimensionConfig.renderSettings.skybox;
		}

		listSwitcher();
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
	 * Create the list of items that are required to be switched.
	 */
	public void listSwitcher() {
		GameObjectsToSwitchOn.Clear();
		GameObjectsToSwitchOff.Clear();
		DepthSwitcher[] switchers = Object.FindObjectsOfTypeAll(typeof(DepthSwitcher)) as DepthSwitcher[];
		foreach (DepthSwitcher switcher in switchers) {
			if (switcher.hideFlags == HideFlags.None) {
				if ((cam.cullingMask & switcher.renderInLayers.value) != 0) {
					GameObjectsToSwitchOn.Add(switcher.gameObject);
				} else {
					GameObjectsToSwitchOff.Add(switcher.gameObject);
				}
			}
		}
	}
	
	/**
	 * Switch specific gameobjects that are lighting or effect related
	 */
	void OnPreCull () {
		if (getDAPController().renderMethod == DimensionsAndPortalsController.RenderMethod.Personal) {
			PreCullPersonal();
		} else if (getDAPController().renderMethod == DimensionsAndPortalsController.RenderMethod.Pro) {
			PreCullPro();
		}

		foreach(GameObject GO in GameObjectsToSwitchOn) {
			GO.SetActive(true);
		}
		foreach(GameObject GO in GameObjectsToSwitchOff) {
			GO.SetActive(false);
		}
	}

	public void PreCullPersonal() {

		setCameraNear();

		if (DAPController.renderSettingsHandler != null) {
			if (DAPController.useOculurRift == true) {	//OVR Support
				if (GetComponent<Camera>().transform.parent.GetComponent<Camera>() != null) {
					if (GetComponent<Camera>().transform.parent.tag == "CamNormal") {
						DAPController.renderSettingsHandler.SwitchToDefault();
					} else if (GetComponent<Camera>().transform.parent.tag == "CamAlternate") {
						DAPController.renderSettingsHandler.SwitchToAlternate();
					}
				}
			} else {
				if (cam == DAPController.rCam) {
					DAPController.renderSettingsHandler.SwitchToDefault();
				} else {
					DAPController.renderSettingsHandler.SwitchToAlternate();
				}
			}
		}
	}

	public void PreCullPro() {
		dimensionConfig.renderSettings.SetCurret();
	}


	/* Fog and other post-processing stuff */
	public void OnPreRender() {
		if (getDAPController() != null && getDAPController().renderMethod == DimensionsAndPortalsController.RenderMethod.Pro) {
			dimensionConfig.renderSettings.SetCurret();
		}
	}

	/**
	 * Unity 4 fix
	 */
	void OnPostRender() {
		if (getDAPController().renderMethod == DimensionsAndPortalsController.RenderMethod.Personal) {
			if (DAPController.useOculurRift == true) {	//OVR Support
				if (cam.depth == 1 || cam.depth == 2) {
					GL.ClearWithSkybox(false,cam);
				}
			} else {
				if (cam.depth == 1) {
					GL.ClearWithSkybox(false,cam);
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
	 * Set the camera near rendering as close as possible to the portal to prevent objects occluding if you are far away from a portal or dimensional rift
	 */
	void setCameraNear() {
		if (cam.depth == 0) {
			float closest = 1000f;
			foreach(DimensionalGateController dimGate in DAPController.activeGates) {
				//if (IsVisibleFrom(portal.renderer,camera)) {
					float distance = Vector3.Distance(transform.position,dimGate.transform.position);
				
					closest = Mathf.Min(distance-dimGate.renderOffset,closest);
				//}
			}
			closest = Mathf.Max(0.001f,closest);
			cam.nearClipPlane = closest;
		} else {
			cam.nearClipPlane = 0.3f;
		}
	}
}
