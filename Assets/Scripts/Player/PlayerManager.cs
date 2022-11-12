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
    
    // Start is called before the first frame update
    void Start()
    {
        _playerView = GetComponent<PlayerView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _playerView.SetAnimation(PlayerAnimState.Front);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            _playerView.SetAnimation(PlayerAnimState.Left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _playerView.SetAnimation(PlayerAnimState.Right);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _playerView.SetAnimation(PlayerAnimState.Back);
        }
        else if (false == _playerView.IsWalking)
        {
            _playerView.SetAnimation(PlayerAnimState.Idle);
        }
        //  移動処理
        _playerView.WalingStart();
    }
}
