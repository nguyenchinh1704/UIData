using System.Collections;
using UnityEngine;


public class Player : Entity
{
	private Animator animator;
	private	SpriteRenderer	spriteRenderer;
	[SerializeField]GameObject panelMinus;
	[SerializeField]GameObject Tree;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		
		base.Setup();
	}

	private void Update()
	{
		
		
	}
	public void Minus()
    {
		MP -= 50;
		panelMinus.SetActive(false);
		Tree.SetActive(false);
		animator.SetTrigger("isCut");
		
    }

    /*private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "Tree")
        {
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Moved)
				{
					MP -= 50;
				}
			}
		}
    }*/

    
    public override float MaxMP => 200;
	public override float MPRecovery => 5;

    public override void TakeDamage(float damage)
    {
       /* HP -= damage;*/

        StartCoroutine("HitAnimation");
    }

    private IEnumerator HitAnimation()
    {
        Color color = spriteRenderer.color;

        color.a = 0.125f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.0515f);

        color.a = 1;
        spriteRenderer.color = color;
    }
}

