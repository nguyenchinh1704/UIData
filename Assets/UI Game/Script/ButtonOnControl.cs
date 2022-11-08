using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOnControl : MonoBehaviour
{
    [SerializeField] GameObject control;
    bool Activate;


    public void Press()
    {

        if (Activate == false)
        {
            control.transform.gameObject.SetActive(true);
            Activate = true;
        }
        else
        {
            control.transform.gameObject.SetActive(false);
            Activate = false;
        }
    }
    
}
