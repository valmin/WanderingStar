using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> linesPrefabs;
    [SerializeField]
    private float maxDelay = 5f;

    private float spawnTime;

    void Update()
    {
        spawnTime -= Time.deltaTime;

        if (spawnTime <= 0)
        {
            spawnTime = Random.Range(0, maxDelay);
            SpawnLines();
        }
    }

    private void SpawnLines()
    {
        GameObject lines = Instantiate(linesPrefabs[Random.Range(0, linesPrefabs.Count)], this.transform);
        lines.transform.position = new Vector3(Random.Range(Camera.main.transform.position.x - BackgroundGenerator.Width, Camera.main.transform.position.x + BackgroundGenerator.Width),
            Camera.main.transform.position.y + BackgroundGenerator.Height, 0);
    }
}
