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
        // 在这里编写打开门的代码
        doorOpen = true;
        
        door07.SetTrigger("OpenDoor07");


    }

    void CloseDoor()
    {
        // 在这里编写关闭门的代码
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
