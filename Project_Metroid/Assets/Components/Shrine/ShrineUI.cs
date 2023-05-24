using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ShrineUI : MonoBehaviour
{
    //when you interact with shrine it automaticly gets your souls.
    [SerializeField] ShrineIncreaseUnit shrineUnit;

    [SerializeField] Image commitmentBar;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI narratingText;

    [SerializeField] GameObject leaveButton;
    [SerializeField] GameObject confirmButton;
    [SerializeField] GameObject template;
    [SerializeField] GameObject unitHolder;
    GameObject holder;
    float storedSoul;
    float gainedSoul;
    int currentLevel;
    ShrineBuffClass buffClass;
    int currentBuff;
    ShrineUnit currentUnit;
    //we havea problem that the thing 

    private void Start()
    {
        holder = transform.GetChild(0).gameObject;
    }

    public void Confirm()
    {
        //apply the selected buff.
        PlayerHandler.instance.ApplyDirectBD(currentUnit.bd);


        if (buffClass.secondList != null && buffClass.secondList.Count > 0)
        {
            if(currentBuff < 1)
            {
                //that means we went through a second time.
                //narrate about the second gift.
                //then create another bless.
                string text = "you deserve more help...";
                StartCoroutine(NarratingProcess(text, false, buffClass.secondList));
                
                return;
            }


        }


        holder.SetActive(false);
        PlayerHandler.instance.RemoveBlock("Shrine");
        //but we also grant the buff.
    }
    public void Cancel()
    {
        holder.SetActive(false);
        PlayerHandler.instance.RemoveBlock("Shrine");
        //make it ready for the next turn.
    }
    public void ChooseBuff(ShrineUnit unit)
    {
        if(currentUnit != null)
        {
            if(unit.index == currentUnit.index)
            {
                //if thats the case then we close it all here.
                currentUnit.HandleSelected(false);
                currentUnit = null;
                confirmButton.SetActive(false);
                return;
            }


            currentUnit.HandleSelected(false);
        }

        currentUnit = unit;
        currentUnit.HandleSelected(true);


        confirmButton.SetActive(true);
    }



    public void ReceiveOrder(int newSoul, int storedSoul,  int currentLevel, ShrineBuffClass shrineBuffClass)
    {
        leaveButton.SetActive(false);
        confirmButton.SetActive(false);
        PlayerHandler.instance.AddBlock("Shrine", PlayerHandler.BlockType.Partial);
        PlayerHandler.instance.MouseVisible(true);
        buffClass = shrineBuffClass;
        currentBuff = 0;

        holder.SetActive(true);
        this.currentLevel = currentLevel;
        gainedSoul = newSoul;
        this.storedSoul = storedSoul;

        UpdateLevelUI(currentLevel);
        UpdateCommitmentUI();


        if(newSoul == 0)
        {
            //no soul to give.
            Nothing();
            return;
        }
        if(newSoul < 10)
        {
            //say that the shrine will award nothing new.
            NotEnough();
            return;
        }
        if(newSoul >= 10)
        {         
            if (newSoul >= 20)
            {
                MoreThanEnough();

            }
            else
            {
                Enough();
            }

            return;
        }
        

    }

    void Nothing()
    {
        //just say there is nothing.
        string text = "You feel the statue staring into you. it wants nothing from you.";

        StartCoroutine(NarratingProcess(text, false, buffClass.firstList));

    }
    void NotEnough()
    {
        //gave something, but not enough for buff.

        string text = "You feel the statue's embrace. it wants something and it takes wihtout asking. a blue essence comes out of your body. " +
            "then you hear in your head.$ Not enough";

        StartCoroutine(NarratingProcess(text, true, buffClass.firstList));
    }
    
    void Enough()
    {
        //give buff.
        string text = "You feel the statue's embrace. it wants something and it takes wihtout asking. a blue essence comes out of your body. " +
            "then you hear in your head.$ a champion arrives...";

        StartCoroutine(NarratingProcess(text, true, buffClass.firstList));
    }

    void MoreThanEnough()
    {
        //give two buffs.
        string text = "You feel the statue's embrace. it wants something and it takes wihtout asking. a blue essence comes out of your body. " +
         "then you hear in your head.$ the chosen arrives...";

        StartCoroutine(NarratingProcess(text, true, buffClass.firstList));
    }

    void UpdateLevelUI(int currentLevel)
    {
        this.currentLevel = currentLevel;
        levelText.text = currentLevel.ToString();
    }

    void UpdateCommitmentUI()
    {
        commitmentBar.fillAmount = (float)storedSoul / (float)GetMaxSoul();
    }


    IEnumerator NarratingProcess(string text, bool barProcess, List<ConsumableClass> buffList)
    {
        narratingText.gameObject.SetActive(true);
        unitHolder.gameObject.SetActive(false);
        narratingText.text = "";

        foreach (char letter in text)
        {
            if (letter.ToString() == "$")
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                narratingText.text += letter;
                yield return new WaitForSeconds(0.02f);
            }
        }

        if (barProcess)
        {
            //we play process.
            StartCoroutine(BarProcess(buffList));
        }
        else
        {
            if(buffList != null && buffList.Count > 0)
            {
                CreateBuffs(buffList);
            }
            else
            {
                leaveButton.SetActive(true);
            }

            //we offer the cancel button.
            
        }
    }

    public void AcceptNewLevel()
    {
        askLevel = false;
    }
    bool askLevel;
    IEnumerator BarProcess(List<ConsumableClass> buffList)
    {
        //after the barprocess is done we offer the buffs.
        //we showed the gained thing.

        while(gainedSoul > 0)
        {
            gainedSoul -= 0.05f;
            storedSoul += 0.05f;

            if(storedSoul >= GetMaxSoul())
            {
                currentLevel += 1;
                storedSoul = 0;
                askLevel = true;
                UpdateLevelUI(currentLevel);

                if(currentLevel == 1)
                {
                    shrineUnit.gameObject.SetActive(true);
                    shrineUnit.SetUp(currentLevel, this);
                }
                else
                {
                    askLevel = false;
                }
                

                yield return new WaitUntil(() => !askLevel);
                
            }

            UpdateCommitmentUI();
            yield return new WaitForSeconds(0.005f);
        }



        if (buffList != null && buffList.Count > 0)
        {
            //otherwise we show buffs.
            CreateBuffs(buffList);
        }
        else
        {
            leaveButton.SetActive(true);
        }

    }

    void CreateBuffs(List<ConsumableClass> consumableList)
    {
        //from where i will take the sprites?

        unitHolder.SetActive(true);
        narratingText.gameObject.SetActive(false);

        for (int i = 0; i < unitHolder.transform.childCount; i++)
        {
            Destroy(unitHolder.transform.gameObject);
        }

            for (int i = 0; i < consumableList.Count; i++)
            {
                GameObject newObject = Instantiate(template, unitHolder.transform.position, Quaternion.identity);
                newObject.SetActive(true);
                newObject.transform.parent = unitHolder.transform;
                unitHolder.GetComponent<HorizontalGrid>().CorrectIt();
                newObject.GetComponent<ShrineUnit>().SetUp(this, consumableList[i], i);
            }
            currentBuff++;
           

    }


    int GetMaxSoul()
    {
        return 10 + (10 * currentLevel);
    }


}
