using UnityEngine;
using LMWidgets;
using System.Collections;

public class ToggleMenuButtonsDataBinder : DataBinderToggle {

    public int associatedMenu;
    private PlayerController playerController;

    override public bool GetCurrentData()
    {
        if (playerController == null)
        {
            playerController = transform.parent.parent.parent.GetComponent<PlayerController>();
        }
        return associatedMenu == playerController.menuShowing;
    }

    override protected void setDataModel(bool value)
    {
        if (playerController == null)
        {
            playerController = transform.parent.parent.parent.GetComponent<PlayerController>();
        }

        playerController.SwitchToMenu(associatedMenu);
    }
}
