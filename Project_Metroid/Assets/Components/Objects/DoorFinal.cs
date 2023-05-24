using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFinal : Door
{


    public override void Open()
    {
        //we dont teleport anywhere. we bring the end credits.
        //then to menu.
        GameHandler.instance.EndGame();
    }

}
