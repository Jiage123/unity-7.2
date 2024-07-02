using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door12 : MonoBehaviour
{
    public Animator door15;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door15.SetTrigger("OpenDoor15");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door15.SetTrigger("CloseDoor15");
        }
    }
}
