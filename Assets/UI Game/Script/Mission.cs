using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    [SerializeField] GameObject btMission;
    [SerializeField] GameObject pnMission;
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

        if (Activate == false)
        {
            pnMission.transform.gameObject.SetActive(true);
            Activate = true;
            StartCoroutine(AutoOff());
        }
        else
        {
            pnMission.transform.gameObject.SetActive(false);
            Activate = false;
        }
    }
    IEnumerator AutoOff()
    {
        yield return new WaitForSeconds(sec);
        pnMission.SetActive(false);
    }
}
