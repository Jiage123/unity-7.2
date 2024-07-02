using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door13 : MonoBehaviour
{
    public Animator door16;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door16.SetTrigger("OpenDoor16");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door16.SetTrigger("CloseDoor16");
        }
    }
}
