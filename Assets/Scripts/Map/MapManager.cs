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
    /// マップの初期表示
    /// </summary>
    private void InitalizeMap()
    {
        foreach (int y in Enumerable.Range(0, 20))
        {
            foreach (int x in Enumerable.Range(0, 20))
            {
                GameObject mapChip = Instantiate(_mapPrefab, _mapParent);
                mapChip.transform.localPosition = new Vector3(x * 32, y * 32, 0);
            }
        }
    }

    private void ReDrawMap()
    {
        
    }
}
