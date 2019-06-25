using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private float width;
    private float height;
    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;
    private Vector3 ro;

    public int counter = 0;
    public GameObject p1;
    public GameObject p2;
    private GameObject parent;

    private void Start()
    {
        lu = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        ro = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        width = Vector3.Distance(lu, ru);
        height = Vector3.Distance(lu, lo);

        float prefabsize = p1.GetComponent<BoxCollider2D>().size.x*p1.GetComponent<Transform>().localScale.x;
        float prefabsizeHalf = prefabsize/2;
        
        for (float x = lu.x; x < ru.x; x+=prefabsize)
        {
            for (float y = lu.y; y < lo.y; y+=prefabsize)
            {
                GameObject spawner = Random.value < 0.5f ? p1 : p2;
                Vector3 spawnPoint = new Vector3(x+prefabsizeHalf, y+prefabsizeHalf, 0);
                GameObject go = Instantiate(spawner, spawnPoint, Quaternion.identity);
                if (parent == null)
                {
                    parent = new GameObject("parent");
                }

                go.transform.parent = parent.transform;
            }
        }
    }
}