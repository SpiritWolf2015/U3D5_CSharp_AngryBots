using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectCache {

    public GameObject prefab;             // 缓存哪种物体
    public int cacheSize = 10;               // 缓存的大小

    private GameObject[] objects;       // 缓存物体数组
    private int cacheIndex = 0;             // 缓存下标

    public void Initialize() {
        objects = new GameObject[cacheSize];
        
        // 实例数组中的物体并设置物体为不激活
        for (int i = 0; i < cacheSize; i++) {
            objects[i] = MonoBehaviour.Instantiate(prefab) as GameObject;
            objects[i].SetActive(false);
            objects[i].name = objects[i].name + i;      // 给实例出来的缓存物体名字加1234567.。。的编号
        }
    }

    public GameObject GetNextObjectInCache() {
        GameObject obj = null;
        
        // 在之前物体创建时缓存下标离开当前的位置，所以当前缓存物体可用，如果不是，遍历缓存直到找到一个可供使用的缓存物体
        for (int i = 0; i < cacheSize; i++)  {
            obj = objects[cacheIndex];
            
            // 如果我们在缓存中找到一个未激活的物体使用
            if (!obj.activeSelf)
                break;
            
            // 如果没有找到，循环增加下标
            cacheIndex = (cacheIndex + 1) % cacheSize;
        }
        
        // 缓存的物体应该为未激活状态。如果不是，提示一个警告并使用之前创建的物体即使这个缓存物体已经是激活状态
        if (obj.activeSelf) {
            Debug.LogWarning("Spawn of " + prefab.name + " exceeds cache size of " + cacheSize + "! Reusing already active object.", obj);
            Spawner.Destroy(obj);
        }
        
        // 循环增加缓存下标
        cacheIndex = (cacheIndex + 1) % cacheSize;

        return obj;
    }

}

public class Spawner : MonoBehaviour {

    public static Spawner spawner;
    public ObjectCache[] caches;
    public Hashtable activeCachedObjects;

    void Awake () {
	    // 设置全局静态变量
	    spawner = this;

	    // 缓存物体的总数
	    int amount = 0;

	    // 遍历缓存
	    for (int i = 0; i < caches.Length; i++) {
		    // 初始化缓存
		    caches[i].Initialize ();

		    // 数量
		    amount += caches[i].cacheSize;
	    }
	    
        // 创建激活物体的哈希表
	    activeCachedObjects = new Hashtable (amount);
    }

    /// <summary>
    /// 创建缓存实例
    /// </summary>    
    public static GameObject Spawn ( GameObject prefab ,   Vector3 position ,   Quaternion rotation  ) {
	    ObjectCache cache = null;
	    
        // 找到指定预制体的缓存
	    if (spawner) {
		    for (int i = 0; i < spawner.caches.Length; i++) {
			    if (spawner.caches[i].prefab == prefab) {
				    cache = spawner.caches[i];
			    }
		    }
	    }
	    
        // 如果没有为此类型的预制体缓存，那么正常实例
	    if (cache == null) {
		    return Instantiate (prefab, position, rotation) as GameObject;
	    }
	    
        // 找到这个缓存的后一个缓存物体
	    GameObject obj = cache.GetNextObjectInCache ();

	    // 设置物体的位置和旋转
	    obj.transform.position = position;
	    obj.transform.rotation = rotation;

	    // 激活物体
	    obj.SetActive (true);
	    spawner.activeCachedObjects[obj] = true;

	    return obj;
    }

    /// <summary>
    /// 销毁缓存实例
    /// </summary>  
    public static void Destroy ( GameObject objectToDestroy  ) {
	    if (null != spawner && spawner.activeCachedObjects.ContainsKey (objectToDestroy)) {
		    objectToDestroy.SetActive (false);
		    spawner.activeCachedObjects[objectToDestroy] = false;
	    } else {
            Object.Destroy(objectToDestroy);      // 这里Object. 不能省略，不然会造成递归调用栈溢出 
	    }
    }

}