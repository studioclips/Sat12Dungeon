using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockController : MonoBehaviour
{
    private float _speed = 0.1f;
    // Update is called once per frame
    void Update()
    {
        var current = Keyboard.current;
        var joy     = Joystick.current;
        if (null == current)
            return;
        if (current.rightArrowKey.isPressed)
        {
            transform.localPosition += Vector3.right * _speed;
        }
        else if (current.leftArrowKey.isPressed)
        {
            transform.localPosition += Vector3.left *_speed;
        }
        if(null == joy)
            return;
        if (0 != joy.stick.left.pressPointOrDefault)
        {
            Debug.Log(joy.stick.left.pressPointOrDefault);
        }
        if (0 != joy.stick.right.pressPointOrDefault)
        {
            Debug.Log(joy.stick.right.pressPointOrDefault);
        }
        if (0 != joy.stick.up.pressPointOrDefault)
        {
            Debug.Log(joy.stick.up.pressPointOrDefault);
        }
        if (0 != joy.stick.down.pressPointOrDefault)
        {
            Debug.Log(joy.stick.down.pressPointOrDefault);
        }
    }
}
