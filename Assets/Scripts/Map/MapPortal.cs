using UnityEngine;

public class MapPortal : MonoBehaviour
{
    [SerializeField] private Transform bossMapStartPoint;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player") && bossMapStartPoint != null)
        {
            _hasTriggered = true;

            FadeController.Instance.FadeOutAndMove(() =>
            {
                other.transform.position = bossMapStartPoint.position;
                other.transform.rotation = bossMapStartPoint.rotation;

                FollowCamera cam = Camera.main.GetComponent<FollowCamera>();
                if (cam != null)
                    cam.SendMessage("SetPlayerCamPos", other.gameObject);
            });
        }
    }
}
