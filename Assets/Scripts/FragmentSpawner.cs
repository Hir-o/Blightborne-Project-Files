using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSpawner : MonoBehaviour 
{
	[SerializeField] private GameObject objectToSpawn;
	[SerializeField] private int minSpawnedObjects = 3;
    [SerializeField] private int maxSpawnedObjects = 5;

	[SerializeField] private Vector3 offset;

	private int randomNumObjectsSpawned;

	private void Start() 
	{
		randomNumObjectsSpawned = UnityEngine.Random.Range(minSpawnedObjects, maxSpawnedObjects);
		StartCoroutine(InstantiateFragments());
	}

	private IEnumerator InstantiateFragments()
    {
        for (int i = 0; i < randomNumObjectsSpawned; i++)
        {
            yield return new WaitForSeconds(.01f);
            Instantiate(objectToSpawn, transform.position + offset, Quaternion.identity);
        }
    }
}
