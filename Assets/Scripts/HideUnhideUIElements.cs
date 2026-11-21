using TMPro;
using UnityEngine;

public class HideUnhideUIElements : MonoBehaviour
{
    public GameObject gameobjectToHide;
    public bool hidden=false;
    public bool changeButtonText = false;
    public TMP_Text textToChange;
    public string hiddenText="";
    public string unhiddenText="";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hidden)
        {
            gameobjectToHide.SetActive(false);
            if(changeButtonText)
            {
                textToChange.text= hiddenText;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleHidden()
    {
        gameobjectToHide.SetActive(hidden);
        hidden = !hidden;
        if (changeButtonText)
        {
            if (hidden)
            {
                textToChange.text= hiddenText;
            }
            else
            {
                textToChange.text = unhiddenText;
            }
        }
    }

}
