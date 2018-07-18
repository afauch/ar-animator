using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CrayonMaterialGeneratorEditor : EditorWindow {

	string _hexValues;
	bool _emissive = false;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Crayon/HexToMaterial")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(CrayonMaterialGeneratorEditor));
	}

	void OnGUI()
	{
		GUILayout.Label ("Hex Values", EditorStyles.boldLabel);
		_hexValues = EditorGUILayout.TextArea(_hexValues, GUILayout.Height(position.height - 100));

		_emissive = EditorGUILayout.Toggle ("Emissive", _emissive);

		if(GUILayout.Button("Generate Materials"))
		{
			Debug.LogWarning (_hexValues);
			CrayonMaterialGenerator.GenerateMaterials (_hexValues, _emissive);
		}

	}

}
