using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ControlleurJeu))]
public class ControlleurJeuEditeur : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if (GUI.changed)
			((ControlleurJeu)target).InitialiserLigneActive();
	}
}
