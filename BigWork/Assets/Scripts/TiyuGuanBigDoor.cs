using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiyuGuanBigDoor : MonoBehaviour
{
    public Animator door11;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door11.SetTrigger("OpenDoor11");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door11.SetTrigger("CloseDoor11");
        }
    }
}
