using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dialogue : MonoBehaviour
{
    public string[] names;

    public string[] sentences;

    public Dialogue dialogue;

    public static GameObject EventTest;

    public static Animator _animator;

    bool DialogueOpen = false;
    public static bool _isChoice = false;   //선택지(NPC) 생성 여부.
    public static bool _isAnswer = false;   //선택지(예, 아니오, 모르겠습니다) 생성 여부.

    /*XmlText.itemList[].Name 이거를 줄이면 X.I[].N 이렇게 됩니다.
    X = XmlText
    I = itemList[]
    N = Name
    */

    void Awake()
    {
        _animator = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("K") && !_animator.GetBool("IsOpen") && DialogueOpen && !_isChoice && !_isAnswer)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
        if (!DialogueOpen)
        {
            sentences = new string[0] { };
            names = new string[0] { };
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            DialogueOpen = true;
            if (other.gameObject.name == "EventTest")
            {
                EventTest.GetComponent<XmlTest>().enabled = true;
                //오브젝트에 꺼져있던 스크립트를 켜
                //오브젝트를 비활성화
                //NPC면 첫번째 박스 콜라이더를 비활성화하고 두번째 박스 콜라이더를 활성화하기
                //챕터, 퀘스트, 상황별 변수를 만들어서 체크하기
            }
        }

    }
   void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            DialogueOpen = false;
            
        }
    }
}
