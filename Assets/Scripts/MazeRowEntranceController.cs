using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRowEntranceController : MonoBehaviour {

    public MazeController maze;

    // When player collides with Maze Entrance, generate first row of maze
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.Translate(new Vector3(10f, 0f, 0f));
            maze.GenerateNextRow();
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            other.gameObject.SetActive(false);
            maze.GenerateLastRow();
        }
    }
}
