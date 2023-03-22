using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject_Ore : BaseResourceObject
{
    public override void TakeDamage(DamageMessage message)
    {
        // 타입에 따른 추가 처리
        // 아이템 무기 타입이 곡괭이일 경우 데미지 가산 처리
        // 데미지 처리
        base.TakeDamage(message);
    }
}
