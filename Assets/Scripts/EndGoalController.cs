using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityStandardAssets.Characters.FirstPerson;

public class EndGoalController : MonoBehaviour
{
    public Text winText;

    // When player enters end goal arch, display win message and disable player actions.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            winText.text = "YOU WIN!";
            other.GetComponent<RigidbodyFirstPersonController>().DisablePlayerActions();
        }
    }
}
