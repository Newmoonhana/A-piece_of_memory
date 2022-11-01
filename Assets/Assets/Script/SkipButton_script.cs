using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton_script : MonoBehaviour
{
    GameObject EventObj;
    Event_script EventScp;
    public bool _skip = false;
    WaitForSeconds count2 = new WaitForSeconds(2f);
    public static bool corutinOn;

    private void Awake()
    {
        EventObj = GameObject.Find("PlayerActor");
        EventScp = EventObj.GetComponent<Event_script>();
    }

    void Update()
    {
        //2초 후 다시 비활성화.
        if (this.gameObject.activeSelf)
            if (!corutinOn)
            {
                StartCoroutine("Countdown2");
            }
        //등장할 상황이 아니면 비활성화.
        if (!Event_script._eventIn)
            gameObject.SetActive(false);
    }

    //2초 후 다시 비활성화 함수.
    IEnumerator Countdown2()
    {
        corutinOn = true;
        yield return count2;
        corutinOn = false;
        gameObject.SetActive(false);
    }

    //스킵 버튼 누를 시.
    public void SkipButton()
    {
        if (_skip == false)
        {
            GameObject.Find("Canvas/Dialogue").GetComponent<Animator>().SetBool("IsOpen", false);
            GameObject.Find("PlayerActor").transform.GetChild(0).GetComponent<Animator>().SetBool("IsMoving", false);
            EventScp.StopAllCoroutines();
            _skip = true;
            switch (gameObject.scene.name)
            {
                case "Event00":
                    EventScp.e = 0;
                    break;
                case "Event01":
                    EventScp.e = 1;
                    EventScp._inEvent = 1;
                    break;
                case "Event02":
                    EventScp.e = 2;
                    EventScp._inEvent = 2;
                    break;
                case "Event03":
                    EventScp.e = 3;
                    EventScp._inEvent = 7;
                    break;
                case "Event04":
                    EventScp.e = 4;
                    EventScp._inEvent = 7;
                    break;
                case "Event05":
                    EventScp.e = 5;
                    EventScp._inEvent = 2;
                    break;
                case "Event06":
                    EventScp.e = 6;
                    EventScp._inEvent = 3;
                    break;
                case "Event07":
                    EventScp.e = 7;
                    EventScp._animator2 = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
                    break;
            }
            EventScp.StartCoroutine("EventClass");
            GameObject.Find("Canvas").transform.Find("SkipButton").gameObject.SetActive(false); //클릭 후 비활성화.
        }
    }
}
