using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject instruction;
    // Start is called before the first frame update
    void Start()
    {
        instruction.SetActive(false);
    }
    public void OpenInstruction()
    {
        instruction.SetActive(true);
    }
    public void CloseInstruction()
    {
        instruction.SetActive(false);
    }
}
