using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door14 : MonoBehaviour
{
    public Animator door17;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door17.SetTrigger("OpenDoor17");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door17.SetTrigger("CloseDoor17");
        }
    }
}
