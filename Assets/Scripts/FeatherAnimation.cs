using UnityEngine;

public class FeatherAnimation : MonoBehaviour
{
    public float amplitude = 0.5f; // How high it moves
    public float frequency = 1f;   // How fast it moves

    private Vector2 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        float newY = startPosition.y + Mathf.PingPong(Time.time * frequency, amplitude);
        transform.position = new Vector3(startPosition.x, newY);
    }
}
