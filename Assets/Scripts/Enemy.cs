using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private int HP = 10;//敌人生命值
    private bool isDeath = false;//敌人是否死亡
    public void HPChanged(int num)//生命值变化
    {
        HP += num;
        if (HP <= 0)
        {
            isDeath = true;
            Destroy(this.gameObject, 1f);
        }
    }


    public NavMeshAgent agent;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        agent.speed = 0.5f;
        agent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
