using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DimensionConfig))]
public class DimensionConfigEditor : Editor {
	private SerializedObject dimObj;
	private bool showDefaultInspectorOptions = false;

	private SerializedProperty
		layersInt,
		layersIntReal,
		renderSettings;

	void OnEnable () {
		dimObj = new SerializedObject(target);
		layersInt = dimObj.FindProperty("layersInt");
		layersIntReal = dimObj.FindProperty("layersIntReal");
		renderSettings = dimObj.FindProperty("renderSettings");
	}

	public override void OnInspectorGUI ()
	{
		DimensionConfig dConfig = target as DimensionConfig;
		layersInt.intValue =  (EditorGUILayout.MaskField("Layers:",layersInt.intValue,UnityEditorInternal.InternalEditorUtility.layers));
		layersIntReal.intValue = convertLayersToReal(layersInt.intValue);

		if (GUILayout.Button("Clone RenderSetting from Scene")) {
			dConfig.renderSettings.CloneCurrent();
		}

			EditorGUILayout.PropertyField(renderSettings,true);

		DefaultInspector();
		
		dimObj.ApplyModifiedProperties();
	}

	void DefaultInspector() {
		showDefaultInspectorOptions = EditorGUILayout.Foldout(showDefaultInspectorOptions,"Default Inspector:");
		if (showDefaultInspectorOptions == true) {
			DrawDefaultInspector();
		}
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