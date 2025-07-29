using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static RobotSpawnController;

public class BlueScoreUpdater : MonoBehaviour
{
    public TextMeshProUGUI blueScoreText;
    public int Bluescore = 0;
    private GameObject robot;
    private Vector3 autonStartPosition;
    private bool robotFound = false;
    private bool autonCheckDone = false;
    private bool threePointsGiven = false;
    GameObject foundRobot;

    public float autonTimeLimit = 15f;
    public float matchTimeLimit = 150f; // Total match time (2.5 minutes)
    public float endgameStartTime = 120f; // When endgame starts (2 minutes)
    private float startTime;
    private float interval = 0.5f;
    private float timer = 0f;

    // Endgame variables
    private bool endgameStarted = false;
    private bool parkScored = false;
    private bool deepClimbScored = false;
    private bool wasInEndgameZone = false; // Track if robot was previously in endgame zone
    private float parkThresholdY = 0.07603697f;
    private float climbThresholdY = 0.25f; // Updated climb threshold

    // Endgame zone bounds (X and Z only, Y handled separately) - FIXED
    private float endgame_minX = -0.9460935f;
    private float endgame_maxX = 1.021242f;
    private float endgame_minZ = -3.574728f;
    private float endgame_maxZ = -0.5f; // Changed from -0.9460935f to -0.5f to include your robot

    // Processor zone
    private Vector3 processorCorner1 = new Vector3(0.82f, 0f, 4.11f);
    private Vector3 processorCorner2 = new Vector3(3.1f, 0.35f, 4.45f);

    // Barge zone
    private Vector3 bargeCorner1 = new Vector3(-0.416f, 1.76f, -3.87f);
    private Vector3 bargeCorner2 = new Vector3(0.435f, 2.759f, -0.132f);

    // L4 Peg zones (min y = 1.59)
    private Vector3 l4Corner1A = new Vector3(4.993f, 1.59f, 0.082f);
    private Vector3 l4Corner2A = new Vector3(5.091f, 2.007f, 0.276f);
    private Vector3 l4Corner1B = new Vector3(5.011f, 1.59f, -0.253f);
    private Vector3 l4Corner2B = new Vector3(5.116f, 2.007f, -0.081f);
    private Vector3 l4Corner1C = new Vector3(4.763f, 1.59f, 0.46f);
    private Vector3 l4Corner2C = new Vector3(4.858f, 2.007f, 0.682f);
    private Vector3 l4Corner1D = new Vector3(4.629f, 1.59f, 0.77f);
    private Vector3 l4Corner2D = new Vector3(4.379f, 2.007f, 0.701f);
    private Vector3 l4Corner1E = new Vector3(4.111f, 1.59f, 0.834f);
    private Vector3 l4Corner2E = new Vector3(3.927f, 2.007f, 0.587f);
    private Vector3 l4Corner1F = new Vector3(3.703f, 1.59f, 0.445f);
    private Vector3 l4Corner2F = new Vector3(3.804f, 2.007f, 0.707f);
    private Vector3 l4Corner1G = new Vector3(3.564f, 1.59f, 0.255f);
    private Vector3 l4Corner2G = new Vector3(3.416f, 2.007f, 0.065f);
    private Vector3 l4Corner1H = new Vector3(3.443f, 1.59f, -0.083f);
    private Vector3 l4Corner2H = new Vector3(3.589f, 2.007f, -0.271f);
    private Vector3 l4Corner1I = new Vector3(3.648f, 1.59f, -0.613f);
    private Vector3 l4Corner2I = new Vector3(3.898f, 2.007f, -0.579f);
    private Vector3 l4Corner1J = new Vector3(3.917f, 1.59f, -0.799f);
    private Vector3 l4Corner2J = new Vector3(4.185f, 2.007f, -0.729f);
    private Vector3 l4Corner1K = new Vector3(4.401f, 1.59f, -0.729f);
    private Vector3 l4Corner2K = new Vector3(4.671f, 2.007f, -0.815f);
    private Vector3 l4Corner1L = new Vector3(4.957f, 1.59f, -0.66f);
    private Vector3 l4Corner2L = new Vector3(4.69f, 2.007f, -0.571f);

