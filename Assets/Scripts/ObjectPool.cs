using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum PoolIndex
{
    Axe
}

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolObj
    {
        public GameObject obj;
        public int initCount;
    }

    public static ObjectPool Instance { get; private set; }

    [SerializeField] PoolObj[] poolObjs;

    Queue<GameObject>[] queue;

    void Awake()
    {
        Instance = this;

        queue = new Queue<GameObject>[poolObjs.Length];

        GameObject temp;

        for (int i = 0; i < queue.Length; i++)
        {
            queue[i] = new Queue<GameObject>();

            for (int count = 0; count < poolObjs[i].initCount; count++)
            {
                temp = CreateObject(poolObjs[i].obj);
                temp.SetActive(false);

                queue[i].Enqueue(temp);
            }
        }
    }

    // 풀링할 오브젝트 생성
    public GameObject CreateObject(GameObject original)
    {
        return Instantiate(original);
    }

    // 가져오기
    public GameObject GetPool(PoolIndex _index)
    {
        GameObject getObject;

        int index = (int)_index;

        if (queue[index].Count > 0)
        {
            getObject = queue[index].Dequeue();
            getObject.SetActive(true);
        }
        else
        {
            getObject = CreateObject(poolObjs[index].obj);
        }

        return getObject;
    }

    // 반환
    public void SetPool(PoolIndex _index, GameObject returnObject)
    {
        returnObject.SetActive(false);
        queue[(int)_index].Enqueue(returnObject);
    }
}
