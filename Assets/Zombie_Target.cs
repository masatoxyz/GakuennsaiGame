using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Zombie_Target : NetworkBehaviour
{

    private NavMeshAgent agent;
    private Transform myTransform;
    private Transform targetTransform;
    //ゾンビが探知するレイヤー
    private LayerMask raycastLayer;
    //ゾンビがPlayerを探知する半径
    private float radius = 100f;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myTransform = transform;
        raycastLayer = 1 << LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SearchForTarget();
        MoveForTarget();
    }

    void SearchForTarget()
    {
        //サーバーじゃなければメソッド終了
        if (!isServer)
        {
            return;
        }

        //Playerをまだ取得していない時
        if (targetTransform == null)
        {
            //Physics.OverlapSphere: ある地点を中心に球を作り、衝突したオブジェクトを取得する
            //第1引数: 中心点 第2引数: 半径 第3引数: 対象のレイヤー
            Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius, raycastLayer);
            if (hitColliders.Length > 0)
            {
                int randomInt = Random.Range(0, hitColliders.Length);
                targetTransform = hitColliders[randomInt].transform;
            }
        }
        //Playerは取得しているがBox Colliderが非アクティブの時 = isDeadがtrueの時
        if (targetTransform != null && targetTransform.GetComponent<BoxCollider>().enabled == false)
        {
            targetTransform = null;
        }
    }

    void MoveForTarget()
    {
        //Playerオブジェクト取得済みで、自分がサーバーの時
        if (targetTransform != null && isServer)
        {
            SetNavDestination(targetTransform);
        }
    }

    void SetNavDestination(Transform dest)
    {
        //ゾンビAIの目的地設定
        agent.SetDestination(dest.position);
    }
}