    // L3 Peg zones (min y = 1.0, max y = 1.3)
    private Vector3 l3Corner1A = new Vector3(4.766f, 1.0f, -0.857f);
    private Vector3 l3Corner2A = new Vector3(4.806f, 1.3f, -0.338f);
    private Vector3 l3Corner1B = new Vector3(4.296f, 1.0f, -0.558f);
    private Vector3 l3Corner2B = new Vector3(4.669f, 1.3f, -0.757f);
    private Vector3 l3Corner1C = new Vector3(4.279f, 1.0f, -0.601f);
    private Vector3 l3Corner2C = new Vector3(3.932f, 1.3f, -0.721f);
    private Vector3 l3Corner1D = new Vector3(3.619f, 1.0f, -0.671f);
    private Vector3 l3Corner2D = new Vector3(3.971f, 1.3f, -0.465f);
    private Vector3 l3Corner1E = new Vector3(3.5f, 1.0f, -0.069f);
    private Vector3 l3Corner2E = new Vector3(3.836f, 1.3f, -0.265f);
    private Vector3 l3Corner1F = new Vector3(3.493422f, 1.002257f, 0.0639979f);
    private Vector3 l3Corner2F = new Vector3(3.693422f, 1.202257f, 0.2639979f);
    private Vector3 l3Corner1G = new Vector3(4.887037f, 1.002014f, -0.2665401f);
    private Vector3 l3Corner2G = new Vector3(5.087037f, 1.202014f, -0.0665401f);
    private Vector3 l3Corner1H = new Vector3(4.889186f, 1.002071f, 0.0662495f);
    private Vector3 l3Corner2H = new Vector3(5.089186f, 1.202071f, 0.2662495f);
    private Vector3 l3Corner1I = new Vector3(4.676985f, 1.003199f, 0.4234671f);
    private Vector3 l3Corner2I = new Vector3(4.876985f, 1.203199f, 0.6234671f);
    private Vector3 l3Corner1J = new Vector3(4.392138f, 1.002126f, 0.5904862f);
    private Vector3 l3Corner2J = new Vector3(4.592138f, 1.202126f, 0.7904862f);
    private Vector3 l3Corner1K = new Vector3(3.981398f, 1.002321f, 0.5832262f);
    private Vector3 l3Corner2K = new Vector3(4.181398f, 1.202321f, 0.7832262f);
    private Vector3 l3Corner1L = new Vector3(3.703553f, 1.003266f, 0.4261331f);
    private Vector3 l3Corner2L = new Vector3(3.903553f, 1.203266f, 0.6261331f);

    // L2 Peg zones (L3 coordinates with y values lowered by 0.4)
    private Vector3 l2Corner1A = new Vector3(4.766f, 0.6f, -0.857f);
    private Vector3 l2Corner2A = new Vector3(4.806f, 0.9f, -0.338f);
    private Vector3 l2Corner1B = new Vector3(4.296f, 0.6f, -0.558f);
    private Vector3 l2Corner2B = new Vector3(4.669f, 0.9f, -0.757f);
    private Vector3 l2Corner1C = new Vector3(4.279f, 0.6f, -0.601f);
    private Vector3 l2Corner2C = new Vector3(3.932f, 0.9f, -0.721f);
    private Vector3 l2Corner1D = new Vector3(3.619f, 0.6f, -0.671f);
    private Vector3 l2Corner2D = new Vector3(3.971f, 0.9f, -0.465f);
    private Vector3 l2Corner1E = new Vector3(3.5f, 0.6f, -0.069f);
    private Vector3 l2Corner2E = new Vector3(3.836f, 0.9f, -0.265f);
    private Vector3 l2Corner1F = new Vector3(3.493422f, 0.602257f, 0.0639979f);
    private Vector3 l2Corner2F = new Vector3(3.693422f, 0.802257f, 0.2639979f);
    private Vector3 l2Corner1G = new Vector3(4.887037f, 0.602014f, -0.2665401f);
    private Vector3 l2Corner2G = new Vector3(5.087037f, 0.802014f, -0.0665401f);
    private Vector3 l2Corner1H = new Vector3(4.889186f, 0.602071f, 0.0662495f);
    private Vector3 l2Corner2H = new Vector3(5.089186f, 0.802071f, 0.2662495f);
    private Vector3 l2Corner1I = new Vector3(4.676985f, 0.603199f, 0.4234671f);
    private Vector3 l2Corner2I = new Vector3(4.876985f, 0.803199f, 0.6234671f);
    private Vector3 l2Corner1J = new Vector3(4.392138f, 0.602126f, 0.5904862f);
    private Vector3 l2Corner2J = new Vector3(4.592138f, 0.802126f, 0.7904862f);
    private Vector3 l2Corner1K = new Vector3(3.981398f, 0.602321f, 0.5832262f);
    private Vector3 l2Corner2K = new Vector3(4.181398f, 0.802321f, 0.7832262f);
    private Vector3 l2Corner1L = new Vector3(3.703553f, 0.603266f, 0.4261331f);
    private Vector3 l2Corner2L = new Vector3(3.903553f, 0.803266f, 0.6261331f);

