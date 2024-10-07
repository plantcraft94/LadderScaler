using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameManager : MonoBehaviour
{
    public Vector2 LastCheckPointPos;
    GameObject Player;
    GameObject Child;
    Animator anim;
    public static int Floor = 0;
    public List<GameObject> Floors;
    public Transform SpawnLocation;
    public List<GameObject> SpawnedFloor;
    GameObject SpawnFloor;
    GameObject VC;
    private void Start()
    {
        anim = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");
        Child = transform.GetChild(0).gameObject;
        VC = GameObject.FindGameObjectWithTag("VC");
    }

    private void Update()
    {
        transform.position = Player.transform.position;
    }
    public void SpawnLevel( int Floor)
    {
        Player.GetComponent<PlayerMovement>().isClimbing = false;
        if(Floor == 8)
        {
            Player.GetComponent<PlayerMovement>().hasDash = true;
        }

        VC.GetComponent<CinemachineConfiner2D>().enabled = (Floor == 15);
        StartCoroutine(Spawn(Floor));
    }
    IEnumerator Spawn( int FloorLevel)
    {
        yield return new WaitForSeconds(1f);
        print("Spawned");
        if (FloorLevel - 2 >= 0)
        {
            Destroy(SpawnedFloor[FloorLevel - 2]);
        }
        SpawnFloor = Instantiate(Floors[FloorLevel], SpawnLocation.transform.position, Quaternion.identity);
        SpawnedFloor.Add(SpawnFloor);
    }























    public void PlayAnimation()
    {
        anim.Play("FinishScene");
    }
    public void DieTransition()
    {
        Player.transform.position = LastCheckPointPos;
        Player.GetComponent<PlayerMovement>().isDead = false;
        Player.GetComponent<PlayerMovement>().isClimbing = false;
        if (SpawnFloor != null)
        {

            Destroy(SpawnFloor);
            SpawnedFloor.RemoveAt(SpawnedFloor.Count - 1);
            SpawnFloor = Instantiate(Floors[Floor], SpawnLocation.transform.position, Quaternion.identity);
            SpawnedFloor.Add(SpawnFloor);
        }
        
    }
    public void TurnOffTransition()
    {
        Child.SetActive(false);
    }
    public void TurnOnTransition()
    {
        Child.SetActive(true);
    }
}
