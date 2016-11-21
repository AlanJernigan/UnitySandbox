using UnityEngine;
using System.Collections;

public class PortalCamera : MonoBehaviour {
	
	public PortalController portalController;
	
	void OnPreCull() {
		portalController.portalMask.gameObject.SetActive(true);
		if (portalController.otherController && portalController.otherController.isEnabled == true) {
			portalController.otherController.attachedToObject.gameObject.SetActive(true);

		}
	}
	
	void OnPostRender() {	
		portalController.portalMask.gameObject.SetActive(false);
	}

}
