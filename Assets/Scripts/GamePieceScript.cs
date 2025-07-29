using System.Collections;
using UnityEngine;

public class GamePieceScript : MonoBehaviour
{
    [SerializeField] private GameObject colliderParent;
    [SerializeField] private Rigidbody rb;

    public GamePieces gamePiece;
    public bool lowPerformanceMode;

    // Assign this in Inspector to your BlueScoreUpdater instance
    public BlueScoreUpdater scoreUpdater;

    private void Start()
    {
        if (lowPerformanceMode)
        {
            rb.solverIterations = 1;
            rb.solverVelocityIterations = 1;
        }
    }

    public IEnumerator ReleaseToWorld(float vel, float sideSpin, float backSpin, float actionDelay, GenerateOutake outakePoint, Direction direction)
    {
        yield return new WaitForSeconds(actionDelay);

        outakePoint.hasObject = false;
        outakePoint.gamePiece = null;
        outakePoint.ejected = false;

        rb.useGravity = true;
        rb.isKinematic = false;

        switch (direction)
        {
            case Direction.forward:
                rb.velocity = transform.forward.normalized * vel;
                break;
            case Direction.up:
                rb.velocity = transform.up.normalized * vel;
                break;
            default:
                rb.velocity = transform.forward.normalized * vel;
                break;
        }

        rb.angularVelocity = transform.TransformDirection(new Vector3(-backSpin, sideSpin, 0));

        transform.parent = transform.root.parent;

        EnableColliders();
        StartCoroutine(IgnoreRobot());
    }

    public void MoveToPose(Transform pos)
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        DisableColliders();
        gameObject.transform.position = pos.position;
        gameObject.transform.rotation = pos.rotation;
        gameObject.transform.parent = pos;
    }

    public IEnumerator TransferObject(Transform pos, float actionDelay)
    {
        yield return new WaitForSeconds(actionDelay);

        if (transform.parent.GetComponent<GenerateIntake>())
        {
            transform.parent.GetComponent<GenerateIntake>().GetGamePiece();
        }
        else if (transform.parent.GetComponent<GenerateStow>())
        {
            transform.parent.GetComponent<GenerateStow>().hasObject = false;
            transform.parent.GetComponent<GenerateStow>().GamePiece = null;
            transform.parent.GetComponent<GenerateStow>().transfering = false;
        }

        rb.useGravity = false;
        rb.isKinematic = true;
        DisableColliders();
        gameObject.transform.position = pos.position;
        gameObject.transform.rotation = pos.rotation;
        gameObject.transform.parent = pos;

        if (pos.gameObject.GetComponent<GenerateStow>())
        {
            pos.gameObject.GetComponent<GenerateStow>().hasObject = true;
            pos.gameObject.GetComponent<GenerateStow>().GamePiece = this;
        }
        else if (pos.gameObject.GetComponent<GenerateOutake>())
        {
            pos.gameObject.GetComponent<GenerateOutake>().hasObject = true;
            pos.gameObject.GetComponent<GenerateOutake>().gamePiece = this;
        }
    }

    private void DisableColliders()
    {
        colliderParent.SetActive(false);
    }

    private void EnableColliders()
    {
        colliderParent.SetActive(true);
    }

    private IEnumerator IgnoreRobot()
    {
        rb.excludeLayers = LayerMask.GetMask("Robot");

        yield return new WaitForSeconds(0.1f);

        rb.excludeLayers = new LayerMask();
    }

    public void Destroy()
    {
        Debug.Log("Destroy called on GamePiece.");

        rb.useGravity = true;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.isKinematic = false;

        EnableColliders();
        transform.parent = null;

        StartCoroutine(ApplyArcTowardTarget());

        if (scoreUpdater != null)
        {
            Debug.Log("Adding 6 points to BlueScore.");
            scoreUpdater.AddBlueScore(6);
        }
        else
        {
            Debug.LogWarning("ScoreUpdater reference not assigned!");
        }
    }

    private IEnumerator ApplyArcTowardTarget()
    {
        yield return new WaitForSeconds(0.1f);

        float zBlend = 0.25f;
        float blendedZ = Mathf.Lerp(transform.position.z, 0f, zBlend);

        Vector3 baseTarget = new Vector3(0.05f, 2f, blendedZ);

        Vector3 randomOffset = new Vector3(
            Random.Range(-0.02f, 0.02f),
            0,
            Random.Range(-0.005f, 0.005f)
        );

        Vector3 target = baseTarget + randomOffset;

        float flightTime = 1.5f;

        Vector3 launchVelocity = CalculateLaunchVelocity(transform.position, target, flightTime);

        rb.velocity = launchVelocity;

        rb.angularVelocity = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f)
        );
    }

    private Vector3 CalculateLaunchVelocity(Vector3 origin, Vector3 target, float timeToTarget)
    {
        Vector3 displacement = target - origin;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float verticalDisplacement = displacement.y;
        float horizontalDistance = displacementXZ.magnitude;

        float gravity = Mathf.Abs(Physics.gravity.y);

        float Vy = (verticalDisplacement / timeToTarget) + (0.5f * gravity * timeToTarget);
        float Vxz = horizontalDistance / timeToTarget;

        Vector3 horizontalDirection = displacementXZ.normalized;

        Vector3 result = horizontalDirection * Vxz;
        result.y = Vy;

        return result;
    }
}
