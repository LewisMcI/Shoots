using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionUI : AddDelegate
{
    public string[] options;

    private int currentOptionIndex = 0;

    private TextMeshProUGUI tmpUI;

    private void Awake()
    {
        tmpUI = GetComponent<TextMeshProUGUI>();
    }

    public void MoveRight()
    {
        currentOptionIndex++;
        if (currentOptionIndex >= options.Length)
            currentOptionIndex = 0;
        Action();
    }
    public void MoveLeft()
    {
        currentOptionIndex--;
        if (currentOptionIndex < 0)
            currentOptionIndex = options.Length - 1;
        Action();
    }

    void Action()
    {
        tmpUI.text = options[currentOptionIndex];
        CallDelegate(currentOptionIndex);
    }
}
