using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChufangDoor : MonoBehaviour
{
    public Animator Chufangdoor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Chufangdoor.SetTrigger("ChuFangOpenDoor");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            Chufangdoor.SetTrigger("ChuFangCloseDoor");
        }
    }
}
