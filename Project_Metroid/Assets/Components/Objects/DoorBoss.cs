using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBoss : Door
{

    //when you use this door it triggers the boss?



    //this is one side and it connects to a fade door.
    

    public override void Open()
    {
        base.Open();
        //if we go through this door we activate the boss and make the door target fade.
        if (targetDoor.GetComponent<DoorFade>() != null) targetDoor.GetComponent<DoorFade>().StartBoss();
        else Debug.LogError("it wasnt door fade");


    }

}