    // L1 Peg zones (y values from 0.3 to 0.6)
    private Vector3 l1Corner1A = new Vector3(4.15f, 0.3f, -1.0f);
    private Vector3 l1Corner2A = new Vector3(5.25f, 0.6f, -0.45f);
    private Vector3 l1Corner1B = new Vector3(3.317f, 0.3f, -1.036f);
    private Vector3 l1Corner2B = new Vector3(4.363f, 0.6f, -0.253f);
    private Vector3 l1Corner1C = new Vector3(3.4f, 0.3f, -0.571f);
    private Vector3 l1Corner2C = new Vector3(3.681f, 0.6f, 0.589f);
    private Vector3 l1Corner1D = new Vector3(3.322f, 0.3f, 0.451f);
    private Vector3 l1Corner2D = new Vector3(4.639f, 0.6f, 0.666f);
    private Vector3 l1Corner1E = new Vector3(3.956f, 0.3f, 0.372f);
    private Vector3 l1Corner2E = new Vector3(5.315f, 0.6f, 0.98f);
    private Vector3 l1Corner1F = new Vector3(4.678f, 0.3f, -0.553f);
    private Vector3 l1Corner2F = new Vector3(5.23f, 0.6f, 0.598f);

    private float processorCooldownTime = 4f;
    private float bargeCooldownTime = 3f;

    private Dictionary<GameObject, float> processorCooldowns = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> bargeLastScoreTime = new Dictionary<GameObject, float>();
    private HashSet<GameObject> algaeInBarge = new HashSet<GameObject>();

    private HashSet<GameObject> coralsScoredInPegZones = new HashSet<GameObject>();
    private HashSet<GameObject> coralsScoredInL3Zones = new HashSet<GameObject>();
    private HashSet<GameObject> coralsScoredInL2Zones = new HashSet<GameObject>();
    private HashSet<GameObject> coralsScoredInL1Zones = new HashSet<GameObject>();

    private List<(Vector3, Vector3)> l4PegZones;
    private List<(Vector3, Vector3)> l3PegZones;
    private List<(Vector3, Vector3)> l2PegZones;
    private List<(Vector3, Vector3)> l1PegZones;

    void Start()
    {
        startTime = Time.time;
        UpdateScoreText();

        l4PegZones = new List<(Vector3, Vector3)>()
        {
            (l4Corner1A, l4Corner2A), (l4Corner1B, l4Corner2B), (l4Corner1C, l4Corner2C),
            (l4Corner1D, l4Corner2D), (l4Corner1E, l4Corner2E), (l4Corner1F, l4Corner2F),
            (l4Corner1G, l4Corner2G), (l4Corner1H, l4Corner2H), (l4Corner1I, l4Corner2I),
            (l4Corner1J, l4Corner2J), (l4Corner1K, l4Corner2K), (l4Corner1L, l4Corner2L)
        };

        l3PegZones = new List<(Vector3, Vector3)>()
        {
            (l3Corner1A, l3Corner2A), (l3Corner1B, l3Corner2B), (l3Corner1C, l3Corner2C),
            (l3Corner1D, l3Corner2D), (l3Corner1E, l3Corner2E), (l3Corner1F, l3Corner2F),
            (l3Corner1G, l3Corner2G), (l3Corner1H, l3Corner2H), (l3Corner1I, l3Corner2I),
            (l3Corner1J, l3Corner2J), (l3Corner1K, l3Corner2K), (l3Corner1L, l3Corner2L)
        };

        l2PegZones = new List<(Vector3, Vector3)>()
        {
            (l2Corner1A, l2Corner2A), (l2Corner1B, l2Corner2B), (l2Corner1C, l2Corner2C),
            (l2Corner1D, l2Corner2D), (l2Corner1E, l2Corner2E), (l2Corner1F, l2Corner2F),
            (l2Corner1G, l2Corner2G), (l2Corner1H, l2Corner2H), (l2Corner1I, l2Corner2I),
            (l2Corner1J, l2Corner2J), (l2Corner1K, l2Corner2K), (l2Corner1L, l2Corner2L)
        };

        l1PegZones = new List<(Vector3, Vector3)>()
        {
            (l1Corner1A, l1Corner2A), (l1Corner1B, l1Corner2B), (l1Corner1C, l1Corner2C),
            (l1Corner1D, l1Corner2D), (l1Corner1E, l1Corner2E), (l1Corner1F, l1Corner2F)
        };
    }

