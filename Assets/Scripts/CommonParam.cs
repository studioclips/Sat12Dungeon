using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonParam
{
#region マップ関連パラメータ

    public static readonly int   MapWidth  = 20;                                          //  マップのX方向のチップ数
    public static readonly int   MapHeight = 20;                                          //  マップのY方向のチップ数
    public static readonly int   ChipSize  = 32;                                          //  チップの大きさ
    public static readonly float StartXPos = -ChipSize * MapWidth / 2f + ChipSize / 2.0f; //  チップの開始X座標
    public static readonly float StartYpos = ChipSize * MapHeight / 2f - ChipSize / 2.0f; //  チップの開始Y座標

#endregion

#region キャラ関連パラメータ

    

#endregion

}
