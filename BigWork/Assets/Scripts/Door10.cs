using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door10 : MonoBehaviour
{
    public Animator door10;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door10.SetTrigger("OpenDoor10");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door10.SetTrigger("CloseDoor10");
        }
    }
}
