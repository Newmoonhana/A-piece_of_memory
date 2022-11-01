using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Event_script : MonoBehaviour
{
    SpriteRenderer _thisSR; //플레이어 대체 오브젝트 스프라이트 렌더러.
    Animator _animator;
    public Animator _animator2; // 다이얼로그 애니메이터.
    public GameObject _player; //실제 플레이어 오브젝트.
    Dialogue dialogue;
    public static bool _eventIn;
    public int e = -1; //이벤트 구별.
    public int _inEvent = 0;   //이벤트 구별 내 대사 구별.
    private readonly WaitForSeconds _waitTime = new WaitForSeconds(1f); //대기 시간.
    int []_cameraVal = new int [2]; //메인카메라 이동 거리 변수(0: 이동 횟수, 1: 한번 당 이동 좌표(거리)).

    //스프라이트.
    public Sprite DamageR_06;
    public Sprite Sword00;

    //오브젝트.
    GameObject _inventory;
    Canvas _invCanvas;
    Camera _camera;
    GameObject _BlackSnow;

    //NPC.
    GameObject _SmithB;
    Animator _SmithB_ani;
    GameObject _Warrior01B;
    GameObject _Soldier01B;
    Animator _Soldier01B_ani;
    GameObject _Doctor01B;
    Animator _Doctor01B_ani;
    GameObject _SanoB;
    Animator _SanoB_ani;
    GameObject _Monster01;
    Animator _Monster01_ani;

    //BGM.
    AudioSource _BGM;
    public static bool _FadeInOutOn;   //true 시 페이드 인/아웃에서 BGM도 페이드 인/아웃. 

    void Awake()
    {
        if (gameObject.scene.name != "MainCutScene")
        {
            _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            //인벤토리 찾기.
            _inventory = GameObject.Find("CanvasInventory");
            _invCanvas = _inventory.GetComponent<Canvas>();
            _invCanvas.worldCamera = _camera;
            _BGM = _inventory.GetComponent<AudioSource>();

            if (gameObject.name == "PlayerActor")   //씬 이벤트일 시 실행.
            {
                _player = GameObject.Find("Player");

                _animator = gameObject.GetComponentInChildren<Animator>();
                _animator2 = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
                dialogue = gameObject.GetComponent<Dialogue>();

                Player_script._FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();

                //인벤토리 비활성화.
                for (int i = 1; i < _inventory.transform.Find("Inventory").childCount; i++)   //아이템 제자리로.
                {
                   Transform _item = _inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().item;
                    if (_item != null)
                    {
                        _item.SetParent(_inventory.transform.Find("Inventory").GetChild(i).GetComponent<InventoryMenu>().select);
                        _item.localPosition = Vector3.zero;
                    }
                }
                _inventory.transform.Find("BackBook01").gameObject.SetActive(false);
                _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(19, -175, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
                _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;
                _inventory.transform.Find("Inventory").gameObject.SetActive(false);
                _inventory.transform.Find("Equip").gameObject.SetActive(false);
                _inventory.transform.Find("Chara").gameObject.SetActive(false);
                _inventory.transform.Find("EquipText").gameObject.SetActive(false);
                _inventory.transform.Find("Page01").gameObject.SetActive(false);
                _inventory.transform.Find("Page").gameObject.SetActive(false);
                if (GameObject.Find("Canvas").transform.Find("PauseChang") != null)
                    GameObject.Find("Canvas").transform.Find("PauseChang").gameObject.SetActive(false);

                //이벤트 시작 NPC 변수 대입 & 설정.
                switch (gameObject.scene.name)
                {
                    case "Event01":
                        _animator.SetInteger("Direction", -1);
                        _SmithB = GameObject.Find("SmithB");
                        _SmithB_ani = _SmithB.GetComponent<Animator>();
                        _Warrior01B = GameObject.Find("Warrior01B");
                        _Doctor01B = GameObject.Find("Doctor01B");
                        _SmithB_ani.SetInteger("Direction", 1);
                        _Warrior01B.GetComponent<Animator>().SetInteger("Direction", 1);
                        _Doctor01B.GetComponent<Animator>().SetInteger("Direction", 1);
                        break;
                    case "Event02":
                        _animator.SetInteger("Direction", 1);
                        break;
                    case "Event04":
                        _animator.SetInteger("Direction", 1);
                        _thisSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
                        break;
                    case "Event06":
                        _animator.SetInteger("Direction", 1);
                        _thisSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
                        _BlackSnow = GameObject.Find("Canvas/BlackSnow");
                        _BlackSnow.SetActive(false);
                        _BGM.Stop();
                        _BGM.GetComponent<AudioSource>().clip = null;
                        break;
                }
            }
            else   //맵 이벤트일 시 실행.
            {
                _animator2 = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
                dialogue = GameObject.Find("Player").GetComponent<Dialogue>();
                //NPC 설정.
                switch (gameObject.scene.name)
                {
                    case "MainScene":
                        _SmithB = GameObject.Find("SmithB");
                        _Warrior01B = GameObject.Find("Warrior01B");
                        _Doctor01B = GameObject.Find("Doctor01B");

                        _SmithB.GetComponent<Animator>().SetInteger("Direction", 1);
                        _Doctor01B.GetComponent<Animator>().SetInteger("Direction", 1);

                        if (InventoryScript._EventOn[3] == 0)
                            _Warrior01B.GetComponent<Animator>().SetInteger("Direction", 1);
                        else
                        {
                            _SmithB.transform.position = new Vector3(-104, -112, 95);
                            _Warrior01B.SetActive(false);
                        }
                        break;
                    case "LakeScene01":
                        if (InventoryScript._EventOn[3] == 1)
                        {
                            _Soldier01B = GameObject.Find("Soldier01B");
                            _Soldier01B.transform.position = new Vector3(110, -113, 95);
                            _Soldier01B.GetComponent<Animator>().SetInteger("Direction", 1);
                        }
                        break;
                }
            }
        }
        else
        {
            StartCoroutine("MainStart");
        }
    }

    void Start()
    {
        if (gameObject.scene.name != "MainCutScene")
        {
            if (gameObject.name == "PlayerActor")   //씬 이벤트일 시 실행.
            {
                _player.SetActive(false);
                //Event00 구별.
                if (gameObject.scene.name != "Event00")
                    StartCoroutine("FadeOut");
                else
                    StartCoroutine("Event00");
            }
        }
    }

    //---화면 이벤트---------------------------------------------------------------------------------.

    //메인화면 시작 시.
    IEnumerator MainStart()
    {
        GameObject _startButton = GameObject.Find("StartButton");
        _startButton.SetActive(false);
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");
        _startButton.SetActive(true);
    }
    //메인화면 버튼 씬 넘어가기 버튼.
    public void StartButton()
    {
        StartCoroutine("SceneWarp");
    }
    //씬 넘어가기.
    IEnumerator SceneWarp()
    {
        StartCoroutine("FadeInEvent");
        yield return StartCoroutine("FadeInEvent");
        SceneManager.LoadScene("Event00");
    }

    //---맵 이벤트-----------------------------------------------------------------------------------.

    public void TriggerStay(Collider2D other)   //플레이어 오브젝트 충돌 여부 함수에서 값을 가져옴.
    {
        if (_animator2.GetBool("IsOpen") == false && !DialogueManager.pauseOn && !InventoryScript.isActive)
        {
            Player_script._dontmove = true;
            GameObject.Find("CanvasInventory").transform.Find("ShortSlot").gameObject.SetActive(false);
            _animator2.SetBool("IsOpen", true);
            switch (other.gameObject.name)
            {
                case "SmithB":
                    StartCoroutine("SmithB01");
                    break;
                case "Warrior01B":
                    StartCoroutine("Warrior01B01");
                    break;
                case "Injured01B":
                    StartCoroutine("Injured01B01");
                    break;
                case "Injured02B":
                    StartCoroutine("Injured02B01");
                    break;
                case "Injured03B":
                    StartCoroutine("Injured03B01");
                    break;
                case "Doctor01B":
                    StartCoroutine("Doctor01B01");
                    break;
                case "Soldier01B":
                    StartCoroutine("Soldier01B01");
                    break;
            }
        }
    }

    IEnumerator EventClass_Map()
    {
        while (e >= 0)
        {
            yield return null;
            if (_animator2.GetBool("IsOpen") == false)
            {
                Player_script._dontmove = false;
                GameObject.Find("CanvasInventory").transform.Find("ShortSlot").gameObject.SetActive(true);
                break;
            }
        }
        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y + 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y - _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
    }

    IEnumerator SmithB01()
    {
        e = 0;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[2]
        {
            x.i[140].t, x.i[0].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[2]
        {
          x.i[3].c, x.i[0].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "말 대장장이";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    IEnumerator Warrior01B01()
    {
        e = 1;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[3]
        {
            x.i[6].t, x.i[58].t, x.i[56].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[3]
        {
          x.i[0].c, x.i[1].c, x.i[0].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "나 건다 말 전사";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    IEnumerator Injured01B01()
    {
        e = 2;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[4]
        {
            x.i[45].t, x.i[54].t, x.i[52].t, x.i[33].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[4]
        {
          x.i[58].c, x.i[54].c, x.i[58].c, x.i[5].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "우리가 그렇게 궁금하시다면";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    IEnumerator Injured02B01()
    {
        e = 3;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[4]
        {
            x.i[128].t, x.i[131].t, x.i[133].t, x.i[23].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[4]
        {
          x.i[128].c, x.i[42].c, x.i[133].c, x.i[42].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "이 세계의 파괴를 막기 위해";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    IEnumerator Injured03B01()
    {
        e = 4;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[4]
        {
            x.i[112].t, x.i[4].t, x.i[137].t, x.i[135].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[4]
        {
            x.i[33].c, x.i[33].c, x.i[0].c, x.i[33].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "사랑과 진실 어둠을 뿌리고 다니는";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    IEnumerator Doctor01B01()
    {
        e = 5;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[8]
        {
            x.i[12].t, x.i[34].t, x.i[89].t, x.i[18].t, x.i[100].t, x.i[57].t, x.i[120].t, x.i[128].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[8]
        {
            x.i[34].c, x.i[0].c, x.i[34].c, x.i[34].c, x.i[34].c, x.i[0].c, x.i[34].c, x.i[0].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "로사! 로이!";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    IEnumerator Soldier01B01()
    {
        e = 6;
        Choice_script.e = e;
        _cameraVal[0] = 50;
        _cameraVal[1] = -2;

        GameObject.Find("Player").GetComponent<Dialogue>().sentences = new string[8]
        {
            x.i[88].t, x.i[56].t, x.i[94].t, x.i[52].t, x.i[63].t, x.i[69].t, x.i[119].t, x.i[58].t
        };

        GameObject.Find("Player").GetComponent<Dialogue>().names = new string[8]
        {
            x.i[26].c, x.i[0].c, x.i[26].c, x.i[0].c, x.i[26].c, x.i[0].c, x.i[26].c, x.i[0].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Choice_script._queName = "시공의 폭풍은 정말 최고야";
        Dialogue._isChoice = true;

        for (int i = 0; i != _cameraVal[0]; i++) //메인 카메라 위치 설정(메인카메라.y - 100).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x, MainCamera_script._camera_xyz.y + _cameraVal[1], MainCamera_script._camera_xyz.z);
            yield return null;
        }
        StartCoroutine("EventClass_Map");
    }

    //---씬 이벤트-----------------------------------------------------------------------------------.

    void MainQue()  //메인 퀘스트 추가.
    {
        if (e != 0)
        {
            GameObject _list = Instantiate(GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(0).GetChild(0).GetChild(0).gameObject);
            _list.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("CanvasInventory").GetComponent<InventoryScript>().ToggleButton(_list));
            _list.gameObject.name = GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(0).GetChild(0).GetChild(0).gameObject.name;
            _list.transform.SetParent(GameObject.Find("CanvasInventory").transform.Find("Page01").Find("QuestList").GetChild(1).GetChild(0));
            _list.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = _list.name;
            _list.transform.localPosition = Vector3.zero;
            _list.transform.localScale = Vector3.one;
            _list.GetComponent<RectTransform>().SetAsFirstSibling();

            //메인퀘 표시.
            for (int i = 0; i < InventoryScript._EventOn.Count; i++)
            {
                if (InventoryScript._EventOn[i] == 1)
                {
                    switch (i)
                    {
                        case 1:
                            //퀘스트 명.
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트01";
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트01";
                            break;
                        case 3:
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트02";
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트02";
                            break;
                        case 5:
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트03";
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트03";
                            break;
                        case 7:
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트04";
                            GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트04";
                            break;
                    }
                    GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).name = GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
                }
            }
        }
    }

    //이벤트 구별.
    IEnumerator EventClass()
    {
        while (e >= 0)
        {
            if (_animator2.GetBool("IsOpen") == false)
            {
                switch (e)
                {
                    case 0:
                        GameObject.Find("CanvasInventory").GetComponent<AudioSource>().clip = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._cilp[0];
                        StartCoroutine("Event00_01");
                        InventoryScript._EventOn[0] = 1;
                        MainQue();
                        e = -1;
                        break;

                    case 1:
                        if (_inEvent == 0)
                            Event01_01();
                        else if (_inEvent == 1)
                        {
                            InventoryScript._EventOn[1] = 1;
                            MainQue();
                            e = -1;
                            StartCoroutine("FadeIn");
                        }
                        break;

                    case 2:
                        if (_inEvent == 0)
                            StartCoroutine("Event02_01");
                        else if (_inEvent == 1)
                            StartCoroutine("Event02_02");
                        else if (_inEvent == 2)
                        {
                            MainCamera_script._camera_xyz = MainCamera_script._camera_xyzBefore;
                            e = -1;
                            InventoryScript._EventOn[2] = 1;
                            StartCoroutine("Event02_03");
                        }
                        break;

                    case 3:
                        if (_inEvent == 0)
                            Event03_01();
                        else if (_inEvent == 1)
                            Event03_02();
                        else if (_inEvent == 2)
                            Event03_03();
                        else if (_inEvent == 3)
                            Event03_04();
                        else if (_inEvent == 4)
                            StartCoroutine("Event03_05");
                        else if (_inEvent == 5)
                            StartCoroutine("Event03_06");
                        else if (_inEvent == 6)
                            StartCoroutine("Event03_07");
                        else if (_inEvent == 7)
                        {
                            InventoryScript._EventOn[3] = 1;
                            MainQue();
                            e = -1;
                            Player_script._sceneBefore = "MainScene";
                            StartCoroutine("FadeIn");
                        }
                        break;
                    case 4:
                        if (_inEvent == 0)
                            Event04_01();
                        else if (_inEvent == 1)
                            StartCoroutine("Event04_02");
                        else if (_inEvent == 2)
                            StartCoroutine("Event04_03");
                        else if (_inEvent == 3)
                            StartCoroutine("Event04_04");
                        else if (_inEvent == 4)
                            StartCoroutine("Event04_05");
                        else if (_inEvent == 5)
                            StartCoroutine("Event04_06");
                        else if (_inEvent == 6)
                            Event04_07();
                        else if (_inEvent == 7)
                        {
                            e = -1;
                            InventoryScript._EventOn[4] = 1;
                            StartCoroutine("Event04_08");
                        }
                        break;
                    case 5:
                        if (_inEvent == 0)
                            StartCoroutine("Event05_01");
                        else if (_inEvent == 1)
                            StartCoroutine("Event05_02");
                        else if (_inEvent == 2)
                        {
                            _player.transform.position = new Vector3(-300f, -113, 10);
                            Player_script._sceneBefore = "Dungen01Scene";
                            InventoryScript._EventOn[5] = 1;
                            MainQue();
                            e = -1;
                            StartCoroutine("FadeIn");
                        }
                        break;
                    case 6:
                        if (_inEvent == 0)
                            StartCoroutine("Event06_01");
                        else if (_inEvent == 1)
                            StartCoroutine("Event06_02");
                        else if (_inEvent == 2)
                        {
                            e = -1;
                            InventoryScript._EventOn[6] = 1;
                            StartCoroutine("Event06_03");
                        }
                        else if (_inEvent == 3)
                        {
                            e = -1;
                            _inEvent = 0;

                            StartCoroutine("FadeInEvent");
                            yield return StartCoroutine("FadeInEvent");

                            _animator.enabled = false;
                            _thisSR.sprite = DamageR_06;
                            
                            StartCoroutine("Event07");
                        }
                        break;
                    case 7:
                        _animator.SetBool("IsMoving", false);
                        Player_script._sceneBefore = "CountryTown00Scene";   //시골 마을.
                        InventoryScript._EventOn[7] = 1;
                        MainQue();
                        e = -1;
                        StopCoroutine("Event07");
                        StartCoroutine("FadeIn");
                        break;
                }
            }
            yield return null;
        }
    }

    //Event00
    IEnumerator Event00()
    {
        yield return _waitTime;
        _eventIn = true;
        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[0].t
        };
        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
          x.i[0].c
        };
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        e = 0;
        StartCoroutine("EventClass");
    }
    IEnumerator Event00_01()
    {
        yield return _waitTime;
        transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("MainScene");
        _player.SetActive(true);
        _player.transform.position = new Vector3(50f, -113, 10);
        Player_script._dontmove = true;
        Player_script._sceneBefore = "Event00";

        yield return new AsyncOperation();
        _player.transform.position = new Vector3(600, -113, 10);
        Player_script._FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
        Dialogue.EventTest = GameObject.Find("EventTest");
        Dialogue._animator = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
        Player_script._FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _waitTime;
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");

        _inventory = GameObject.Find("CanvasInventory");
        _inventory.transform.Find("ShortSlot").gameObject.SetActive(true);
        _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-310, -230, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
        _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;
        Player_script._dontmove = false;
        Player_script._isFade = false;
        //퀘스트 명(메인 퀘스트00 한정).
        GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트00";
        GameObject.Find("CanvasInventory").transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트00";
        Destroy(gameObject);
    }

    //---------------------------------------------------------Event01
    IEnumerator Event01()
    {
        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x > 55; x -= 2)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);

        for (int i = 0; i != 170; i++) //메인 카메라 위치 설정(메인카메라.x - 170).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x - 1, MainCamera_script._camera_xyz.y, MainCamera_script._camera_xyz.z);
            yield return null;
        }

        gameObject.GetComponent<Dialogue>().sentences = new string[13]
        {
            x.i[1].t, x.i[2].t, x.i[3].t, x.i[4].t, x.i[5].t, x.i[6].t, x.i[7].t, x.i[8].t, x.i[9].t, x.i[10].t,
            x.i[11].t, x.i[12].t, x.i[13].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[13]
        {
          x.i[1].c, x.i[2].c, x.i[3].c, x.i[4].c, x.i[5].c, x.i[6].c, x.i[7].c, x.i[8].c, x.i[9].c, x.i[10].c
         ,x.i[11].c, x.i[12].c, x.i[13].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        e = 1;
        StartCoroutine("EventClass");
        // 임의의 변수 값을 설정한다.
        // 다른 함수를 실행하고 그 함수에는 switch문으로 변수 값에 맞는 명령을 실행한다.
    }
    void Event01_01()
    {
        _SmithB_ani.SetInteger("Direction", -1);
        gameObject.GetComponent<Dialogue>().sentences = new string[6]
        {
            x.i[14].t, x.i[15].t, x.i[16].t, x.i[17].t, x.i[18].t, x.i[19].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[6]
        {
          x.i[14].c, x.i[15].c, x.i[16].c, x.i[17].c, x.i[18].c, x.i[19].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Dialogue._isAnswer = true;

        _inEvent = 1;
    }

    //---------------------------------------------------------Event02, 03
    IEnumerator Event02()
    {
        _Soldier01B = GameObject.Find("Soldier01B");
        _Soldier01B_ani = _Soldier01B.GetComponent<Animator>();
        yield return _waitTime;
        _animator.SetBool("IsMoving", false);
        gameObject.GetComponent<Dialogue>().sentences = new string[3]
        {
            x.i[20].t, x.i[21].t, x.i[22].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[3]
        {
          x.i[20].c, x.i[21].c, x.i[22].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        e = 2;
        StartCoroutine("EventClass");
    }
    IEnumerator Event02_01()
    {
        _inEvent = -1;
        yield return _waitTime;
        _Soldier01B.transform.position = new Vector3(280, -113, 95);
        yield return null;
        yield return null;
        yield return null;

        yield return _waitTime;
        _Soldier01B_ani.SetBool("IsMoving", true);
        for (float x = _Soldier01B.transform.position.x; x > 170; x -= 2)
        {
            _Soldier01B.transform.position = new Vector3(x, -113, 95);
            yield return null;
        }
        _Soldier01B_ani.SetBool("IsMoving", false);

        gameObject.GetComponent<Dialogue>().sentences = new string[3]
        {
            x.i[23].t, x.i[24].t, x.i[25].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[3]
        {
          x.i[23].c, x.i[24].c, x.i[25].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 1;
    }
    IEnumerator Event02_02()
    {
        _inEvent = -1;
        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x < 280; x += 2)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
        yield return _waitTime;
        transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;

        _Soldier01B_ani.SetBool("IsMoving", true);
        for (float x = _Soldier01B.transform.position.x; x > 107; x -= 2)
        {
            _Soldier01B.transform.position = new Vector3(x, -113, 95);
            yield return null;
        }
        _Soldier01B_ani.SetBool("IsMoving", false);
        yield return _waitTime;
        _Soldier01B_ani.SetInteger("Direction", 1);
        yield return _waitTime;
        yield return _waitTime;

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[26].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
          x.i[26].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 2;
    }
    IEnumerator Event02_03()
    {
        StartCoroutine("FadeInEvent");
        yield return StartCoroutine("FadeInEvent");
        StartCoroutine("Event03");
    }
    IEnumerator Event03()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("Event03");
        yield return new AsyncOperation();

        transform.position = new Vector3(-600f, -113, 10);
        _animator = gameObject.GetComponentInChildren<Animator>();
        _animator2 = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
        Player_script._FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
        transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;

        _SmithB = GameObject.Find("SmithB");
        _SmithB_ani = _SmithB.GetComponent<Animator>();
        _Warrior01B = GameObject.Find("Warrior01B");
        _Doctor01B = GameObject.Find("Doctor01B");
        _Doctor01B_ani = _Doctor01B.GetComponent<Animator>();
        _SmithB_ani.SetInteger("Direction", 1);
        _Doctor01B_ani.SetInteger("Direction", 1);

        Player_script._FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _waitTime;
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");

        StartCoroutine("Move01");
        for (int i = 0; i != 170; i++) //메인 카메라 위치 설정(메인카메라.x + 170).
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x + 1, MainCamera_script._camera_xyz.y, MainCamera_script._camera_xyz.z);
            yield return null;
        }

        gameObject.GetComponent<Dialogue>().sentences = new string[24]
        {
            x.i[27].t, x.i[28].t, x.i[29].t, x.i[30].t, x.i[31].t, x.i[32].t, x.i[33].t, x.i[34].t, x.i[35].t, x.i[36].t,
            x.i[37].t, x.i[38].t, x.i[39].t, x.i[40].t, x.i[41].t, x.i[42].t, x.i[43].t, x.i[44].t, x.i[45].t, x.i[46].t,
            x.i[47].t, x.i[48].t, x.i[49].t, x.i[50].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[24]
        {
            x.i[27].c, x.i[28].c, x.i[29].c, x.i[30].c, x.i[31].c, x.i[32].c, x.i[33].c, x.i[34].c, x.i[35].c, x.i[36].c,
            x.i[37].c, x.i[38].c, x.i[39].c, x.i[40].c, x.i[41].c, x.i[42].c, x.i[43].c, x.i[44].c, x.i[45].c, x.i[46].c,
            x.i[47].c, x.i[48].c, x.i[49].c, x.i[50].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 0;

        e = 3;
        StartCoroutine("EventClass");
    }
    IEnumerator Move01()    //카메라랑 동시 재생을 위해 코루틴 사용.
    {
        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x < -400; x += 2)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
    }
    void Event03_01()
    {
        _inEvent = -1;
        _SmithB_ani.SetInteger("Direction", -1);
        _Doctor01B_ani.SetInteger("Direction", -1);

        gameObject.GetComponent<Dialogue>().sentences = new string[10]
        {
             x.i[51].t, x.i[52].t, x.i[53].t, x.i[54].t, x.i[55].t, x.i[56].t, x.i[57].t, x.i[58].t, x.i[59].t, x.i[60].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[10]
        {
            x.i[51].c, x.i[52].c, x.i[53].c, x.i[54].c, x.i[55].c, x.i[56].c, x.i[57].c, x.i[58].c, x.i[59].c, x.i[60].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 1;
    }
    void Event03_02()
    {
        _inEvent = -1;
        _SmithB_ani.SetInteger("Direction", 1);
        _Doctor01B_ani.SetInteger("Direction", 1);

        gameObject.GetComponent<Dialogue>().sentences = new string[10]
        {
            x.i[61].t, x.i[62].t, x.i[63].t, x.i[64].t, x.i[65].t, x.i[66].t, x.i[67].t, x.i[68].t, x.i[69].t, x.i[70].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[10]
        {
            x.i[61].c, x.i[62].c, x.i[63].c, x.i[64].c, x.i[65].c, x.i[66].c, x.i[67].c, x.i[68].c, x.i[69].c, x.i[70].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 2;
    }
    void Event03_03()
    {
        _inEvent = -1;
        _SmithB_ani.SetInteger("Direction", -1);

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[71].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[71].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 3;
    }
    void Event03_04()
    {
        _inEvent = -1;
        _SmithB_ani.SetInteger("Direction", 1);

        gameObject.GetComponent<Dialogue>().sentences = new string[13]
        {
            x.i[72].t, x.i[73].t, x.i[74].t, x.i[75].t, x.i[76].t, x.i[77].t, x.i[78].t, x.i[79].t, x.i[80].t, x.i[81].t,
            x.i[82].t, x.i[83].t, x.i[84].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[13]
        {
            x.i[72].c, x.i[73].c, x.i[74].c, x.i[75].c, x.i[76].c, x.i[77].c, x.i[78].c, x.i[79].c, x.i[80].c, x.i[81].c,
            x.i[82].c, x.i[83].c, x.i[84].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 4;
    }
    IEnumerator Event03_05()
    {
        _inEvent = -1;
        StartCoroutine("FadeInEvent");
        yield return _waitTime;

        transform.position = new Vector3(50, -113, 10);
        _animator.SetInteger("Direction", -1);
        _SmithB.transform.position = new Vector3(-104, -112, 95);
        _Warrior01B.SetActive(false);
        MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyzBefore.x - 170, MainCamera_script._camera_xyz.y, MainCamera_script._camera_xyz.z);

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[85].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[85].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        _inEvent = 5;
    }
    IEnumerator Event03_06()
    {
        _inEvent = -1;

        Player_script._FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _waitTime;
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");

        yield return _waitTime;

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[86].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[86].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        _inEvent = 6;
    }
    IEnumerator Event03_07()
    {
        _animator.SetInteger("Direction", 1);
        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[87].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[87].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        _inEvent = 7;
        for (int i = 0; i != 85; i++)
        {
            MainCamera_script._camera_xyz = new Vector3(MainCamera_script._camera_xyz.x + 2, MainCamera_script._camera_xyz.y, MainCamera_script._camera_xyz.z);
            yield return null;
        }
    }

    //---------------------------------------------------------Event04, 05
    IEnumerator Event04()
    {
        _SmithB = GameObject.Find("SmithB");
        _SmithB_ani = _SmithB.GetComponent<Animator>();

        _animator.SetInteger("Direction", 1);
        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x < 550; x += 2)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);

        _SmithB_ani.SetInteger("Direction", 1);

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[88].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[88].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        _SmithB_ani.SetBool("IsMoving", true);
        for (float x = _SmithB.transform.position.x; x < 370; x += 2)
        {
            _SmithB.transform.position = new Vector3(x, -113, 95);
            yield return null;
        }
        _SmithB_ani.SetBool("IsMoving", false);
        e = 4;
        StartCoroutine("EventClass");
    }
    void Event04_01()
    {
        _inEvent = -1;

        gameObject.GetComponent<Dialogue>().sentences = new string[13]
        {
            x.i[89].t, x.i[90].t, x.i[91].t, x.i[92].t, x.i[93].t, x.i[94].t, x.i[95].t, x.i[96].t, x.i[97].t, x.i[98].t,
            x.i[99].t, x.i[100].t, x.i[101].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[13]
        {
            x.i[89].c, x.i[90].c, x.i[91].c, x.i[92].c, x.i[93].c, x.i[94].c, x.i[95].c, x.i[96].c, x.i[97].c, x.i[98].c,
            x.i[99].c, x.i[100].c, x.i[101].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 1;
    }
    IEnumerator Event04_02()
    {
        _inEvent = -1;

        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x < 600; x += 1)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);

        gameObject.GetComponent<Dialogue>().sentences = new string[4]
        {
            x.i[102].t, x.i[103].t, x.i[104].t, x.i[105].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[4]
        {
            x.i[102].c, x.i[103].c, x.i[104].c, x.i[105].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 2;
    }
    void Event04_03()
    {
        _inEvent = -1;
        _animator.SetInteger("Direction", -1);

        gameObject.GetComponent<Dialogue>().sentences = new string[4]
        {
            x.i[106].t, x.i[107].t, x.i[108].t, x.i[109].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[4]
        {
            x.i[106].c, x.i[107].c, x.i[108].c, x.i[109].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 3;
    }
    IEnumerator Event04_04()
    {
        _inEvent = -1;

        _SmithB_ani.SetInteger("Direction", -1);
        _animator.SetBool("IsMoving", true);
        _SmithB_ani.SetBool("IsMoving", true);
        for (float x = _SmithB.transform.position.x; x > -93; x -= 2)
        {
            _SmithB.transform.position = new Vector3(x, -113, 95);
            transform.position = new Vector3(x + 230, -113, 10);
            yield return null;
        }
        _SmithB_ani.SetInteger("Direction", 1);
        _SmithB_ani.SetBool("IsMoving", false);

        for (float x = transform.position.x; x > -30; x -= 2)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);

        _thisSR.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        _thisSR.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sword00;

        gameObject.GetComponent<Dialogue>().sentences = new string[6]
        {
            x.i[110].t, x.i[111].t, x.i[112].t, x.i[113].t, x.i[114].t, x.i[115].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[6]
        {
            x.i[110].c, x.i[111].c, x.i[112].c, x.i[113].c, x.i[114].c, x.i[115].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 4;
    }
    void Event04_05()
    {
        _inEvent = -1;
        _animator.SetInteger("Direction", 1);

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[116].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[116].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 5;
    }
    IEnumerator Event04_06()
    {
        _inEvent = -1;

        gameObject.GetComponent<Dialogue>().sentences = new string[4]
        {
            x.i[117].t, x.i[118].t, x.i[119].t, x.i[120].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[4]
        {
            x.i[117].c, x.i[118].c, x.i[119].c, x.i[120].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x < 600; x += 2)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
        _inEvent = 6;
    }
    void Event04_07()
    {
        _inEvent = -1;

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[121].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[121].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 7;
    }
    IEnumerator Event04_08()
    {
        _FadeInOutOn = false;

        StartCoroutine("FadeInEvent");
        yield return StartCoroutine("FadeInEvent");
        StartCoroutine("Event05");
    }
    IEnumerator Event05()
    {
        _inEvent = 0;

        DontDestroyOnLoad(this);
        SceneManager.LoadScene("Event05");
        yield return new AsyncOperation();
        if (Player_script._AdminMode)
        {
            GameObject.Find("CanvasInventory").GetComponent<AudioSource>().clip = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._cilp[1];
            _BGM.Play();
        }

        transform.position = new Vector3(-280f, -113, 10);
        _animator = gameObject.GetComponentInChildren<Animator>();
        _animator2 = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
        Player_script._FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
        transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;

        _Monster01 = GameObject.Find("Monster01");
        _Monster01_ani = _Monster01.GetComponent<Animator>();

        Player_script._FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _waitTime;
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");

        yield return _waitTime;

        gameObject.GetComponent<Dialogue>().sentences = new string[2]
        {
            x.i[122].t, x.i[123].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[2]
        {
            x.i[122].c, x.i[123].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        e = 5;
        StartCoroutine("EventClass");
    }
    IEnumerator Event05_01()
    {
        _inEvent = -1;

        _animator.SetBool("IsMoving", true);
        for (float x = transform.position.x; x < -150; x += 1)
        {
            transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _animator.SetBool("IsMoving", false);

        gameObject.GetComponent<Dialogue>().sentences = new string[3]
        {
            x.i[124].t, x.i[125].t, x.i[126].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[3]
        {
            x.i[124].c, x.i[125].c, x.i[126].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        _inEvent = 1;
    }
    IEnumerator Event05_02()
    {
        _inEvent = -1;
        _Monster01_ani.SetInteger("MonsterID", 1);
        _Monster01_ani.SetInteger("Direction", -1);
        _Monster01_ani.SetBool("IsMoving", true);
        for (float x = _Monster01.transform.position.x; x > 100; x -= 2)
        {
            _Monster01.transform.position = new Vector3(x, -118, 10);
            yield return null;
        }
        _Monster01_ani.SetBool("IsMoving", false);

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[127].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[127].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        _inEvent = 2;
    }

    //Event06, 07
    IEnumerator Event06()
    {
        if (Player_script._AdminMode)
        {
            _BGM.Stop();
            _BGM.GetComponent<AudioSource>().clip = null;
        }
        yield return _waitTime;

        gameObject.GetComponent<Dialogue>().sentences = new string[5]
        {
            x.i[128].t, x.i[129].t, x.i[130].t, x.i[131].t, x.i[132].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[5]
        {
            x.i[128].c, x.i[129].c, x.i[130].c, x.i[131].c, x.i[132].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        e = 6;
        StartCoroutine("EventClass");
    }
    IEnumerator Event06_01()
    {
        _inEvent = -1;
        gameObject.GetComponent<Dialogue>().sentences = new string[8]
        {
            x.i[133].t, x.i[134].t, x.i[135].t, x.i[136].t, x.i[137].t, x.i[138].t, x.i[139].t, x.i[140].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[8]
        {
            x.i[133].c, x.i[134].c, x.i[135].c, x.i[136].c, x.i[137].c, x.i[138].c, x.i[139].c, x.i[140].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 1;
        _animator.SetInteger("Direction", -1);
        yield return _waitTime;
        _animator.SetInteger("Direction", 1);
    }
    IEnumerator Event06_02()
    {
        _inEvent = -1;
        StartCoroutine("FadeInEvent");
        yield return StartCoroutine("FadeInEvent");
        _eventIn = true;

        gameObject.GetComponent<Dialogue>().sentences = new string[1]
        {
            x.i[141].t
        };

        gameObject.GetComponent<Dialogue>().names = new string[1]
        {
            x.i[141].c
        };

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        _inEvent = 2;
    }
    IEnumerator Event06_03()
    {
        _animator.enabled = false;
        _thisSR.sprite = DamageR_06;
        _BlackSnow.SetActive(true);

        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");

        yield return _waitTime;

        StartCoroutine("FadeInEvent");
        yield return StartCoroutine("FadeInEvent");

        _inEvent = 3;
        StartCoroutine("Event07");
    }
    IEnumerator Event07()
    {
        DontDestroyOnLoad(this);
        _player.transform.position = new Vector3(-400f, -113, 10);
        SceneManager.LoadScene("Event07");
        yield return new AsyncOperation();
        _animator = gameObject.GetComponentInChildren<Animator>();
        _animator2 = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
        Player_script._FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();

        _SanoB = GameObject.Find("SanoB");
        _SanoB_ani = _SanoB.GetComponent<Animator>();

        _thisSR.color = new Color32(145, 132, 185, 255);
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");

        yield return _waitTime;

        _SanoB_ani.SetBool("IsMoving", true);
        for (float x = _SanoB.transform.position.x; x > 100; x -= 1)
        {
            _SanoB.transform.position = new Vector3(x, -113, 10);
            yield return null;
        }
        _SanoB_ani.SetBool("IsMoving", false);

        yield return _waitTime;

        _animator.SetBool("IsMoving", false);
        Player_script._sceneBefore = "CountryTown00Scene";   //시골 마을.
        InventoryScript._EventOn[7] = 1;
        StartCoroutine("FadeIn");
    }

    //페이드 인.
    IEnumerator FadeIn()
    {
        if (Player_script._FadeImage.color != new Color32(0, 0, 0, 0))
        {
            StartCoroutine("FadeInEvent");
        }
        yield return StartCoroutine("FadeInEvent");
        transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;
        DontDestroyOnLoad(this);

        //플레이어 포지션값.
        switch (Player_script._sceneBefore)
        {
            case "MainScene":
                _player.transform.position = new Vector3(55f, -113, 10);
                break;

            case "Dungen01Scene":
                _player.transform.position = new Vector3(-300f, -113, 10);
                break;
        }

        SceneManager.LoadScene(Player_script._sceneBefore);
        _player.SetActive(true);
        Player_script._dontmove = true;

        yield return new AsyncOperation();
        Player_script._FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
        Dialogue.EventTest = GameObject.Find("EventTest");
        Dialogue._animator = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
        MainCamera_script._camera_xyz = MainCamera_script._camera_xyzBefore;

        //씬 이동 후 플레이어 설정.
        switch (Player_script._sceneBefore)
        {
            case "MainScene":
                if (InventoryScript._EventOn[3] == 1)
                {
                    Player_script._animator.SetInteger("Direction", 1);
                }
                else
                {
                    Player_script._animator.SetInteger("Direction", -1);
                }
                break;

            case "Dungen01Scene":
                Player_script._animator.SetInteger("Direction", 1);
                Player_script._MonsterCheck = GameObject.Find("Monster").transform.childCount;
                break;

            case "CountryTown00Scene":
                Player_script._animator.SetInteger("Direction", 1);
                Player_script._animator.SetBool("IsMoving", false);
                break;
        }

        Player_script._FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _waitTime;
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");
        _inventory.transform.Find("ShortSlot").gameObject.SetActive(true);
        _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-310, -230, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
        _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;

        //인벤토리 true에서 씬으로 넘어가 본래 켜졌을 때 활성화.
        /* 인벤토리 시 일시정지 되게 코드가 바뀌어 사용되지 않아서 보류.
        if (Player_script._sceneBefore != "Event00")
        {
            GameObject _UIManager = GameObject.Find("UIManager") as GameObject;
            if (gameObject.name == "PlayerActor")
                if (_UIManager != null)
                    if (InventoryScript.isActive == true)
                    {
                        _inventory.transform.Find("BackBook01").gameObject.SetActive(true);
                        _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-111, -51, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
                        _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 10;
                        _inventory.transform.Find("Inventory").gameObject.SetActive(true);
                        _inventory.transform.Find("Equip").gameObject.SetActive(true);
                        _inventory.transform.Find("Chara").gameObject.SetActive(true);
                        _inventory.transform.Find("EquipText").gameObject.SetActive(true);
                        _inventory.transform.Find("Page").gameObject.SetActive(true);
                    }
        }*/

        Player_script._dontmove = false;
        Player_script._isFade = false;
        Destroy(gameObject);
    }

    //---기타 이벤트-----------------------------------------------------------------------------------.

    //페이드 아웃.
    IEnumerator FadeOut()
    {
        Player_script._FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _waitTime;
        StartCoroutine("FadeOutEvent");
        yield return StartCoroutine("FadeOutEvent");
        StartCoroutine(Player_script._sceneName);
    }

    IEnumerator FadeInEvent()
    {
        if (gameObject.scene.name == "MainCutScene")    //메인화면 한정 페이드 인.
        {
            Image _fadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
            for (int i = 0; i < 255; i += 10)
            {
                _fadeImage.color = new Color32(0, 0, 0, (byte)i);
                //음악 페이드인.
                if (_BGM != null && _FadeInOutOn)
                    _BGM.volume -= 0.05f;
                yield return null;
            }
        }
        else
        {
            for (int i = 0; i < 255; i += 10)
            {
                Player_script._FadeImage.color = new Color32(0, 0, 0, (byte)i);
                //음악 페이드인.
                if (_BGM != null && _FadeInOutOn)
                    _BGM.volume -= 0.05f;
                yield return null;
            }
        }
        if (_BGM != null && _FadeInOutOn)
            _BGM.volume = 0f;
        _eventIn = false;
    }

    IEnumerator FadeOutEvent()
    {
        //음악 재생(재생하기 싫을 땐 클립을 null로).
        if (GameObject.Find("CanvasInventory") != null)
            if (GameObject.Find("CanvasInventory").GetComponent<AudioSource>().clip != null)
                if (!GameObject.Find("CanvasInventory").GetComponent<AudioSource>().isPlaying)
                    GameObject.Find("CanvasInventory").GetComponent<AudioSource>().Play();

        if (gameObject.scene.name == "MainCutScene")    //메인화면 한정 페이드 아웃.
        {
            Image _fadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
            _fadeImage.color = new Color32(0, 0, 0, 255);
            yield return _waitTime;
            for (int i = 255; i > 0; i -= 10)
            {
                _fadeImage.color = new Color32(0, 0, 0, (byte)i);
                //음악 페이드 아웃.
                if (_BGM != null && _FadeInOutOn)
                    _BGM.volume += 0.05f;
                yield return null;
            }
        }
        else
        {
            for (int i = 255; i > 0; i -= 10)
            {
                Player_script._FadeImage.color = new Color32(0, 0, 0, (byte)i);
                yield return null;
                //음악 페이드 아웃.
                if (_BGM != null && _FadeInOutOn)
                    _BGM.volume += 0.05f;
            }
            Player_script.Guard_Sr.color = new Color32(255, 255, 255, 0);
        }
        if (_BGM != null && _FadeInOutOn)
            _BGM.volume = 1f;
        _eventIn = true;
    }

    //-----------------------------------------------------------------------------------------------
}