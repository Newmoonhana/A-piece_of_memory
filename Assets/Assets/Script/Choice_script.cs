using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice_script : MonoBehaviour
{
    //SubQue(string name, int e)에 대입할 함수.
    public static string _queName;  //퀘스트 이름.
    public static int e;    //퀘스트 ID.

    public void ChoiceButton(GameObject _choice)    //선택지 버튼 눌렀을 때.
    {
        Dialogue._isChoice = false;
        Dialogue._isAnswer = false;
        switch (_choice.name)
        {
            case "ShopChoice":  //상점.
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().DisplayNextSentence();
                break;
            case "QuestChoice01":   //퀘스트.
            case "QuestChoice02":
            case "QuestChoice03":
                SubQue(_queName, e);
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().DisplayNextSentence();
                break;
            case "OtherChoice": //잡담.
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().DisplayNextSentence();
                break;
            case "YesChoice":   //예 선택지.
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().DisplayNextSentence();
                break;
            case "NoChoice":    //아니오 선택지.
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().DisplayNextSentence();
                break;
            case "WhatChoice":  //모르겠습니다 선택지.
                GameObject.Find("DialogueManager").GetComponent<DialogueManager>().DisplayNextSentence();
                break;
            case "CloseChoice": //창 닫기.
                GameObject.Find("Canvas/Dialogue").GetComponent<Animator>().SetBool("IsOpen", false);
                FindObjectOfType<DialogueManager>().StartDialogue(GameObject.Find("Player").GetComponent<Dialogue>().dialogue);
                break;
        }
    }

    void SubQue(string name, int e) //서브퀘스트 추가.
    {
        if (InventoryScript._SubQue[e] == -1)   //퀘스트 수락.
        {
            GameObject _list = Instantiate(GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._ListObj);
            _list.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("CanvasInventory").GetComponent<InventoryScript>().ToggleButton(_list));
            _list.name = name;
            _list.transform.SetParent(GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0));
            _list.transform.localPosition = Vector3.zero;
            _list.transform.localScale = Vector3.one;
            _list.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = _list.name;
            _list.GetComponent<RectTransform>().SetAsLastSibling();
            InventoryScript._SubQue[e] = 0;
        }
        else if (InventoryScript._SubQue[e] == 0)   //퀘스트 완료.
        {
            InventoryScript._SubQue[e] = 1;
            for (int i = 0; i < GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(0).GetChild(0).childCount; i++)
            {
                if (GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(0).GetChild(0).GetChild(i).name == name)
                {
                    GameObject _list = Instantiate(GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(0).GetChild(0).GetChild(i).gameObject);
                    _list.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("CanvasInventory").GetComponent<InventoryScript>().ToggleButton(_list));
                    _list.name = name;
                    _list.transform.SetParent(GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(1).GetChild(0));
                    _list.transform.localPosition = Vector3.zero;
                    _list.transform.localScale = Vector3.one;
                    _list.GetComponent<RectTransform>().SetAsFirstSibling();
                    Destroy(GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(0).GetChild(0).GetChild(i).gameObject);
                    break;
                }
            }
        }
    }
}
