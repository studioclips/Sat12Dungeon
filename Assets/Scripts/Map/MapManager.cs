using System.Collections;
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
    
    private static readonly int   _mapWidth  = 20;                                             //  マップのX方向のチップ数
    private static readonly int   _mapHeight = 20;                                             //  マップのY方向のチップ数
    private static readonly int   _chipSize  = 32;                                             //  チップの大きさ
    private readonly        float _startXPos = -_chipSize * _mapWidth / 2f + _chipSize / 2.0f; //  チップの開始X座標
    private readonly        float _startYpos = _chipSize * _mapHeight / 2f - _chipSize / 2.0f; //  チップの開始Y座標
    //  コンパクトにまとめるとこんな感じ
    // private readonly        float _startXPos = -_chipSize * (_mapWidth - 1) / 2f; //  チップの開始X座標
    // private readonly        float _startYpos = _chipSize * (_mapHeight - 1) / 2f; //  チップの開始Y座標
    
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
        
        foreach (int y in Enumerable.Range(0, _mapHeight))
        {
            foreach (int x in Enumerable.Range(0, _mapWidth))
            {
                //  マップチップをプレハブから生成する
                GameObject mapChip = Instantiate(_mapPrefab, _mapParent);
                //  マップチップの初期座標を設定する
                mapChip.transform.localPosition = new Vector3(_startXPos + x * _chipSize, _startYpos - y * _chipSize, 0);
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
    
}
