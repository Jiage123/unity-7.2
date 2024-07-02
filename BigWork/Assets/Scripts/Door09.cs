using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door09 : MonoBehaviour
{
    public Animator door09;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door09.SetTrigger("OpenDoor09");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door09.SetTrigger("CloseDoor09");
        }
    }
}
