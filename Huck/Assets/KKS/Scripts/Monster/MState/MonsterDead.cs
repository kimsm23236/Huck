using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDead : IMonsterState
{
    private MonsterController mController;
    private CapsuleCollider monsterCollider = default;
    public void StateEnter(MonsterController _mController)
    {
        this.mController = _mController;
        mController.enumState = MonsterController.MonsterState.DEAD;
        //Debug.Log($"�������� ���� : {mController.monster.monsterName}");
        monsterCollider = mController.gameObject.GetComponent<CapsuleCollider>();
        mController.isDelay = false;
        // ���ͽ�ü �浹�� �������� Ʈ���� true
        monsterCollider.isTrigger = true;
        // ������ Ÿ�Ժ� ����ó��
        switch (mController.monster.monsterType)
        {
            case Monster.MonsterType.BOSS:
                // �������ʹ� ���� ������ �޶� ���� ó��
                mController.monster.BossDead();
                break;
            case Monster.MonsterType.NAMEED:
                mController.CoroutineDeligate(Dead());
                break;
            case Monster.MonsterType.NOMAL:
                mController.CoroutineDeligate(Dead());
                break;
        }
    } // StateEnter
    public void StateFixedUpdate()
    {
        /*Do Nothing*/
    } // StateFixedUpdate
    public void StateUpdate()
    {
        /*Do Nothing*/
    } // StateUpdate
    public void StateExit()
    {
        monsterCollider.isTrigger = false;
    } // StateExit

    //! ���� ���� ó�� �Լ�
    private IEnumerator Dead()
    {
        mController.monsterAni.SetBool("isDead", true);
        yield return null;
        yield return new WaitForSeconds(mController.monsterAni.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1.5f);
        if (mController.monster.monsterType == Monster.MonsterType.NAMEED)
        {
            GameManager.Instance.IsMidBossClear = true;
        }
        // ������ ��ü�� �������� �ϱ����� �׺�Ž� ��Ȱ��ȭ
        mController.mAgent.enabled = false;
        // 4�ʿ� ���� �� 2f��ŭ ������ ������ �ڿ� ��Ʈ����
        float deadTime = 0f;
        while (deadTime < 4f)
        {
            deadTime += Time.deltaTime;
            float deadSpeed = Time.deltaTime * 0.5f;
            mController.transform.position += Vector3.down * deadSpeed;
            yield return null;
        }
        mController.DestroyObj(mController.gameObject);
    } // Dead
} // MonsterDead
