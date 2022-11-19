using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerManager : MonoBehaviour
{
    //  アニメーションを呼び出すときに使用する型
    public enum PlayerAnimState
    {
        Idle  = 0,
        Front = 1,
        Right = 2,
        Left  = 3,
        Back  = 4
    }

    //  プレイヤーの描画コンポーネント
    private PlayerView _playerView = null;

    //  マップ上の位置
    private Vector3Int _plyerPos = new Vector3Int(1, 1, 0);
    //  マップの１ブロックのサイズ
    private static readonly int _blockSize = 32;
    //  表示開始の先頭座標
    private Vector3 _playerStartPos = Vector3.zero;
    //  座標計算の補助のために使用する
    private Vector3Int _adjustPos = new Vector3Int(1, -1, 1);

    //  移動可能かどうかの判断を行う関数の登録
    private System.Func<Vector3Int, bool> _isMoveEnableFunc = null;

    // Start is called before the first frame update
    void Start()
    {
        _playerStartPos                     = new Vector3(CommonParam.StartXPos, CommonParam.StartYpos, 0);
        _playerView                         = GetComponent<PlayerView>();
        _playerView.transform.localPosition = _playerStartPos + (Vector3)(_plyerPos * _blockSize * _adjustPos);
    }

    /// <summary>
    /// 移動終了検知コールバックを playerview に渡す
    /// </summary>
    /// <param name="walkEndCallback">コールバック関数</param>
    // public void SetupWalkEndCallback(System.Action walkEndCallback)
    // {
    //     _playerView.SetupWalkEndCallback(walkEndCallback);
    // }
    
    /// <summary>
    /// 移動可能かどうかを取得する関数
    /// </summary>
    /// <param name="moveEnableFunc">移動可能かどうかを判断するコールバック関数</param>
    public void SetupMoveEnableFunc(System.Func<Vector3Int, bool> moveEnableFunc)
    {
        _isMoveEnableFunc = moveEnableFunc;
    }

    // Update is called once per frame
    void Update()
    {
        //  関数が登録されていなければ何もしない
        if (null == _isMoveEnableFunc) return;
        //  移動中はキーを受け付けない
        if(_playerView.IsWalking) return;
        if (Input.GetKey(KeyCode.DownArrow) && _isMoveEnableFunc(_plyerPos + Vector3Int.up))
        {
            _playerView.SetAnimation(PlayerAnimState.Front);
            _plyerPos += Vector3Int.up;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && _isMoveEnableFunc(_plyerPos + Vector3Int.left))
        {
            _playerView.SetAnimation(PlayerAnimState.Left);
            _plyerPos += Vector3Int.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && _isMoveEnableFunc(_plyerPos + Vector3Int.right))
        {
            _playerView.SetAnimation(PlayerAnimState.Right);
            _plyerPos += Vector3Int.right;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && _isMoveEnableFunc(_plyerPos + Vector3Int.down))
        {
            _playerView.SetAnimation(PlayerAnimState.Back);
            _plyerPos += Vector3Int.down;
        }
        else if (false == _playerView.IsWalking)
        {
            _playerView.SetAnimation(PlayerAnimState.Idle);
        }
        //  移動処理
        _playerView.WalingStart();
    }
}
