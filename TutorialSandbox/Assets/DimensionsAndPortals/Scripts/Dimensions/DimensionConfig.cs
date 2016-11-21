using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DimensionConfig : ScriptableObject {
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Dimension Config")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<DimensionConfig> ();
	}
	#endif

	[System.Serializable]
	public class DimensionRenderSettings {
		public Color ambientEquatorColor;
		public Color ambientGroundColor;
		public float ambientIntensity;
		public Color ambientLight;
		public AmbientMode ambientMode;
		public SphericalHarmonicsL2 ambientProbe;
		public Color ambientSkyColor;
		public Cubemap customReflection;
		public DefaultReflectionMode defaultReflectionMode;
		public int defaultReflectionResolution;
		public float flareFadeSpeed;
		public float flareStrength;
		public bool fog;
		public Color fogColor;
		public float fogDensity;
		public float fogEndDistance;
		public FogMode fogMode;
		public float fogStartDistance;
		public float haloStrength;
		public int reflectionBounces;
		public float reflectionIntensity;
		public Material skybox;

		public void CloneCurrent() {
			ambientEquatorColor = RenderSettings.ambientEquatorColor;
			ambientGroundColor = RenderSettings.ambientGroundColor;
			ambientIntensity = RenderSettings.ambientIntensity;
			ambientLight = RenderSettings.ambientLight;
			ambientMode = RenderSettings.ambientMode;
			ambientProbe = RenderSettings.ambientProbe;
			ambientSkyColor = RenderSettings.ambientSkyColor;
			customReflection = RenderSettings.customReflection;
			defaultReflectionMode = RenderSettings.defaultReflectionMode;
			defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
			flareFadeSpeed = RenderSettings.flareFadeSpeed;
			flareStrength = RenderSettings.flareStrength;
			fog = RenderSettings.fog;
			fogColor = RenderSettings.fogColor;
			fogDensity = RenderSettings.fogDensity;
			fogEndDistance = RenderSettings.fogEndDistance;
			fogMode = RenderSettings.fogMode;
			fogStartDistance = RenderSettings.fogStartDistance;
			haloStrength = RenderSettings.haloStrength;
			reflectionBounces = RenderSettings.reflectionBounces;
			reflectionIntensity = RenderSettings.reflectionIntensity;
			skybox = RenderSettings.skybox;
		}

		public void SetCurret() {
			RenderSettings.ambientEquatorColor = ambientEquatorColor;
			RenderSettings.ambientGroundColor = ambientGroundColor;
			RenderSettings.ambientIntensity = ambientIntensity;
			RenderSettings.ambientLight = ambientLight;
			RenderSettings.ambientMode = ambientMode;
			RenderSettings.ambientProbe = ambientProbe;
			RenderSettings.ambientSkyColor = ambientSkyColor;
			RenderSettings.customReflection = customReflection;
			RenderSettings.defaultReflectionMode = defaultReflectionMode;
			RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
			RenderSettings.flareFadeSpeed = flareFadeSpeed;
			RenderSettings.flareStrength = flareStrength;
			RenderSettings.fog = fog;
			RenderSettings.fogColor = fogColor;
			RenderSettings.fogDensity = fogDensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogMode = fogMode;
			RenderSettings.fogStartDistance = fogStartDistance;
			RenderSettings.haloStrength = haloStrength;
			RenderSettings.reflectionBounces = reflectionBounces;
			RenderSettings.reflectionIntensity = reflectionIntensity;
			RenderSettings.skybox = skybox;
		}
	}

	public DimensionRenderSettings renderSettings;

	public int layersInt = 0;
	public int layersIntReal = 0;

	public void CloneCurrent() {
		renderSettings.ambientEquatorColor = RenderSettings.ambientEquatorColor;
		renderSettings.ambientGroundColor = RenderSettings.ambientGroundColor;
		renderSettings.ambientIntensity = RenderSettings.ambientIntensity;
		renderSettings.ambientLight = RenderSettings.ambientLight;
		renderSettings.ambientMode = RenderSettings.ambientMode;
		renderSettings.ambientProbe = RenderSettings.ambientProbe;
		renderSettings.ambientSkyColor = RenderSettings.ambientSkyColor;
		renderSettings.customReflection = RenderSettings.customReflection;
		renderSettings.defaultReflectionMode = RenderSettings.defaultReflectionMode;
		renderSettings.defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
		renderSettings.flareFadeSpeed = RenderSettings.flareFadeSpeed;
		renderSettings.flareStrength = RenderSettings.flareStrength;
		renderSettings.fog = RenderSettings.fog;
		renderSettings.fogColor = RenderSettings.fogColor;
		renderSettings.fogDensity = RenderSettings.fogDensity;
		renderSettings.fogEndDistance = RenderSettings.fogEndDistance;
		renderSettings.fogMode = RenderSettings.fogMode;
		renderSettings.fogStartDistance = RenderSettings.fogStartDistance;
		renderSettings.haloStrength = RenderSettings.haloStrength;
		renderSettings.reflectionBounces = RenderSettings.reflectionBounces;
		renderSettings.reflectionIntensity = RenderSettings.reflectionIntensity;
		renderSettings.skybox = RenderSettings.skybox;
	}
}
