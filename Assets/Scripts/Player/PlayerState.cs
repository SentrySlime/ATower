using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{

    public enum StateMachine
    {
        defaultMove,
        grapple,
        swing,
        freeze,
        flying

    }

    public StateMachine moveState = StateMachine.defaultMove;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
