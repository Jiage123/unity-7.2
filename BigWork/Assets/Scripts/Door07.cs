using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door07 : MonoBehaviour
{
    
    private bool doorOpen = false;
    public Animator door07;
    public GameObject interactUI;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!doorOpen)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ShowInteractUI(true);
            
        }
        else
        {
            ShowInteractUI(false);
        }
    }

   

    void OpenDoor()
    {
        // �������д���ŵĴ���
        doorOpen = true;
        
        door07.SetTrigger("OpenDoor07");


    }

    void CloseDoor()
    {
        // �������д�ر��ŵĴ���
        doorOpen = false;
        door07.SetTrigger("CloseDoor07");
    }
    void ShowInteractUI(bool show)
    {
        interactUI.SetActive(show);
        if (show)
        {
            StartCoroutine(HideUIAfterDelay(2f));
        }
    }

    IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactUI.SetActive(false);
    }
}
