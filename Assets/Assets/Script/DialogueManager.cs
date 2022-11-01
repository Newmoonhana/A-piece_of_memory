using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public GameObject nameText;
    public GameObject dialogueText;

    public Animator animator;

    private Queue<string> sentences;
    private Queue<string> names;

    public GameObject _Next;
    GameObject _Choice;

    private readonly WaitForSeconds _waitTime = new WaitForSeconds(0.02f); //대기 시간2.

    public static bool pauseOn = false;

    private bool TextOk = true;
    //float TextSkip = 0.01f;
    int i = 0;
    private bool Display = true;

    void Start()
    {
        sentences = new Queue<string>();
        names = new Queue<string>();
        _Next = GameObject.Find("Canvas/Dialogue/Inv_arrow");
        _Choice = GameObject.Find("Canvas/Dialogue/Choice");
    }

    void Update()
    {
        //스페이스바로 대사 넘기기
        if (Input.GetKeyDown(KeyCode.Space) == true && TextOk == true || Input.GetKeyDown(KeyCode.Return) == true && TextOk == true || Input.GetKeyDown(KeyCode.Z) == true)
        {
                DisplayNextSentence();
        }

        //일시정지
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (GameObject.Find("Canvas").transform.Find("SkipButton") != null) //씬 이벤트(스킵 버튼).
            {
                if (GameObject.Find("Canvas").transform.Find("SkipButton").gameObject.activeSelf)
                {
                    GameObject.Find("Canvas").transform.Find("SkipButton").GetComponent<SkipButton_script>().SkipButton();
                }
                else if (Event_script._eventIn && !GameObject.Find("Canvas").transform.Find("SkipButton").GetComponent<SkipButton_script>()._skip && !Dialogue._isChoice && !Dialogue._isAnswer)
                {
                    SkipButton_script.corutinOn = false;
                    GameObject.Find("Canvas").transform.Find("SkipButton").gameObject.SetActive(true);
                }
            }
            else if (!Player_script._isFade && !InventoryScript.isActive)   //맵 일시정지 창.
            {
                if (GameObject.Find("Canvas").transform.Find("PauseChang").gameObject.activeSelf)
                {
                    GameObject.Find("Canvas").transform.Find("PauseChang").gameObject.SetActive(false);
                }
                else
                {
                    GameObject.Find("Canvas").transform.Find("PauseChang").gameObject.SetActive(true);
                }
                pauseOn = !pauseOn;
                InvSet();
                Time.timeScale = pauseOn == true ? 0f : 1.0f;
            }
        }
    }

    //인벤토리 온오프 여부 함수.
    void InvSet()
    {
        GameObject _inventory = GameObject.Find("CanvasInventory");
        if (pauseOn)    //일시정지 시 인벤토리 닫기.
        {
            _inventory.transform.Find("BackBook01").gameObject.SetActive(false);
            _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-310, -230, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
            _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;
            _inventory.transform.Find("Inventory").gameObject.SetActive(false);
            _inventory.transform.Find("Equip").gameObject.SetActive(false);
            _inventory.transform.Find("Chara").gameObject.SetActive(false);
            _inventory.transform.Find("EquipText").gameObject.SetActive(false);
            _inventory.transform.Find("Page01").gameObject.SetActive(false);
            _inventory.transform.Find("Page").gameObject.SetActive(false);
            //아이템 제자리로.
            for (int i = 1; i < _inventory.transform.Find("Inventory").childCount; i++)
            {
                Transform _item = _inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().item;
                if (_item != null)
                {
                    _item.SetParent(_inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().select);
                    _item.localPosition = Vector3.zero;
                }
            }
        }
        else   //인벤토리 일시정지 해제 시 활성화.
        {
            GameObject _UIManager = GameObject.Find("UIManager") as GameObject;
            if (_UIManager != null)
                if (InventoryScript.isActive == true)
                {
                    _inventory.transform.Find("BackBook01").gameObject.SetActive(true);
                    _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-111, -51, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().position.z);
                    _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 10;
                    _inventory.transform.Find("Inventory").gameObject.SetActive(true);
                    _inventory.transform.Find("Equip").gameObject.SetActive(true);
                    _inventory.transform.Find("Chara").gameObject.SetActive(true);
                    _inventory.transform.Find("EquipText").gameObject.SetActive(true);
                    _inventory.transform.Find("Page").gameObject.SetActive(true);
                }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        sentences.Clear();
        names.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence); //sentence값을 sentences에 넣기
        }

        foreach (string name in dialogue.names)
        {
            names.Enqueue(name);
        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (!pauseOn && !Dialogue._isChoice && !Dialogue._isAnswer)
        {
            if (TextOk)
            {
                if (sentences.Count == 0)
                {
                    EndDialogue();
                    return;
                }
                string sentence = sentences.Dequeue();
                string name = names.Dequeue();
                StopAllCoroutines();
                StartCoroutine(TypeSentence(sentence));
                nameText.GetComponent<TMPro.TextMeshProUGUI>().text = name;
            }
            else
                Display = true;

            //대화 하나라도 넘어가면 스킵 버튼 꺼지게.
            if (GameObject.Find("Canvas").transform.Find("SkipButton") != null)
                GameObject.Find("Canvas").transform.Find("SkipButton").gameObject.SetActive(false);
        }
    }
    IEnumerator TypeSentence(string sentence) //텍스트 한글자씩 출력
    {
        dialogueText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        Display = false;
        i = 0;
        TextOk = false;
        _Next.SetActive(false);
        _Choice.SetActive(false);
        for (int i = 0; i < _Choice.transform.childCount; i++)
            _Choice.transform.GetChild(i).gameObject.SetActive(false);
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.GetComponent<TMPro.TextMeshProUGUI>().text += letter;

            if (i <= 6)
                i = ++i;

            if (Input.GetKey(KeyCode.Space) && i >= 5)
            {

            }
            else if (Input.GetKey(KeyCode.Return) && i >= 5)
            {

            }
            else if (Display && i >= 6)
            {

            }
            else
            {
                yield return _waitTime;
            }
        }
        if (Dialogue._isChoice || Dialogue._isAnswer)   //선택지 활성화 여부.
        {
            _Next.SetActive(false);
            _Choice.SetActive(true);
            bool A;
            if (Dialogue._isChoice)
            {
                A = true;
            }
            else
            {
                A = false;
            }
            for (int i = 0; i < _Choice.transform.childCount; i++)
            {
                switch (_Choice.transform.GetChild(i).gameObject.name)
                {
                    case "ShopChoice":
                        if (GameObject.Find("ScriptObj") != null)
                        {
                            if (GameObject.Find("ScriptObj").GetComponent<Event_script>().e != 0)
                                _Choice.transform.GetChild(i).gameObject.SetActive(A);
                        }
                        else
                        {
                            if (GameObject.Find("PlayerActor").GetComponent<Event_script>().e != 0)
                                _Choice.transform.GetChild(i).gameObject.SetActive(A);
                        }
                        break;
                    case "QuestChoice01":
                    case "QuestChoice02":
                    case "QuestChoice03":
                    case "OtherChoice":
                    case "CloseChoice":
                        _Choice.transform.GetChild(i).gameObject.SetActive(A);
                        break;
                    default:
                        _Choice.transform.GetChild(i).gameObject.SetActive(!A);
                        break;
                }
            }
        }
        else
        {
            _Next.SetActive(true);
            _Choice.SetActive(false);
            for (int i = 0; i < _Choice.transform.childCount; i++)
            {
                _Choice.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        Display = false;
        TextOk = true;
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }
}