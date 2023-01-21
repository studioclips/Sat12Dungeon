using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region 参照データ

    //  マップチップ用のプレハブ
    [SerializeField]
    private GameObject _mapPrefab = null;

    //  マップを生成する親の座標
    [SerializeField]
    private Transform _mapParent = null;

    //  マップに表示するスプライトイメージを登録
    [SerializeField]
    private List<Sprite> _mapSpriteLists = new List<Sprite>();

    #endregion

    #region 内部データ

    //  マップデータ フロア番号、Y方向、X方向
    private int[,,] _mapData = new int[5, 20, 20];

    //  階数（フロア番号）
    private int _mapFloor = 0;

    //  最初に読み込むファイル名
    private string _firstMapFileName = "Data/originalMap";

    //  生成したマップチップの情報を保存する場所
    private List<MapImageView> _mapImageLists = new List<MapImageView>();

    //  鍵の保存場所
    private List<int> _doorKeys = new List<int>() { 0, 0, 0, 0 };

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //  マップテキストファイルを読み込む
        var mapText = Resources.Load<TextAsset>(_firstMapFileName);
        //  テキストCSVファイルのみ取り出す
        string readMapText = mapText.text;
        //  CSV データをマップデータに変換
        MakeMapData(readMapText);
        //  マップの初期表示
        InitalizeMap();
    }

    #region マップ表示関連

    /// <summary>
    /// CSVデータからマップデータへコンバートする
    /// </summary>
    private void MakeMapData(string readMapText)
    {
        //  今回はマップフロア１階層だけで対応
        //  TODO: そのうち５階までコンバート出来る様に対応する
        //  テキストを改行で区切って配列に直す
        string[] mapLines = readMapText.Replace("\r\n", "\n")
                                       .Replace("\r", "\n")
                                       .Split('\n');
        //  Y のインデックス初期化
        int y = 0;
        //  Y のラインデータの個数分繰り返し対応する
        foreach (string mapLine in mapLines)
        {
            //  １行のデータに "floor" の文字があれば階層を表示している行なので特別な処理はしないで階層の値を取得する
            if (mapLine.IndexOf("floor") >= 0)
            {
                //  「,」で文字列を区切って配列にする
                string[] lines = mapLine.Split(',');
                //  文字列の中から "floor" を削除すると階層の数値の文字が残るはず
                var floorCount = lines[0].Replace("floor", "");
                //  文字列を数値に変換してみる。うまくいくと true が返ってきて out int num に変換された数値が入る
                if (int.TryParse(floorCount, out int num))
                    _mapFloor = num;
                else
                    _mapFloor++;
                //  Y座標をリセットする
                y = 0;
                continue;
            }
            //  「,」で文字列を区切って配列にする
            string[] mapXDatas = mapLine.Split(',');
            if (mapXDatas.Length < 20)
                continue;
            //  X のインデックス初期化
            int x = 0;
            //  x データの個数分繰り返し対応する
            foreach (string mapXData in mapXDatas)
            {
                //  マップデータを数値に変換してマップに保存する
                if (int.TryParse(mapXData, out int mData))
                    _mapData[_mapFloor, y, x] = mData;
                //  16進数の場合こちらに来る
                else
                {
                    //  16進のはずなので先頭に0xがあるはず。これを取り払う。
                    var mxData = mapXData.Replace("0x", "");
                    _mapData[_mapFloor, y, x] = System.Convert.ToInt32(mxData, 16);
                }
                //  X のインデックスを１つ進める
                x++;
            }
            //  Y のインデックスを１つ進める
            y++;
        }
        //  １階にいる状態に戻す
        _mapFloor = 0;
    }

    /// <summary>
    /// マップデータの取得
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns>マップチップの値</returns>
    private int GetMapData(int x, int y)
    {
        return _mapData[_mapFloor, y, x] & 0xff;
    }

    /// <summary>
    /// マップステータスの取得
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns>マップステータスの値</returns>
    private int GetMapStat(int x, int y)
    {
        return _mapData[_mapFloor, y, x] >> 8;
    }

    /// <summary>
    /// マップの初期表示
    /// </summary>
    private void InitalizeMap()
    {

        foreach (int y in Enumerable.Range(0, CommonParam.MapHeight))
        {
            foreach (int x in Enumerable.Range(0, CommonParam.MapWidth))
            {
                //  マップチップをプレハブから生成する
                GameObject mapChip = Instantiate(_mapPrefab, _mapParent);
                //  マップチップの初期座標を設定する
                mapChip.transform.localPosition =
                    new Vector3(CommonParam.StartXPos + x * CommonParam.ChipSize,
                                CommonParam.StartYpos - y * CommonParam.ChipSize,
                                0);
                int mData = GetMapData(x, y);
                int sData = GetMapStat(x, y);
                //  マップチップイメージ画像を設定する
                var mapImageView = mapChip.GetComponent<MapImageView>();
                mapImageView.SetMapImage(_mapSpriteLists[mData]);
                _mapImageLists.Add(mapImageView);
            }
        }
    }

    /// <summary>
    /// マップの再描画
    /// </summary>
    private void ReDrawMap()
    {
        foreach (int y in Enumerable.Range(0, CommonParam.MapHeight))
        {
            foreach (int x in Enumerable.Range(0, CommonParam.MapWidth))
            {
                var mData = GetMapData(x, y);
                var mStat = GetMapStat(x, y);
                var index = y * CommonParam.MapWidth + x;
                _mapImageLists[index].SetMapImage(_mapSpriteLists[mData]);
            }
        }
    }

    #endregion

    #region マップ移動関連

    /// <summary>
    /// 当たり判定テーブル（ここに登録したチップは上を通り抜けできない）
    /// </summary>
    /// <typeparam name="CommonParam.MapChipType">マップチップ番号</typeparam>
    /// <returns></returns>
    private List<CommonParam.MapChipType> _hitChipTypes = new List<CommonParam.MapChipType>()
    {
        CommonParam.MapChipType.Wall,
        CommonParam.MapChipType.Door,
        CommonParam.MapChipType.OpenTBox,
        CommonParam.MapChipType.CloseTBox
    };

    /// <summary>
    /// 指定された座標に移動可能かどうかのチェック
    /// </summary>
    /// <param name="pos">指定された座標</param>
    /// <returns>移動可能かどうかの判断</returns>
    public bool IsMapMoveEnable(Vector3Int pos)
    {
        int mData = GetMapData(pos.x, pos.y);
        int mStat = GetMapStat(pos.x, pos.y);
        //  壁であり、尚且つ通り抜けられる場合は移動可能
        if ((int)CommonParam.MapChipType.Wall == mData && 1 == mStat) return true;
        //  hitchip を順番に取り出して value に入れる
        foreach (int value in _hitChipTypes)
        {
            //  value がマップデータと等しければそこは移動できないので false を返す
            if (value == mData)
                return false;
        }
        //  どのパターンにも引っ掛からなかったら移動可能なので true を返す
        return true;
    }

    /// <summary>
    /// 階段を登ったかチェックして登っていたらマップ書き換え
    /// </summary>
    /// <param name="pos">現在の座標</param>
    /// <returns>登っていたら true</returns>
    public bool IsFloorUp(Vector3Int pos, System.Action<CommonParam.MessageID, int> callback = null)
    {
        int mData = GetMapData(pos.x, pos.y);
        if ((int)CommonParam.MapChipType.UpStep == mData)
        {
            _mapFloor++;
            ReDrawMap();
            //  コールバックが null でなければ現在の階層(0からなので) + 1 を引数に渡す
            callback ? .Invoke(CommonParam.MessageID.UpMessage, _mapFloor + 1);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 階段を降りれるかのチェック、降りれるようなら降りる
    /// </summary>
    /// <param name="pos">現在の座標</param>
    public void IsFloorDown(Vector3Int pos, System.Action<CommonParam.MessageID, int> callback = null)
    {
        int mData = GetMapData(pos.x, pos.y);
        if ((int)CommonParam.MapChipType.DownStep == mData)
        {
            _mapFloor--;
            ReDrawMap();
            //  コールバックが null でなければ現在の階層(0からなので) + 1 を引数に渡す
            callback ? .Invoke(CommonParam.MessageID.DownMessage, _mapFloor + 1);
        }
    }

    /// <summary>
    /// 落とし穴チェック
    /// </summary>
    /// <param name="pos">現在の座標</param>
    public void HoleCheck(Vector3Int pos, System.Action<CommonParam.MessageID, int> callback = null)
    {
        int mData = GetMapData(pos.x, pos.y);
        if ((int)CommonParam.MapChipType.Hole == mData)
        {
            _mapFloor--;
            ReDrawMap();
            //  コールバックが null でなければ現在の階層(0からなので) + 1 を引数に渡す
            callback ? .Invoke(CommonParam.MessageID.HoleMessage, _mapFloor + 1);
        }
    }


    #endregion

    #region 宝箱

    //  辞書テーブルの用意（向きのenumとその方向のベクトルをセットにして登録）
    private Dictionary<PlayerManager.PlayerAnimState, Vector3Int> _directionVecLists =
        new Dictionary<PlayerManager.PlayerAnimState, Vector3Int>()
        {
            {PlayerManager.PlayerAnimState.BackIdle, Vector3Int.down},
            {PlayerManager.PlayerAnimState.LeftIdle, Vector3Int.left},
            {PlayerManager.PlayerAnimState.RightIdle, Vector3Int.right},
            {PlayerManager.PlayerAnimState.FrontIdle, Vector3Int.up}
        };

    /// <summary>
    /// 宝箱開閉チェック
    /// </summary>
    /// <param name="playerPos">プレイヤー座標</param>
    /// <param name="playerAnimState">プレイヤーの向き</param>
    public void TreasureCheck(Vector3Int playerPos, PlayerManager.PlayerAnimState playerAnimState, System.Action<CommonParam.MessageID, int> callback = null)
    {
        //  向いている方向の座標を取得
        var checkPos = playerPos + _directionVecLists[playerAnimState];
        //  マップチップデータを取得
        var mData = GetMapData(checkPos.x, checkPos.y);
        //  マップステータスを取得
        var sData = GetMapStat(checkPos.x, checkPos.y);
        //  マップチップデータが閉じた宝箱ならば処理する
        if ((int)CommonParam.MapChipType.CloseTBox == mData)
        {
            //  宝箱の鍵の番号を調整（１〜なので０〜に直すために１マイナスする）
            sData--;
            //  持っている鍵が追加される
            _doorKeys[sData]++;
            //  閉じた宝箱を開いた宝箱に変更
            _mapData[_mapFloor, checkPos.y, checkPos.x] = (int)CommonParam.MapChipType.OpenTBox;
            //  マップチップの配置場所を取得
            var index = checkPos.y * 20 + checkPos.x;
            //  対象のスプライトを入れ替える
            _mapImageLists[index].SetMapImage(_mapSpriteLists[(int)CommonParam.MapChipType.OpenTBox]);
            //  コールバックが null でなければ鍵の取得と鍵の番号を引数に渡す
            callback ? .Invoke(CommonParam.MessageID.GetKeyMessage, sData);
        }
    }

    #endregion

    #region 扉

    /// <summary>
    /// 扉処理
    /// </summary>
    /// <param name="playerPos">プレイヤー座標</param>
    /// <param name="playerAnimState">プレイヤーの向き</param>
    public void DoorCheck(Vector3Int playerPos, PlayerManager.PlayerAnimState playerAnimState, System.Action<CommonParam.MessageID, int> callback = null)
    {
        //  向いている方向の座標を取得
        var checkPos = playerPos + _directionVecLists[playerAnimState];
        //  マップチップデータを取得
        var mData = GetMapData(checkPos.x, checkPos.y);
        //  マップステータスを取得
        var sData = GetMapStat(checkPos.x, checkPos.y);
        //  マップチップデータが扉ならば処理する
        if ((int)CommonParam.MapChipType.Door == mData)
        {
            //  宝箱の鍵の番号を調整（１〜なので０〜に直すために１マイナスする）
            sData--;
            if (0 < _doorKeys[sData])
            {
                //  持っている鍵が消費される
                _doorKeys[sData]--;
                //  扉を地面に変更
                _mapData[_mapFloor, checkPos.y, checkPos.x] = (int)CommonParam.MapChipType.Floor;
                //  マップチップの配置場所を取得
                var index = checkPos.y * 20 + checkPos.x;
                //  対象のスプライトを入れ替える
                _mapImageLists[index].SetMapImage(_mapSpriteLists[(int)CommonParam.MapChipType.Floor]);
                //  コールバックが null でなければ扉を開いたメッセージとどの鍵を使ったか鍵の番号を引数に渡す
                callback ? .Invoke(CommonParam.MessageID.OpenDoorMessage, sData);
            }
            else
            {
                //  コールバックが null でなければ鍵がなくて扉が開かないメッセージとどの鍵をないか鍵の番号を引数に渡す
                callback ? .Invoke(CommonParam.MessageID.NoKeyMessage, sData);
            }
        }
    }

    #endregion

    #region ワープ


    /// <summary>
    /// ワープのチェックと移動
    /// </summary>
    /// <param name="pos">移動後の座標</param>
    /// <return>移動先の場所を返す</return>
    public Vector3Int WarpCheck(Vector3Int pos)
    {
        //  マップチップデータ
        int mData = GetMapData(pos.x, pos.y);
        //  マップステータスからワープ先の場所を取得
        int sData = GetMapStat(pos.x, pos.y) >> 4;
        if ((int)CommonParam.MapChipType.Warp == mData)
        {
            //  移動先を探しにいく
            return GetWarpPos(sData);
        }
        return Vector3Int.zero;
    }

    /// <summary>
    /// ワープ先を探す
    /// </summary>
    /// <param name="moveToPoint">ワープ先の番号</param>
    /// <returns>ワープ座標</returns>
    private Vector3Int GetWarpPos(int moveToPoint)
    {
        foreach(var y in Enumerable.Range(0, CommonParam.MapHeight))
        {
            foreach(var x in Enumerable.Range(0, CommonParam.MapWidth))
            {
                //  マップチップデータの取得
                int mData = GetMapData(x, y);
                //  ワープ場所ならばワープ先のチェックを行う
                if((int)CommonParam.MapChipType.Warp == mData)
                {
                    //  ワープの元データを取得する
                    int mStat = GetMapStat(x, y) & 0xf;
                    //  ワープ元データと移動先データが一致したらその座標を返す
                    if(mStat == moveToPoint)
                        return new Vector3Int(x, y, 0);
                }
            }
        }
        return Vector3Int.zero;
    }

    #endregion

}
