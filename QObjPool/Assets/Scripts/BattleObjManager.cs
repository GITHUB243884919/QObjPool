﻿/// <summary>
/// 战斗对象管理，提供对象的借与还
/// author : fanzhengyong
/// date  : 2017-02-24
/// 
/// 从服务器拿到需要生成坦克的下行消息后
/// 1、从BattleObjManager借一个CharObj对象（函数BorrowCharObj）
/// 2、对调用CharObj上的AI_XX函数对其进行操作（到达，攻击。。。。详见CharObj的注释）
/// 3、把不用的CharObj对象还给BattleObjManager(函数ReturnCharObj)
/// 4、一次性把之前借的CharObj全部归还（ReturnAllBorrowCharObjs）
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleObjManager : MonoBehaviour 
{
    public enum E_BATTLE_OBJECT_TYPE
    {
        //测试用
        TANK,
        SOLDIER,
        
        //以下保持和美术资源名字一致，正式资源
        M_ARM_TANK,            //坦克
        M_ARM_AIRPLANE_01,     //飞机
        M_ARM_ENGINEERCAR,     //工程车
        M_ARM_ENGINEERCORPS,   //士兵
        M_BUILD_JEEP,          //救护吉普
        M_ARM_ARTILLERY        //火炮
    }
    //for test begin
    public int EffectCount { get; set; }
    //for test end
    private static BattleObjManager s_Instance = null;
    public static BattleObjManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(BattleObjManager))
                        as BattleObjManager;
                if (s_Instance == null)
                {
                    Debug.LogError("场景中没有持有BattleObjManager的对象");
                }
            }
            return s_Instance;
        }
    }

    private void Init()
    {
        //for test begin
        EffectCount = 0;
        //for test end
        //Debug.Log("begin CharObjPoolManager Init " + Time.realtimeSinceStartup);
        CharObjPoolManager.Instance.Init();
        //Debug.Log("end CharObjPoolManager Init " + Time.realtimeSinceStartup);
        CharObjCache.Instance.Init();
        //Debug.Log("CharObjCache Init");
        //BNGObjPoolManager.Instance.Init();
        //Debug.Log("BNGObjPoolManager Init");
        ParticleObjPoolManager.Instance.Init();
        //Debug.Log("ParticleObjPoolManager Init");
    }

    /// <summary>
    /// 在场景取一个对象必须是已知类型，服务器的两个编号的
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serverEntityID"></param>
    /// <param name="serverEntityType">暂时冗余参数，目前暂时全部填0</param>
    /// <returns></returns>
    public CharObj BorrowCharObj(BattleObjManager.E_BATTLE_OBJECT_TYPE type,
        int serverEntityID, int serverEntityType)
    {
        bool    retCode = false;
        CharObj obj     = null;

        //Debug.Log("BorrowCharObj " + serverEntityID);
        //先从缓存中取
        obj = CharObjCache.Instance.Find(serverEntityID);
        if (obj != null)
        {
            //Debug.Log("缓存中找到 " + serverEntityID);
            //obj.Deactive();
            return obj;
        }

        //缓存中没有才从对象池中取
        obj = CharObjPoolManager.Instance.BorrowObj(type);
        if (obj == null)
        {
            Debug.LogError("CharObjPoolManager.Instance.BorrowObj 取到空Obj " + type.ToString());
            return obj;
        }

        //retCode = obj.IsValid();
        //if (!retCode)
        //{
        //    Debug.LogError("取到CharObj非法，请联系BattleObjManager作者");
        //    return obj;
        //}

        //为CharObj打上身份证号:）
        obj.ServerEntityID                = serverEntityID;
        obj.Type                          = type;
        //obj.CharController.ServerEntityID = serverEntityID;
        //obj.CharController.CharType       = type;
        //Debug.Log(type.ToString());

        //从对象池中取出的对象要放入缓存中
        CharObjCache.Instance.Add(obj);

        return obj;
    }

    /// <summary>
    /// 向BattleManager还CharObj对象
    /// 还的对象是之前借走的。
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnCharObj(CharObj obj)
    {
        //obj.Deactive();
        //先从缓存中移除
        CharObjCache.Instance.Remove(obj);
        //再还给对象池
        CharObjPoolManager.Instance.ReturnObj(obj);
    }

    /// <summary>
    /// 把之前借走的CharObj对象还给BattleObjManager
    /// </summary>
    public void ReturnAllBorrowCharObjs()
    {
        Dictionary<int, CharObj> cache = CharObjCache.Instance.Cache;

        foreach (KeyValuePair<int, CharObj> pair in cache)
        {
            ReturnCharObj(pair.Value);
        }
    }

    //public GameObject BorrowBNGObj(string path)
    //{
    //    GameObject obj = null;
    //    obj = BNGObjPoolManager.Instance.BorrowObj(path);
    //    return obj;
    //}

    //public void ReturnBNGObj(GameObject obj, string path)
    //{
    //    BNGObjPoolManager.Instance.ReturnObj(obj, path);
    //}

    ////////////////////////////////////////////////////////////////////////////////
    public GameObject BorrowParticleObj(string path)
    {
        GameObject obj = null;
        obj = ParticleObjPoolManager.Instance.BorrowObj(path);
        return obj;
    }

    public void ReturnParticleObj(GameObject obj, string path)
    {
        ParticleObjPoolManager.Instance.ReturnObj(obj, path);
    }

    //Unity
    void Start()
    {
        //Debug.Log("BattleObjManager Start " + Time.realtimeSinceStartup);
        Init();
    }

}
