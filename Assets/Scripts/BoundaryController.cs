using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    public MazeController maze;

    // If player fires a projectile at an outer boundary, destroy the projectile and finish the maze.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            other.gameObject.SetActive(false);

            if (!maze.GetMazeCompleted())
            {
                maze.GenerateLastRow();
            }
        }
    }
}
