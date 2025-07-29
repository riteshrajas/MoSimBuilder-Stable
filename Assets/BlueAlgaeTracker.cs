using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BlueAlgaeTracker : MonoBehaviour
{
    public TextMeshProUGUI blueAlgaeText;

    private Vector3 processorCorner1 = new Vector3(0.82f, 0f, 4.11f);
    private Vector3 processorCorner2 = new Vector3(3.1f, 0.35f, 4.45f);

    private Vector3 bargeCorner1 = new Vector3(-0.416f, 1.76f, -3.87f);
    private Vector3 bargeCorner2 = new Vector3(0.435f, 2.759f, -0.132f);

    private HashSet<GameObject> algaeInProcessor = new HashSet<GameObject>();
    private HashSet<GameObject> algaeInBarge = new HashSet<GameObject>();

    private int totalAlgaeProcessed = 0;

    // Cooldown in seconds before the same algae can be scored again in processor
    private float processorCooldown = 4f;

    // Track last scored time for each algae
    private Dictionary<GameObject, float> lastProcessedTime = new Dictionary<GameObject, float>();

    void Update()
    {
        GameObject[] algaeObjects = GameObject.FindGameObjectsWithTag("Algae");

        algaeInBarge.Clear();  // reset barge list each frame

        foreach (GameObject algae in algaeObjects)
        {
            Vector3 pos = algae.transform.position;
            float currentTime = Time.time;

            // === Processor Logic ===
            if (IsWithinBounds(pos, processorCorner1, processorCorner2))
            {
                bool canScore = false;

                if (!lastProcessedTime.ContainsKey(algae))
                {
                    canScore = true; // never scored before
                }
                else if (currentTime - lastProcessedTime[algae] >= processorCooldown)
                {
                    canScore = true; // cooldown passed
                }

                if (canScore)
                {
                    if (!algaeInProcessor.Contains(algae))
                    {
                        algaeInProcessor.Add(algae);
                    }
                    totalAlgaeProcessed++;
                    lastProcessedTime[algae] = currentTime;
                }
            }
            else
            {
                algaeInProcessor.Remove(algae); // allow re-processing if leaves and reenters
            }

            // === Barge Tracking ===
            if (IsWithinBounds(pos, bargeCorner1, bargeCorner2))
            {
                algaeInBarge.Add(algae);
            }
        }

        UpdateAlgaeText();
    }

    void UpdateAlgaeText()
    {
        int totalTracked = totalAlgaeProcessed + algaeInBarge.Count;
        blueAlgaeText.text = totalTracked.ToString();
    }

    private bool IsWithinBounds(Vector3 pos, Vector3 cornerA, Vector3 cornerB)
    {
        float minX = Mathf.Min(cornerA.x, cornerB.x);
        float maxX = Mathf.Max(cornerA.x, cornerB.x);
        float minY = Mathf.Min(cornerA.y, cornerB.y);
        float maxY = Mathf.Max(cornerA.y, cornerB.y);
        float minZ = Mathf.Min(cornerA.z, cornerB.z);
        float maxZ = Mathf.Max(cornerA.z, cornerB.z);

        return pos.x >= minX && pos.x <= maxX &&
               pos.y >= minY && pos.y <= maxY &&
               pos.z >= minZ && pos.z <= maxZ;
    }
}
