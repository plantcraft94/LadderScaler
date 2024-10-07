using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrap : MonoBehaviour
{
    public float delay;
    public float Cooldown;
    Animator anim;
    GameObject Hitbox;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Hitbox = transform.GetChild(0).gameObject;
        StartCoroutine(DelayPlayAnimation());
    }

    IEnumerator DelayPlayAnimation()
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(PlayAnimation());
    }
    IEnumerator PlayAnimation()
    {
        
        print("played");
        anim.Play("Traps");
        yield return new WaitForSeconds(Cooldown);
        StartCoroutine(PlayAnimation());
    }

    public void TurnOnHitBox()
    {
        Hitbox.SetActive(true);
    }
    public void TurnOffHitBox()
    {
        Hitbox.SetActive(false);
    }
}
