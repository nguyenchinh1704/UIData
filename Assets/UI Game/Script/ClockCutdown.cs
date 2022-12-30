using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockCutdown : MonoBehaviour
{
    public Text timer;
    public float currenttime;
    public float time = 2f;
    [SerializeField] GameObject fruitMax;
    [SerializeField] GameObject fruitMin;
    [SerializeField] GameObject clock;


    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (currenttime > 0)
        {
            currenttime -= Time.deltaTime;
        }
        else
        {
            currenttime = 0;
        }
        DisplayTime(currenttime);
        if(currenttime == 0)
        {
            timer.text = currenttime.ToString("Done");
            fruitMax.SetActive(true);
            fruitMin.SetActive(false);
            /*StartCoroutine(AutoOff());*/
        }
    }
     void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay <0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float sec = Mathf.FloorToInt(timeToDisplay % 60);

        timer.text = string.Format("{0:00}:{1:00}", minutes, sec);
    }

    /*IEnumerable AutoOff()
    {
        yield return new WaitForSeconds(time);
        clock.SetActive(false);
    }*/
}
