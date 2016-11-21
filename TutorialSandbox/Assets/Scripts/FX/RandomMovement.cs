using UnityEngine;
using System.Collections;

public class RandomMovement : MonoBehaviour {
	
	public bool rotate = false;
	public bool occilate = false;
	public bool pulsate = false;
	public bool randomPhase = true;
	
	
	public Vector3 rotations = Vector3.zero;
	public Vector3 occilation = Vector3.zero;
	public Vector3 occilationSpeed = Vector3.zero;
	public Vector3 occilationPhase = Vector3.zero;
	
	public Vector3 pulsation = Vector3.zero;
	public Vector3 pulsationSpeed = Vector3.zero;
	public Vector3 pulsationPhase = Vector3.zero;
	
	protected Vector3 originalPos;
	protected Quaternion originalRot;
	protected Vector3 orgScale;
	
	protected float twoPi = (Mathf.PI*2);
	
	// Use this for initialization
	void Awake () {
		originalPos	= transform.localPosition;
		originalRot = transform.localRotation;
		orgScale = transform.localScale;
		
		twoPi = Mathf.PI*2;
		
		setRandomPhase();
	}
	
	public void setRandomPhase() {
		if (randomPhase == true) {
			transform.localRotation = originalRot;
			occilationPhase.x = Random.Range(0f,twoPi);
			occilationPhase.y = Random.Range(0f,twoPi);
			occilationPhase.z = Random.Range(0f,twoPi);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (rotate == true) {
			transform.Rotate(rotations*Time.deltaTime);
		}
		if (occilate == true) {
			transform.localPosition = originalPos + new Vector3(Mathf.Sin (occilationPhase.x)*occilation.x,Mathf.Sin (occilationPhase.y)*occilation.y,Mathf.Sin (occilationPhase.z)*occilation.z);
			
			occilationPhase.x += (Time.deltaTime * occilationSpeed.x);
			occilationPhase.y += (Time.deltaTime * occilationSpeed.y);
			occilationPhase.z += (Time.deltaTime * occilationSpeed.z);
			
			if (occilationPhase.x > twoPi) {
				occilationPhase.x = (occilationPhase.x-twoPi);
			} else if (occilationPhase.x < -twoPi) {
				occilationPhase.x = (occilationPhase.x+twoPi);
			}
			if (occilationPhase.y > twoPi) {
				occilationPhase.y = (occilationPhase.y-twoPi);
			} else if (occilationPhase.y < -twoPi) {
				occilationPhase.y = (occilationPhase.y+twoPi);
			}
			if (occilationPhase.z > twoPi) {
				occilationPhase.z = (occilationPhase.z-twoPi);
			} else if (occilationPhase.z < -twoPi) {
				occilationPhase.z = (occilationPhase.z+twoPi);
			}
		}
		
		if (pulsate == true) {
			
			transform.localScale = orgScale + new Vector3(Mathf.Max(-orgScale.x,Mathf.Sin (pulsationPhase.x)*pulsation.x),Mathf.Max(-orgScale.y,Mathf.Sin (pulsationPhase.y)*pulsation.y),Mathf.Max(-orgScale.z,Mathf.Sin (pulsationPhase.z)*pulsation.z));
			
			pulsationPhase.x += (Time.deltaTime * pulsationSpeed.x);
			pulsationPhase.y += (Time.deltaTime * pulsationSpeed.y);
			pulsationPhase.z += (Time.deltaTime * pulsationSpeed.z);
			
			if (pulsationPhase.x > twoPi) {
				pulsationPhase.x = (pulsationPhase.x-twoPi);
			} else if (pulsationPhase.x < -twoPi) {
				pulsationPhase.x = (pulsationPhase.x+twoPi);
			}
			if (pulsationPhase.y > twoPi) {
				pulsationPhase.y = (pulsationPhase.y-twoPi);
			} else if (pulsationPhase.y < -twoPi) {
				pulsationPhase.y = (pulsationPhase.y+twoPi);
			}
			if (pulsationPhase.z > twoPi) {
				pulsationPhase.z = (pulsationPhase.z-twoPi);
			} else if (pulsationPhase.z < -twoPi) {
				pulsationPhase.z = (pulsationPhase.z+twoPi);
			}
		}
	}
}
