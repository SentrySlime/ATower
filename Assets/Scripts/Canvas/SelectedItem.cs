using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItem : MonoBehaviour
{

    public ItemPanel selectedPanel;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetSelectedPanel(ItemPanel incomingPanel)
    {
        UnSelectedItem();
        selectedPanel = incomingPanel;
    }

    public void UnSelectedItem()    {
        if(selectedPanel == null ) { return; }
        selectedPanel.UnSelected();

    }

}
