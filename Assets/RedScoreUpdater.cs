using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RedScoreUpdater : MonoBehaviour
{
    public TextMeshProUGUI redScoreText;
    public int redScore = 0;

    // Processor zone corners (updated)
    private Vector3 processorCorner1 = new Vector3(-1.37f, -0.05f, -4.7f);
    private Vector3 processorCorner2 = new Vector3(-3.08f, 0.52f, -4.3f);

    // Barge zone corners (updated)
    private Vector3 bargeCorner1 = new Vector3(0.48f, 1.72f, 0.17f);
    private Vector3 bargeCorner2 = new Vector3(-0.41f, 2.68f, 3.9f);

    private float processorCooldownTime = 4f;
    private float bargeCooldownTime = 3f;

    private Dictionary<GameObject, float> processorCooldowns = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> bargeLastScoreTime = new Dictionary<GameObject, float>();
    private HashSet<GameObject> algaeInBarge = new HashSet<GameObject>();

    void Start()
    {
        UpdateScoreText();
    }

    public void AddRedScore(int points)
    {
        redScore += points;
        UpdateScoreText();
    }

    public void SubtractRedScore(int points)
    {
        redScore -= points;
        if (redScore < 0) redScore = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        redScoreText.text = redScore.ToString();
    }

    void Update()
    {
        GameObject[] algaeObjects = GameObject.FindGameObjectsWithTag("Algae");

        foreach (GameObject algae in algaeObjects)
        {
            Vector3 pos = algae.transform.position;

            // Processor scoring
            if (IsWithinBounds(pos, processorCorner1, processorCorner2))
            {
                if (!processorCooldowns.ContainsKey(algae) || Time.time - processorCooldowns[algae] >= processorCooldownTime)
                {
                    AddRedScore(6);
                    processorCooldowns[algae] = Time.time;
                    Debug.Log($"Processor scored for algae {algae.name} at {pos}");
                }
            }

            // Barge scoring
            bool isInBarge = IsWithinBounds(pos, bargeCorner1, bargeCorner2);
            bool hasScored = algaeInBarge.Contains(algae);
            bool cooldownReady = !bargeLastScoreTime.ContainsKey(algae) || Time.time - bargeLastScoreTime[algae] >= bargeCooldownTime;

            if (isInBarge)
            {
                if (!hasScored && cooldownReady)
                {
                    AddRedScore(4);
                    algaeInBarge.Add(algae);
                    bargeLastScoreTime[algae] = Time.time;
                    Debug.Log($"Barge scored for algae {algae.name} at {pos}");
                }
            }
            else
            {
                if (hasScored)
                {
                    SubtractRedScore(4);
                    algaeInBarge.Remove(algae);
                    Debug.Log($"Algae {algae.name} left barge at {pos}");
                }
            }
        }
    }

    private bool IsWithinBounds(Vector3 pos, Vector3 cornerA, Vector3 cornerB)
    {
        float minX = Mathf.Min(cornerA.x, cornerB.x);
        float maxX = Mathf.Max(cornerA.x, cornerB.x);
        float minY = Mathf.Min(cornerA.y, cornerB.y);
        float maxY = Mathf.Max(cornerA.y, cornerB.y);
        float minZ = Mathf.Min(cornerA.z, cornerB.z);
        float maxZ = Mathf.Max(cornerA.z, cornerB.z);

        // Inclusive bounds check (change to strict if needed)
        return (pos.x >= minX && pos.x <= maxX) &&
               (pos.y >= minY && pos.y <= maxY) &&
               (pos.z >= minZ && pos.z <= maxZ);
    }
}
