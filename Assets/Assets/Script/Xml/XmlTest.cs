using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmlTest : MonoBehaviour {

    public GameObject _player;

    void Awake()
    {
        _player = GameObject.Find("Player");
    }

    void Update()
    {
        _player.GetComponent<Dialogue>().sentences = new string[30]
        { x.i[1].t, x.i[2].t, x.i[3].t, x.i[4].t, x.i[5].t, x.i[6].t, x.i[7].t, x.i[8].t, x.i[9].t, x.i[10].t
         ,x.i[11].t, x.i[12].t, x.i[13].t, x.i[14].t, x.i[15].t, x.i[16].t, x.i[17].t, x.i[18].t, x.i[19].t, x.i[20].t
         ,x.i[21].t, x.i[22].t, x.i[23].t, x.i[24].t, x.i[25].t, x.i[26].t, x.i[27].t, x.i[28].t, x.i[29].t, x.i[30].t};



        _player.GetComponent<Dialogue>().names = new string[30] { x.i[1].c, x.i[2].c, x.i[3].c, x.i[4].c, x.i[5].c, x.i[6].c, x.i[7].c, x.i[8].c, x.i[9].c, x.i[10].c
         ,x.i[11].c, x.i[12].c, x.i[13].c, x.i[14].c, x.i[15].c, x.i[16].c, x.i[17].c, x.i[18].c, x.i[19].c, x.i[20].c
         ,x.i[21].c, x.i[22].c, x.i[23].c, x.i[24].c, x.i[25].c, x.i[26].c, x.i[27].c, x.i[28].c, x.i[29].c, x.i[30].c
};
        gameObject.GetComponent<XmlTest>().enabled = false;
    }

}