    public void AddBlueScore(int points)
    {
        Bluescore += points;
        UpdateScoreText();
    }

    public void SubtractBlueScore(int points)
    {
        Bluescore -= points;
        if (Bluescore < 0) Bluescore = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        blueScoreText.text = Bluescore.ToString();
    }

    bool InAutonomous()
    {
        return (Time.time - startTime) <= autonTimeLimit + 0.3f;
    }

    bool InEndgame()
    {
        float elapsedTime = Time.time - startTime;
        return elapsedTime >= endgameStartTime && elapsedTime <= matchTimeLimit;
    }

    bool MatchEnded()
    {
        return (Time.time - startTime) > matchTimeLimit;
    }

    void Update()
    {
        foundRobot = null;
        float elapsedTime = Time.time - startTime;
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f; // Reset the timer

            // Try to find robot once it's spawned
            if (!robotFound)
            {
                foundRobot = GameObject.Find("25-1323-2(Clone)");
                /*
                RobotSpawnController controller = gameHanlder.GetComponent<RobotSpawnController>();
                if (controller != null){
                    string robotName = controller.getRobotName() + "(Clone)";
                    foundRobot = GameObject.Find(robotName);
                }
                */
                if (foundRobot != null)
                {
                    robot = foundRobot;
                    autonStartPosition = robot.transform.position;
                    robotFound = true;
                    Debug.Log("Robot found and tracking started.");
                }
            }

            // Only check during auton
            if (robotFound && !autonCheckDone && elapsedTime <= autonTimeLimit)
            {
                float distance = Vector3.Distance(robot.transform.position, autonStartPosition);

                if (distance > 0.1f && !threePointsGiven)
                {
                    AddBlueScore(3);
                    threePointsGiven = true;
                    Debug.Log("Robot moved in auton � 3 points awarded to Blue!");
                }
            }

            // Stop checking after auton ends
            if (elapsedTime > autonTimeLimit)
            {
                autonCheckDone = true;
            }

            // Check for endgame start
            if (!endgameStarted && elapsedTime >= endgameStartTime)
            {
                endgameStarted = true;
                Debug.Log("Endgame started!");
            }

            // Endgame scoring logic - only if match hasn't ended
            if (robotFound && endgameStarted && elapsedTime <= matchTimeLimit + 3f)
            {
                Vector3 robotPos = robot.transform.position;

                Debug.Log($"Endgame check - Robot position: {robotPos}, Time: {elapsedTime}");

                // Check if robot is in the endgame zone (X and Z bounds)
                bool inEndgameZone = (robotPos.x >= endgame_minX && robotPos.x <= endgame_maxX &&
                                    robotPos.z >= endgame_minZ && robotPos.z <= endgame_maxZ);

                Debug.Log($"Robot in endgame zone: {inEndgameZone}");
                Debug.Log($"Zone bounds - X: [{endgame_minX}, {endgame_maxX}], Z: [{endgame_minZ}, {endgame_maxZ}]");

                if (inEndgameZone)
                {
                    // Robot entered the endgame zone
                    if (!wasInEndgameZone)
                    {
                        wasInEndgameZone = true;
                        Debug.Log("Robot entered endgame zone!");
                    }

                    // Check if robot is doing deep climb (Y > climbThresholdY AND in endgame zone)
                    if (robotPos.y > climbThresholdY)
                    {
                        // Robot is doing deep climb
                        if (!deepClimbScored)
                        {
                            // If previously scored park, subtract those points first
                            if (parkScored)
                            {
                                SubtractBlueScore(2);
                                parkScored = false;
                                Debug.Log("Upgrading from park to deep climb");
                            }
                            AddBlueScore(12);
                            deepClimbScored = true;
                            Debug.Log($"Robot deep climb at Y={robotPos.y} � 12 points awarded to Blue!");
                        }
                    }
                    else if (robotPos.y <= parkThresholdY)
                    {
                        // Robot is parked (below park threshold) AND in endgame zone
                        if (!parkScored && !deepClimbScored)
                        {
                            AddBlueScore(2);
                            parkScored = true;
                            Debug.Log($"Robot parked at Y={robotPos.y} � 2 points awarded to Blue!");
                        }
                        else if (deepClimbScored)
                        {
                            // Robot dropped from climb to park level
                            SubtractBlueScore(12);
                            deepClimbScored = false;
                            AddBlueScore(2);
                            parkScored = true;
                            Debug.Log($"Robot dropped from climb to park at Y={robotPos.y} � 12 points removed, 2 points awarded!");
                        }
                    }
                    else
                    {
                        // Robot is between park and climb thresholds (parkThresholdY < Y <= climbThresholdY)
                        if (deepClimbScored)
                        {
                            // Robot dropped from climb but is still above park level
                            SubtractBlueScore(12);
                            deepClimbScored = false;
                            Debug.Log($"Robot dropped from climb at Y={robotPos.y} � 12 points removed, no park points (above park threshold)!");
                        }
                        // No park points awarded since Y > parkThresholdY
                    }
                }
                else
                {
                    // Robot left the endgame zone or was never in it, remove any endgame points
                    if (wasInEndgameZone)
                    {
                        wasInEndgameZone = false;
                        Debug.Log("Robot left endgame zone!");
                    }

                    if (parkScored)
                    {
                        SubtractBlueScore(2);
                        parkScored = false;
                        Debug.Log("Robot left park zone � 2 points removed from Blue!");
                    }
                    if (deepClimbScored)
                    {
                        SubtractBlueScore(12);
                        deepClimbScored = false;
                        Debug.Log("Robot left deep climb zone � 12 points removed from Blue!");
                    }
                }
            }

            // Only process scoring if match hasn't ended
            if (!MatchEnded())
            {
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name == "Coral(Clone)" && obj.tag != "Coral")
                    {
                        obj.tag = "Coral";
                    }
                }

