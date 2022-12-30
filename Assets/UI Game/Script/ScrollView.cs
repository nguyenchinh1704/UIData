using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.UI;

public class ScrollView : MonoBehaviour
{
    [SerializeField] UIElement If1;
    [SerializeField] UIElement If2;
    [SerializeField] UIElement If3;
    [SerializeField] GameObject pnOpen;
    [SerializeField] UIElement pnCultivation;
    /*[SerializeField] UIElement show;*/
    /*[SerializeField] UIElement close;*/


    public void Button1()
    {
        If1.show(true);
        If2.close();
        If3.close();
    }

    public void Button2()
    {
        If1.close();
        If3.close();
        If2.show(true);
    }
    public void Button3()
    {
        If1.close();
        If2.close();
        If3.show(true);
    }

    public void OpenCultivation()
    {
        pnOpen.SetActive(false);
        pnCultivation.show();
    }

    /*public void Show()
    {
        show.show();
    }*/
}
