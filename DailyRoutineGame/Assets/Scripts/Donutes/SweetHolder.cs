using System.Collections;
using UnityEngine;

public class SweetHolder : MonoBehaviour
{
    [SerializeField] Collider spawnArea;
    [SerializeField] GameObject[] prefabs;
    public bool Spawn = false;

    private float spawnTime = 0;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (Spawn)
        {
            spawnTime += Time.deltaTime;
            if (spawnTime > 0.1f)
            {
                spawnTime = 0;
                Vector3 newPos = new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                       Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                       Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
                       );
                Instantiate(prefabs[Random.Range(0, prefabs.Length)], newPos, Quaternion.identity, transform.parent);
            }
        }
    }

    public IEnumerator ReturnToInitialPos()
    {
        float time = 0;
        Vector3 fromPos = transform.position;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(fromPos, startPos, time / 0.4f);
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 180), Quaternion.Euler(Vector3.zero), time / 0.4f);
            yield return null;
        }
    }

}