                GameObject[] algaeObjects = GameObject.FindGameObjectsWithTag("Algae");

                foreach (GameObject algae in algaeObjects)
                {
                    Vector3 pos = algae.transform.position;

                    if (IsWithinBounds(pos, processorCorner1, processorCorner2))
                    {
                        if (!processorCooldowns.ContainsKey(algae) || Time.time - processorCooldowns[algae] >= processorCooldownTime)
                        {
                            AddBlueScore(6);
                            processorCooldowns[algae] = Time.time;
                        }
                    }

                    bool isInBarge = IsWithinBounds(pos, bargeCorner1, bargeCorner2);
                    bool hasScored = algaeInBarge.Contains(algae);
                    bool cooldownReady = !bargeLastScoreTime.ContainsKey(algae) || Time.time - bargeLastScoreTime[algae] >= bargeCooldownTime;

                    if (isInBarge)
                    {
                        if (!hasScored && cooldownReady)
                        {
                            AddBlueScore(4);
                            algaeInBarge.Add(algae);
                            bargeLastScoreTime[algae] = Time.time;
                        }
                    }
                    else if (hasScored)
                    {
                        SubtractBlueScore(4);
                        algaeInBarge.Remove(algae);
                    }
                }

                GameObject[] l4CoralObjects = GameObject.FindGameObjectsWithTag("Coral");

                // L4 Peg zone scoring (5 points)
                HashSet<GameObject> coralsCurrentlyInL4PegZones = new HashSet<GameObject>();

                // L4 Peg zone scoring
                foreach (GameObject coral in l4CoralObjects)
                {
                    Vector3 pos = coral.transform.position;
                    foreach (var (corner1, corner2) in l4PegZones)
                    {
                        if (IsWithinBounds(pos, corner1, corner2))
                        {
                            coralsCurrentlyInL4PegZones.Add(coral);
                            break;
                        }
                    }
                }

