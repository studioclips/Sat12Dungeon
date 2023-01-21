using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour
{
    //  マップマネージャー
    [SerializeField]
    private MapManager _mapManager = null;
    //  プレイヤーマネージャー
    [SerializeField]
    private PlayerManager _playerManager = null;
    //  メッセージウィンドウ
    [SerializeField]
    private MessageWindow _messageWindow = null;
    //  メッセージの表示が終了するまで閉じられないようにする
    private bool _isEnableClose = false;
    //  メッセージ表示中フラグ
    private bool _isMessgeDisp = false;
    // [SerializeField]
    // private Button _button1 = null;
    //
    // [SerializeField]
    // private Button _button2 = null;

    private void Start()
    {
        //  移動終了を検知して呼び出すコールバックの登録
        // _playerManager.SetupWalkEndCallback(WalkEnd);
        // _button1.onClick.AddListener(Button1Press1);
        // _button1.onClick.AddListener(Button1Press2);
        // _button2.onClick.AddListener(Button2Press);
        _playerManager.SetupMoveEnableFunc(IsMapMoveEnable);
        _playerManager.SetupWalkEndCallback(WalkEnd);
        //  メッセージの表示が終了したら呼び出される
        _messageWindow.Init(() => _isEnableClose = true);
    }

    /// <summary>
    /// 指定された座標のマップチップが移動可能な場所かどうかを判断する
    /// </summary>
    /// <param name="pos">指定されたマップ座標</param>
    /// <returns>移動可能かどうかの判断</returns>
    private bool IsMapMoveEnable(Vector3Int pos)
    {
        return _mapManager.IsMapMoveEnable(pos);
    }

    // private void Button1Press1()
    // {
    //     Debug.Log("Button1 Press1");
    // }
    //
    // private void Button1Press2()
    // {
    //     Debug.Log("Button1 Press2");
    // }
    //
    // private void Button2Press()
    // {
    //     if(null != _button1)
    //     {
    //         _button1.onClick.Invoke();
    //     }
    // }
    
    // Update is called once per frame
    void Update()
    {
        //  メッセージ表示中は何もしない
        if(_isMessgeDisp)
        {
            if(_isEnableClose && Input.anyKey)
            {
                _isEnableClose = false;
                _isMessgeDisp = false;
                _playerManager.IsDisableAction = false;
                _messageWindow.HideMessage();
            }
            return;
        }
        //  スペースキーを押す
        if(Input.GetKeyDown(KeyCode.Space) && !_playerManager.IsWalking)
        {
            MapEventCheck();
        }
    }

    /// <summary>
    /// マップイベントのチェック
    /// </summary>
    private void MapEventCheck()
    {
        if(false == _mapManager.IsFloorUp(_playerManager.PlayerPos, SetMessage))
            _mapManager.IsFloorDown(_playerManager.PlayerPos, SetMessage);
        _mapManager.TreasureCheck(_playerManager.PlayerPos, _playerManager.PlayerDirection, SetMessage);
        _mapManager.DoorCheck(_playerManager.PlayerPos, _playerManager.PlayerDirection, SetMessage);
    }

    /// <summary>
    /// メッセージの表示
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="param"></param>
    private void SetMessage(CommonParam.MessageID messageID, int param)
    {
        _isMessgeDisp = true;
        _playerManager.IsDisableAction = true;
        _messageWindow.SetMessage(messageID, param);
        _messageWindow.ShowMessage();
    }

    /// <summary>
    /// 移動終了を検出
    /// </summary>
    private void WalkEnd()
    {
        //  落とし穴チェック
        _mapManager.HoleCheck(_playerManager.PlayerPos, SetMessage);
        //  ワープチェック
        _playerManager.SetPlayerPos(_mapManager.WarpCheck(_playerManager.PlayerPos));
    }
}
