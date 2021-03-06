﻿/// <summary>
/// 战斗中角色使用的对象池管理器
/// author: fanzhengyong
/// date: 2017-02-24 
/// 
/// 持有多个对象池，这些对象都是meshbaker合并后的，分别用于坦克，士兵。。。。
/// 取用对象BorrowObj，归还用ReturnObj，
/// BorrowObj的GameObject不能删除！！！
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharObjPoolManager
{
    Dictionary<BattleObjManager.E_BATTLE_OBJECT_TYPE, QObjPool<CharObj>> m_pools =
        new Dictionary<BattleObjManager.E_BATTLE_OBJECT_TYPE, QObjPool<CharObj>>();

    private static CharObjPoolManager s_Instance = null;
    public static CharObjPoolManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new CharObjPoolManager();
            }
            return s_Instance;
        }
    }

    public  void Init()
    {
        //InitBattleObjPool(
        //    CharObjPoolConfigerMediator.Instance.
        //        GetConfiger(BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK).Paths,
        //    BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK,
        //    CharObjPoolConfigerMediator.Instance.
        //        GetConfiger(BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK).Count);

        //int countSoldier = 32;
        //string[] pathsSoldier = new string[4] 
        //{
        //    "SoldierRuntime_Bake/SoldierMeshBaker",
        //    "SoldierRuntime_Bake/SoldierRuntime_Bake-mat",
        //    "SoldierRuntime_Bake/SoldierRuntime_Bake",
        //    "SoldierRuntime_Bake/Soldier_Seed"
        //};
        //Debug.Log("Init Soldier pool");
        //InitBattleObjPool(pathsSoldier,
        //    BattleScene2.E_BATTLE_OBJECT_TYPE.SOLDIER, countSoldier);
    }

    public CharObj BorrowObj(BattleObjManager.E_BATTLE_OBJECT_TYPE type)
    {
        CharObj obj = null;
        QObjPool<CharObj> pool = null;
        m_pools.TryGetValue(type, out pool);
        if (pool == null)
        {
            InitBattleObjPool(
                CharObjPoolConfigerMediator.Instance.GetConfiger(type).Paths,
                type,
                CharObjPoolConfigerMediator.Instance.GetConfiger(type).Count);

            m_pools.TryGetValue(type, out pool);
            //return obj;
        }

        if (pool == null)
        {
            return obj;
        }

        obj = pool.BorrowObj();

        return obj;
    }

    public void ReturnObj(CharObj obj)
    {
        QObjPool<CharObj> pool = null;
        m_pools.TryGetValue(obj.Type, out pool);
        if (pool == null)
        {
            return;
        }
        CharObjCreator creator = pool.BAK_CREATOR as CharObjCreator;
        creator.HideObject(obj);
        pool.ReturnObj(obj);
    }

    private void InitBattleObjPool(string[] paths, BattleObjManager.E_BATTLE_OBJECT_TYPE type, int count)
    {
        QObjCreatorFactory<CharObj> creatorFactory = new CharObjCreatorFactory(
            paths, type, count);

        QObjPool<CharObj> pool = new QObjPool<CharObj>();
        pool.Init(null, creatorFactory);

        m_pools.Add(type, pool);
    }
}
