using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaursManager : MonoBehaviour
{
    public static MinotaursManager Instance;

    [SerializeField]
    private GameObject minotaurPrefab;

    [SerializeField]
    private Vector3[] spawnPositions;

    [SerializeField]
    private Transform[] patrolPointGroups;

    private List<GameObject> minotaurPool = new List<GameObject>();
    private int poolSize = 7;

    private Transform monsterParent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        GameObject monsterObj = GameObject.Find("Monster");
        if (monsterObj != null)
        {
            monsterParent = monsterObj.transform;
        }

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject minotaur = Instantiate(minotaurPrefab);

            if (monsterParent != null)
                minotaur.transform.SetParent(monsterParent);

            minotaur.SetActive(false);
            minotaurPool.Add(minotaur);
        }
    }

    public void SpawnAllMinotaurs()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject minotaur = minotaurPool[i];
            minotaur.transform.position = spawnPositions[i];
            minotaur.SetActive(true);

            MinotaurAI ai = minotaur.GetComponent<MinotaurAI>();
            if (ai != null && patrolPointGroups.Length > i)
            {
                Transform group = patrolPointGroups[i];
                Transform[] points = new Transform[group.childCount];
                for (int j = 0; j < points.Length; j++)
                    points[j] = group.GetChild(j);

                ai.PatrolPoints = points;
            }
        }
    }

    public void DeactivateAllMinotaurs()
    {
        foreach (GameObject minotaur in minotaurPool)
        {
            minotaur.SetActive(false);
        }
    }

    public void RespawnMinotaur(GameObject minotaur, Vector3 spawnPosition, Transform[] patrolPoints)
    {
        StartCoroutine(RespawnCoroutine(minotaur, spawnPosition, patrolPoints));
    }

    private IEnumerator RespawnCoroutine(GameObject minotaur, Vector3 spawnPosition, Transform[] patrolPoints)
    {
        yield return new WaitForSeconds(10f);

        minotaur.transform.position = spawnPosition;
        minotaur.GetComponent<Minotaurs>().ResetMinotaur();

        MinotaurAI ai = minotaur.GetComponent<MinotaurAI>();
        ai.enabled = true;
        if (patrolPoints != null)
            ai.PatrolPoints = patrolPoints;
        ai.Initialize();

        minotaur.SetActive(true);
    }
}
