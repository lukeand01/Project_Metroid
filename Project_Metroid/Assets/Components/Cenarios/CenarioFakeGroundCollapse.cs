using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenarioFakeGroundCollapse : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //when something collides with it.
        if (collision.gameObject.tag == "Enemy") return;

        //otherwise we will crumble the floor.
        //or dagger or arrows.

    }



}
