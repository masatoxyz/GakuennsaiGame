using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class EnemyController : MonoBehaviour
{
    [System.NonSerialized]
    public bool isActive = false;

    [SerializeField]
    Transform target;

    NavMeshAgent agent;
    ThirdPersonCharacter character;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();

        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    private void Update()
    {
        if (isActive)
        {
            agent.isStopped = false;

            if (target != null)
                agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }
        else
        {
            character.Move(Vector3.zero, false, false);
            agent.isStopped = true;

            return;
        }

    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    //他のオブジェクトに接触したときの処理
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("ゲームオーバーです。");

            isActive = false;
        }
    }

    //検出範囲に入った時の処理
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isActive = true;
        }
    }

    //検出範囲から出たときの処理
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isActive = false;
        }
    }
}