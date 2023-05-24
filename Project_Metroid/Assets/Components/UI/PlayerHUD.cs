using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    //show stimna, health

    #region BUFF AND DEBUFF
    [Separator("BUFF AND DEBUFF")]
    [SerializeField] GameObject BDHolder;
    [SerializeField] GameObject BDUnitHolder;
    [SerializeField] GameObject BDTemplate;
    
    //how to identify them?
    public void AddBD(ConsumableClass consumable, PlayerConsumption handler, int index)
    {

        //we give the list through here to handler.
        GameObject newObject = Instantiate(BDTemplate, BDUnitHolder.transform.position, Quaternion.identity);
        newObject.SetActive(true);
        newObject.transform.parent = BDUnitHolder.transform;
        BDUnit unit = newObject.GetComponent<BDUnit>();
        unit.SetUp(consumable, handler, index);
        handler.bdList.Add(unit);

    }


    public void SetUpBD()
    {

    }

   
    void ClearUI(GameObject targetHolder)
    {
        for (int i = 0; i < targetHolder.transform.childCount; i++)
        {
            Destroy(targetHolder.transform.GetChild(i).gameObject);
        }
    }


    #endregion


    #region HEALTH AND STAMINA
    [Separator("HEALTH AND STAMINA")]
    [SerializeField] GameObject HSHolder;
    [SerializeField] Image healthBar;
    [SerializeField] Image staminaBar;




    public void UpdateHealth(float old, float current, float max)
    {
        //how to show the grow of resources? 


        healthBar.fillAmount = current / max;
    }

    //it needs a delay and an animation.
    public void CannotUseStamina()
    {
        //a little red glow around the stamina bar.


    }


    public void UpdateStamina(float old, float current, float max)
    {
        staminaBar.fillAmount = current / max;
    }

    #endregion


    #region WEAPON AND SKILL
    [Separator("WEAPON AND SKILL")]
    [SerializeField] GameObject SwordHolder;
    [SerializeField] GameObject SkillHolder;

    #endregion

}
