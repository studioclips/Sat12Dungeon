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
        if(false == _mapManager.IsFloorUp(_playerManager.PlayerPos))
            _mapManager.IsFloorDown(_playerManager.PlayerPos);
    }

    /// <summary>
    /// 移動終了を検出
    /// </summary>
    // private void WalkEnd()
    // {
    //     Debug.Log("移動終了!!!");
    // }
}
