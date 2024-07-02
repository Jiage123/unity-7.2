using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door11 : MonoBehaviour
{
    public Animator door14;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door14.SetTrigger("OpenDoor14");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door14.SetTrigger("CloseDoor14");
        }
    }
}
