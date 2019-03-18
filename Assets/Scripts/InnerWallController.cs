using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerWallController : MonoBehaviour {

    private int damage;
    public Material firstHitMaterial;
    public Material secondHitMaterial;

    // Wall is destroyed if three projectiles collide with it.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            other.gameObject.SetActive(false);
            damage++;
            if (damage == 1)
            {
                // Change color of wall.
                GetComponent<MeshRenderer>().material = firstHitMaterial;
            }
            else if (damage == 2)
            {
                // Change color of wall.
                GetComponent<MeshRenderer>().material = secondHitMaterial;
            }
            else if (damage >= 3)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
