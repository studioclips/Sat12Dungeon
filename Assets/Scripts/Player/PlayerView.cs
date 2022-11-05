using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : MonoBehaviour
{
    //  アニメーター
    private Animator _animator = null;

    private static readonly int PlayerAnimState = Animator.StringToHash("PlayerAnimState");

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAnimation(PlayerManager.PlayerAnimState playerAnimState)
    {
        _animator.SetInteger(PlayerAnimState, (int)playerAnimState);
    }
}
