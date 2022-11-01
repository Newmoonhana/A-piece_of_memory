using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;

    void Awake()
    {
        if (GameObject.Find("Player") == null)
            dialogue = GameObject.Find("PlayerActor").GetComponent<Dialogue>();
        else
            dialogue = GameObject.Find("Player").GetComponent<Dialogue>();
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
