using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Player_script : MonoBehaviour
{
    public static bool _AdminMode = true;   //개발자 모드 코드.

    public static int _player_hp = 10;
    public static int _player_maxhp = 20;
    public static int _player_fever = 0;
    public static int _player_maxfever = 6;
    public static int _player_power = 10;   //플레이어 공격력.

    public static float _player_x;
    public static bool _dontmove = true; //true 시 못움직임.

    public static float _movePower = 170f;
    public static float _jumpPower = 400f;
    //애니메이션 점프 파라미터 설정.
    public bool _isJumping = false;

    public static int _attacking = 0;   //0: 공격 가능, 1: 공격 중, 2: 공격 딜레이.
    private readonly WaitForSeconds _attackTime = new WaitForSeconds(0.5f); //공격 스프라이트 길이.
    private readonly WaitForSeconds _attackTime2 = new WaitForSeconds(0.6f); //공격 스프라이트 길이.
    private readonly WaitForSeconds _attackTime3 = new WaitForSeconds(1.2f); //공격 스프라이트 길이.

    public static int _player_noDamage = -1; //-1: false, 0: 경직&무적시간, 1: 무적시간.
    private readonly WaitForSeconds _cantMove = new WaitForSeconds(0.6f);    //0: 경직&무적시간.
    private readonly WaitForSeconds _noDamageTime = new WaitForSeconds(1f);  //1: 무적시간.
    private readonly WaitForSeconds _gameoverTime = new WaitForSeconds(5f); //게임오버 스프라이트 길이+대기 시간.

    Rigidbody2D _rigid2d;
    public static Animator _animator;
    Vector3 _movement;
    private static Player_script _player_Instance = null;

    public static Image _FadeImage;
    public static string _sceneName;
    public static string _sceneBefore;
    public static bool _isFade = true;
    private readonly WaitForSeconds _FadeTime = new WaitForSeconds(1f); //페이드 인/아웃 사이 간격.
    public static bool _isGameOver = false;
    public static int _MonsterCheck;

    public int _player_GuardPercent = 25;    //방어률 = player_GuardPercent/100.

    //---------------- 방어 -------------
    private readonly WaitForSeconds _waitTime = new WaitForSeconds(0.03f); //대기 시간.
    public static SpriteRenderer Guard_Sr;
    //---------------- 끝 -----------------

    //-----------카메라--------
    private readonly WaitForSeconds ScreenshakeTime = new WaitForSeconds(0.2f);
    public AudioSource _BGM;
    AudioSource _playerSE;
    public AudioClip[] _SEcilp;   //효과음 목록(CanvasInventory에서 가져옴).
    //----------------------------------------  

    //--------------------- 대시-------------
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;
    private float dashCooltime;
    private bool dashCooltimeOn = false;
    public int dashCount = 2;
    //------------------------------------------

    //----------콤보 공격---------
    private int AttackCount = 0;
    private float AttackTime;
    private float AttackDelay;
    private bool AttackTimeOn = false;
    //---------------------------


    public sbyte jumpcount = 2;

    //스프라이트.
    public Sprite _playerSword;
    public Sprite _playerSword2;

    void Awake()
    {
        if (_player_Instance != null)   //싱글톤.
        {
            Destroy(gameObject);
            return;
        }
        _player_Instance = this;
        DontDestroyOnLoad(this);    //씬 이동 후 오브젝트 보존.

        _rigid2d = GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        _animator.SetInteger("Direction", -1);   //-1 왼쪽, 1 오른쪽.
        _FadeImage = GameObject.Find("Fade").GetComponent<Image>();
        _rigid2d.gravityScale = 25;

        for (int i = 0; i <= 7; i++)    //i <= 이벤트 갯수(0~End).
        {
            InventoryScript._EventOn.Add(0);
        }
        for (int i = 0; i <= 7; i++)    //i <= 이벤트 갯수(0~End).
        {
            InventoryScript._SubQue.Add(-1);
        }

        StartCoroutine("FadeOut");
        dashTime = startDashTime; // 대시

        _playerSE = this.GetComponent<AudioSource>();
        _SEcilp = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._SEcilp;

        Guard_Sr = GameObject.Find("Player/MainPlayer/Guard").GetComponent<SpriteRenderer>(); //가드
        _playerSword = transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
        _playerSword2 = transform.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {

        if (AttackTimeOn)
            AttackTime += Time.deltaTime;

        if (dashCooltimeOn)
            dashCooltime += Time.deltaTime;

        if (direction == 0) //대시
        {
            if (dashCooltime >= 1f)
            {
                if(dashCount == 2)
                dashCooltimeOn = false;

                dashCooltime = 0f;
                if (dashCount <= 1)
                    dashCount++;
            }

            if (Input.GetMouseButtonDown(1) && dashCount > 0)
            {
                dashCount--;
                dashCooltimeOn = true;
                if (_animator.GetInteger("Direction")==-1)
                {
                    if (_player_noDamage == -1)
                        this.gameObject.layer = 11;
                    direction = 1;
                }
                else
                {
                    if (_player_noDamage == -1)
                        this.gameObject.layer = 11;
                    direction = 2;
                }
            }
        }
        else
        {
            if (dashTime <= 0)
            {
                direction = 0;
                dashTime = startDashTime;
                _rigid2d.velocity = Vector2.zero;
                if (_player_noDamage == -1)
                    this.gameObject.layer = 9;
                _animator.SetBool("Dash", false);
            }
            else
            {
                dashTime -= Time.deltaTime;
                if (direction == 1)
                {
                    _rigid2d.velocity = new Vector2(-800, 0);
                    StartCoroutine("Screenshake", ScreenshakeTime);
                    _animator.SetBool("Dash", true);
                    dashCooltime = 0f;
                }
                else if (direction == 2)
                {
                    _rigid2d.velocity = new Vector2(800, 0);
                    StartCoroutine("Screenshake", ScreenshakeTime);
                    _animator.SetBool("Dash", true);
                    dashCooltime = 0f;
                }

            }
        }

        if (!InventoryScript.isActive)
        {
            _player_x = this.transform.position.x;

            //HP, 피버게이지 최대치 안넘게.
            if (_player_maxhp < _player_hp)
                _player_hp = _player_maxhp;
            if (_player_maxfever < _player_fever)
                _player_fever = _player_maxfever;

            //이동 애니메이션.
            if (_player_noDamage != 0)
                if (!_dontmove && !DialogueManager.pauseOn)
                {
                    if (_animator.GetBool("Attacking") == false)
                    {
                        if (Input.GetAxisRaw("Horizontal") == 0)
                            _animator.SetBool("IsMoving", false);
                        else if (Input.GetAxisRaw("Horizontal") < 0)
                        {
                            _animator.SetInteger("Direction", -1);
                            _animator.SetBool("IsMoving", true);
                        }
                        else if (Input.GetAxisRaw("Horizontal") > 0)
                        {
                            _animator.SetInteger("Direction", 1);
                            _animator.SetBool("IsMoving", true);
                        }
                    }

                    if (jumpcount > 0)
                    {
                        if (Input.GetButtonDown("Jump"))
                        {
                            //transform.Translate(10f, 0, 1f * Time.deltaTime);
                            _rigid2d.velocity = new Vector3(0, 250, 0);
                            jumpcount--;
                        }
                    }

                    /* 처참히 봉인되버린 기존 점프의 흔적.
                    if (Input.GetButtonDown("Jump") && !_animator.GetBool("IsJumping"))
                    {
                        _isJumping = true;
                        _animator.SetBool("IsJumping", true);
                        _animator.SetTrigger("doJumping");
                    }
                    */
                }

            //공격.
            if (_attacking == 0)
                if(!_dontmove && !DialogueManager.pauseOn && !_animator.GetBool("IsJumping"))
                if (_player_noDamage != 0)
                {
                    if (AttackTime >= 4f)
                    {
                        AttackTime = 0f;
                        AttackCount = 0;
                        Debug.Log("초기화");
                        AttackTimeOn = false;
                    }
                    if (Input.GetMouseButton(0))
                    {
                        AttackTimeOn = true;
                        if (AttackCount == 0)
                        {
                            AttackTimeOn = false;
                            Debug.Log("첫번째 공격");
                            _attacking = 1;
                            _animator.SetBool("Attacking", true);
                            StartCoroutine("Attacking");
                            AttackCount++;

                        }
                        else if (AttackCount == 1)
                        {
                            AttackTimeOn = false;
                            Debug.Log("두번째 공격");
                            _attacking = 1;
                            _animator.SetBool("Attacking2", true);
                            StartCoroutine("Attacking2");
                            AttackTime = 0f;
                            AttackCount++;
                        }
                        else if (AttackCount == 2)
                        {
                            AttackTimeOn = false;
                            Debug.Log("세번째 공격");
                            _attacking = 1;
                            _animator.SetBool("Attacking3", true);
                            StartCoroutine("Attacking3");
                            AttackTime = 0f;
                            AttackCount = 0;
                        }
                    }

                }

            //몬스터 처치 후 이벤트 실행.
            if (_isFade == false)
            {
                switch (_FadeImage.gameObject.scene.name)
                {
                    case "Dungen01Scene":
                        if (InventoryScript._EventOn[6] == 0)
                        {
                            if (_MonsterCheck == 0)
                            {
                                _animator.SetBool("Attacking", false);
                                _sceneName = "Event06";
                                Event_script._FadeInOutOn = true;
                                StartCoroutine("FadeIn");
                            }
                        }
                        break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!_dontmove && !_isGameOver)
        {
            if (!_animator.GetBool("Damage"))   //넉백 상태 X.
            {
                Move();
                Jump();
            }
        }
        else   //움직임 봉인.
        {
            _animator.SetBool("IsMoving", false);
        }
    }

    //-------------------------------------이동영역----------------------------------------.
    // 좌, 우.
    void Move()
    {
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            _animator.SetInteger("Direction", -1);
            moveVelocity = Vector3.left;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            _animator.SetInteger("Direction", 1);
            moveVelocity = Vector3.right;
        }

        transform.position += moveVelocity * _movePower * Time.deltaTime;
    }

    // 점프.
    void Jump()
    {
    }

    //-------------------------------------이동영역----------------------------------------.

    void OnTriggerEnter2D(Collider2D other)
    {
        //땅과 충돌체크, 무한 점프 방지.
        if (other.gameObject.layer == 12)
        {
            _animator.SetBool("IsJumping", false);
            //넉백 시 뒤로 밀림 방지.
            if (_player_noDamage != -1)
            {
                _rigid2d.bodyType = RigidbodyType2D.Static;
                _rigid2d.bodyType = RigidbodyType2D.Dynamic;
            }
            jumpcount = 2;
        }
        //이벤트 실행(자동).
        if (!_isFade)
        {
            switch (other.name)
            {
                case "Event01":
                    if (InventoryScript._EventOn[1] == 0)
                    {
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                    }
                    break;
                case "Event04":
                    if (InventoryScript._EventOn[3] == 1)
                        if (InventoryScript._EventOn[4] == 0)
                        {
                            _sceneName = other.gameObject.name;
                            StartCoroutine("FadeIn");
                        }
                    break;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //몬스터와 충돌.
        if (other.gameObject.layer == 10)
        {
            if (other.GetComponent<Rigidbody2D>() != null)
            {
                other.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                other.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }

            if (_player_noDamage == -1 && !_isFade && !_isGameOver)
            {

                gameObject.layer = 11;
                int randomVaule = Random.Range(1, 100);
                if (_player_GuardPercent <= randomVaule)
                {
                    if (other.transform.parent != null && other.transform.parent.name != "Monster")
                    {
                        if (other.transform.parent.parent != null)   //몬스터의 공격에 
                        {
                            if (other.transform.parent.parent.name == "Monster")
                                _player_hp -= other.transform.parent.GetComponent<Monster_script>()._monster_damage;
                        }
                        else if (other.transform.parent.gameObject.layer == 10)   //보스 공격에 피격.
                            switch (other.transform.parent.gameObject.name)
                            {
                                case "Boss01":
                                    _player_hp -= other.transform.parent.GetComponent<Boss01_script>()._monster_damage;
                                    break;
                            }
                        else   //몸통에 피격.
                            _player_hp -= 1;
                    }
                    if (other.transform.parent == null) //보스 몸통에 피격.
                        _player_hp -= (int)other.transform.GetComponent<Boss01_script>()._monster_damage / 2;

                    if (_player_hp > 0)    //대미지.
                    {
                        //경직 모션.
                        _animator.SetBool("IsMoving", false);
                        _animator.SetBool("IsJumping", false);
                        StartCoroutine("Screenshake", ScreenshakeTime);
                        _animator.SetBool("Damage", true);
                        _player_noDamage = 0;

                        //넉백.
                        _rigid2d.bodyType = RigidbodyType2D.Static;
                        _rigid2d.bodyType = RigidbodyType2D.Dynamic;
                        if (other.gameObject.name != "Monster02Attack")
                        {
                            if (_animator.GetBool("IsJumping"))   //점프하면서 피격됬을 시.
                            {

                            }
                            else   //땅에서 피격됬을 시.
                            {
                                if (_animator.GetInteger("Direction") == -1)
                                    _rigid2d.AddForce(new Vector3(100f, _jumpPower * 0.3f, 0f), ForceMode2D.Impulse);
                                else
                                    _rigid2d.AddForce(new Vector3(-100f, _jumpPower * 0.3f, 0f), ForceMode2D.Impulse);
                            }
                        }
                        else
                        {
                            if (other.transform.position.x <= _player_x)
                            {

                            }
                            else
                            {

                            }
                        }
                        StartCoroutine("NoDamageTime");
                    }
                    else   //게임 오버.
                    {
                        _animator.SetBool("GameOver", true);
                        _dontmove = true;
                        StartCoroutine("GameOver");
                    }
                }
                else   //가드 성공.
                {
                    StopCoroutine("GuardFadeIn");
                    StartCoroutine("GuardFadeIn");
                    Debug.Log("가드");
                    _player_noDamage = 1;    //경직 X.
                    StartCoroutine("NoDamageTime");
                }
            }
        }
        //포탈.
        if (Input.GetKeyDown("f"))
        {
            if (!_isFade)
            {
                switch (other.gameObject.name)
                {
                    case "MainScene":
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                        break;

                    case "LakeScene01":
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                        break;

                    case "Dungen01Scene":
                        //개발자 코드(포탈로 바로 던전 씬).
                        if (_AdminMode)
                        {
                            _sceneName = other.gameObject.name;
                            GameObject.Find("CanvasInventory").GetComponent<AudioSource>().clip = GameObject.Find("CanvasInventory").GetComponent<InventoryScript>()._cilp[1];
                            GameObject.Find("CanvasInventory").GetComponent<AudioSource>().Play();
                            StartCoroutine("FadeIn");
                        }
                        break;

                    case "CountDung01Scene":
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                        break;

                    case "CountDung02Scene":
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                        break;

                    case "CountDung03Scene":
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                        break;
                }
                //이벤트 실행(결정키).
                if (other.name == "Event02")
                {
                    if (InventoryScript._EventOn[2] == 0)
                    {
                        _sceneName = other.gameObject.name;
                        StartCoroutine("FadeIn");
                    }
                }
                //NPC에게 말 걸기.

                switch (other.gameObject.name)
                {
                    case "SmithB":
                    case "Warrior01B":
                    case "Injured01B":
                    case "Injured02B":
                    case "Injured03B":
                    case "Doctor01B":
                    case "Soldier01B":
                        GameObject.Find("ScriptObj").GetComponent<Event_script>().TriggerStay(other);
                        break;
                }
            }
        }

        //무기 스위칭 코드.
        if (Input.GetKeyDown("tab"))
        {
            if (!_isFade && !_dontmove && !_isGameOver && _attacking != 1)
                WeaponChange();
        }

    }

    public IEnumerator Screenshake(WaitForSeconds _shakeTime) //화면 흔들림.
    {
        CinemachineBasicMultiChannelPerlin _cine = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cine.m_AmplitudeGain = 2f;
        _cine.m_FrequencyGain = 2f;
        yield return _shakeTime;
        _cine.m_AmplitudeGain = 0;
        _cine.m_FrequencyGain = 0;
        yield return null;
    }

    IEnumerator Screenshake2() //화면 흔들림2.
    {
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2.5f;
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 2.5f;
        yield return new WaitForSeconds(0.9f);
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        yield return null;
    }

    //무기 스위칭 함수.
    void WeaponChange()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(!transform.GetChild(0).GetChild(0).gameObject.activeSelf);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(!transform.GetChild(0).GetChild(1).gameObject.activeSelf);
    }

    IEnumerator GuardFadeIn() //가드
    {
        for (int i = 0; i < 255f; i += 10)
        {
            Guard_Sr.color = new Color32(255, 255, 255, (byte) i);
            yield return _waitTime;
            yield return null;
        }
        for (int i = 255; i > -0f; i -= 10)
        {
            Guard_Sr.color = new Color32(255, 255, 255, (byte) i);
            yield return _waitTime;
            yield return null;
        }
        yield return null;
    }

    IEnumerator Attacking() //공격.
    {
        _dontmove = true;
        _playerSE.clip = _SEcilp[0];    //검 공격 소리.
        _playerSE.Play();
        yield return _attackTime;

        _animator.SetBool("Attacking", false);
        _attacking = 2;
        _dontmove = false;
        yield return null;
        _attacking = 0;
        AttackTimeOn = true;
    }

    IEnumerator Attacking2() //공격2.
    {
        _dontmove = true;
        _playerSE.clip = _SEcilp[0];    //검 공격 소리.
        _playerSE.Play();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine("Screenshake", ScreenshakeTime);

        if (_animator.GetInteger("Direction")==1)
            _rigid2d.velocity = new Vector2(400, 0);
        //transform.Translate(new Vector2(-100, 0));
        else if(_animator.GetInteger("Direction")==-1)
            _rigid2d.velocity = new Vector2(-400, 0);
        Debug.Log("공격 이동");
        yield return new WaitForSeconds(0.1f);
        _rigid2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("Attacking2", false);
        _attacking = 2;
        _dontmove = false;
        yield return null;
        _attacking = 0;
        AttackTimeOn = true;
       
    }

    IEnumerator Attacking3() //공격3.
    {
        _dontmove = true;
        _playerSE.clip = _SEcilp[0];    //검 공격 소리.
        _playerSE.Play();
        yield return new WaitForSeconds(0.7f);
        StartCoroutine("Screenshake2");
        yield return new WaitForSeconds(1.5f);
        _animator.SetBool("Attacking3", false);
        _attacking = 2;
        _dontmove = false;
        yield return null;
        _attacking = 0;
        AttackTimeOn = true;
    }

    IEnumerator NoDamageTime()  //무적시간.
    {
        yield return _cantMove;
        _animator.SetBool("Damage", false);
        _player_noDamage = 1;
        yield return _noDamageTime;
        gameObject.layer = 9;
        _player_noDamage = -1;
    }

    IEnumerator FadeIn() //장소 이동.
    {
        //페이드 인.
        _dontmove = true;
        _isFade = true;
        Event_script._eventIn = false;
        //InvSet();
        GameObject.Find("CanvasInventory").transform.Find("BackBook01").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("ShortSlot").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("Inventory").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("Equip").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("Chara").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("EquipText").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("Page").gameObject.SetActive(false);
        GameObject.Find("CanvasInventory").transform.Find("Page01").gameObject.SetActive(false);
        if (GameObject.Find("Canvas").transform.Find("PauseChang") != null)
            GameObject.Find("Canvas").transform.Find("PauseChang").gameObject.SetActive(false);
        //페이드 인 효과.
        _FadeImage = GameObject.Find("Fade").GetComponent<Image>();
        for (int i = 0; i < 255; i += 10)
        {
            _FadeImage.color = new Color32(0, 0, 0, (byte) i);
            if (_BGM != null && Event_script._FadeInOutOn)
                _BGM.volume -= 0.05f;
            yield return null;
        }
        //음악 페이드 인.
        if (_BGM != null && Event_script._FadeInOutOn)
            _BGM.volume = 0f;
        gameObject.layer = 9;
        _player_noDamage = -1;

        StopCoroutine("Attacking");
        _animator.SetBool("Attacking", false);
        _attacking = 0;
        _sceneBefore = _FadeImage.gameObject.scene.name;

        switch (_sceneName)
        {
            case "MainScene":
                if (_sceneBefore == "Dungen01Scene")
                {
                    _animator.SetInteger("Direction", -1);
                    transform.position = new Vector3(600f, -113, 10);
                }
                else if (_sceneBefore == "LakeScene01")
                {
                    _animator.SetInteger("Direction", 1);
                    transform.position = new Vector3(-600f, -113, 10);
                }
                break;

            case "LakeScene01":
                _animator.SetInteger("Direction", -1);
                transform.position = new Vector3(280f, -113, 10);
                break;

            case "Dungen01Scene":
                _animator.SetInteger("Direction", 1);
                transform.position = new Vector3(-300f, -113, 10);
                break;

            case "CountDung01Scene":
                    _animator.SetInteger("Direction", -1);
                    transform.position = new Vector3(1340f, -113, 10);
                break;

            case "CountDung02Scene":
                if (_sceneBefore == "CountDung01Scene")
                {
                    _animator.SetInteger("Direction", 1);
                    transform.position = new Vector3(-265f, -113, 10);
                }
                else if (_sceneBefore == "CountDung03Scene")
                {
                    _animator.SetInteger("Direction", -1);
                    transform.position = new Vector3(1363f, -113, 10);
                }
                break;

            case "CountDung03Scene":
                _animator.SetInteger("Direction", -1);
                transform.position = new Vector3(-277f, -113, 10);
                break;
        }

        //씬 이동 후 설정.
        SceneManager.LoadScene(_sceneName);
        yield return new AsyncOperation();
        _FadeImage = GameObject.Find("Canvas/Fade").GetComponent<Image>();
        Dialogue._animator = GameObject.Find("Canvas/Dialogue").GetComponent<Animator>();
        _isGameOver = false;
        StopCoroutine("GuardFadeIn");
        Guard_Sr.color = new Color32(255, 255, 255, 0);
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = _playerSword;
        transform.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = _playerSword2;
        _animator.SetBool("GameOver", false);

        if (GameObject.Find("Monster") != null)
            _MonsterCheck = GameObject.Find("Monster").transform.childCount;

        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()   //페이드 아웃.
    {
        if (GameObject.Find("CanvasInventory") != null)
            _BGM = GameObject.Find("CanvasInventory").GetComponent<AudioSource>();
        _FadeImage.color = new Color32(0, 0, 0, 255);
        yield return _FadeTime;
        for (int i = 255; i > 0; i -= 10)
        {
            _FadeImage.color = new Color32(0, 0, 0, (byte) i);
            if (_BGM != null && Event_script._FadeInOutOn)
                _BGM.volume += 0.05f;
            yield return null;
        }
        if (_BGM != null && Event_script._FadeInOutOn)
            _BGM.volume = 1f;
        _isFade = false;
        _dontmove = false;
        Event_script._eventIn = true;
        //InvSet();
        GameObject.Find("CanvasInventory").transform.Find("ShortSlot").gameObject.SetActive(true);
        GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-310, -230, GameObject.Find("CanvasInventory").transform.GetChild(1).GetComponent<RectTransform>().localPosition.z);
        GameObject.Find("CanvasInventory").transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;
    }

    //인벤토리 true에서 씬으로 넘어가 꺼졌을 때.
    /*현재 코드 상 인벤토리 상태에서 넘어가지 못해서 보류.
    void InvSet()
    {
        GameObject _inventory = GameObject.Find("CanvasInventory");
        if (_isFade == true)   //씬으로 넘어갈 때 인벤토리 활성화.
        {
            _inventory.transform.Find("BackBook01").gameObject.SetActive(false);
            _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(19, -175, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
            _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 10;
            _inventory.transform.Find("Inventory").gameObject.SetActive(false);
            _inventory.transform.Find("Equip").gameObject.SetActive(false);
            _inventory.transform.Find("Chara").gameObject.SetActive(false);
            _inventory.transform.Find("EquipText").gameObject.SetActive(false);
            _inventory.transform.Find("Page").gameObject.SetActive(false);
            _inventory.transform.Find("Page01").gameObject.SetActive(false);
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
        else   //씬으로 돌아올 때 인벤토리 활성화.
        {
            GameObject _UIManager = GameObject.Find("UIManager") as GameObject;
            if (_UIManager != null)
                if (InventoryScript.isActive == true)
                {
                    _inventory.transform.Find("BackBook01").gameObject.SetActive(true);
                    _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition = new Vector3(-310, -230, _inventory.transform.Find("ShortSlot").GetComponent<RectTransform>().localPosition.z);
                    _inventory.transform.Find("ShortSlot").GetComponent<HorizontalLayoutGroup>().spacing = 0;
                    _inventory.transform.Find("Inventory").gameObject.SetActive(true);
                    _inventory.transform.Find("Equip").gameObject.SetActive(true);
                    _inventory.transform.Find("Chara").gameObject.SetActive(true);
                    _inventory.transform.Find("EquipText").gameObject.SetActive(true);
                    _inventory.transform.Find("Page").gameObject.SetActive(true);
                }
        }
    }*/

    //게임 오버.
    IEnumerator GameOver()
    {
        _isGameOver = true;
        Event_script._FadeInOutOn = true;
        _playerSword = transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
        _playerSword2 = transform.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite;
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
        yield return _gameoverTime;
        _sceneName = _FadeImage.gameObject.scene.name;  //현재 사망 시 다시 원래 장소 그 자리(예외:던전01씬)에서 부활.
        _player_hp = _player_maxhp;
        StopCoroutine("GuardFadeIn");
        Guard_Sr.color = new Color32(255, 255, 255, 0);
        StartCoroutine("FadeIn");
    }
}
