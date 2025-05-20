using UnityEngine;

public class MapPortal : MonoBehaviour
{
    [SerializeField] private Transform bossMapStartPoint; // BossMapStartPoint 빈 오브젝트

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player") && bossMapStartPoint != null)
        {
            _hasTriggered = true;

            // 1. 플레이어 조작과 애니메이션 정지
            PlayerKnight.Instance.SetControlEnabled(false);

            // 2. 페이드 아웃 → 이동 → 페이드 인 → 조작 재허용
            FadeController.Instance.FadeOutAndMove(
                // 화면 완전 어두워졌을 때 실행
                () =>
                {
                    other.transform.position = bossMapStartPoint.position;
                    other.transform.rotation = bossMapStartPoint.rotation;

                    // 카메라 위치 갱신
                    FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
                    if (cam != null)
                        cam.SendMessage("SetPlayerCamPos", other.gameObject);
                },
                // 페이드 인 완료 시 실행
                () =>
                {
                    PlayerKnight.Instance.SetControlEnabled(true);
                }
            );
        }
    }
}
