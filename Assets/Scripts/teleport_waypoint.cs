using UnityEngine;

public class TeleportWaypoint : MonoBehaviour
{
    [Header("Waypoint Tags")]
    public string waypoint1Tag = "Waypoint1";
    public string waypoint2Tag = "Waypoint2";
    public string waypoint3Tag = "Waypoint3";
    public string waypoint4Tag = "Waypoint4";
    public string waypoint5Tag = "Waypoint5";
    public string waypoint6Tag = "Waypoint6";
    public string waypoint7Tag = "Waypoint7";
    public string waypoint8Tag = "Waypoint8";

    [Header("GameObjects to Switch")]
    public GameObject object1;  // First game object
    public GameObject object2;  // Second game object

    [Header("Settings")]
    public float detectionRadium= 1f;
    public float teleportCooldown = 0.1f;

    private Transform waypoint1;
    private Transform waypoint2;
    private Transform waypoint3;
    private Transform waypoint4;
    private Transform waypoint5;
    private Transform waypoint6;
    private Transform waypoint7;
    private Transform waypoint8;
    private bool canTeleport = true;
    private bool showObject1 = true;

    void Start()
    {
        waypoint1 = FindAndLogWaypoint(waypoint1Tag);
        waypoint2 = FindAndLogWaypoint(waypoint2Tag);
        waypoint3 = FindAndLogWaypoint(waypoint3Tag);
        waypoint4 = FindAndLogWaypoint(waypoint4Tag);
        waypoint5 = FindAndLogWaypoint(waypoint5Tag);
        waypoint6 = FindAndLogWaypoint(waypoint6Tag);
        waypoint7 = FindAndLogWaypoint(waypoint7Tag);
        waypoint8 = FindAndLogWaypoint(waypoint8Tag);

        // Initialize objects
        if (object1 != null) object1.SetActive(true);
        if (object2 != null) object2.SetActive(false);
    }

    private Transform FindAndLogWaypoint(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj != null)
        {
            Debug.Log($"✅ Found waypoint with tag '{tag}': {obj.name}");
            return obj.transform;
        }
        else
        {
            Debug.LogWarning($"❌ Could not find waypoint with tag '{tag}'");
            return null;
        }
    }

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
        if (source == null || destination == null)
        {
            Debug.LogWarning("One or both waypoints are null");
            return;
        }

        float dist = Vector2.Distance(transform.position, source.position);
        Debug.Log($"[DEBUG] Dist to {source.name}: {dist}, Radius: {detectionRadium}");

        if (dist < detectionRadium)
        {
            Debug.Log($"✅ Teleporting to {destination.name}!");
            TeleportPlayer(destination);
        }
        else
        {
            Debug.Log($"❌ Too far to teleport to {source.name}");
        }
    }

    void TeleportPlayer(Transform destination)
    {
        canTeleport = false;

        // Teleport player
        transform.position = destination.position;

        // Switch active game object
        showObject1 = !showObject1;
        if (object1 != null) object1.SetActive(showObject1);
        if (object2 != null) object2.SetActive(!showObject1);

        Invoke(nameof(ResetTeleport), teleportCooldown);
    }

    void ResetTeleport()
    {
        Debug.Log("✅ Teleport cooldown reset");
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