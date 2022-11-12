using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    //  アニメーター
    private Animator _animator = null;

    private static readonly int PlayerAnimState = Animator.StringToHash("PlayerAnimState");

    //  現在のアニメーションステータス
    private PlayerManager.PlayerAnimState _playerAnimState = PlayerManager.PlayerAnimState.Idle;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 指定のアニメーションを呼び出す
    /// </summary>
    /// <param name="playerAnimState">呼び出すアニメーションのタイプ</param>
    public void SetAnimation(PlayerManager.PlayerAnimState playerAnimState)
    {
        //  同じアニメーションの再生中ならば再度リクエストしない
        if(_playerAnimState == playerAnimState) return;
        //  アニメーションステータスの変更
        _playerAnimState = playerAnimState;
        //  アニメーションリクエスト
        _animator.SetInteger(PlayerAnimState, (int)playerAnimState);
    }
}
