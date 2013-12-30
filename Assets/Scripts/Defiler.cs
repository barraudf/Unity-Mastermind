using UnityEngine;
using System.Collections;

public class Defiler : MonoBehaviour
{
	private Vector3 _PositionSourisCyclePrecedent = Vector3.zero;
	private bool _DefilementEnCours = false;

	void OnMouseDown()
	{
		_PositionSourisCyclePrecedent = Input.mousePosition;
		_DefilementEnCours = true;
	}
	
	void OnMouseDrag()
	{
		if(_DefilementEnCours == true)
		{
			transform.position += Vector3.up * (Input.mousePosition.y - _PositionSourisCyclePrecedent.y);
			_PositionSourisCyclePrecedent = Input.mousePosition;

			if(transform.position.y < ControlleurJeu.Instance.MinDefilement)
				transform.position = new Vector2(transform.position.x, ControlleurJeu.Instance.MinDefilement);
			if(transform.position.y > ControlleurJeu.Instance.MaxDefilement)
				transform.position = new Vector2(transform.position.x, ControlleurJeu.Instance.MaxDefilement);
		}
	}

	void OnMouseUp()
	{
		_DefilementEnCours = false;
	}
}
