using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ControlleurJeu : Singleton<ControlleurJeu>
{
	public Color CouleurNumeroLigne = Color.white;
	public Sprite[] ListePions;
	public Sprite SpriteVerifBienPlacee;
	public Sprite SpriteVerifMalPlacee;
	public Sprite SpriteEmplacementVerif;
	[Range(2,8)]
	public int TailleCodeSecret = 4;
	[Range(2,8)]
	public int NombreCouleurCodeSecret = 6;
	public int NombreTentativesMax = 0;
	public float DistanceSeparationLignes = 10f;
	public float DistanceSeparationPions = 16f;
	public float DistanceSeparationVerifs = 0f;
	public float DistanceSeparationLigneActive = 16f;
	public GameObject PrefabPion;
	public GameObject PrefabVerif;
	public GameObject PrefabEmplacement;
	public GameObject Surbrillance;
	public GameObject PrefabNumeroLigne;
	public GameObject PrefabPionActif;
	[HideInInspector]
	public float LargeurPion;
	[HideInInspector]
	public float HauteurPion;
	public GUISkin Skin;
	public Texture IconeBtnVerif;
	public EtatsPartie EtatPartie = EtatsPartie.Accueil;

	protected int[] _CodeSecret = null;
	protected ControlleurLigneActive _LigneActive;
	protected GameObject _EmplacementHistorique;
	private ControlleurJeu() {}
	private Rect _EmplacementBoutonValider;
	private BoxCollider2D _ZoneDefilable;
	private Vector3 _PositionHistoriqueOrigine;
	private float _MinDefilement;
	private float _MaxDefilement;

	public enum EtatsPartie { Accueil, PartieEnCours, Perdu, Gagne };

	public float MinDefilement
	{
		get { return _MinDefilement; }
	}
	
	public float MaxDefilement
	{
		get { return _MaxDefilement; }
	}

	public int Tentatives
	{
		get { return _EmplacementHistorique.transform.childCount; }
	}

	void Start()
	{
		_EmplacementHistorique = (GameObject)GameObject.Find("EmplacementHistorique");
		if(_EmplacementHistorique == null)
			throw new UnityException("Impossible de trouver le GameObject EmplacementHistorique");
		_ZoneDefilable = _EmplacementHistorique.GetComponent<BoxCollider2D>();
		_PositionHistoriqueOrigine = _EmplacementHistorique.transform.position;
		if(_ZoneDefilable == null)
			throw new UnityException("Le GameObject EmplacementHistorique ne possède pas de composant BoxCollider2d");

		if(PrefabPion == null)
			throw new UnityException("Le Prefab Pion doit etre initialisé");

		SpriteRenderer sr = PrefabPion.GetComponent<SpriteRenderer>();
		if(sr == null)
			throw new UnityException("Le Prefab Pion doit avoir un composant SpriteRenderer");
		LargeurPion = sr.sprite.rect.width;
		HauteurPion = sr.sprite.rect.height;

		GameObject emplacementLignes = (GameObject)GameObject.Find("EmplacementLigneActive");
		_LigneActive = emplacementLignes.GetComponent<ControlleurLigneActive>();

		MettreAZero();
	}

	public int[] GenererCodeSecret()
	{
		List<int> retour = new List<int>(TailleCodeSecret);

		for(int i = 0; i < TailleCodeSecret; i++)
		{
			int indexCouleurAleatoire = Mathf.RoundToInt(Random.value * (NombreCouleurCodeSecret - 1));
			retour.Add(indexCouleurAleatoire);
		}

		return retour.ToArray();
	}

	void OnGUI()
	{
		GUI.skin = Skin;
		switch(EtatPartie)
		{
		case EtatsPartie.Gagne:
                AfficherMenuPrincipal("Gagné !");
                break;
		case EtatsPartie.Perdu:
                AfficherMenuPrincipal("Perdu...");
                break;
		case EtatsPartie.Accueil:
                AfficherMenuPrincipal("Mastermind");
                break;
		case EtatsPartie.PartieEnCours:
				if(GUI.Button(_EmplacementBoutonValider, IconeBtnVerif ) == true)
				{
					if(_LigneActive != null)
					{
						int b, m;
						Sprite[] codeActuel = _LigneActive.LireCodeActuel();

						if(codeActuel == null)
							return;
						
						bool gagne = VerifierCode( codeActuel, out b, out m) == true;
						AjouterLigneHistorique(codeActuel, b, m);

                        if (gagne == true)
                        {
                            EtatPartie = EtatsPartie.Gagne;
                            MettreAZero();
                        }
                        else
                        {
                            if (NombreTentativesMax > 0 && Tentatives >= NombreTentativesMax)
                            {
                                EtatPartie = EtatsPartie.Perdu;
                                MettreAZero();
                            }
                        }
						
					}
				}
			break;
		}
	}

    protected void AfficherMenuPrincipal(string titre)
    {
        GUI.Box(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300), titre);
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 15, 200, 30), "Lancer partie") == true)
            LancerNouvellePartie();
    }

	public bool VerifierCode(Sprite[] CouleursPions, out int BienPlace, out int MalPlace)
	{
		int bienPlace = 0;
		int malPlace = 0;
		List<int> indexPropositionDejaTraite = new List<int>(TailleCodeSecret);
		List<int> indexSolutionDejaTraite = new List<int>(TailleCodeSecret);

		// Vérification des couleurs bien placées
		for(int i = 0; i < TailleCodeSecret; i++)
		{
			if(CouleursPions[i] == ListePions[_CodeSecret[i]])
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

				if(CouleursPions[i] == ListePions[_CodeSecret[y]])
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
		_LigneActive.Initialiser();
		Vector3 positionBoutonValider = PositionnerElementGUI(CalculerXBoutonValider(), CalculerYBoutonValider());
		_EmplacementBoutonValider = new Rect(positionBoutonValider.x, positionBoutonValider.y, 50, 30);
	}

	public void AjouterLigneHistorique(Sprite[] code, int bienPlace, int malPlace)
	{
		int indexLigne = Tentatives;
		GameObject ligne = new GameObject("Ligne_" + indexLigne.ToString());
		for(int i = 0; i < code.Length; i++)
		{
			GameObject pion = (GameObject) GameObject.Instantiate(PrefabPion);
			pion.name = "Pion_" + i.ToString();
			pion.transform.parent = ligne.transform;
			pion.transform.localPosition = pion.transform.position + Vector3.right * i * (LargeurPion + DistanceSeparationPions);
			pion.GetComponent<SpriteRenderer>().sprite = code[i];
		}
		_EmplacementHistorique.transform.position += Vector3.up * (HauteurPion + DistanceSeparationLignes);
		ligne.transform.parent = _EmplacementHistorique.transform;
		ligne.transform.localPosition = Vector3.down * (indexLigne + 1) * (HauteurPion + DistanceSeparationLignes);

		AjouterVerif(ligne, bienPlace, malPlace);
		AjouterNumeroLigne(ligne, indexLigne+1);

		MettreAJourZoneDefilable();

		_MaxDefilement = _EmplacementHistorique.transform.position.y;
		if(_MaxDefilement > Camera.main.orthographicSize * 2)
			_MinDefilement = Camera.main.orthographicSize * 2;
		else
			_MinDefilement = _MaxDefilement;
	}

	public void AjouterNumeroLigne(GameObject ligne, int numLigne)
	{
		GameObject goNum = (GameObject) GameObject.Instantiate(PrefabNumeroLigne);
		goNum.name = "NumLigne_" + numLigne.ToString();
		goNum.transform.parent = ligne.transform;
		goNum.transform.localPosition = new Vector3(-10, LargeurPion/2, -1);
		TextMesh tm = goNum.GetComponent<TextMesh>();
		tm.text = numLigne.ToString() + ".";
		tm.color = CouleurNumeroLigne;
	}

	public void AjouterVerif(GameObject ligne, int bienPlace, int malPlace)
	{
		int nombrePionsVerifParLigne = Mathf.CeilToInt(TailleCodeSecret / 2);
		float xDepartVerif = TailleCodeSecret * (LargeurPion + DistanceSeparationPions);
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
			pion.transform.localPosition = new Vector3(x, y, -1);
			if(i >= bienPlace + malPlace)
				pion.GetComponent<SpriteRenderer>().sprite = SpriteEmplacementVerif;
			else if(i >= bienPlace)
				pion.GetComponent<SpriteRenderer>().sprite = SpriteVerifMalPlacee;
			else
				pion.GetComponent<SpriteRenderer>().sprite = SpriteVerifBienPlacee;
		}
	}

	public void InitialiserPionsActifs()
	{
		GameObject emplacementPions = (GameObject)GameObject.Find("EmplacementPions");

		for(int i = 0; i < NombreCouleurCodeSecret; i++)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(PrefabPionActif);
			obj.name = "Pion_" + i.ToString();
			obj.transform.parent = emplacementPions.transform;
			obj.transform.localPosition = obj.transform.position + Vector3.up * i * (HauteurPion);
			obj.GetComponent<SpriteRenderer>().sprite = ListePions[i];
		}
	}

	// Retourne l'abscisse du bouton valider dans le monde (pas sur la caméra!)
	protected float CalculerXBoutonValider()
	{
		return _LigneActive.transform.position.x +
			TailleCodeSecret * (LargeurPion + DistanceSeparationLigneActive);
	}

	// Retourne l'ordonnée du bouton valider dans le monde (pas sur la caméra!)
	protected float CalculerYBoutonValider()
	{
		return _LigneActive.transform.position.y + (HauteurPion /2) + 15f;
	}

	public static Vector3 PositionnerElementGUI(float x, float y)
	{
		Vector3 positionElement = Camera.main.WorldToScreenPoint(new Vector3(x, y, 0f));
		positionElement.y = Screen.height - positionElement.y;
		return positionElement;
	}

	public void MettreAJourZoneDefilable()
	{
		float hauteur, largeur;
		hauteur = _EmplacementHistorique.transform.childCount * (HauteurPion + DistanceSeparationLignes);
		float largeurVerif = SpriteVerifBienPlacee.rect.width;
		largeur = TailleCodeSecret * (LargeurPion + DistanceSeparationPions) + Mathf.CeilToInt(TailleCodeSecret / 2) * largeurVerif + 50f;
		_ZoneDefilable.size = new Vector2(largeur, hauteur);
		_ZoneDefilable.center = new Vector2(_ZoneDefilable.size.x / 2 - 30f, -(HauteurPion+DistanceSeparationLignes)/2 *_EmplacementHistorique.transform.childCount);
	}

	public void MettreAZero()
	{
		_LigneActive.MettreAZero();
		Surbrillance.SetActive(false);

		GameObject emplacementPions = (GameObject)GameObject.Find("EmplacementPions");
		for (var i = emplacementPions.transform.childCount - 1; i >= 0; i--)
		{
			Transform pion = emplacementPions.transform.GetChild(i);
			if(Application.isEditor == true)
				DestroyImmediate(pion.gameObject);
			else
				Destroy(pion.gameObject);
		}
		for (var i = _EmplacementHistorique.transform.childCount - 1; i >= 0; i--)
		{
			Transform pion = _EmplacementHistorique.transform.GetChild(i);
			if(Application.isEditor == true)
				DestroyImmediate(pion.gameObject);
			else
				Destroy(pion.gameObject);
		}
		_EmplacementHistorique.transform.position = _PositionHistoriqueOrigine;
	}

	public void LancerNouvellePartie()
	{
		MettreAZero();
		InitialiserLigneActive();
		InitialiserPionsActifs();
		_CodeSecret = GenererCodeSecret();
		EtatPartie = EtatsPartie.PartieEnCours;
	}
}