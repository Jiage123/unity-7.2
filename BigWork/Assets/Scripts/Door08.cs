using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door08 : MonoBehaviour
{
    public Animator door08;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            door08.SetTrigger("OpenDoor08");
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            door08.SetTrigger("CloseDoor08");
        }
    }
}
