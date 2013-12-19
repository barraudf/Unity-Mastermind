using UnityEngine;
using System.Collections;

public class CliquerDeposer : MonoBehaviour
{
	public float VitesseAnimationRetour = 50;

	private Vector3 Decallage;
	private Vector3 CoordonneesOrigine;
	private Vector3 CoordonneesRelache;
	private bool RetourOrigine = false;
	private float TempsEcouleAnimationRetour;
	private float TempsAnimationRetour;
	private GameObject emplacementCible = null;
	private SpriteRenderer RendererObjet;

	void Start()
	{
		RendererObjet = gameObject.GetComponent<SpriteRenderer>();
	}
	void Update()
	{
		if(RetourOrigine == true)
		{
			transform.position = Vector3.Lerp(CoordonneesRelache, CoordonneesOrigine, TempsEcouleAnimationRetour / TempsAnimationRetour);
			TempsEcouleAnimationRetour += Time.deltaTime;

			if(TempsEcouleAnimationRetour > TempsAnimationRetour)
			{
				RetourOrigine = false;
				transform.position = CoordonneesOrigine;
			}
		}
	}

	void OnMouseDrag()
	{
		Vector3 coordonneesEcranActuelles = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		
		Vector3 coordonneesActuelles = Camera.main.ScreenToWorldPoint(coordonneesEcranActuelles) + Decallage;
		transform.position = coordonneesActuelles;
	}

	void OnMouseDown()
	{
		CoordonneesOrigine = transform.position;
		Decallage = CoordonneesOrigine - Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
		rigidbody2D.isKinematic = false;
	}

	void OnMouseUp()
	{
		CoordonneesRelache = transform.position;
		RetourOrigine = true;
		TempsEcouleAnimationRetour = 0;
		TempsAnimationRetour = Vector3.Distance(CoordonneesOrigine, CoordonneesRelache) / VitesseAnimationRetour;
		rigidbody2D.isKinematic = true;

		if(emplacementCible != null)
		{
			SpriteRenderer RendererCible = emplacementCible.GetComponent<SpriteRenderer>();
			RendererCible.sprite = RendererObjet.sprite;
			RendererCible.color = RendererObjet.color;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.CompareTag("Emplacements") == true)
		{
			//Debug.Log("OnTriggerEnter2D", col.gameObject);
			emplacementCible = col.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.gameObject == emplacementCible)
		{
			//Debug.Log("OnTriggerExit2D", col.gameObject);
			emplacementCible = null;
		}
	}
}
