using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMap : MonoBehaviour
{

    [SerializeField] GameObject left;
    /*[SerializeField] GameObject right;
    [SerializeField] GameObject video;*/
    public float sec = 5.5f;

    public void Click()
    {
        AutoOff();
    }
   

    IEnumerator AutoOff()
    {
        yield return new WaitForSeconds(sec);
        left.SetActive(false);
        /*right.SetActive(false);
        video.SetActive(false);*/
    }
}
