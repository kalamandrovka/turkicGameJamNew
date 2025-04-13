using UnityEngine;

public class teleport_waypoint : MonoBehaviour
{
    [Header("Waypoint Assignments")]
    public Transform waypoint1; // Assign Waypoint 1
    public Transform waypoint2; // Assign Waypoint 2
    public Transform waypoint3; // Assign Waypoint 3
    public Transform waypoint4; // Assign Waypoint 4
    public Transform waypoint5; // Assign Waypoint 5
    public Transform waypoint6; // Assign Waypoint 6
    public Transform waypoint7; // Assign Waypoint 7
    public Transform waypoint8; // Assign Waypoint 8

    [Header("Settings")]
    public float detectionRadius = 0.5f;
    public float teleportCooldown = 0.1f;

    private bool canTeleport = true;

    void Update()
    {
        if (!canTeleport) return;

        CheckTeleport(waypoint1, waypoint2);
        CheckTeleport(waypoint3, waypoint4);
        CheckTeleport(waypoint5, waypoint6);
        CheckTeleport(waypoint7, waypoint8);
    }

    void CheckTeleport(Transform source, Transform destination)
    {
        if (source != null && destination != null &&
            Vector3.Distance(transform.position, source.position) < detectionRadius)
        {
            TeleportPlayer(destination);
        }
    }

    void TeleportPlayer(Transform destination)
    {
        canTeleport = false;
        transform.position = destination.position;
        Invoke(nameof(ResetTeleport), teleportCooldown);
    }

    void ResetTeleport()
    {
        canTeleport = true;
    }

    void OnDrawGizmosSelected()
    {
        DrawTeleportPair(waypoint1, waypoint2);
        DrawTeleportPair(waypoint3, waypoint4);
        DrawTeleportPair(waypoint5, waypoint6);
        DrawTeleportPair(waypoint7, waypoint8);
    }

    void DrawTeleportPair(Transform a, Transform b)
    {
        if (a != null && b != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(a.position, 0.3f);
            Gizmos.DrawSphere(b.position, 0.3f);
            Gizmos.DrawLine(a.position, b.position);
        }
    }
}
