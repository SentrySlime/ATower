using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItem : MonoBehaviour
{

    public ItemPanel selectedPanel;
    public GameObject selectedGameObject;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetSelectedPanel(ItemPanel incomingPanel)
    {
        UnSelectedItem();

        selectedGameObject.GetComponent<MeshFilter>().mesh = incomingPanel.itemMesh;
        selectedGameObject.GetComponent<MeshRenderer>().material = incomingPanel.itemMaterial;

        selectedPanel = incomingPanel;
    }

    public void UnSelectedItem()    {
        if(selectedPanel == null ) { return; }
        selectedPanel.UnSelected();

    }

}
