using UnityEngine;

public class WaveZoneTrigger : MonoBehaviour
{
    [SerializeField] private int zoneIndex; // 0 or 1

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaveManager.Instance.PlayerEnteredZone(zoneIndex);
        }
    }
}