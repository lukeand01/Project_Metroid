using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject button;
    [SerializeField] TextMeshProUGUI buttonText;

    GameObject holder;
    Vector3 originalPos;

    private void Start()
    {
        holder = transform.GetChild(0).gameObject;
        Observer.instance.EventStartDialogue += StartDialogue;
        originalPos = holder.transform.position;
    }

    public void StartDialogue(List<string> dialogueList)
    {
        buttonText.text = PlayerHandler.instance.GetKey("Interact").ToString();       
        StartCoroutine(DisplayProcess(dialogueList));
    }

    IEnumerator DisplayProcess(List<string> dialogueList)
    {
        PlayerHandler.instance.AddBlock("Dialogue", PlayerHandler.BlockType.Partial);
        holder.SetActive(true);
        dialogueText.text = "";
        button.SetActive(false);
        while (originalPos.y - 100 < holder.transform.position.y)
        {

            holder.transform.position -= new Vector3(0, 1, 0);
            yield return new WaitForSeconds(0.001f);

        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(DialogueProcess(dialogueList));
    }

    IEnumerator DisplayDownProcess()
    {
        while(holder.transform.position.y < originalPos.y)
        {

            holder.transform.position += new Vector3(0, 1, 0);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.1f);
        PlayerHandler.instance.RemoveBlock("Dialogue");
        holder.SetActive(false);
        PlayerHandler.instance.inventoryUI.OpenMerchant();
    }

    IEnumerator DialogueProcess(List<string> dialogueList)
    {
        
        for (int i = 0; i < dialogueList.Count; i++)
        {

            dialogueText.text = "";
            foreach (char letter in dialogueList[i])
            {
                if (letter.ToString() == "$")
                {
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    dialogueText.text += letter;
                    yield return new WaitForSeconds(0.05f);
                }              
            }
            button.SetActive(true);
            yield return new WaitUntil(() => Input.GetKeyDown(PlayerHandler.instance.GetKey("Interact")));
            button.SetActive(false);
            
        }

        StartCoroutine(DisplayDownProcess());
    }


}
