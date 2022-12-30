using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCultivation : MonoBehaviour
{
    [SerializeField] GameObject fruitMin;
    public float time = 12f;
    [SerializeField] GameObject clock;


    public void Click()
    {
        clock.SetActive(true);
        fruitMin.SetActive(true);
        /*StartCoroutine(AutoOff());*/

    }

    /*IEnumerable AutoOff()
    {
        yield return new WaitForSeconds(time);
        clock.SetActive(false);
    }*/
}
