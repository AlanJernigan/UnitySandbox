using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DimensionalGateController : MonoBehaviour {
	public enum DimensionalGateDirection {Both = 0, Real2Alternate = 1,Alternate2Real=2,OtherBoth=3,OtherReal2Alternate = 4,OtherAlternate2Real=5};

	public bool canTravenTrough = false;
	public bool displaced = false;
	public float renderOffset = 0f;
	public DimensionalGateController otherGate;
	public DimensionalGateDirection travelDirection = DimensionalGateDirection.Both;
	public bool matchOtherGateRotationAndSize = false;
	public LayerMask otherReflectsLayer;
	
	private List<Transform> ignoreObjects = new List<Transform>();
	private DimensionsAndPortalsController DAPController;

	public DimensionConfig pointsTowardsDimension;

	private Camera proCam;
	private RenderTexture proTexture;

	private bool canInit;

	// Use this for initialization
	void Start () {
		canInit = true;

		getDAPController();

		if (canTravenTrough == true) {
			if (!GetComponent<Collider>()) {
				Debug.LogWarning(name + ": Warning! You can't travel trough Dimensional Gates if there is no collider in the gate!");
			} else {
				if (!GetComponent<Collider>().isTrigger) {
					Debug.LogWarning(name + ": The attached collider must be set to trigger!");
				}
			}
		}

		if (getDAPController().renderMethod == DimensionsAndPortalsController.RenderMethod.Pro) {
			if (pointsTowardsDimension == null) {
				canInit = false;
				Debug.LogError(name + ": A dimensionalgate must at least have a dimension to point towards!");
			} else {
				if (canTravenTrough == true) {
					bool inDimensions = false;
					foreach(DimensionConfig dc in getDAPController().dimensions) {
						if (dc == pointsTowardsDimension) {
							inDimensions = true;
							break;
						}
					}

					if (inDimensions == false) {
						Debug.LogError(name + ": If a dimensionalgate points towards a dimenension and can be traveled trough it must be in the DAPController list in order to function properly!");
					}
				}
			}

			if (Application.isPlaying) {
				if (proCam != null) {
					DisablePro();
				}

				InitPro();
			}
		}


	}

	public void OnWillRenderObject()
	{
		if (Application.isPlaying) {	// Placeholder condition
			if (getDAPController().renderMethod == DimensionsAndPortalsController.RenderMethod.Pro) {
				if (proCam == null && Application.isPlaying) {
					InitPro();
				}
				if (proCam != null) {
					UpdateCamPosition();
					proCam.Render();
				}
			}
		}
	}

	void OnDisable() {
		DisablePro();
	}

	void DisablePro() {
		if (proCam != null) {
			Destroy(proCam.gameObject);
		}
	}

	
	void OnApplicationQuit() {
		DisablePro();
	}

	/* Init Pro Features */
	void InitPro() {
		DisablePro();

		if (canInit == true) {
			// Setup Camera
			GameObject cameraGO = GameObject.Instantiate(getDAPController().GetRealCam().gameObject) as GameObject;
			cameraGO.transform.parent = transform;
			proCam = cameraGO.GetComponent<Camera>();
			proCam.enabled = false;
			proTexture = new RenderTexture(proCam.pixelWidth,proCam.pixelHeight,16);
			proCam.targetTexture = proTexture;

			// Clean old switchers
			CamSwitcher[] switchers = proCam.GetComponents<CamSwitcher>();
			foreach(CamSwitcher swt in switchers) {
				DestroyImmediate(swt);
			}

			proCam.gameObject.AddComponent<CamSwitcher>();
			SetPointsTowardsDimension(pointsTowardsDimension);

			//CamSwitcher switcher = proCam.gameObject.AddComponent<CamSwitcher>();
			//switcher.SetDimensionSettings(pointsTowardsDimension);

			if (proCam.GetComponent<AudioListener>() != null) {
				Destroy(proCam.GetComponent<AudioListener>());
			}

			Material mat = Instantiate(getDAPController().dimensionPro) as Material;
			mat.SetTexture("_MainTex",proTexture);
			GetComponent<Renderer>().material = mat;
		}
	}

	public void SetPointsTowardsDimension(DimensionConfig dimPoint) {
		pointsTowardsDimension = dimPoint;
		proCam.GetComponent<CamSwitcher>().SetDimensionSettings(pointsTowardsDimension);
	}
	
	// Update is called once per frame
	void Update () {
		if (displaced == true) {
			if (otherGate == false) {
				GetComponent<Renderer>().enabled = false;
			} else {
				GetComponent<Renderer>().enabled = true;
			}
		} else {
			GetComponent<Renderer>().enabled = true;
		}
	}

	public void UpdateCamPosition() {
		Vector3 mainCamPos = getDAPController().GetRealCam().transform.position;

		if (displaced == false) { // Parallel: Plain copy paste main cam.
			proCam.transform.position = mainCamPos;
			proCam.transform.rotation = getDAPController().GetRealCam().transform.rotation;
		} else {
			proCam.transform.position = otherGate.transform.TransformPoint(transform.InverseTransformPoint(mainCamPos));
			proCam.transform.forward = otherGate.transform.TransformDirection(transform.InverseTransformDirection(getDAPController().GetRealCam().transform.forward));
		}

		proCam.nearClipPlane = Mathf.Max(0.01f,Vector3.Distance(transform.position,mainCamPos) - renderOffset);
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
	 * Detect if this is a solitary portal (overlapping scenes) or one that is connected to another one
	 */
	public bool isConnectedToOther() {
		if (displaced && otherGate && (travelDirection == DimensionalGateDirection.OtherBoth || travelDirection == DimensionalGateDirection.OtherAlternate2Real || travelDirection == DimensionalGateDirection.OtherReal2Alternate)) {
			return true;
		} else {
			return false;	
		}
	}
	
	/**
	 * Add an object to temporary ignore with OnTriggerEnter
	 */
	public void addIgnoreObject(Transform object2ignore) {
		ignoreObjects.Add(object2ignore);
	}
	
	/**
	 * Ignore the object for 1 seconds so OnTriggerEnter is not called after teleportation
	 */	
	IEnumerator removeIgnoredObject(Transform object2remove) {
		yield return new WaitForSeconds(1f);
		if (ignoreObjects.Contains(object2remove)) {
			ignoreObjects.Remove(object2remove);
		}
		yield return null;
	}
	
	/**
	 * Detect if a object that is portable passed through the portal
	 */
    void OnTriggerEnter(Collider other) {
		if (canTravenTrough == true) {
	    	if (other.tag == getDAPController().portableTag) {
				if (!ignoreObjects.Contains(other.transform)) {
					if(getDAPController().PortableSwitchesRealm(other.transform,this)) {	//Did the object travel through?
						//We can do something here if you like
						SendMessage("TransformWentThroughPortal",other.transform,SendMessageOptions.DontRequireReceiver);
					}
				} else {
					StartCoroutine(removeIgnoredObject(other.transform));
				}
			}
		}
    }

}
