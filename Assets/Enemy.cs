using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]


public class Enemy : MonoBehaviour {

    public GameObject target;
    NavMeshAgent agent;
   

    // Use this for initialization
    void Start () {

        agent = GetComponent<NavMeshAgent>();
        

    }
	
	// Update is called once per frame
	void Update () {

        agent.destination = target.transform.position;
		
	}
}
