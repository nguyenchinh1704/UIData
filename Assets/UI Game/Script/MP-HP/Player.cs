using System.Collections;
using UnityEngine;


public class Player : Entity
{
	private Animator animator;
	private	SpriteRenderer	spriteRenderer;
	[SerializeField]GameObject panelMinusTree;
	[SerializeField] GameObject panelMinusStone;
	[SerializeField]GameObject Tree;
	[SerializeField] GameObject Stone;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		
		base.Setup();
	}

	private void Update()
	{
		
		
	}
	public void MinusTree()
    {
		MP -= 50;
		panelMinusTree.SetActive(false);
		Tree.SetActive(false);
		animator.SetTrigger("isCut");
		
    }
	public void MinusStone()
    {
		MP -= 30;
		panelMinusStone.SetActive(false);
		Stone.SetActive(false);
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

        color.a = 0.5f;
        spriteRenderer.color = color;
    }
}

