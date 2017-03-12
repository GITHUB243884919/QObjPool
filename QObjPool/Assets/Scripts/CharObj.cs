/// <summary>
/// 角色对象定义(坦克，士兵等)
/// 
/// author: fanzhengyong
/// date: 2017-03-12
/// 
/// 包含了其AI的接口
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharObj
{
    //服务器定义的唯一编号和类型编号
    public int             ServerEntityID { get; set; }
    //public int ServerEntityType { get; set; }

    //角色对应的prefab 坦克，士兵之类的prefab
    public GameObject      GameObject     { get; set; }

    //对象类型
    public BattleObjManager.E_BATTLE_OBJECT_TYPE Type { get; set; }

    public CharObj()
    {
        GameObject     = null;
    }

}
