using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //  マップチップ用のプレハブ
    [SerializeField]
    private GameObject _mapPrefab = null;

    //  マップを生成する親の座標
    [SerializeField]
    private Transform _mapParent = null;

    //  マップに表示するスプライトイメージを登録
    [SerializeField]
    private List<Sprite> _mapSpriteLists = new List<Sprite>();

    //  マップデータ フロア番号、Y方向、X方向
    private int[,,] _mapData = new int[5, 20, 20];
    
    //  階数（フロア番号）
    private int _mapFloor = 0;
    
    //  最初に読み込むファイル名
    private string _firstMapFileName = "Data/originalMap";
    
    
    // Start is called before the first frame update
    void Start()
    {
        //  マップテキストファイルを読み込む
        var mapText     = Resources.Load<TextAsset>(_firstMapFileName);
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
            //  「,」で文字列を区切って配列にする
            string[] mapXDatas = mapLine.Split(',');
            //  X のインデックス初期化
            int      x         = 0;
            //  x データの個数分繰り返し対応する
            foreach (string mapXData in mapXDatas)
            {
                //  マップデータを数値に変換してマップに保存する
                _mapData[_mapFloor, y, x] = int.Parse(mapXData);
                //  X のインデックスを１つ進める
                x++;
            }
            //  Y のインデックスを１つ進める
            y++;
        }
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
                mapChip.GetComponent<MapImageView>().SetMapImage(_mapSpriteLists[mData]);
            }
        }
    }

    private void ReDrawMap()
    {

    }

#endregion

#region マップ移動関連

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
    
#endregion
    
}
