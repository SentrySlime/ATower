using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kobold_Shaman : MonoBehaviour
{
    
    public Animator animator;

    void Start()
    {
        Invoke("PlayAnimation", 1);
    }

    
    void Update()
    {
        
    }

    private void PlayAnimation()
    {
        animator.SetBool("Move", true);
    }
}
