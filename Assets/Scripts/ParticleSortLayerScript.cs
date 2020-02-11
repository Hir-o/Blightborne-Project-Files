using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSortLayerScript : MonoBehaviour {

	[SerializeField] private string sortingLayerName = "Particles";
	[SerializeField] private int sortingLayerOrder = 1;

	private void Start() 
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "sortingLayerName";
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = sortingLayerOrder;
	}

}
