using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BibitProducer : MonoBehaviour
{
    public GameObject bibitPrefab;
    public int count;

    public int additionalBibitsProduced = -5;

    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;
    private Vector3 ro;

    [SerializeField] private int maxGeneration;
    [SerializeField] private int maxCurrentGeneration;


    private List<GameObject> allBibits;
    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        lu = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        ro = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        allBibits = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        maxCurrentGeneration = 0;
        count = allBibits.Count;
        if (allBibits.Count > 100)
        {
            Debug.LogError("EXIT APPLICATION, TOO MANY BIBITS");
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit(1);
        }

        foreach (GameObject go in allBibits)
        {
            int gen = go.GetComponent<Bibit>().getGeneration();
            if (gen > maxGeneration)
            {
                maxGeneration = gen;
            }

            if (gen > maxCurrentGeneration)
            {
                maxCurrentGeneration = gen;
            }
        }

//        if (count > 30)
//        {
//            foreach (GameObject go in allBibits)
//            {
//                Bibit b = go.GetComponent<Bibit>();
//                if (!b.hasModifiedAgeModifier)
//                {
//                    b.ageModifier *= 3;
//                    b.hasModifiedAgeModifier = true;
//                }
//            }
//        }
//        else
//        {
//            foreach (GameObject go in allBibits)
//            {
//                Bibit b = go.GetComponent<Bibit>();
//                if (b.hasModifiedAgeModifier)
//                {
//                    b.ageModifier /= 3;
//                    b.hasModifiedAgeModifier = false;
//                }
//            }
//        }

        while (allBibits.Count < Camera.main.orthographicSize * 2)
//        while (allBibits.Count < 50)
        {
            spawnBibit();
            additionalBibitsProduced++;
        }

        if (allBibits.Count >= 65)
        {
            startWithEvolvedBibids();
        }

        while (allBibits.Count > 70)
        {
            removeBibit(allBibits[Random.Range(0, allBibits.Count)]);
        }
    }

    private void startWithEvolvedBibids()
    {
        for (int i = 0; i < 65 - Camera.main.orthographicSize * 2; i++)
        {
            GameObject removeObject = allBibits[Random.Range(0, allBibits.Count)];
            allBibits.Remove(removeObject);
            Destroy(removeObject);
        }
        Debug.Log("restarted!");
    }

    private void spawnBibit()
    {
        if (allBibits.Count < 70)
        {
            Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y));
            GameObject newBibit = Instantiate(bibitPrefab, spawnPos, Quaternion.identity);
            newBibit.GetComponent<Bibit>().pseudoConstructor1();
            addBibit(newBibit);
        }
    }

    public void addBibit(GameObject bibit)
    {
        allBibits.Add(bibit);
        if (parent == null)
        {
            parent = new GameObject("Bibits");
        }

        bibit.transform.parent = parent.transform;
    }

    public void removeBibit(GameObject bibit)
    {
        allBibits.Remove(bibit);
    }

    public void spawnChild(GameObject o)
    {
        if (allBibits.Count < 70)
        {
            Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y));
            GameObject newBibit = Instantiate(bibitPrefab, spawnPos, Quaternion.identity);
            newBibit.GetComponent<Bibit>().pseudoConstructor2(o.GetComponent<Bibit>());
            addBibit(newBibit);
        }
    }
}