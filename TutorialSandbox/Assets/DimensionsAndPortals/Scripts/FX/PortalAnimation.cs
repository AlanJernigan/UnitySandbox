using UnityEngine;
 
public class PortalAnimation : MonoBehaviour 
{
	public float plasmaSpeed;
	public float noiseSpeed;
	public bool userRandomForNoise = false;
 
	private Material mMaterial;
	private float mTime;
	private float mTime2;

 
	// Use this for initialization
	void Awake () 
	{
		mMaterial = GetComponent<Renderer>().material;
		mTime = 0.0f;
		mTime2 = 0.0f;
	}
 
	// Update is called once per frame
	void Update () 
	{
		mTime += Time.deltaTime * plasmaSpeed;
		while (mTime > 1) {
			mTime = mTime - 1;
		}
		while (mTime < 0) {
			mTime = mTime + 1;
		}
		
		if (userRandomForNoise == true) {
			mTime2 = Random.Range(0f,1f);
		} else {
			mTime2 += Time.deltaTime * noiseSpeed;
			while (mTime2 > 1) {
				mTime2 = mTime2 - 1;
			}
			while (mTime2 < 0) {
				mTime2 = mTime2 + 1;
			}
		}
		
		mMaterial.SetFloat("_Offset",mTime);
		mMaterial.SetFloat("_Offset2",mTime2);
	}
}