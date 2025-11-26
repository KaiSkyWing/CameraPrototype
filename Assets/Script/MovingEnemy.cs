using UnityEngine;
using System.Collections;

public class MovingEnemy : MonoBehaviour 
{

	UnityEngine.AI.NavMeshAgent agent;
	GameObject player;

	float hp=100;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> (); 
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () 
	{
		agent.SetDestination (player.transform.position);
	}


	//上村編集・よくない書き方なので真似しないで！
	void Attacked(float damage)
	{
		Debug.Log("Attacked");
		hp-=damage;
		if(hp<0)
		{
			Destroy(this.gameObject);
		}
	}
}
