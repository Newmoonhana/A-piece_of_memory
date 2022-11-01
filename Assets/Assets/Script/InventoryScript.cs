using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{

    private static InventoryScript _inventory_Instance = null;
    public static bool isActive = false;
    public Toggle[] toggles;
    public GameObject[] equipMenus;

    public List<GameObject> _slotList = new List<GameObject>();

    public Sprite[] _itemIcon;
    public int[] _itemM;    //칸 번호:id, 변수 값:소지수.

    public static int _bookPage = 0;   //0: 인벤토리/장비, 1: 퀘스트.
    public static List<int> _EventOn = new List<int>();   //메인퀘 이벤트 실행 여부, 0: false, 1: true.
    public static List<int> _SubQue = new List<int>();   //서브퀘 이벤트 실행 여부, -1: 퀘스트를 받지 않음, 0: false, 1: true.
    public GameObject _ListObj;  //서브퀘 목록 프리팹.

    public AudioClip[] _cilp;   //배경음악 목록.
    public AudioClip[] _SEcilp;   //효과음 목록.

    void Awake()
    {
        if (_inventory_Instance != null)   //싱글톤.
        {
            Destroy(gameObject);
            return;
        }
        _inventory_Instance = this;
        DontDestroyOnLoad(this);    //씬 이동 후 오브젝트 보존.
        _itemM = new int[900];

        //첫 실행 시 첫 메뉴 빼고 비활성화.
        for (int i = 0; i < equipMenus.Length; i++)
        {
            if (i == 0)
                continue;
            equipMenus[i].SetActive(false);
        }

        GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        this.transform.Find("BackBook01").gameObject.SetActive(isActive);
        this.transform.Find("Inventory").gameObject.SetActive(isActive);
        this.transform.Find("Equip").gameObject.SetActive(isActive);
        this.transform.Find("Chara").gameObject.SetActive(isActive);
        this.transform.Find("EquipText").gameObject.SetActive(isActive);
        this.transform.Find("Page").gameObject.SetActive(isActive);
        if (isActive)
            this.transform.Find("Page01").gameObject.SetActive(false);
        this.transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).name = "퀘스트00";
    }

    //토클 버튼 & 인벤토리 버튼들.
    public void ToggleButton(GameObject pageButton)
    {
        if (pageButton.transform.parent == GameObject.Find("CanvasInventory").transform.Find("Page"))  //인벤토리 옆칸(인벤토리, 퀘스트) 버튼.
        {
            if (pageButton.transform.parent.name == "Page")
            {
                switch (pageButton.name)
                {
                    case "Page00":
                        _bookPage = 0;
                        this.transform.Find("Inventory").gameObject.SetActive(true);
                        this.transform.Find("Equip").gameObject.SetActive(true);
                        this.transform.Find("Chara").gameObject.SetActive(true);
                        this.transform.Find("EquipText").gameObject.SetActive(true);
                        this.transform.Find("ShortSlot").gameObject.SetActive(true);

                        this.transform.Find("Page01").gameObject.SetActive(false);
                        break;
                    case "Page01":
                        _bookPage = 1;
                        this.transform.Find("Inventory").gameObject.SetActive(false);
                        this.transform.Find("Equip").gameObject.SetActive(false);
                        this.transform.Find("Chara").gameObject.SetActive(false);
                        this.transform.Find("EquipText").gameObject.SetActive(false);
                        this.transform.Find("ShortSlot").gameObject.SetActive(false);

                        this.transform.Find("Page01").gameObject.SetActive(true);
                        this.transform.Find("Page01").transform.Find("QuestList").GetChild(0).gameObject.SetActive(true);
                        this.transform.Find("Page01").transform.Find("QuestList").GetChild(1).gameObject.SetActive(false);
                        this.transform.Find("Page01").GetChild(3).gameObject.name = "Change00";
                        this.transform.Find("Page01").Find("Change00").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "완료";

                        //메인퀘 표시.
                        FirstQuest(pageButton);
                        break;
                }
            }
        }
        else if (_bookPage == 0)
        {
            //토글 버튼.
            if (pageButton.transform.parent == GameObject.Find("CanvasInventory").transform.Find("Inventory").Find("MenuToggle"))
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    equipMenus[i].SetActive(toggles[i].isOn);
                }
            }
        }
        else if (_bookPage == 1)
        {
            if (pageButton.name == "Change00")  //완료 버튼.
            {
                this.transform.Find("Page01").transform.Find("QuestList").GetChild(0).gameObject.SetActive(false);
                this.transform.Find("Page01").transform.Find("QuestList").GetChild(1).gameObject.SetActive(true);
                pageButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "진행";
                pageButton.name = "Change01";
            }
            else if (pageButton.name == "Change01")  //진행중 버튼.
            {
                this.transform.Find("Page01").transform.Find("QuestList").GetChild(0).gameObject.SetActive(true);
                this.transform.Find("Page01").transform.Find("QuestList").GetChild(1).gameObject.SetActive(false);
                pageButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "완료";
                pageButton.name = "Change00";
            }
            FirstQuest(pageButton);
        }
    }

    //첫번째 퀘스트 내용 표시.
    void FirstQuest(GameObject _pageButton)
    {
        if (_pageButton.transform.parent.name == "Page" || _pageButton.name == "Change00" || !_pageButton == this.transform.Find("Page01").transform.Find("QuestList").GetChild(0).GetChild(0).GetChild(0).gameObject)
        {   //메인 퀘스트.
            for (int i = 0; i < _EventOn.Count; i++)
            {
                if (_EventOn[i] == 1)
                {
                    switch (i)
                    {
                        case 0:
                            //퀘스트 명.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트00";
                            //퀘스트 내용.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "내는 모른다\n걍 직진하시든가";
                            break;
                        case 1:
                            //퀘스트 명.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트01";
                            //퀘스트 내용.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "전사의 말이 맞다.\n쳐맞는 말.";
                            break;
                        case 3:
                            //퀘스트 명.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트02";
                            //퀘스트 내용.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "텐트 숲을 지나서가자\n엉금엉금 지나서가자";
                            break;
                        case 5:
                            //퀘스트 명.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트03";
                            //퀘스트 내용.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "XXX! 너로 정했다!";
                            break;
                        case 7:
                            //퀘스트 명.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "퀘스트04";
                            //퀘스트 내용.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "여기가...어디오?\n아, 안심하세요. 천국입니다.";
                            break;
                        default:
                            //퀘스트 명.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            //퀘스트 내용.
                            this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            break;
                    }
                }
            }
        }
        else
        {
            //서브 퀘스트.
            int j;
            j = this.transform.Find("Page01").Find("Change00") != null ? 0 : 1;
            if (_pageButton.name != "Change01")
            {
                for (int i = 0; i <= this.transform.Find("Page01").transform.Find("QuestList").GetChild(j).GetChild(0).childCount - 1; i++)
                {
                    if (_pageButton.name == this.transform.Find("Page01").transform.Find("QuestList").GetChild(j).GetChild(0).GetChild(i).gameObject.name)
                    {
                        this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = _pageButton.name;
                        SelectQuest(_pageButton);
                    }
                }
            }
            else
            {
                if (this.transform.Find("Page01").transform.Find("QuestList").GetChild(1).GetChild(0).childCount != 0)
                {
                    //퀘스트 명.
                    this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.transform.Find("Page01").transform.Find("QuestList").GetChild(1).GetChild(0).GetChild(0).name;
                    SelectQuest(this.transform.Find("Page01").transform.Find("QuestList").GetChild(1).GetChild(0).GetChild(0).gameObject);
                }
                else
                {
                    //퀘스트 명.
                    this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                    //퀘스트 내용.
                    this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                }
            }
        }
    }

    //인벤토리 왼쪽 버튼 클릭 시 첫번째 퀘스트 내용 표시.
    void SelectQuest(GameObject _pageButton00)
    {
        switch (_pageButton00.name)
        {
            //-----------------------메인 퀘스트.
            case "퀘스트00":
                //퀘스트 내용.
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "내는 모른다\n걍 직진하시든가";
                break;
            case "퀘스트01":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "전사의 말이 맞다.\n쳐맞는 말.";
                break;
            case "퀘스트02":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "텐트 숲을 지나서가자\n엉금엉금 지나서가자";
                break;
            case "퀘스트03":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "XXX! 너로 정했다!";
                break;
            case "퀘스트04":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "여기가...어디오?\n아, 안심하세요. 천국입니다.";
                break;
            //-----------------------서브 퀘스트.
            case "말 대장장이":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "후스 데얼 포켓몬!!";
                break;
            case "나 건다 말 전사":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "퓌까츄!!!";
                break;
            case "우리가 그렇게 궁금하시다면":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "대답해 드리는게 인지상정";
                break;
            case "이 세계의 파괴를 막기 위해":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "이 세계의 평화를 지키기 위해";
                break;
            case "사랑과 진실 어둠을 뿌리고 다니는":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "포켓몬의 감초 귀염둥이 악당!";
                break;
            case "로사! 로이!":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "난 나옹이다옹~";
                break;
            case "시공의 폭풍은 정말 최고야":
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "새로운 영웅은 언제나 환영이야!\n너도";
                break;
            default:
                //퀘스트 명.
                this.transform.Find("Page01").transform.Find("Quest").GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                //퀘스트 내용.
                this.transform.Find("Page01").transform.Find("Quest").GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                break;
        }
        return;
    }
}
