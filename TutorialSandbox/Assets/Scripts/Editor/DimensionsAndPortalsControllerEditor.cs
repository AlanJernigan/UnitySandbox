using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DimensionsAndPortalsController))]
public class DimensionsAndPortalsControllerEditor : Editor {
	
	private GUIStyle warningStyle = new GUIStyle();
	private GUIStyle boldStyle = new GUIStyle();
	
	private SerializedObject dimObj;

	private bool showDefaultInspectorOptions = false;

	
	private SerializedProperty
		useLegacy,
		renderMethod,
		dimensions,
		personalMat,
		proMat,
		portalDisabledLayerInt,
		defaultLayerInt,
		alternateLayerInt,
		defaultAlternateLayerInt,
		portableTag,
		portalTag,
		portalLayer,
		portalMaskLayer,
		defaultLayers,
		defaultLayersReal,
		alternateLayers,
		alternateLayersReal,
		handleCamLayerSettings,
		useOculurRift;
	
	void OnEnable () {
		dimObj = new SerializedObject(target);
		useLegacy = dimObj.FindProperty("useLegacy");
		renderMethod = dimObj.FindProperty("renderMethod");
		dimensions = dimObj.FindProperty("dimensions");
		personalMat = dimObj.FindProperty("dimensionPersonal");
		proMat = dimObj.FindProperty("dimensionPro");
		portalDisabledLayerInt = dimObj.FindProperty("portalDisabledLayerInt");
		defaultLayerInt = dimObj.FindProperty("defaultLayerInt");
		alternateLayerInt = dimObj.FindProperty("alternateLayerInt");
		defaultAlternateLayerInt = dimObj.FindProperty("defaultAlternateLayerInt");
		portableTag = dimObj.FindProperty("portableTag");
		portalTag = dimObj.FindProperty("portalTag");
		portalLayer = dimObj.FindProperty("portalLayer");
		portalMaskLayer = dimObj.FindProperty("portalMaskLayer");
		defaultLayers = dimObj.FindProperty("defaultLayers");
		defaultLayersReal = dimObj.FindProperty("defaultLayersReal");
		alternateLayers = dimObj.FindProperty("alternateLayers");
		alternateLayersReal = dimObj.FindProperty("alternateLayersReal");
		handleCamLayerSettings = dimObj.FindProperty("handleCamLayerSettings");
		useOculurRift  = dimObj.FindProperty("useOculurRift");
		
		warningStyle.fontStyle = FontStyle.Bold;
		warningStyle.normal.textColor = Color.red;
		
		boldStyle.fontStyle = FontStyle.Bold;
	}
	
	
    public override void OnInspectorGUI ()
    {
		bool hasPro = UnityEditorInternal.InternalEditorUtility.HasPro();
		Debug.Log(hasPro);

		GUILayout.Label("Legacy Setup:",boldStyle);
		useLegacy.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Legacy Mode : ","Use old style DAP handling."), useLegacy.boolValue);
		if (useLegacy.boolValue == true) {
			EditorGUILayout.HelpBox("Upcoming versions may not have legacy support! \nPleas check carefully before updating!",MessageType.Warning);
		}

		GUILayout.Label("Basic settings:",boldStyle);
		GUILayout.Label("Uncheck 'DAP handles cameras' if you use portals!");
		handleCamLayerSettings.boolValue = EditorGUILayout.Toggle(new GUIContent("DAP handles cameras : ","Let DAP handle your camera settings (default). Uncheck if you use custom camera settings or use portals."), handleCamLayerSettings.boolValue);
		useOculurRift.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Oculus Rift: ","Special Oculus Rift settings."), useOculurRift.boolValue);
		if (useOculurRift.boolValue == true) {
			GUILayout.Label("Please check the manual how to\nproperly set up DAP with Oculus Rift.",warningStyle);
		}

		// Portal Tag
		portalTag.stringValue = EditorGUILayout.TagField(new GUIContent("Portal Tag:","Tags used for the objects that can be used as a portal."),portalTag.stringValue);
		
		//Portable Tag
		portableTag.stringValue = EditorGUILayout.TagField(new GUIContent("Portalable Tag:","Tags used for the objects that can pass trough portals."), portableTag.stringValue);

		if (useLegacy.boolValue == true) {
			GUILayout.Label("Legacy Layer settings:",boldStyle);
			portalLayer.intValue = EditorGUILayout.LayerField("Portal Layer:", portalLayer.intValue);
			portalDisabledLayerInt.intValue = EditorGUILayout.LayerField("Portal Disabled Layer:", portalDisabledLayerInt.intValue);
			portalMaskLayer.intValue = EditorGUILayout.LayerField("Portal Mask Layer:", portalMaskLayer.intValue);
			defaultLayerInt.intValue = EditorGUILayout.LayerField("Default Layer:", defaultLayerInt.intValue);
			alternateLayerInt.intValue = EditorGUILayout.LayerField("Alternate Layer:", alternateLayerInt.intValue);
			defaultAlternateLayerInt.intValue = EditorGUILayout.LayerField("Default & Alternate Layer:", defaultAlternateLayerInt.intValue);

			defaultLayers.intValue =  (EditorGUILayout.MaskField("Default World:",defaultLayers.intValue,UnityEditorInternal.InternalEditorUtility.layers));
			alternateLayers.intValue = EditorGUILayout.MaskField ("Alternate World:",alternateLayers.intValue,UnityEditorInternal.InternalEditorUtility.layers);
			
			defaultLayersReal.intValue = convertLayersToReal(defaultLayers.intValue);
			alternateLayersReal.intValue = convertLayersToReal(alternateLayers.intValue);
		} else {
			GUILayout.Label("Dimension settings:",boldStyle);
			bool usePro = false;
			if (hasPro == true) {
				if (renderMethod.enumValueIndex == 1) {
					usePro = true;
				}
				usePro = EditorGUILayout.Toggle(new GUIContent("Use PRO rendering : ","Use renderexture based rendering."), usePro);
			}

			if (usePro == false) {
				renderMethod.enumValueIndex = 0;
				EditorGUILayout.PropertyField(personalMat,true);
			} else {
				renderMethod.enumValueIndex = 1;
				EditorGUILayout.PropertyField(proMat,true);
			}

			portalLayer.intValue = EditorGUILayout.LayerField("Portal Layer:", portalLayer.intValue);

			GUILayout.Label("The first dimension is your main:");
			EditorGUILayout.PropertyField(dimensions,true);

			if (usePro == false) {
				if (hasPro == false && dimensions.arraySize > 2) {
					EditorGUILayout.HelpBox("Warning: In Personal you can only have two active dimensions at once! Use manual switching if you want more.",MessageType.Warning);
				}
			}
		}

		GUILayout.Label("Default Inspector:",boldStyle);
		showDefaultInspectorOptions = EditorGUILayout.Toggle(new GUIContent("Show Default Inspector Options: ","Shows more (and overlapping) options."), showDefaultInspectorOptions);
		if (showDefaultInspectorOptions == true) {
			DrawDefaultInspector();
		}
		
		dimObj.ApplyModifiedProperties();
		
    }
	
	
	/**
	 * Convert unity style maksfields to camera masks
	 */
	private int convertLayersToReal(int mask) {
		int returnMask = 0;
		if (mask == 0 ){					// Fake everything
			returnMask = -1;
		} else if (mask == -1) {			// Fake nothing
			returnMask = 0;
		} else {
			for (int i=0; i<UnityEditorInternal.InternalEditorUtility.layers.Length; i++) {
				if ((mask & (int) Mathf.Pow(2,i)) == Mathf.Pow(2,i)) {
					for (int j = 0; j < 32; j++)
					{
					    string name = UnityEditorInternal.InternalEditorUtility.GetLayerName(j);
					    if (name == UnityEditorInternal.InternalEditorUtility.layers[i])
					    {
							returnMask |=  1<<j;
							break;
						}
					}
				}
			}
		}
		
		return returnMask;
	}
}
