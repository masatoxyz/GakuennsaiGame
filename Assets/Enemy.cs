using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(ThirdPersonCharacter))]

public class Enemy : MonoBehaviour {

    public GameObject target;
    NavMeshAgent agent;
    ThirdPersonCharacter character;

    // Use this for initialization
    void Start () {

        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();

    }
	
	// Update is called once per frame
	void Update () {

        agent.destination = target.transform.position;
		
	}
}
