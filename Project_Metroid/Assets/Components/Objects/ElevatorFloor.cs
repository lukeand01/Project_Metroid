using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloor : MonoBehaviour
{
    //it desactives teh barriers when the elevator arrives.
    [SerializeField] GameObject barrier1;
    [SerializeField] GameObject barrier2;


    public void Activate(bool choice)
    {
        barrier1.SetActive(choice);
        barrier2.SetActive(choice);
    }


}
