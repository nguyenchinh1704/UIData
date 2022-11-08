using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.UI;

public class CloseAnimBT : MonoBehaviour
{
    [SerializeField] GameObject pnClose;
    /*[SerializeField] UIElement pnUI;*/
    bool Activate;

    public void Close()
    {

        if (Activate == false)
        {
            pnClose.transform.gameObject.SetActive(true);
            Activate = true;
            
        }
        else
        {
            pnClose.transform.gameObject.SetActive(false);
            Activate = false;
          /*  pnUI.close();*/

        }
    }
}
