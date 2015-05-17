using UnityEngine;
using System.Collections;

//2013年7月28日15:50:32，郭志程

/*
        This class can be used like an interface.
        Inherit from it to define your own movement motor that can control
        the movement of characters, enemies, or other entities.         
    */

/// <summary>
/// 这个类被当做一个接口使用。继承这个类定义你自己的移动业务逻辑，用来控制角色，敌人等的移动
/// </summary>

public class MovementMotor : MonoBehaviour { 

    // The direction the character wants to move in, in world space. 
    // The vector should have a length between 0 and 1. 
    /// <summary>
    /// 角色在世界坐标系中要移动的方向.向量的长度值应该在0到1之间
    /// </summary>
    [HideInInspector] //在监视面板不显示出来的公共变量   
    public Vector3 movementDirection;

    // Simpler motors might want to drive movement based on a target purely
    /// <summary>
    /// 移动的目标点，比如右键点击，角色走到对应的那个位置
    /// </summary>
    [HideInInspector]
    public Vector3 movementTarget;

    // The direction the character wants to face towards, in world space.
    /// <summary>
    /// 角色面朝哪个方向，在世界坐标系中，角色是面朝前的
    /// </summary>
    [HideInInspector]
    public Vector3 facingDirection;

}