using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01_script : MonoBehaviour
{
    GameObject _player; //플레이어.
    float _playersDis; //플레이어와의 거리.

    WaitForSeconds _waitTime = new WaitForSeconds(1f);    //행동 패턴 대기 시간.
    WaitForSeconds _attackTimeStart = new WaitForSeconds(0.6f);   //공격 비활성화→활성화 시간.
    WaitForSeconds _attackTimeEnd = new WaitForSeconds(0.2f);   //공격 활성화→비활성화 시간.
    WaitForSeconds _SpAttack01Start = new WaitForSeconds(0.7f);   //스킬공격01 비활성화→활성화 시간.
    WaitForSeconds _SpAttack01End = new WaitForSeconds(0.2f);   // 스킬공격01 활성화→비활성화 시간.

    public static int _monster_hp = 25; //HP.
    public static int _monster_maxhp = 25;  //최대 HP.
    public int _monster_damage = 2; //공격력.
    int _monster_Fdamage = 2; //원래 공격력.
    public float _monster_speed = 20;    //이동속도.
    public static int _SpMode = 0;   //공격 종류.
    public static int _modetime = 0;    //모드 시전 전까지 대기 시간.
    bool _isTile = true;    //몬스터가 땅에 있는 지(땅에 있을 때 true, 없으면 false).

    public int _monster_noDamage = -1; //-1: false, 0: 경직&무적시간, 1: 경직.

    GameObject MonsterAttack; //몬스터 자식 오브젝트(공격 콜라이더).
    Rigidbody2D _rigid2d;
    public Animator _mon_animator;
    private readonly WaitForSeconds ScreenshakeTime = new WaitForSeconds(0.5f);
    private readonly WaitForSeconds ScreenshakeTime2 = new WaitForSeconds(1f);

    void Awake()
    {
        _player = GameObject.Find("Player");
        _mon_animator = this.GetComponent<Animator>();

        _monster_hp = 250;
        _SpMode = 0;
        _modetime = 0;
        _monster_damage = _monster_Fdamage;
        _isTile = true;

        _rigid2d = this.GetComponent<Rigidbody2D>();
        _rigid2d.gravityScale = 0;
        _mon_animator.SetInteger("Direction", -1);   //-1 왼쪽, 1 오른쪽.
        MonsterAttack = transform.GetChild(0).gameObject;
        MonsterAttack.SetActive(false);

        _playersDis = 99999999; //시작하자마자 공격하는 버그 오류 수정용.
        StartCoroutine("Boss01Move");
    }

    void Update()
    {
        //(회복 시)최대 체력 넘는 현상 방지.
        if (_monster_hp > _monster_maxhp)
            _monster_hp = _monster_maxhp;
    }

    void FixedUpdate()
    {
        //플레이어 거리 측정.
        _playersDis = Mathf.Abs(_player.transform.position.x - transform.position.x);
    }

    IEnumerator Boss01Move()
    {
        while (_modetime < 300)
        {
            int i = _mon_animator.GetInteger("Direction");  //애니메이션 값 대입 용 변수.
            i = transform.position.x < _player.transform.position.x ? 1 : -1;
            _mon_animator.SetInteger("Direction", i);
            if (_playersDis <= 50f && _monster_hp > 0) //플레이어와의 거리 가까울 시(공격).
            {
                _mon_animator.SetBool("IsMoving", false);
                _mon_animator.SetBool("IsAttack", false);
                yield return _waitTime;

                i = transform.position.x < _player.transform.position.x ? 1 : -1;
                _mon_animator.SetInteger("Direction", i);
                _mon_animator.SetBool("IsMoving", false);
                _mon_animator.SetBool("IsDamage", false);
                _mon_animator.SetBool("IsAttack", true);

                yield return _attackTimeStart;
                MonsterAttack = transform.GetChild(0).gameObject;
                if (_monster_hp > 0)
                    MonsterAttack.SetActive(true);

                if (_monster_hp > 0)
                    _player.GetComponent<Player_script>().StartCoroutine("Screenshake", ScreenshakeTime);

                yield return _attackTimeEnd;
                MonsterAttack.SetActive(false);
                yield return _waitTime;
                _mon_animator.SetBool("IsAttack", false);
                yield return _waitTime;
            }
            else
            {
                if (_monster_noDamage == -1)
                {
                    _mon_animator.SetBool("IsMoving", true);
                    transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, _player.transform.position.x, _monster_speed * Time.deltaTime), transform.position.y, transform.position.z);
                }
                _modetime += 1;
                yield return null;
            }
        }
        StartCoroutine("Boss01SpAttack01");
    }

    IEnumerator Boss01SpAttack01()
    {
        _SpMode = 1;
        _mon_animator.SetBool("IsMoving", false);
        _mon_animator.SetInteger("IsSpAttack", 1);
        yield return _SpAttack01Start;

        for (int i = 0; i < 20; i++)
        {
            for (int monsterMove = 0; monsterMove < 1; monsterMove++)
            {
                if (Time.timeScale == 0)    //일시정시 시 for문 무한루프.
                    monsterMove--;
                else
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + i, this.transform.position.z);
                }
                yield return null;
            }
        }
        MonsterAttack = transform.GetChild(0).gameObject;
        _monster_damage = _monster_damage + ((int)_monster_damage / 2);
        if (_monster_hp > 0)
            MonsterAttack.SetActive(true);
        _player.GetComponent<Player_script>().StartCoroutine("Screenshake", ScreenshakeTime2);

        int j = _mon_animator.GetInteger("Direction");  //애니메이션 값 대입 용 변수.
        j = transform.position.x < _player.transform.position.x ? 1 : -1;
        _mon_animator.SetInteger("Direction", j);
        yield return _SpAttack01End;

        Vector3 direction = (_player.transform.position - transform.position).normalized;
        for (int i = 0; !_isTile; i++)
        {
            for (int monsterMove = 0; monsterMove < 1; monsterMove++)
            {
                if (Time.timeScale == 0)    //일시정시 시 for문 무한루프.
                    monsterMove--;
                else
                {
                    this.transform.position = new Vector3(transform.position.x + (direction.x * 20), transform.position.y + (direction.y * 20), transform.position.z);
                }
                yield return null;
            }
        }
        if (_monster_hp > 0)
        {
            _rigid2d.gravityScale = 60;

            MonsterAttack.SetActive(false);
            _monster_damage = _monster_Fdamage;
            yield return _waitTime;
            yield return _waitTime;
            yield return _waitTime;

            _modetime = 0;
            _SpMode = 0;
            _mon_animator.SetInteger("IsSpAttack", 0);
            _rigid2d.gravityScale = 0;
            StartCoroutine("Boss01Move");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 12)
            _isTile = true;

        //몬스터가 플레이어 공격에 충돌.
        if (other.CompareTag("Attack"))
            if (Player_script._attacking == 1)
            {
                if (_monster_noDamage != 0 && !MonsterAttack.activeSelf && _SpMode == 0)
                    StartCoroutine("DamageTime");
                else
                    Debug.Log("몬스터 무적시간");
            }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.parent != null)
            if (other.transform.parent.gameObject.name == "Wall")
                _isTile = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 12)
            _isTile = false;
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.parent != null)
            if (other.transform.parent.gameObject.name == "Wall")
                _isTile = false;
    }

    //몬스터가 플레이어 공격에 충돌 함수.
    IEnumerator DamageTime()
    {
        _monster_hp -= Player_script._player_power;
        Debug.Log("몬스터 HP " + _monster_hp);

        //경직&넉백&무적시간.
        if (_monster_hp > 0)
        {
            if (!_mon_animator.GetBool("IsAttack"))
            {
                _rigid2d.gravityScale = 60;
                _mon_animator.SetBool("IsMoving", false);
                _mon_animator.SetBool("IsDamage", true);

                if (Player_script._animator.GetInteger("Direction") == -1) //넉백.
                {
                    _mon_animator.SetInteger("Direction", 1);
                    _rigid2d.AddForce(new Vector3(-100f, 0f, 0f), ForceMode2D.Impulse);
                }
                else
                {
                    _mon_animator.SetInteger("Direction", -1);
                    _rigid2d.AddForce(new Vector3(100f, 0f, 0f), ForceMode2D.Impulse);
                }
            }
        }
        else    //몬스터 사망모션.
        {
            _monster_noDamage = 0;
            _mon_animator.SetBool("IsMoving", false);
            _mon_animator.SetBool("IsAttack", false);
            _mon_animator.SetTrigger("IsDead");
            gameObject.layer = 16;
            Player_script._MonsterCheck -= 1;
            MonsterAttack.SetActive(false);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            _rigid2d.gravityScale = 0;
            yield break;
        }

        while (Player_script._attacking == 1)    //도트 데미지 X.
        {
            _monster_noDamage = 0;  //(연속 공격 시)코루틴 겹침 방지.
            yield return null;
        }
        yield return _waitTime;

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
}
