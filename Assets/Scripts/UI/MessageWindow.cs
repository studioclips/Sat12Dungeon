using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MessageWindow : MonoBehaviour
{
    //  メッセージ表示テキスト
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI = null;
    //  メッセージ表示速度
    [SerializeField] private float _stringSpeed = 0.1f;

    //  ウィンドウ表示関連
    private CanvasGroup _canvasGroup = null;
    //  メッセージ表示中フラグ
    private bool _isAction = false;
    //  メッセージ表示終了呼び出し
    private System.Action _stringEndCallback = null;
    //  表示対象メッセージ
    private string _originalMessage = "";

    //  ダンジョン内で発生するメッセージ
    private List<string> _dungeonMessageLists = new List<string>();

    //  ダンジョンで使用する鍵の名前
    private List<string> _keyNameLists = new List<string>()
    {
        "普通", "銀", "金", "最後"
    };

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private async UniTask Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        var keyText = await Addressables.LoadAssetAsync<TextAsset>("KeyText");
        _dungeonMessageLists = keyText.text.Replace("\r\n","\n").Replace("\r","\n").Split("\n").ToList();
        Addressables.Release(keyText);
    }

    /// <summary>
    /// 初期化（コールバックを登録しておく）
    /// </summary>
    /// <param name="stringCallback">コールバック関数</param>    
    public void Init(System.Action stringCallback)
    {
        _stringEndCallback = stringCallback;
        _textMeshProUGUI.text = "";
        _isAction = false;
    }

    /// <summary>
    /// メッセージの設定
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    public void SetMessage(string message)
    {
        _originalMessage = message;_textMeshProUGUI.text = "";
        _isAction = false;
    }

    /// <summary>
    /// メッセージの設定（メッセージIDでメッセージを指定する）
    /// </summary>
    /// <param name="messageID">メッセージの種類</param>
    /// <param name="param">必要なパラメータ</param>
    public void SetMessage(CommonParam.MessageID messageID, int param = 0)
    {
        //  鍵関連
        if(CommonParam.MessageID.KeyStart <= messageID && CommonParam.MessageID.KeyEnd >= messageID)
            _originalMessage = string.Format(_dungeonMessageLists[(int)messageID], _keyNameLists[param]);
        //  鍵以外
        else
            _originalMessage = string.Format(_dungeonMessageLists[(int)messageID], param);
        _textMeshProUGUI.text = "";
        _isAction = false;
    }

    /// <summary>
    /// メッセージの表示処理
    /// </summary>
    public void ShowMessage()
    {
        //  現在表示中なら何もしない
        if(_isAction) return;
        _isAction = true;
        _canvasGroup.alpha = 1f;
        StartCoroutine(StartMessage());
    }

    /// <summary>
    /// 子ルーチンで文字を一文字ずつ出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartMessage()
    {
        int index = 0;
        while(_originalMessage.Length > index)
        {
            //  文字表示処理を行う
            index++;
            var str = _originalMessage.Substring(0, index);
            _textMeshProUGUI.text = str;
            yield return new WaitForSeconds(_stringSpeed);
        }
        //  文字表示処理終了
        _isAction = false;
        //  コールバックが存在するなら実行される
        _stringEndCallback ? .Invoke();
    }

    /// <summary>
    /// メッセージウィンドウを非表示にする
    /// </summary>
    public void HideMessage()
    {
        _isAction = false;
        _canvasGroup.alpha = 0f;
    }
}
