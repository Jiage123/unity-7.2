using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiyuGuanSmallDoor : MonoBehaviour
{
    public Animator door12;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door12.SetTrigger("OpenDoor12");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door12.SetTrigger("CloseDoor12");
        }
    }
}

