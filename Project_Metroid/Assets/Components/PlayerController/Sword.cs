using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    //if this ever touchs an enemy then deal damage.s

    float damage;
    string target;
    float pushModifier;
    bool cannotBeDodged = false;

    [HideInInspector]bool hasSwordCondition;
    [HideInInspector]public float swordState = 100;
    public void SetUp(string target, float damage, float pushModifier = 0, bool cannotBeDodged = false)
    {
        
        //we give current damage.
        this.target = target;
        this.damage = damage;
        this.pushModifier = pushModifier;
        this.cannotBeDodged = cannotBeDodged;

    }

    float CalculateSwordStateModifier()
    {
        //it reduces little up to half of the sword.
        //after half it it starts to affect heavily.
        //in 0 it gets to 60% less damage.

        float value = 0;

        if(swordState == 0)
        {
            return value;
        }

       

        if(swordState > 50 && swordState < 80)
        {
            float valueHalf = swordState - 50;

            //i want to achieve a 1 to 20% debuff.

            //if i have 50 i lost nothing.
            //if i have 1 then i lost 49.
            //49 = 20%
            //lose 2.4 per value.

            //50 * 0.80 = 40;
            //50 * 0.02 = 1;
            //1 * 0.80 = 0.8
            //2 * 0.80 = 1.6
           
            //


            //
            //most at 0.8
        }
        else
        {
            //from 20% to 60%
            //lose 4.8 per value



        }



        return value;
    }


    //can only take damage once from teh same source
    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (cannotBeDodged)
        {
            if(collision.gameObject.tag == "Invisible")
            {
                if (collision.gameObject.GetComponent<IDamageable>() != null)
                {
                    float swordStateModifier = 1;
                    if (hasSwordCondition)
                    {
                        //the way this works. it reduces up to half 
                        swordStateModifier = CalculateSwordStateModifier();
                        swordStateModifier -= 3.5f;
                        swordStateModifier = Mathf.Clamp(swordStateModifier, 0, 100);
                        Observer.instance.OnUpdateSwordState(swordState);
                    }


                    collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage * swordStateModifier, transform.parent.parent.gameObject, pushModifier);
                    return;
                }
            }


        }


        if(collision.gameObject.tag == target)
        {
            if (collision.gameObject.GetComponent<IDamageable>() != null)
            {


                float swordStateModifier = 1;
                if (hasSwordCondition)
                {
                    //the way this works. it reduces up to half 
                    swordStateModifier = CalculateSwordStateModifier();
                    swordStateModifier -= 3.5f;
                    swordStateModifier = Mathf.Clamp(swordStateModifier, 0, 100);
                    Observer.instance.OnUpdateSwordState(swordState);
                }


                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage * swordStateModifier, transform.parent.parent.gameObject, pushModifier) ;
            }

        }


    }


   public void RepairSword()
    {
        swordState = 100;
        Observer.instance.OnUpdateSwordState(swordState);
    }

    /*
    public object CaptureState()
    {
        return null;
        return new SaveData
        {
            swordState = swordState,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        swordState = saveData.swordState;
        Observer.instance.OnUpdateSwordState(swordState);
    }
    [System.Serializable]
    struct SaveData
    {
        public float swordState;
    }
    */
}
