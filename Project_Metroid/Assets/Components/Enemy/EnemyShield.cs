using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : EnemyBase
{



    protected override void Behavior()
    {
        base.Behavior();
        if (!CanBehave()) return;
        //he approaches slowly. he is immune to projectils.
        //he might raise his shield, and if he does that you can no longer just walk through him without being pushed back.
        //they are more likely to raise shield if there are archers nearby.


    }


}
