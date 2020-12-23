using UnityEngine;

[System.Serializable]
public class SweetsPrefabHolder 
{
    public GameObject[] prefabs;
}

public class SweetsSpawnerfiller : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform sweetsholder;
    [SerializeField] Collider spawnArea;
    [SerializeField] SweetsPrefabHolder[] sweetsPrefabHolders;
    private SweetsPrefabHolder selectedHolder;

    private bool Spawn = false;
    private float spawnTime = 0;
    private Vector3 newPos;

    private void Start()
    {
        selectedHolder = sweetsPrefabHolders[Random.Range(0, sweetsPrefabHolders.Length)];
    }

    public void StartSpawning()
    {
        Spawn = true;
    }

    public void StopSpawning()
    {
        Spawn = false;
    }

    private void Update()
    {
        if (Spawn)
        {
            spawnTime += Time.deltaTime;
            if (spawnTime > 0.175f)
            {
                spawnTime = 0;
                newPos = new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                       Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                       Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z) ); 
                Instantiate(selectedHolder.prefabs[Random.Range(0, selectedHolder.prefabs.Length)], newPos,
                    Quaternion.Euler(Vector3.one * Random.value * 360), sweetsholder);
            }
        }
    }
 
}
