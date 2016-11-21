using UnityEngine;
using System.Collections;

public class SimplePortalGateDetector : MonoBehaviour {
	
	public SimplePortalTransportController transportController;
	public bool earlyDetector = false;
	
	void OnTriggerEnter(Collider other) {
		if (transportController) {
			transportController.ManualTriggerEnter(other,earlyDetector);	
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (transportController && earlyDetector == false) {
			transportController.ManualTriggerExit(other);	
		}
	}
}
