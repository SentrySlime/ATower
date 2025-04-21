using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractInterface
{
    void Interact();

 
}

public interface IInteractWeaponInterface
{

    GameObject InteractWeaponPickUp();
}