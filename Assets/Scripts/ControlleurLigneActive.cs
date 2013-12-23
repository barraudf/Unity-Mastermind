using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ControlleurLigneActive : MonoBehaviour
{	
	private List<SpriteRenderer> _ListeSprites;
	
	public void Initialiser()
	{
		for (var i = transform.childCount - 1; i >= 0; i--)
		{
			Transform ancienEmplacement = transform.GetChild(i);
			if(Application.isEditor == true)
				DestroyImmediate(ancienEmplacement.gameObject);
			else
				Destroy(ancienEmplacement.gameObject);
		}

		_ListeSprites = new List<SpriteRenderer>(ControlleurJeu.Instance.TailleCodeSecret);

		for(int i = 0; i < ControlleurJeu.Instance.TailleCodeSecret; i++)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(ControlleurJeu.Instance.PrefabEmplacement);
			obj.name = "Emplacement_" + i.ToString();
			obj.transform.parent = transform;
			obj.transform.localPosition = Vector3.right * i * (ControlleurJeu.Instance.LargeurPion + ControlleurJeu.Instance.DistanceSeparationLigneActive);
			_ListeSprites.Add(obj.GetComponent<SpriteRenderer>());
		}
	}

	public Sprite[] LireCodeActuel()
	{
		List<Sprite> ret = new List<Sprite>(_ListeSprites.Count);
		for(int i = 0; i < _ListeSprites.Count; i++)
		{
			if(ControlleurJeu.Instance.PrefabEmplacement.GetComponent<SpriteRenderer>().sprite == _ListeSprites[i].sprite)
				return null;

			ret.Add(_ListeSprites[i].sprite);
		}

		return ret.ToArray();
	}
}
