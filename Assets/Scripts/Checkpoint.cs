using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager GM;
    CinemachineVirtualCamera VC;
    CinemachineConfiner2D VC2;
    Transform Child;
    Transform Child2;
    Transform Child3;
    bool HasCollided = false;
    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        VC = GameObject.FindGameObjectWithTag("VC").GetComponent<CinemachineVirtualCamera>();
        
        Child = transform.GetChild(0);
        Child2 = transform.GetChild(1);

        if (transform.childCount >= 3)
        {
            Child3 = transform.GetChild(2);
            Child = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else if (transform.childCount < 3)
        {
            print("ok");
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().isClimbing = false;
            collision.gameObject.GetComponent<PlayerMovement>().isonladder = false;
            GM.LastCheckPointPos = transform.position;
            VC.Follow = Child;
            if (!HasCollided)
            {
                GameManager.Floor += 1;
                GM.SpawnLocation = Child2;
                GM.SpawnLevel(GameManager.Floor);
                HasCollided = true;
            }
        }
    }
}
