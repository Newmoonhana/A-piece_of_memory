using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_script : MonoBehaviour
{
    GameObject _player; //플레이어.
    float _playersDis; //플레이어와의 거리.

    bool _attention = false; //어그로 여부.
    WaitForSeconds _waitTime;    //어그로 X 상태 행동 패턴 대기 시간.
    WaitForSeconds _deadTime;   //사망 스프라이트 길이.
    WaitForSeconds _attackDelay;    //공격 전 대기 시간.
    WaitForSeconds _attackTimeStart;   //공격 비활성화→활성화 시간.
    WaitForSeconds _attackTimeEnd;   //공격 활성화→비활성화 시간.
    WaitForSeconds _attackDelayEnd; //공격 마친 후 딜레이.
    bool _jumpOn;   //점프 여부(true = on false = off).

    public int _monster_hp; //HP.
    public int _monster_maxhp;  //최대 HP.
    public int _monster_damage; //공격력.
    public float _monster_speed;    //이동속도.
    public float _monster_attspeed;    //어그로 시 이동속도.
    public int _monsterMove;    //이동 시 for문 횟수(이동속도 관련 변수).
    public float _monster_x;    //x좌표.

    public int _monster_noDamage = -1; //-1: false, 0: 경직&무적시간, 1: 경직.

    GameObject MonsterAttack; //몬스터 자식 오브젝트(공격 콜라이더).
    Rigidbody2D _rigid2d;
    public Animator _mon_animator;

    void Start()
    {
        _player = GameObject.Find("Player");
        _mon_animator = this.GetComponent<Animator>();

        //Monster01
        switch (gameObject.name)
        {
            case "Monster01":
                _mon_animator.SetInteger("MonsterID", 1);

                _monster_hp = 100;
                _monster_maxhp = 100;
                _monster_damage = 1;
                _monster_speed = 50f;
                _monster_attspeed = 75f;
                _monsterMove = 50;

                _waitTime = new WaitForSeconds(1f);
                _deadTime = _waitTime;
                _attackDelay = new WaitForSeconds(0.1f);
                _attackTimeStart = new WaitForSeconds(0.6f);
                _attackTimeEnd = new WaitForSeconds(0.4f);
                _attackDelayEnd = _waitTime;

                _jumpOn = true;
                break;
            case "Monster02":
                _mon_animator.SetInteger("MonsterID", 2);

                _monster_hp = 50;
                _monster_maxhp = 50;
                _monster_damage = 2;
                _monster_x = this.transform.position.x;   //x축 고정. 

                _waitTime = new WaitForSeconds(2f);
                _deadTime = new WaitForSeconds(1f);
                _attackDelay = new WaitForSeconds(0.5f);
                _attackTimeStart = new WaitForSeconds(0.7f);
                _attackDelayEnd = new WaitForSeconds(1.5f);

                _jumpOn = false;
                break;

            case "Slime00":
                _mon_animator.SetInteger("MonsterID", 3);

                _monster_hp = 40;
                _monster_maxhp = 40;
                _monster_damage = 1;
                _monster_speed = 10f;
                _monster_attspeed = 50f;
                _monsterMove = 200;

                _waitTime = new WaitForSeconds(0.5f);
                _deadTime = new WaitForSeconds(1.1f);
                _attackDelay = new WaitForSeconds(0.3f);
                _attackTimeStart = new WaitForSeconds(0.4f);
                _attackTimeEnd = new WaitForSeconds(0.2f);
                _attackDelayEnd = new WaitForSeconds(1f);

                _jumpOn = true;
                break;
            case "Pig00":
                _mon_animator.SetInteger("MonsterID", 4);

                _monster_hp = 60;
                _monster_maxhp = 60;
                _monster_damage = 1;
                _monster_speed = 50f;
                _monster_attspeed = 100f;
                _monsterMove = 60;

                _waitTime = new WaitForSeconds(1f);
                _deadTime = new WaitForSeconds(1.1f);
                _attackDelay = new WaitForSeconds(0.9f);
                _attackTimeStart = new WaitForSeconds(0.5f);
                _attackTimeEnd = new WaitForSeconds(0.2f);
                _attackDelayEnd = new WaitForSeconds(2f);

                _jumpOn = true;
                break;
        }

        _rigid2d = this.GetComponent<Rigidbody2D>();
        _mon_animator.SetInteger("Direction", -1);   //-1 왼쪽, 1 오른쪽.
        MonsterAttack = transform.GetChild(0).gameObject;
        MonsterAttack.SetActive(false);

        StartCoroutine("MonsterMove");
    }

    void Update()
    {
        //(회복 시)최대 체력 넘는 현상 방지.
        if (_monster_hp > _monster_maxhp)
            _monster_hp = _monster_maxhp;
    }

    void FixedUpdate()
    {
        //어그로 범위 설정.
        _playersDis = Vector3.Distance(_player.transform.position, transform.position);
        switch (this.gameObject.name)
        {
            case "Monster01":
            case "Pig00":
                _attention = _playersDis <= 200f ? true : false;
                break;
            case "Monster02":
                _attention = _playersDis <= 100f ? true : false;
                this.transform.position = new Vector3(_monster_x, transform.position.y, transform.position.z);
                break;
            case "Slime00":
                _attention = _playersDis <= 100f ? true : false;
                break;
        }
    }

    //몬스터 이동패턴.
    IEnumerator MonsterMove()
    {
        while (true)
        {
            if (_attention) //어그로.
            {
                switch (this.gameObject.name)
                {
                    case "Monster01":
                    case "Slime00":
                    case "Pig00":
                        StartCoroutine("DefaultAttention00");
                        yield return StartCoroutine("DefaultAttention00");
                        break;
                        
                    case "Monster02":
                        //플레이어와의 거리 가까울 시(공격).
                        _mon_animator.SetBool("IsDelay", true);
                        yield return _attackDelay;

                        _mon_animator.SetBool("IsAttack", true);
                        _mon_animator.SetBool("IsDelay", false);
                        if (_monster_hp > 0)
                            MonsterAttack.SetActive(true);

                        yield return _attackTimeStart;

                        MonsterAttack.SetActive(false);
                        _mon_animator.SetBool("IsAttack", false);
                        yield return _attackDelayEnd;
                        break;
                }
                yield return null;
            }
            else   //어그로 X.
            {
                switch (this.gameObject.name)
                {
                    case "Monster01":
                    case "Slime00":
                    case "Pig00":
                        StartCoroutine("DefaultMove00");
                        yield return StartCoroutine("DefaultMove00");
                        break;

                    case "Monster02":
                        yield return null;
                        break;
                }
            }
        }
    }

    //어그로 상태 기본 인공지능.
    IEnumerator DefaultAttention00()
    {
        int i = _mon_animator.GetInteger("Direction");  //애니메이션 값 대입 용 변수.
        if (!_attention)    //어그로 도중 어그로 해제 시.
        {
            _mon_animator.SetBool("IsMoving", false);
            yield return _waitTime;  //어그로 풀릴 시의 대기시간 = _waitTime.
        }
        else
        {
            i = transform.position.x < _player.transform.position.x ? 1 : -1;
            _mon_animator.SetInteger("Direction", i);
            if (_playersDis <= 50f) //플레이어와의 거리 가까울 시(공격).
            {
                _mon_animator.SetBool("IsMoving", false);
                yield return _attackDelay;

                _mon_animator.SetBool("IsAttack", true);

                yield return _attackTimeStart;
                MonsterAttack = transform.GetChild(0).gameObject;
                if (_monster_hp > 0)
                    MonsterAttack.SetActive(true);

                yield return _attackTimeEnd;
                MonsterAttack.SetActive(false);
                yield return _attackDelayEnd;
                _mon_animator.SetBool("IsAttack", false);
            }
            else
            {
                if (_monster_noDamage == -1)
                {
                    _mon_animator.SetBool("IsMoving", true);
                    transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, _player.transform.position.x, _monster_attspeed * Time.deltaTime), transform.position.y, transform.position.z);
                }
            }
        }
    }

    //이동(어그로 X) 기본 인공지능.
    IEnumerator DefaultMove00()
    {
        int i = _mon_animator.GetInteger("Direction");  //애니메이션 값 대입 용 변수.
        _mon_animator.SetBool("IsMoving", false);
        yield return _waitTime;

        //방향 전환.
        i = i == -1 ? 1 : -1;
        _mon_animator.SetInteger("Direction", i);
        _mon_animator.SetBool("IsMoving", true);

        if (_mon_animator.GetInteger("Direction") == -1)  //왼쪽.
        {
            for (int monsterMove = 0; monsterMove <= _monsterMove; monsterMove++)
            {
                if (_monster_noDamage == -1)
                {
                    if (Time.timeScale == 0)    //일시정시 시 for문 무한루프.
                        monsterMove--;
                    else
                        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, transform.position.x - 5f, _monster_speed * Time.deltaTime), transform.position.y, transform.position.z);
                }
                if (_attention)
                    break;
                yield return null;
            }
        }
        else if (_mon_animator.GetInteger("Direction") == 1)    //오른쪽.
        {
            for (int monsterMove = 0; monsterMove <= _monsterMove; monsterMove++)
            {
                if (_monster_noDamage == -1)
                {
                    if (Time.timeScale == 0)    //일시정시 시 for문 무한루프.
                        monsterMove--;
                    else
                        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, transform.position.x + 5f, _monster_speed * Time.deltaTime), transform.position.y, transform.position.z);
                }
                if (_attention) //이동 중 어그로 끌림.
                    break;
                yield return null;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.gameObject.layer == 12)   //땅 충돌 체크.
        //    _rigid2d.gravityScale = 0;

        //몬스터가 플레이어 공격에 충돌.
        if (other.CompareTag("Attack"))
            if (Player_script._attacking == 1)
                if (_monster_noDamage != 0)
                    StartCoroutine("DamageTime");
                else
                    Debug.Log("몬스터 무적시간");

        if (other.gameObject.layer == 17)
        {
            if (_jumpOn)
                this._rigid2d.AddForce(Vector2.up * 100f, ForceMode2D.Impulse);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        //if (other.gameObject.layer == 12)   //땅 충돌 체크.
        //    _rigid2d.gravityScale = 25;
    }

    //몬스터가 플레이어 공격에 충돌 함수.
    IEnumerator DamageTime()
    {
        _monster_hp -= Player_script._player_power;
        Debug.Log("몬스터 HP " + _monster_hp);

        //경직&넉백&무적시간.
        //_rigid2d.gravityScale = 25;
        if (this.gameObject.name != "Monster02")
        {
            if (!_mon_animator.GetBool("IsAttack"))
            {
                if (Player_script._animator.GetInteger("Direction") == -1) //넉백.
                {
                    _mon_animator.SetInteger("Direction", 1);
                    _rigid2d.AddForce(new Vector3(-200f, 0f, 0f), ForceMode2D.Impulse);
                }
                else
                {
                    _mon_animator.SetInteger("Direction", -1);
                    _rigid2d.AddForce(new Vector3(200f, 0f, 0f), ForceMode2D.Impulse);
                }
            }

            if (_monster_hp > 0)
            {
                if (_mon_animator.GetBool("IsAttack") == false)
                {
                    _mon_animator.SetBool("IsMoving", false);
                    _mon_animator.SetBool("IsDamage", true);
                }
            }
            else    //몬스터 사망모션.
            {
                ItemDrop();
                _monster_noDamage = 0;
                _mon_animator.SetBool("IsMoving", false);
                _mon_animator.SetBool("IsAttack", false);
                _mon_animator.SetTrigger("IsDead");
                gameObject.layer = 16;
                Player_script._MonsterCheck -= 1;
                MonsterAttack.SetActive(false);
                yield return _deadTime;
                gameObject.SetActive(false);
                yield break;
            }
        }
        else if (this.gameObject.name == "Monster02")
        {
            if (_monster_hp > 0)
            {
                if (_mon_animator.GetBool("IsAttack") == false)
                    if (_mon_animator.GetBool("IsDelay") == false)
                        _mon_animator.SetBool("IsDamage", true);
            }
            else    //몬스터 사망모션.
            {
                ItemDrop();
                _monster_noDamage = 0;
                _mon_animator.SetBool("IsAttack", false);
                _mon_animator.SetBool("IsDelay", false);
                _mon_animator.SetTrigger("IsDead");
                gameObject.layer = 16;
                Player_script._MonsterCheck -= 1;
                MonsterAttack.SetActive(false);
                yield return _deadTime;
                gameObject.SetActive(false);
                yield break;
            }
        }

        while (Player_script._attacking == 1)    //도트 데미지 X.
        {
            _monster_noDamage = 0;  //(연속 공격 시)코루틴 겹침 방지.
            yield return null;
        }

        //경직.
        _mon_animator.SetBool("IsDamage", false);
        _monster_noDamage = 1;
        _rigid2d.bodyType = RigidbodyType2D.Static;
        _rigid2d.bodyType = RigidbodyType2D.Dynamic;
        yield return _waitTime;

        if (_monster_noDamage == 1) //(연속 공격 시)코루틴 겹침 방지.
        {
            _monster_noDamage = -1;
        }
    }

    //몬스터 사망 후 아이템 드롭 함수.
    void ItemDrop()
    {
        bool _minus1 = false;   //-1값 여부(아무것도 드롭되지 않음), true일 시 안나올 수 있음.
        int []Count = new int [11]; //ID값 표기(-1은 빈 칸(-1에 걸릴 시 코드가 다시 실행되 -1이 아닌 값을 새로 찾음)).
        int _num = 1;   //드롭 개수.
        switch (this.gameObject.name)
        {
            case "Monster01":
                _minus1 = false;
                Count[0] = 000;
                Count[1] = 001;
                Count[2] = -1;
                Count[3] = -1;
                Count[4] = -1;
                Count[5] = -1;
                Count[6] = -1;
                Count[7] = -1;
                Count[8] = -1;
                Count[9] = -1;
                Count[10] = -1;
                _num = Random.Range(1, 3);  //1~2개.
                break;
            case "Monster02":
                _minus1 = true;
                Count[0] = 100;
                Count[1] = 101;
                Count[2] = 200;
                Count[3] = 201;
                Count[4] = 300;
                Count[5] = 400;
                Count[6] = 500;
                Count[7] = 600;
                Count[8] = 700;
                Count[9] = -1;
                Count[10] = -1;
                break;
            case "Slime00":
                _minus1 = false;
                Count[0] = 100;
                Count[1] = 101;
                Count[2] = -1;
                Count[3] = -1;
                Count[4] = -1;
                Count[5] = -1;
                Count[6] = -1;
                Count[7] = -1;
                Count[8] = -1;
                Count[9] = -1;
                Count[10] = -1;
                _num = 2;
                break;
            case "Pig00":
                _minus1 = false;
                Count[0] = 300;
                Count[1] = 400;
                Count[2] = 500;
                Count[3] = 600;
                Count[4] = 300;
                Count[5] = 400;
                Count[6] = 500;
                Count[7] = 600;
                Count[8] = 700;
                Count[9] = -1;
                Count[10] = -1;
                break;
        }
        GameObject.FindObjectOfType<ItemDrop>().ItemRandom(gameObject.transform.position, _minus1, Count, _num);
    }
}