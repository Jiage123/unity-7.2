using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginPanelUI : MonoBehaviour
{
    public GameObject downUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
        SceneManager.LoadScene(1);
    }

    public void DownUI()
    {
        downUI.gameObject.SetActive(true);
    }
}
