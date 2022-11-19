using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    //  アニメーター
    private Animator _animator = null;

    private static readonly int PlayerAnimState = Animator.StringToHash("PlayerAnimState");

    //  現在のアニメーションステータス
    private PlayerManager.PlayerAnimState _playerAnimState = PlayerManager.PlayerAnimState.Idle;
    //  １回の移動距離
    private static readonly float _walkDistance = 32;
    //  移動中かどうかの判断
    private bool                                               _isWalking = false;
    public  bool                                               IsWalking => _isWalking;
    private Dictionary<PlayerManager.PlayerAnimState, Vector3> _addTable = 
        new Dictionary<PlayerManager.PlayerAnimState, Vector3>()
    {
        {PlayerManager.PlayerAnimState.Front , Vector3.down},
        {PlayerManager.PlayerAnimState.Right , Vector3.right},
        {PlayerManager.PlayerAnimState.Left , Vector3.left},
        {PlayerManager.PlayerAnimState.Back , Vector3.up},
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
        _animator.SetInteger(PlayerAnimState, (int)playerAnimState);
    }
    
    /// <summary>
    /// 移動処理
    /// </summary>
    public void WalingStart()
    {
        //  すでに移動中あるいはアイドリング状態ならば何もしない
        if(_isWalking || PlayerManager.PlayerAnimState.Idle == _playerAnimState) return;
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
