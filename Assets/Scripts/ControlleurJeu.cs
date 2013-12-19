﻿using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ControlleurJeu : MonoBehaviour
{
	public Color[] CouleursPions = new Color[] { Color.blue, Color.red, Color.green, Color.cyan, Color.magenta, Color.yellow };
	public Color CouleurBienPlacee = Color.white;
	public Color CouleurMalPlacee = Color.black;
	public Sprite SpriteVerifBienPlacee;
	public Sprite SpriteVerifMalPlacee;
	public Sprite SpriteEmplacementVerif;
	[Range(2,8)]
	public int TailleCodeSecret = 4;
	public int NombreTentativesMax = 0;
	public float DistanceSeparationLignes = 10f;
	public float DistanceSeparationPions = 16f;
	public float DistanceSeparationVerifs = 0f;
	public float DistanceSeparationLigneActive = 16f;
	public GameObject PrefabPion;
	public GameObject PrefabVerif;
	public GameObject PrefabEmplacement;


	protected Color[] _CodeSecret = null;
	protected ControlleurLigneActive _LigneActive;
	protected GameObject _EmplacementHistorique;
	float _LargeurPion;
	float _HauteurPion;

	void Start()
	{
		InitialiserLigneActive();
		_CodeSecret = GenererCodeSecret();
		_EmplacementHistorique = (GameObject)GameObject.Find("EmplacementHistorique");

		if(_EmplacementHistorique == null)
			throw new UnityException("Impossible de trouver le GameObject EmplacementHistorique");

		if(PrefabPion == null)
			throw new UnityException("Le Prefab Pion doit etre initialisé");

		SpriteRenderer sr = PrefabPion.GetComponent<SpriteRenderer>();

		if(sr == null)
			throw new UnityException("Le Prefab Pion doit avoir un composant SpriteRenderer");
		_LargeurPion = sr.sprite.rect.width;
		_HauteurPion = sr.sprite.rect.height;
	}

	public Color[] GenererCodeSecret()
	{
		List<Color> retour = new List<Color>(TailleCodeSecret);

		for(int i = 0; i < TailleCodeSecret; i++)
		{
			int indexCouleurAleatoire = Mathf.RoundToInt(Random.value * (CouleursPions.Length - 1));
			retour.Add(CouleursPions[indexCouleurAleatoire]);
		}

		return retour.ToArray();
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(20, 20, 100, 30), "Vérifier Code") == true)
		{
			if(_LigneActive != null)
			{
				int b, m;
				Color[] codeActuel = _LigneActive.LireCodeActuel();

				if(codeActuel == null)
					return;

				if(VerifierCode( codeActuel, out b, out m) == true)
					Debug.Log("Gagné!");
				else
					Debug.Log("Perdu - B=" + b.ToString() + ",M=" + m.ToString());

				AjouterLigneHistorique(codeActuel, b, m);
			}
		}
	}

	public bool VerifierCode(Color[] CouleursPions, out int BienPlace, out int MalPlace)
	{
		int bienPlace = 0;
		int malPlace = 0;
		List<int> indexPropositionDejaTraite = new List<int>(TailleCodeSecret);
		List<int> indexSolutionDejaTraite = new List<int>(TailleCodeSecret);

		// Vérification des couleurs bien placées
		for(int i = 0; i < TailleCodeSecret; i++)
		{
			if(CouleursPions[i] == _CodeSecret[i])
			{
				bienPlace++;
				indexPropositionDejaTraite.Add(i);
				indexSolutionDejaTraite.Add(i);
			}
		}

		for(int i = 0; i < TailleCodeSecret; i++)
		{
			if(indexPropositionDejaTraite.Contains(i) == true)
				continue;

			for(int y = 0; y < TailleCodeSecret; y++)
			{
				if(indexSolutionDejaTraite.Contains(y) == true || i == y)
				   continue;

				if(CouleursPions[i] == _CodeSecret[y])
				{
					malPlace++;
					indexSolutionDejaTraite.Add(y);
					break;
				}
			}
		}

		MalPlace = malPlace;
		BienPlace = bienPlace;
		return BienPlace == TailleCodeSecret;
	}

	public void InitialiserLigneActive()
	{
		GameObject emplacementLignes = (GameObject)GameObject.Find("EmplacementLigneActive");
		ControlleurLigneActive ligne = emplacementLignes.GetComponent<ControlleurLigneActive>();
		ligne.Initialiser(this);
		_LigneActive = ligne;
	}

	public void AjouterLigneHistorique(Color[] code, int bienPlace, int malPlace)
	{
		int indexLigne = _EmplacementHistorique.transform.childCount;
		GameObject ligne = new GameObject("Ligne_" + indexLigne.ToString());
		for(int i = 0; i < code.Length; i++)
		{
			GameObject pion = (GameObject) GameObject.Instantiate(PrefabPion);
			pion.name = "Pion_" + i.ToString();
			pion.transform.parent = ligne.transform;
			pion.transform.localPosition = Vector3.right * i * (_LargeurPion + DistanceSeparationPions);
			pion.GetComponent<SpriteRenderer>().color = code[i];
		}
		_EmplacementHistorique.transform.position += Vector3.up * (_HauteurPion + DistanceSeparationLignes);
		ligne.transform.parent = _EmplacementHistorique.transform;
		ligne.transform.localPosition = Vector3.down * indexLigne * (_HauteurPion + DistanceSeparationLignes);

		AjouterVerif(ligne, bienPlace, malPlace);
	}

	public void AjouterVerif(GameObject ligne, int bienPlace, int malPlace)
	{
		int nombrePionsVerifParLigne = Mathf.CeilToInt(TailleCodeSecret / 2);
		float xDepartVerif = TailleCodeSecret * _LargeurPion + TailleCodeSecret * DistanceSeparationPions;
		float largeurVerif = SpriteEmplacementVerif.rect.width;
		float hauteurVerif = SpriteEmplacementVerif.rect.height;

		for(int i = 0; i < TailleCodeSecret; i++)
		{
			float x,y;
			if(i >= nombrePionsVerifParLigne)
			{
				y = hauteurVerif + DistanceSeparationVerifs;
				x = xDepartVerif + (i - nombrePionsVerifParLigne) * (largeurVerif + DistanceSeparationVerifs);
			}
			else
			{
				y = 0;
				x = xDepartVerif + i * (largeurVerif + DistanceSeparationVerifs);
			}

			GameObject pion = (GameObject) GameObject.Instantiate(PrefabVerif);
			pion.name = "Verif_" + i.ToString();
			pion.transform.parent = ligne.transform;
			pion.transform.localPosition = new Vector3(x, y, 0);
			if(i >= bienPlace + malPlace)
				pion.GetComponent<SpriteRenderer>().sprite = SpriteEmplacementVerif;
			else if(i >= bienPlace)
				pion.GetComponent<SpriteRenderer>().sprite = SpriteVerifMalPlacee;
			else
				pion.GetComponent<SpriteRenderer>().sprite = SpriteVerifBienPlacee;
		}
	}
}