using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RedAlgaeTracker : MonoBehaviour
{
    public TextMeshProUGUI redAlgaeText;

    private Vector3 processorCorner1 = new Vector3(-1.37f, -0.05f, -4.7f);
    private Vector3 processorCorner2 = new Vector3(-3.08f, 0.52f, -4.3f);

    private Vector3 bargeCorner1 = new Vector3(0.48f, 1.72f, 0.17f);
    private Vector3 bargeCorner2 = new Vector3(-0.41f, 2.68f, 3.9f);

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
                    canScore = true;
                }
                else if (currentTime - lastProcessedTime[algae] >= processorCooldown)
                {
                    canScore = true;
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
                algaeInProcessor.Remove(algae); // allow reprocessing if they leave and re-enter
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
        redAlgaeText.text = totalTracked.ToString();
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
