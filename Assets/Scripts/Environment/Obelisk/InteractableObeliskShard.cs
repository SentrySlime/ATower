using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObeliskShard : MonoBehaviour, IInteractInterface
{
    public Mesh damageMesh;
    public Mesh utilityMesh;
    public Mesh healthMesh;

    public MeshFilter filter;
    CurseReward curseReward;
    public InteractInfo interactInfo;

    Obelisk obelisk;

    public void Interact()
    {
        obelisk.reward = curseReward.gameObject;
        obelisk.Interact();
    }

    public void SetReward(GameObject reward, Obelisk obelisk)
    {
        this.obelisk = obelisk;

        curseReward = reward.GetComponent<CurseReward>();
        SetInteractInfoDescription();
    }

    public void SetInteractInfoDescription()
    {
        if(curseReward.rewardCategory == CurseReward.RewardCategory.Damage)
            filter.mesh = damageMesh;

        if (curseReward.rewardCategory == CurseReward.RewardCategory.Utility)
            filter.mesh = utilityMesh;

        if (curseReward.rewardCategory == CurseReward.RewardCategory.Health)
            filter.mesh = healthMesh;


        interactInfo.description = curseReward.ReturnDescription();
    }
    
}