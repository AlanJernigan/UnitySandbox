using UnityEngine;
using System.Collections;

public static class DAPPhysics : object {
	
	public static Ray transformRay(Ray ray, Vector3 hit, DimensionalGateController DGC) {
		Transform sourceGate = DGC.transform;
		Transform targetGate = DGC.otherGate.transform;
		
		Vector3 targetPos = targetGate.TransformPoint(sourceGate.InverseTransformPoint(hit));
		Vector3 targetDir = targetGate.TransformDirection(sourceGate.InverseTransformDirection(ray.direction));
		
		return new Ray (targetPos, targetDir);
	}
	
	/* Not yet Implemented */
	public static Ray transformRay(Ray ray, Vector3 hit, PortalController portalController) {
		return ray;
	}
	
	public static bool Raycast (Ray ray) {
		RaycastHit hitInfo;
		return Raycast(ray, out hitInfo, Mathf.Infinity, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Ray ray, float distance) {
		RaycastHit hitInfo;
		return Raycast(ray, out hitInfo,Mathf.Infinity, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Ray ray, out RaycastHit hitInfo) {
		return Raycast(ray, out hitInfo, Mathf.Infinity, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Vector3 origin, Vector3 direction) {
		RaycastHit hitInfo;
		return Raycast(new Ray(origin,direction), out hitInfo, Mathf.Infinity, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Vector3 origin, Vector3 direction, float distance) {
		RaycastHit hitInfo;
		return Raycast(new Ray(origin,direction), out hitInfo, distance, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Ray ray, float distance, int layerMask) {
		RaycastHit hitInfo;
		return Raycast(ray, out hitInfo, distance, layerMask);
	}
	
	public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance) {
		return Raycast(ray, out hitInfo, distance, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo) {
		return Raycast(new Ray(origin,direction), out hitInfo, Mathf.Infinity, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask) {
		RaycastHit hitInfo;
		return Raycast(new Ray(origin,direction), out hitInfo, distance, layerMask);
	}
	
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance) {
		return Raycast(new Ray(origin,direction), out hitInfo, distance, Physics.DefaultRaycastLayers);
	}
	
	public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance = Mathf.Infinity , int layerMask = Physics.DefaultRaycastLayers) {
		bool gotHit = Physics.Raycast(ray,out hitInfo,distance,layerMask);
		
		if (gotHit == true) {
			
			if (hitInfo.transform.gameObject.tag == "Portal") {
				DimensionalGateController DGC = hitInfo.transform.GetComponent<DimensionalGateController>();
				if (DGC != null) { // TODO: Also check for direction!!!!
					if (distance != Mathf.Infinity) {
						distance -= Vector3.Distance(ray.origin,hitInfo.point);
					}
					#if UNITY_EDITOR
					Vector3 dir = hitInfo.point - ray.origin;
					Debug.DrawRay(ray.origin,dir,Color.green);
					#endif
					
					Ray newRay = transformRay(ray,hitInfo.point, DGC);
					return SecondCast(newRay, out hitInfo, distance, layerMask);
				} else {
					PortalController PC = hitInfo.transform.parent.GetComponent<PortalController>();
					if (PC != null) {
						if (distance != Mathf.Infinity) {
							distance -= Vector3.Distance(ray.origin,hitInfo.point);
						}
						
						#if UNITY_EDITOR
						Vector3 dir = hitInfo.point - ray.origin;
						Debug.DrawRay(ray.origin,dir,Color.green);
						#endif
						
						Ray newRay = transformRay(ray,hitInfo.point, PC);
						return SecondCast(newRay, out hitInfo, distance, layerMask);
					}
					
					#if UNITY_EDITOR
					Vector3 dirV = hitInfo.point-ray.origin;
					Debug.DrawRay(ray.origin,dirV,Color.red);
					#endif
					return false; // Faulty setup dimensional gate!
				}
			} else {
				#if UNITY_EDITOR
				Vector3 dir = hitInfo.point - ray.origin;
				Debug.DrawRay(ray.origin,dir,Color.green);
				#endif
				return true; // Non dimensional gate hit
			}
		} else {
			#if UNITY_EDITOR
			Vector3 dir = hitInfo.point-ray.origin;
			Debug.DrawRay(ray.origin,dir,Color.red);
			#endif
			return false; // No hit
		}
	}
	
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask) {
		return Raycast(new Ray(origin,direction), out hitInfo, distance, layerMask);
	}
	
	private static bool SecondCast(Ray newRay, out RaycastHit hitInfo,float distance,int layerMask) {
		
		// Exclude secondary portal colissions
		layerMask ^= 1 << LayerMask.NameToLayer("DAP_Portal");
		
		bool gotHit = Physics.Raycast(newRay,out hitInfo,distance,layerMask);
		if (gotHit == true) {
			#if UNITY_EDITOR
			Vector3 dir = hitInfo.point - newRay.origin;
			Debug.DrawRay(newRay.origin,dir,Color.green);
			#endif
			return true; // Hit a non-portal object
		} else {
			#if UNITY_EDITOR
			Debug.DrawRay(newRay.origin,newRay.direction*Mathf.Min(1000f,distance),Color.red);
			#endif
			return false; // No hit after gate
		}
	}
}
