using UnityEngine;
using System.Collections; // Add this to use IEnumerator

public class SelfDestructionBullet : MonoBehaviour // Class name corrected
{
    void Start()
    {
        StartCoroutine(Destruct()); // Method name capitalized (convention)
    }

    IEnumerator Destruct() // Method name capitalized (convention)
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}