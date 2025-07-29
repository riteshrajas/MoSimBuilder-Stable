using UnityEngine;

public class ProcessorZoneScore : MonoBehaviour
{
    public BlueScoreUpdater scoreUpdater;

    private Vector3 minBound = new Vector3(0.82f, 0.0f, 4.11f);
    private Vector3 maxBound = new Vector3(3.1f, 0.35f, 4.45f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Algea"))
        {
            Vector3 pos = other.transform.position;

            if (IsWithinBounds(pos))
            {
                Debug.Log("Algea entered processor zone.");
                if (scoreUpdater != null)
                {
                    scoreUpdater.AddBlueScore(6);
                }
                else
                {
                    Debug.LogWarning("ScoreUpdater reference not set.");
                }
            }
        }
    }

    private bool IsWithinBounds(Vector3 position)
    {
        return position.x >= minBound.x && position.x <= maxBound.x &&
               position.y >= minBound.y && position.y <= maxBound.y &&
               position.z >= minBound.z && position.z <= maxBound.z;
    }
}
