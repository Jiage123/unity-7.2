using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuiYIShiDoor : MonoBehaviour
{
    public Animator HuiYiShidoor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HuiYiShidoor.SetTrigger("HuiYiShiDoorOpen");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            HuiYiShidoor.SetTrigger("HuiYiShiDoorClose");
        }
    }
}
