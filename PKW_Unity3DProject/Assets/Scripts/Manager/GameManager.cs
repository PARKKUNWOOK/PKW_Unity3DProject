using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private GameObject _player;
    public GameObject Player
    { get { return _player; } }

    private GameObject _minotaurs;
    public GameObject Minotaurs
    { get { return _minotaurs; } }

    private void Awake()
    {
        _instance = this;
        SpawnPlayer();        
    }

    private void Start()
    {
        MinotaursManager.Instance.SpawnAllMinotaurs();
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");
        
        _player = Instantiate(prefab);
    }
}
