using UnityEngine;

public class MapPortal : MonoBehaviour
{
    [SerializeField] private Transform bossMapStartPoint; // BossMapStartPoint �� ������Ʈ

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player") && bossMapStartPoint != null)
        {
            _hasTriggered = true;

            // 1. �÷��̾� ���۰� �ִϸ��̼� ����
            PlayerKnight.Instance.SetControlEnabled(false);

            // 2. ���̵� �ƿ� �� �̵� �� ���̵� �� �� ���� �����
            FadeController.Instance.FadeOutAndMove(
                // ȭ�� ���� ��ο����� �� ����
                () =>
                {
                    other.transform.position = bossMapStartPoint.position;
                    other.transform.rotation = bossMapStartPoint.rotation;

                    // ī�޶� ��ġ ����
                    FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
                    if (cam != null)
                        cam.SendMessage("SetPlayerCamPos", other.gameObject);
                },
                // ���̵� �� �Ϸ� �� ����
                () =>
                {
                    PlayerKnight.Instance.SetControlEnabled(true);
                }
            );
        }
    }
}
