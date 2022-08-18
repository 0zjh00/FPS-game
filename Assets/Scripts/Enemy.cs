using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private int HP = 10;//��������ֵ
    private bool isDeath = false;//�����Ƿ�����
    public void HPChanged(int num)//����ֵ�仯
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
