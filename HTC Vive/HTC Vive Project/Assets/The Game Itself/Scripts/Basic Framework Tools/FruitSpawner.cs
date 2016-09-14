using UnityEngine;
using System.Collections;

public class FruitSpawner : MonoBehaviour 
{
    public GameObject[] FreshProduce;   //fruit spawning stuff

    public int count = 0;

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(SpawnFruits());
	}

    IEnumerator SpawnFruits()
    {
        while(true)
        {
            GameObject obj = Instantiate(FreshProduce[Random.Range(0, FreshProduce.Length)]);
            Rigidbody temp = obj.GetComponent<Rigidbody>();

            temp.velocity = new Vector3(0f, 5f, 0.5f);
            temp.angularVelocity = new Vector3(Random.Range(-5f,5f), 0f, Random.Range(-5f,5f));
            temp.useGravity = true;
            
            Vector3 pos = transform.position;
            pos.x += Random.Range(-500f, 500f);
            obj.transform.position = pos;

            count++;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
