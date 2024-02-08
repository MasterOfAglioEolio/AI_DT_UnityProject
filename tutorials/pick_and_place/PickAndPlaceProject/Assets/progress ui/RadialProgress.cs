using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    public Text ProgressIndicator;
    public Image LoadingBar;
    public float speed;

    private float currentValue = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        // uiElement.position = screenPos;
        Debug.Log("currentValue:"+currentValue);
        if (currentValue < 100)
        {
            // currentValue += speed * Time.deltaTime;
            // ProgressIndicator.text = ((int)currentValue).ToString() + "%";
            LoadingBar.fillAmount=currentValue/100;
            ProgressIndicator.text = ((int)currentValue).ToString() + "%";
        }
        else
        {
            LoadingBar.fillAmount = 1.0f;
            ProgressIndicator.text = "Done";
        }


    }
    public void IncrementProgress()
    {
        if (currentValue < 100)
        {
            currentValue += 1.0f;
        }
    }
}
