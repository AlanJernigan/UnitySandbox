using UnityEngine;
using System.Collections;

public class RenderSettingHandler : MonoBehaviour {

	public bool useCurrentRenderSettingsForDefault = true;

	public PseudoRenderSettings defaultRenderSettings = new PseudoRenderSettings();
	public PseudoRenderSettings alternateRenderSettings = new PseudoRenderSettings();
	public PseudoRenderSettings[] moreRenderSettings;

	[System.Serializable]
	public class PseudoRenderSettings {
		public bool fog;
		public Color fogColor;
		public FogMode fogMode;
		public float fogDensity;
		public float fogStartDistance;
		public float fogEndDistance;
		public Color ambientLight;
		public Material skybox; 
		public float haloStrength;
		public float flareFadeSpeed;
		public float flareStrength;

	}

	// Use this for initialization
	void Awake () {
		if (useCurrentRenderSettingsForDefault == true) {
			defaultRenderSettings.fog = RenderSettings.fog;
			defaultRenderSettings.fogColor = RenderSettings.fogColor;
			defaultRenderSettings.fogMode = RenderSettings.fogMode;
			defaultRenderSettings.fogDensity = RenderSettings.fogDensity;
			defaultRenderSettings.fogStartDistance = RenderSettings.fogStartDistance;
			defaultRenderSettings.fogEndDistance = RenderSettings.fogEndDistance;
			defaultRenderSettings.ambientLight = RenderSettings.ambientLight;
			defaultRenderSettings.skybox = RenderSettings.skybox;
			defaultRenderSettings.haloStrength = RenderSettings.haloStrength;
			defaultRenderSettings.flareFadeSpeed = RenderSettings.flareFadeSpeed;
			defaultRenderSettings.flareStrength = RenderSettings.flareStrength;
		}
	}

	public void SwitchToDefault() {
		SwitchRenderSetting(defaultRenderSettings);
	}

	public void SwitchToAlternate() {
		SwitchRenderSetting(alternateRenderSettings);
	}

	public void SwitchToMore(int moreId) {
		SwitchRenderSetting(moreRenderSettings[moreId]);
	}

	public void SwitchRenderSetting(PseudoRenderSettings pseudoRenderSettings) {
		RenderSettings.fog = pseudoRenderSettings.fog;
		RenderSettings.fogColor = pseudoRenderSettings.fogColor;
		RenderSettings.fogMode = pseudoRenderSettings.fogMode;
		RenderSettings.fogDensity = pseudoRenderSettings.fogDensity;
		RenderSettings.fogStartDistance = pseudoRenderSettings.fogStartDistance;
		RenderSettings.fogEndDistance = pseudoRenderSettings.fogEndDistance;
		RenderSettings.ambientLight = pseudoRenderSettings.ambientLight;
		RenderSettings.skybox = pseudoRenderSettings.skybox;
		RenderSettings.haloStrength = pseudoRenderSettings.haloStrength;
		RenderSettings.flareFadeSpeed = pseudoRenderSettings.flareFadeSpeed;
		RenderSettings.flareStrength = pseudoRenderSettings.flareStrength;
	}
}
