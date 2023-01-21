using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  Image コンポーネントが存在しなければ強制的にオブジェクトにアタッチする命令
// [RequireComponent(typeof(Image))]
public class MapImageView : MonoBehaviour
{
    //  マップとして表示するイメージ
    [SerializeField]
    private Image _mapImage = null;

    /// <summary>
    /// マップいめじにスプライトを登録する
    /// </summary>
    /// <param name="sprite">登録するスプライト</param>
    public void SetMapImage(Sprite sprite)
    {
        _mapImage.sprite = sprite;
    }
}
