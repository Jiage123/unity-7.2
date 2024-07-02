using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobalMap : MonoBehaviour
{
    public GameObject uiPanel;
    // Start is called before the first frame update
    void Start()
    {
        uiPanel.SetActive(false);
    }




    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == "GobalMap")
                {
                    uiPanel.SetActive(true);
                }
            }
        }
    }

    public void UIClose()
    {
        uiPanel.SetActive(false);
    }
}
