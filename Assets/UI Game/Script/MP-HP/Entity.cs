using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
	private	Stats	stats;		
	public	Entity	target;		
	
	
	/*public	float	HP
	{
		set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
		get => stats.HP;
	}*/
	
	public	float	MP
	{
		set => stats.MP = Mathf.Clamp(value, 0, MaxMP);
		get => stats.MP;
	}

	
	/*public	abstract float	MaxHP		{ get; }	
	public	abstract float	HPRecovery	{ get; }	*/
	public	abstract float	MaxMP		{ get; }	
	public	abstract float	MPRecovery	{ get; }	

	protected void Setup()
	{
		/*HP = MaxHP;*/
		MP = MaxMP;

		StartCoroutine("Recovery");
	}

	
	protected IEnumerator Recovery()
	{
		while ( true )
		{
			/*if ( HP < MaxHP ) HP += HPRecovery;*/
			if ( MP < MaxMP ) MP += MPRecovery;

			yield return new WaitForSeconds(60);
		}
	}

	
	public abstract void TakeDamage(float damage);
}

[System.Serializable]
public struct Stats
{

	/*[HideInInspector]
	public	float	HP;*/
	[HideInInspector]
	public	float	MP;
}

