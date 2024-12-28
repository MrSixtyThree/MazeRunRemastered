using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] Transform goal;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject controller;
    public GameObject player;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask allyMask;
    [SerializeField] LayerMask obstacleMask1;
    [SerializeField] LayerMask obstacleMask2;

    public int health = 100;
    public int maxHealth = 100;
    public int healDelay = 0;
    public int regenRate = 30;
    public float movementSpeed = 1.0f;
    public float accelleration = 8.0f;
    public float increaseIncrement = 0.2f;
    public bool stunned = false;
    public int damage = 0;
    public int delay = 0;

    public bool attackMode = false;
    public float movementSpeedAttack = 2.5f;
    public float accellerationAttack = 20.0f;
    public float viewRadius = 20.0f;
    public float alertRadius = 3.0f;
    public float cooldownBase = 500;
    public float cooldown = 500;
    [Range(0, 360)]
    public float viewAngle = 75.0f;
    


    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        agent.speed = movementSpeed;
        alertRadius = 1.5f + (controller.GetComponent<MazeGen>().MazeHeight / 2);
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // ***** HEALTH *****

        damage = maxHealth - health;
        float colorDifference = (1.0f / maxHealth) * damage;
        enemy.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.9f + colorDifference, 0.9f - colorDifference, 0.9f - colorDifference));

        if (health < maxHealth)
        {
            healDelay++;

            if (healDelay > regenRate)
            {
                healDelay = 0;
                health++;
            }
        }

        if (health <= 0)
        {
            stunned = true;
        }

        // ***** STUNNED *****

        if (stunned)
        {
            attackMode = false;
            agent.speed = 0;
            agent.isStopped = true;
            enemy.GetComponent<Light>().intensity = 0;
            if (health == 100)
            {
                float increase = 1.0f + increaseIncrement;
                if (increase >= 2.5f)
                {
                    increase = 2.5f;
                }
                else
                {
                    movementSpeed *= increase;
                    accelleration *= increase;
                    movementSpeedAttack *= increase;
                    movementSpeedAttack *= increase;
                    accellerationAttack *= increase;
                    accellerationAttack *= increase;
                    cooldownBase *= increase;
                    regenRate = (int)(regenRate / increase);
                    increaseIncrement *= 2;
                }
                
                enemy.GetComponent<Light>().intensity = 5;
                stunned = false;
                agent.speed = movementSpeed;
                agent.isStopped = false;
            }
        }

        // ***** ATTACK MODE *****

        findVisibleTargets();

        if (delay > cooldown)
        {
            delay = 0;
            attackMode = false;
            //Debug.LogError("Attack mode turned to false");
        }

        if (attackMode)
        {
            //Debug.LogError("Cooldown: " + cooldown);
            delay++;
            //Debug.LogError("Delay: " + delay);
            agent.speed = movementSpeedAttack;
            agent.acceleration = accellerationAttack;
            enemy.GetComponent<Light>().color = new Color(1.0f, 0.0f, 0.0f);
            agent.destination = player.transform.position;
            //Debug.LogError("Destination: " + agent.destination);
        }

        if (!attackMode)
        {
            agent.speed = movementSpeed;
            agent.acceleration = accelleration;
            enemy.GetComponent<Light>().color = new Color(1.0f, 1.0f, 1.0f);
            agent.destination = goal.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }

    public void setAgent(GameObject gameObject)
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        enemy = gameObject;
    }

    public void setGoal(Transform target)
    {
        goal = target;
    }

    public void setMasks(LayerMask target, LayerMask obstacle1, LayerMask obstacle2, LayerMask ally)
    {
        targetMask =  target;
        obstacleMask1 = obstacle1;
        obstacleMask2 = obstacle2;
        allyMask = ally;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Destroy(goal.gameObject); 
            controller.GetComponent<MazeGen>().newTarget(enemy);
        }
    }

    public Vector3 DirectionFromAngle(float angle, bool angleisGlobal)
    {
        if (!angleisGlobal)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void findVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 direction = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, direction) < viewAngle / 2)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, direction, distance, obstacleMask1) && !Physics.Raycast(transform.position, direction, distance, obstacleMask2))
                {
                    alertEnemy(cooldownBase);
                    findNearbyAllies();
                }
            }
        }
    }

    void findNearbyAllies()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, alertRadius, allyMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            if(targetsInViewRadius[i] != enemy)
            {
                //float distance = Vector3.Distance(transform.position, targetsInViewRadius[i].transform.position);
                targetsInViewRadius[i].GetComponent<EnemyUI>().alertEnemy(cooldownBase); // * distance
            }
        }
    }

    void alertEnemy(float number)
    {
        attackMode = true;
        cooldown = number;
    }

    public void takeDamage(int damageTaken)
    {
        if (!stunned)
        {
            attackMode = true;
            health = health - damageTaken;
            findNearbyAllies();
        }
    }
}
