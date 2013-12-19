using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ControlleurLigneActive))]
public class ControlleurLigneActiveEditeur : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();
		
		if (GUI.changed)
			((ControlleurLigneActive)target).Initialiser();
	}
}
