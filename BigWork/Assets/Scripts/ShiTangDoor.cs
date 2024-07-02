using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiTangDoor : MonoBehaviour
{
    public Animator door13;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door13.SetTrigger("OpenDoor13");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door13.SetTrigger("CloseDoor13");
        }
    }
}
