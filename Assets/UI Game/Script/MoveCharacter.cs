using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter: MonoBehaviour
{
	/*private CharacterController controller;*/
	private Animator animator;
    [SerializeField]private Joystick joystick;
    [SerializeField]private float speed;
    [SerializeField] GameObject panelMP;
    [SerializeField] GameObject panelMP1;
    [SerializeField] GameObject pnOpen;
    [SerializeField] GameObject pnCultivation;
    [SerializeField] GameObject clock;

    public static MoveCharacter instance;
    private void OnEnable()
    {
        instance = this;
    }


    private void Start()
    {
        /*controller = GetComponent<CharacterController>();*/
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        float x = joystick.Horizontal;
        float y = joystick.Vertical;

        Vector3 dir = new Vector3(x, 0f, y).normalized;
        

        if(dir.magnitude >= 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            transform.position += new Vector3(x, 0f, y) * speed * Time.deltaTime;

        }
        if (x != 0 || y != 0)
        {
            animator.SetTrigger("isRun");

        }
        else
        {
            animator.SetTrigger("isIdle");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tree")
        {
            panelMP.SetActive(true);
        }
        else
        {
            panelMP.SetActive(false);
        }
        if(collision.gameObject.tag == "Stone")
        {
            panelMP1.SetActive(true);
        }
        else
        {
            panelMP1.SetActive(false);
        }
        if(collision.gameObject.tag == "Soil")
        {
            pnOpen.SetActive(true);
        }
        else
        {
            pnOpen.SetActive(false);
        }
        if(collision.gameObject.tag == "Cultivation Min")
        {
            clock.SetActive(true);
            pnOpen.SetActive(false);
        }
        else
        {
            clock.SetActive(false);
        }
    }
}
