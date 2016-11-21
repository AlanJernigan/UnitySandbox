using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class PlasmaOrb : MonoBehaviour {
	public ParticleSystem particleSystem;

	public Mesh arcMesh;
	public int numArcs;
	public Vector2 spread;
	public Vector3 scale = Vector3.one;

	public Vector2 lifeTime = Vector2.one;
	public Color particleColor;


	public List<Arc> arcs = new List<Arc>();
	private List<Arc> arcsToDestroy = new List<Arc>();

	[System.Serializable]
	public class Arc {
		public string debugName = "debugName";
		public PlasmaOrb parentSystem;


		public Mesh mesh;

		public float liveTime = 1f;
		public float startTime;

		public Vector3 startPos;
		public Vector3 endPos;
		public float zDist;

		public bool mustDestroy = false;

		public Arc(float lifeTime, string arcDebugName,PlasmaOrb parentSys, Vector3 start, Vector3 end, float zDistance) {
			startTime = Time.time;
			debugName = arcDebugName;
			parentSystem = parentSys;
			startPos = start;
			endPos = end;
			zDist = zDistance;
			liveTime = lifeTime;

			Vector3 middlePos = ((startPos + endPos) / 2f);
			Vector3 zPos = middlePos.normalized * zDistance;

			float radX = Vector3.Distance(middlePos,startPos);
			float radY = Vector3.Distance(middlePos,zPos);

			int maxPoints = 30;
			for (int i=0;i<=maxPoints;i++) {
				Vector3 newPos = ArcPoint((float)i/(float)maxPoints,radX,radY);
				newPos = middlePos + Quaternion.FromToRotation(Vector3.down,middlePos.normalized)*newPos;
				parentSystem.particleSystem.Emit(newPos,Random.onUnitSphere,Random.Range(0.1f,1f),liveTime,parentSys.particleColor);
			}
		 }



		public Vector3 ArcPoint(float t,float radX,float radY) {
			float min = Mathf.PI*0.5f;
			float max = Mathf.PI*1.5f;
			float tVal = min + t*(max-min);

			Vector3 returnVal = Vector3.zero;

			returnVal.y = radY * Mathf.Cos (tVal);
			returnVal.z = radX * Mathf.Sin (tVal);
			return returnVal;
		}

		public bool mustDestroyArc() {
			if (Time.time > (startTime + liveTime)) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	void Awake () {
		setMesh();
		setParticleSystem();
	}

	void setMesh() {
		if (!arcMesh) {
			arcMesh = this.GetComponent<MeshFilter>().mesh;
		}
	}

	void setParticleSystem() {
		if (!particleSystem) {
			particleSystem = this.GetComponent<ParticleSystem>();
		}
	}


	void Update () {
		while (arcs.Count < numArcs) {
			Vector3 start = 8f * arcMesh.vertices[Random.Range(0,arcMesh.vertexCount)];
			Vector3 end = 8f * arcMesh.vertices[Random.Range(0,arcMesh.vertexCount)];
			float zDistance = Random.Range(spread.x,spread.y);

			Arc newArc = new Arc(Random.Range(lifeTime.x,lifeTime.y),"debugName",this,start,end,zDistance);
			arcs.Add(newArc);
		}

		foreach(Arc arc in arcs) {
			if (arc.mustDestroyArc() == true) {
				arcsToDestroy.Add(arc);
			}
		}

		foreach(Arc arcToDestroy in arcsToDestroy) {
			arcs.Remove(arcToDestroy);
			//Destroy(arcToDestroy);
		}

		arcsToDestroy.Clear();
	}
}
