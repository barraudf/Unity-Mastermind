using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ControlleurLigneActive : MonoBehaviour
{
	public enum enumOrientations { Horizontale, Verticale };
	public enumOrientations Orientation = enumOrientations.Horizontale;

	private ControlleurJeu _ControlleurJeu;
	private List<SpriteRenderer> _ListeSprites;
	
	public void Initialiser(ControlleurJeu controlleurJeu = null)
	{
		if(_ControlleurJeu == null && controlleurJeu == null)
			throw new UnityException("Impossible d'initialiser la ligne active, le controlleur de jeu n'a pas été initialisé");

		if(controlleurJeu != null)
			_ControlleurJeu = controlleurJeu;

		for (var i = transform.childCount - 1; i >= 0; i--)
		{
			Transform ancienEmplacement = transform.GetChild(i);
			if(Application.isEditor == true)
				DestroyImmediate(ancienEmplacement.gameObject);
			else
				Destroy(ancienEmplacement.gameObject);
		}

		_ListeSprites = new List<SpriteRenderer>(_ControlleurJeu.TailleCodeSecret);

		for(int i = 0; i < _ControlleurJeu.TailleCodeSecret; i++)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(_ControlleurJeu.PrefabEmplacement);
			obj.name = "Emplacement_" + i.ToString();
			obj.transform.parent = transform;
			if(Orientation == enumOrientations.Horizontale)
				obj.transform.localPosition = Vector3.right * (i * 64 + i * _ControlleurJeu.DistanceSeparationLigneActive);
			else
				obj.transform.localPosition = Vector3.down * (i * 64 + i * _ControlleurJeu.DistanceSeparationLigneActive);
			_ListeSprites.Add(obj.GetComponent<SpriteRenderer>());
		}
	}

	public Color[] LireCodeActuel()
	{
		List<Color> ret = new List<Color>(_ListeSprites.Count);
		for(int i = 0; i < _ListeSprites.Count; i++)
		{
			if(_ControlleurJeu.PrefabEmplacement.GetComponent<SpriteRenderer>().sprite == _ListeSprites[i].sprite)
				return null;

			ret.Add(_ListeSprites[i].color);
		}

		return ret.ToArray();
	}
}
