using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMerchant : Door
{
    //the only different is that this remembers

    

    public void SetUpNewDoor(Door newDoor)
    {
        targetDoor = newDoor;

    }

}
