using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform _target;

    private Vector3 _targetPos;
    private Vector3 _destPos;

    private float _distance = 9.89f;
    private float _height = 16.8f;
    private float _moveDamping = 10.0f;

    private void Awake()
    {

    }

    private void Start()
    {
        _target = GameManager.Instance.Player.transform;

        _targetPos = _target.position;
        _destPos = _targetPos + Vector3.back * _distance + Vector3.up * _height;
        transform.position = _destPos;
    }

    private void LateUpdate()
    {
        if (!_target) return;

        _targetPos = _target.position;
        _destPos = _targetPos + Vector3.back * _distance + Vector3.up * _height;

        transform.position = Vector3.Lerp(transform.position, _destPos, _moveDamping * Time.deltaTime);
    }

    public void SetPlayerCamPos(GameObject player)
    {
        _target = player.transform;
        _targetPos = _target.position;
        _destPos = _targetPos + Vector3.back * _distance + Vector3.up * _height;
        transform.position = _destPos;
    }
}
