using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    //  アニメーター
    private Animator _animator = null;

    private static readonly int PlayerFrontWalkAnimState = Animator.StringToHash("FrontWalk");
    private static readonly int PlayerFrontIdleAnimState = Animator.StringToHash("FrontIdle");
    private static readonly int PlayerBackWalkAnimState = Animator.StringToHash("BackWalk");
    private static readonly int PlayerBackIdleAnimState = Animator.StringToHash("BackIdle");
    private static readonly int PlayerLeftWalkAnimState = Animator.StringToHash("LeftWalk");
    private static readonly int PlayerLeftIdleAnimState = Animator.StringToHash("LeftIdle");
    private static readonly int PlayerRightWalkAnimState = Animator.StringToHash("RightWalk");
    private static readonly int PlayerRightIdleAnimState = Animator.StringToHash("RightIdle");

    //  現在のアニメーションステータス
    private PlayerManager.PlayerAnimState _playerAnimState = PlayerManager.PlayerAnimState.FrontIdle;
    //  １回の移動距離
    private static readonly float _walkDistance = 32;
    //  移動中かどうかの判断
    private bool                                               _isWalking = false;
    public  bool                                               IsWalking => _isWalking;
    private Dictionary<PlayerManager.PlayerAnimState, Vector3> _addTable = 
        new Dictionary<PlayerManager.PlayerAnimState, Vector3>()
    {
        {PlayerManager.PlayerAnimState.FrontWalk , Vector3.down},
        {PlayerManager.PlayerAnimState.LeftWalk , Vector3.left},
        {PlayerManager.PlayerAnimState.RightWalk , Vector3.right},
        {PlayerManager.PlayerAnimState.BackWalk , Vector3.up},
    };
    private Dictionary<PlayerManager.PlayerAnimState, int> _animHashTable =
        new Dictionary<PlayerManager.PlayerAnimState, int>()
        {
            {PlayerManager.PlayerAnimState.FrontIdle, PlayerFrontIdleAnimState},
            {PlayerManager.PlayerAnimState.LeftIdle,  PlayerLeftIdleAnimState},
            {PlayerManager.PlayerAnimState.RightIdle, PlayerRightIdleAnimState},
            {PlayerManager.PlayerAnimState.BackIdle,  PlayerBackIdleAnimState},
            {PlayerManager.PlayerAnimState.FrontWalk, PlayerFrontWalkAnimState},
            {PlayerManager.PlayerAnimState.LeftWalk,  PlayerLeftWalkAnimState},
            {PlayerManager.PlayerAnimState.RightWalk, PlayerRightWalkAnimState},
            {PlayerManager.PlayerAnimState.BackWalk,  PlayerBackWalkAnimState},
        };

    //  １フレームでの移動距離（ドット数）
    [SerializeField]
    private float _walkStep = 0.1f;

    //  移動終了のコールバック関数登録場所
    // private System.Action _walkEndCallback = null;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 移動終了のコールバック関数登録用の関数
    /// </summary>
    // public void SetupWalkEndCallback(System.Action walkEndCallback)
    // {
    //     _walkEndCallback = walkEndCallback;
    // }

    /// <summary>
    /// 指定のアニメーションを呼び出す
    /// </summary>
    /// <param name="playerAnimState">呼び出すアニメーションのタイプ</param>
    public void SetAnimation(PlayerManager.PlayerAnimState playerAnimState)
    {
        //  同じアニメーションの再生中ならば再度リクエストしない
        if(_playerAnimState == playerAnimState || _isWalking) return;
        //  アニメーションステータスの変更
        _playerAnimState = playerAnimState;
        //  アニメーションリクエスト
        _animator.SetTrigger(_animHashTable[playerAnimState]);
    }

    /// <summary>
    /// 移動状態から通常状態へ変更する
    /// </summary>
    public void SetIdle()
    {
        //  現在の状態がアイドリングのどれかだった場合は何もしない
        if (_playerAnimState < PlayerManager.PlayerAnimState.IdleEnd)
            return;
        //  移動ステータスからアイドリングステータスへの変更
        _playerAnimState = (PlayerManager.PlayerAnimState)((int)_playerAnimState - 4);
        //  アニメーションリクエスト
        _animator.SetTrigger(_animHashTable[_playerAnimState]);
    }
    
    /// <summary>
    /// 移動処理
    /// </summary>
    public void WalingStart()
    {
        //  すでに移動中あるいはアイドリング状態ならば何もしない
        if(_isWalking || PlayerManager.PlayerAnimState.IdleEnd > _playerAnimState) return;
        _isWalking = true;
        //  移動処理を呼び出す
        WalingTask(this.GetCancellationTokenOnDestroy()).Forget();
    }

    /// <summary>
    /// 移動処理実行
    /// </summary>
    private async UniTask WalingTask(CancellationToken token)
    {
        //  移動開始座標取得
        Vector3 orgPos = transform.localPosition;
        //  １フレームごとの加算ドット
        Vector3 addPos = _addTable[_playerAnimState];
        //  トータル移動量
        Vector3 stepPos = Vector3.zero;
        while (stepPos.magnitude < _walkDistance)
        {
            //  キャンセル（アクセスできない状態）になったら呼ばれる
            if (token.IsCancellationRequested)
                break;
            //  マイフレームの座標加算
            stepPos += addPos * _walkStep;
            //  加算値が移動距離を超えた時、丁度に調整する
            if (stepPos.magnitude >= _walkDistance) stepPos = addPos * _walkDistance;
            //  オブジェクトの座標更新
            transform.localPosition = orgPos + stepPos;
            //  １フレーム待機
            await UniTask.Yield();
        }
        //  移動終了
        _isWalking = false;
        // if (null != _walkEndCallback)
        //     _walkEndCallback();
        //  _walkEndCallback.Invoke();
    }
}
