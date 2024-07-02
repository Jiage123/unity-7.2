using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Animator Right_Door;
    public Animator Left_Door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            Left_Door.SetTrigger("OpenLeft_Door");
            Right_Door.SetTrigger("OpenRight_Door");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            Left_Door.SetTrigger("CloseLeft_Door");
            Right_Door.SetTrigger("CloseRight_Door");
        }
    }
}
