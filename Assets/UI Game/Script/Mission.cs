using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    [SerializeField] GameObject btMission;
    [SerializeField] GameObject pnMission;
    [SerializeField] GameObject pnMission1;
    [SerializeField] GameObject pnMission2;
    [SerializeField] GameObject pnMission3;
    [SerializeField] GameObject pnMission4;
    [SerializeField] GameObject btClose;

    bool Activate;
    public float sec = 3f;

    private void Update()
    {
        MissionActivate();
    }

    private void MissionActivate()
    {
        pnMission.transform.position = new Vector3(pnMission.transform.position.x, btMission.transform.position.y, pnMission.transform.position.z);
    }

    public void OpenAndClose()
    {

       /* if (Activate == false)
        {*/
            /*Activate = false;*/
            pnMission.transform.gameObject.SetActive(true);
            pnMission1.SetActive(false);
            pnMission2.SetActive(false);
            pnMission3.SetActive(false);
            pnMission4.SetActive(false);
            StartCoroutine(AutoOff());
       /* }*/
        /*else
        {
            Activate = false;
            pnMission.transform.gameObject.SetActive(false);
        }*/
    }
    IEnumerator AutoOff()
    {
        yield return new WaitForSeconds(sec);
        pnMission.SetActive(false);
        btClose.SetActive(false);
    }
}
