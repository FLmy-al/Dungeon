using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public int key;
    public List<Enemy> value;
}
[CreateAssetMenu(fileName = "EnemyPool", menuName = "EnemyPool")]
public class EnemyPool : ScriptableObject
{
    public static EnemyPool instance;

    public List<EnemyGroup> enemyGroupsList = new List<EnemyGroup>();

    private Dictionary<int, List<Enemy>> enemyGroupsDict;
    public Dictionary<int, List<Enemy>> EnemyGroupsDict
    {
        get
        {
            if (enemyGroupsDict == null)
            {
                enemyGroupsDict = new Dictionary<int, List<Enemy>>();
                foreach (var group in enemyGroupsList)
                {
                    if (!enemyGroupsDict.ContainsKey(group.key))
                        enemyGroupsDict[group.key] = group.value;
                }
            }
            return enemyGroupsDict;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        instance = Resources.Load<EnemyPool>("ScriptableObject/EnemyPool");
    }
}