                foreach (var coral in coralsCurrentlyInL4PegZones)
                {
                    if (!coralsScoredInPegZones.Contains(coral))
                    {
                        AddBlueScore(InAutonomous() ? 7 : 5);
                        coralsScoredInPegZones.Add(coral);
                    }
                }

                var coralsLeftL4 = new List<GameObject>();
                foreach (var coral in coralsScoredInPegZones)
                {
                    if (!coralsCurrentlyInL4PegZones.Contains(coral))
                    {
                        SubtractBlueScore(InAutonomous() ? 7 : 5);
                        coralsLeftL4.Add(coral);
                    }
                }
                foreach (var coral in coralsLeftL4)
                {
                    coralsScoredInPegZones.Remove(coral);
                }

                // L3 Peg zone scoring
                HashSet<GameObject> coralsCurrentlyInL3PegZones = new HashSet<GameObject>();
                foreach (GameObject coral in l4CoralObjects)
                {
                    Vector3 pos = coral.transform.position;
                    foreach (var (corner1, corner2) in l3PegZones)
                    {
                        if (IsWithinBounds(pos, corner1, corner2))
                        {
                            coralsCurrentlyInL3PegZones.Add(coral);
                            break;
                        }
                    }
                }

                foreach (var coral in coralsCurrentlyInL3PegZones)
                {
                    if (!coralsScoredInL3Zones.Contains(coral))
                    {
                        AddBlueScore(InAutonomous() ? 6 : 4);
                        coralsScoredInL3Zones.Add(coral);
                    }
                }

                var coralsLeftL3 = new List<GameObject>();
                foreach (var coral in coralsScoredInL3Zones)
                {
                    if (!coralsCurrentlyInL3PegZones.Contains(coral))
                    {
                        SubtractBlueScore(InAutonomous() ? 6 : 4);
                        coralsLeftL3.Add(coral);
                    }
                }
                foreach (var coral in coralsLeftL3)
                {
                    coralsScoredInL3Zones.Remove(coral);
                }

                // L2 Peg zone scoring
                HashSet<GameObject> coralsCurrentlyInL2PegZones = new HashSet<GameObject>();
                foreach (GameObject coral in l4CoralObjects)
                {
                    Vector3 pos = coral.transform.position;
                    foreach (var (corner1, corner2) in l2PegZones)
                    {
                        if (IsWithinBounds(pos, corner1, corner2))
                        {
                            coralsCurrentlyInL2PegZones.Add(coral);
                            break;
                        }
                    }
                }

                foreach (var coral in coralsCurrentlyInL2PegZones)
                {
                    if (!coralsScoredInL2Zones.Contains(coral))
                    {
                        AddBlueScore(InAutonomous() ? 4 : 3);
                        coralsScoredInL2Zones.Add(coral);
                    }
                }

                var coralsLeftL2 = new List<GameObject>();
                foreach (var coral in coralsLeftL2)
                {
                    coralsScoredInL2Zones.Remove(coral);
                }

                // L1 zone scoring
                HashSet<GameObject> coralsCurrentlyInL1PegZones = new HashSet<GameObject>();
                foreach (GameObject coral in l4CoralObjects)
                {
                    Vector3 pos = coral.transform.position;
                    foreach (var (corner1, corner2) in l1PegZones)
                    {
                        if (IsWithinBounds(pos, corner1, corner2))
                        {
                            coralsCurrentlyInL1PegZones.Add(coral);
                            break;
                        }
                    }
                }

                foreach (var coral in coralsCurrentlyInL1PegZones)
                {
                    if (!coralsScoredInL1Zones.Contains(coral))
                    {
                        AddBlueScore(InAutonomous() ? 3 : 2);
                        coralsScoredInL1Zones.Add(coral);
                    }
                }

                var coralsLeftL1 = new List<GameObject>();
                foreach (var coral in coralsScoredInL1Zones)
                {
                    if (!coralsCurrentlyInL1PegZones.Contains(coral))
                    {
                        SubtractBlueScore(InAutonomous() ? 3 : 2);
                        coralsLeftL1.Add(coral);
                    }
                }
                foreach (var coral in coralsLeftL1)
                {
                    coralsScoredInL1Zones.Remove(coral);
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

        return pos.x >= minX && pos.x <= maxX &&
               pos.y >= minY && pos.y <= maxY &&
               pos.z >= minZ && pos.z <= maxZ;
    }
}