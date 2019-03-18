using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterWallController : MonoBehaviour
{
    // When player fires a projectile at an outer wall, the projectile is destroyed.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
