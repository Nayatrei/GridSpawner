using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CsGridObjectSpawner : EditorWindow
{

    [SerializeField] private GameObject SpawnParents;

    [SerializeField] private float flWorldWidth = 0f;
    [SerializeField] private float flWorldLength = 0f;

    [SerializeField] private float m_BaseWidth;
    [SerializeField] private float m_BaseLength;

    [SerializeField] private bool b_WallOrientation = false;
    [SerializeField] private float flRaiseWallAmount = 0f;

    [SerializeField] private float m_WallWidth;

    [SerializeField] private float m_PrefabDistance;

    [SerializeField] private GameObject SpawnGrounds;
    [SerializeField] private GameObject SpawnWalls;
    [SerializeField] private GameObject SpawnPillars;



    [MenuItem("Tools/Toolkit/Grid Spawner")]
    static public void ShowWindow()
    {
        CsGridObjectSpawner window = EditorWindow.GetWindow<CsGridObjectSpawner>();
        window.titleContent.text = "Grid Spawner";
    }

    private void OnGUI()
    {
        GUILayout.Label("World Grid Spawner", EditorStyles.boldLabel);

        // Object field for selecting GameObject
        SpawnParents = EditorGUILayout.ObjectField("Parent Object", SpawnParents, typeof(GameObject), true) as GameObject;

        // Float field for width
        flWorldWidth = EditorGUILayout.FloatField("World Width (Meters)", flWorldWidth);

        // Float field for length
        flWorldLength = EditorGUILayout.FloatField("World Length (Meters)", flWorldLength);


        // Button to perform action with the selected GameObject
        if (SpawnParents != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Spawn Base Prefab According to World Size", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            SpawnGrounds = (GameObject)EditorGUILayout.ObjectField("Ground Prefab", SpawnGrounds, typeof(GameObject), false);
            if (SpawnGrounds != null)
            {
                m_BaseWidth = GetObjectLength(SpawnGrounds);
                m_BaseLength = GetObjectWidth(SpawnGrounds);
                EditorGUILayout.LabelField("Width of this Ground Object is = " + m_BaseWidth + " Meters", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Length of this Ground Object is = " + m_BaseLength + " Meters", EditorStyles.boldLabel);
                // Calculate how many objects fit in the length and width
                int objectsInLength = Mathf.FloorToInt(flWorldLength / m_BaseLength);
                int objectsInWidth = Mathf.FloorToInt(flWorldWidth / m_BaseWidth);

                EditorGUILayout.LabelField("# of Objects to Spawn = " + objectsInLength * objectsInWidth + " ", EditorStyles.boldLabel);
                if (GUILayout.Button("Spawn Ground objects", GUILayout.Height(30)))
                {
                    SpawnGroundObjects(SpawnGrounds,objectsInLength, objectsInWidth);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Spawn Wall Prefab According to World Size", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            SpawnWalls = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", SpawnWalls, typeof(GameObject), false);
            if (SpawnWalls != null)
            {
                flRaiseWallAmount = EditorGUILayout.FloatField("Raise Wall from Ground (Meters) = ", flRaiseWallAmount);
                b_WallOrientation = EditorGUILayout.Toggle("Change Wall Orientation", b_WallOrientation);

                if (b_WallOrientation)
                {
                    m_WallWidth = GetObjectLength(SpawnWalls);
                }
                if (!b_WallOrientation)
                {
                    m_WallWidth = GetObjectWidth(SpawnWalls);
                }
                EditorGUILayout.LabelField("Width of this Wall Object is = " + m_WallWidth + " Meters", EditorStyles.boldLabel);

                // Calculate how many objects fit in the length and width
                int wallObjectsInWidth = Mathf.FloorToInt(flWorldWidth / m_WallWidth);
                int wallObjectsInLength = Mathf.FloorToInt(flWorldLength / m_WallWidth);

                EditorGUILayout.LabelField("# of Objects to Spawn = " + wallObjectsInWidth * wallObjectsInLength * 2 + " ", EditorStyles.boldLabel);

                if (GUILayout.Button("Spawn Wall objects", GUILayout.Height(30)))
                {
                    SpawnWallObjects(SpawnWalls, wallObjectsInLength, wallObjectsInWidth, flRaiseWallAmount);
                }
            }


            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Spawn Prefab According to World Size", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            SpawnPillars = (GameObject)EditorGUILayout.ObjectField("Add Prefab", SpawnPillars, typeof(GameObject), false);
            if (SpawnPillars != null)
            {
                flRaiseWallAmount = EditorGUILayout.FloatField("Raise Prefab from Ground (Meters) = ", flRaiseWallAmount);
                m_PrefabDistance = EditorGUILayout.FloatField("Distance between Prefabs = ", m_PrefabDistance);

                int PrefabAmountInWidth = Mathf.FloorToInt(flWorldWidth / m_PrefabDistance);
                int PrefabAmountInLength = Mathf.FloorToInt(flWorldLength / m_PrefabDistance);
                string prefabName = "Spawn " + SpawnPillars.name;
                EditorGUILayout.LabelField("# of Objects to Spawn = " + PrefabAmountInWidth * PrefabAmountInLength + " ", EditorStyles.boldLabel);

                if (GUILayout.Button(prefabName, GUILayout.Height(30)))
                {
                    SpawnPrefabs(SpawnPillars, PrefabAmountInLength, PrefabAmountInWidth, flRaiseWallAmount);
                }
            }
        }

    }

    float GetObjectHeight(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds.size.y;
        }
        else
        {
            return 0f;
        }
    }

    float GetObjectLength(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }
        else
        {
            return 0f;
        }
    }

    float GetObjectWidth(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds.size.z;
        }
        else
        {
            return 0f;
        }
    }


    public void SpawnGroundObjects(GameObject SpawnGroundPrefab, int xAxis, int zAxis)
    {
        GameObject BaseParent = new GameObject("Ground");
        BaseParent = Instantiate(BaseParent);
        BaseParent.transform.parent = SpawnParents.transform;
        BaseParent.transform.localPosition = Vector3.zero;

        for (int i = 0; i < xAxis; i++)
        {
            for (int j = 0; j < zAxis; j++)
            {

                var prefabType = PrefabUtility.GetPrefabAssetType(SpawnGroundPrefab);
                if (prefabType != PrefabAssetType.NotAPrefab)
                {
                    GameObject newObject;

                    Vector3 spawnPosition = new Vector3(i * m_BaseWidth, 0, j * m_BaseLength);
                    newObject = PrefabUtility.InstantiatePrefab(SpawnGroundPrefab) as GameObject;
                    newObject.transform.SetParent(BaseParent.transform, true);
                    newObject.transform.localPosition = spawnPosition;
                    newObject.transform.localRotation = Quaternion.identity;

                }
            }
        }
    }


    public void SpawnWallObjects(GameObject SpawnGroundPrefab, int xAxis, int zAxis, float raiseAmount)
    {
        GameObject WestWall = new GameObject("WestWall");
        WestWall = Instantiate(WestWall);
        WestWall.transform.parent = SpawnParents.transform;
        if (SpawnGrounds != null)
        {
            WestWall.transform.localPosition = new Vector3(-m_BaseWidth / 2, raiseAmount, 0f);
        }


        //for (int i = 0; i < xAxis; i++)
        //{
            for (int j = 0; j <= zAxis; j++)
            {

                var prefabType = PrefabUtility.GetPrefabAssetType(SpawnGroundPrefab);
                if (prefabType != PrefabAssetType.NotAPrefab)
                {
                    GameObject newObject;

                    Vector3 spawnPosition = new Vector3(0, 0, j * m_BaseLength);
                    newObject = PrefabUtility.InstantiatePrefab(SpawnGroundPrefab) as GameObject;
                    newObject.transform.SetParent(WestWall.transform, true);
                    newObject.transform.localPosition = spawnPosition;
                    newObject.transform.localRotation = Quaternion.identity;

                }
            }
        //}
        GameObject EastWall = new GameObject("EastWall");
        EastWall = Instantiate(EastWall);
        EastWall.transform.parent = SpawnParents.transform;
        if (SpawnGrounds != null)
        {
            EastWall.transform.localPosition = new Vector3(flWorldLength - m_BaseLength / 2, raiseAmount, flWorldWidth - m_BaseWidth);
        }
        if(SpawnGrounds == null)
        {
            EastWall.transform.localPosition = new Vector3(flWorldLength, raiseAmount, flWorldWidth);
        }
        EastWall.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

        for (int j = 0; j <= zAxis; j++)
        {

            var prefabType = PrefabUtility.GetPrefabAssetType(SpawnGroundPrefab);
            if (prefabType != PrefabAssetType.NotAPrefab)
            {
                GameObject newObject;

                Vector3 spawnPosition = new Vector3(0, 0, j * m_BaseLength);
                newObject = PrefabUtility.InstantiatePrefab(SpawnGroundPrefab) as GameObject;
                newObject.transform.SetParent(EastWall.transform, true);
                newObject.transform.localPosition = spawnPosition;
                newObject.transform.localRotation = Quaternion.identity;

            }
        }

        GameObject NorthWall = new GameObject("NorthWall");
        NorthWall = Instantiate(NorthWall);
        NorthWall.transform.parent = SpawnParents.transform;
        if (SpawnGrounds != null)
        {
            NorthWall.transform.localPosition = new Vector3(0f, raiseAmount, flWorldWidth - m_BaseLength/2);
        }
        NorthWall.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

        for (int i = 0; i <= xAxis; i++)
        {


            var prefabType = PrefabUtility.GetPrefabAssetType(SpawnGroundPrefab);
            if (prefabType != PrefabAssetType.NotAPrefab)
            {
                GameObject newObject;

                Vector3 spawnPosition = new Vector3(0, 0, i * m_BaseLength);
                newObject = PrefabUtility.InstantiatePrefab(SpawnGroundPrefab) as GameObject;
                newObject.transform.SetParent(NorthWall.transform, true);
                newObject.transform.localPosition = spawnPosition;
                newObject.transform.localRotation = Quaternion.identity;


            }
        }

        GameObject SouthWall = new GameObject("SouthWall");
        SouthWall = Instantiate(SouthWall);
        SouthWall.transform.parent = SpawnParents.transform;
        if (SpawnGrounds != null)
        {
            SouthWall.transform.localPosition = new Vector3(flWorldLength - m_BaseLength, raiseAmount, - m_BaseLength / 2);
        }
        SouthWall.transform.localEulerAngles = new Vector3(0f, -90f, 0f);

        for (int i = 0; i <= xAxis; i++)
        {


            var prefabType = PrefabUtility.GetPrefabAssetType(SpawnGroundPrefab);
            if (prefabType != PrefabAssetType.NotAPrefab)
            {
                GameObject newObject;

                Vector3 spawnPosition = new Vector3(0, 0, i * m_BaseLength);
                newObject = PrefabUtility.InstantiatePrefab(SpawnGroundPrefab) as GameObject;
                newObject.transform.SetParent(SouthWall.transform, true);
                newObject.transform.localPosition = spawnPosition;
                newObject.transform.localRotation = Quaternion.identity;


            }
        }

    }

    public void SpawnPrefabs(GameObject SpawnPrefab, int xAxis, int zAxis, float raiseAmount)
    {
        GameObject BaseParent = new GameObject();
        BaseParent.name += SpawnPrefab.name + "_Groups";
        BaseParent = Instantiate(BaseParent);
        BaseParent.transform.parent = SpawnParents.transform;
        if (SpawnGrounds != null)
        {
            BaseParent.transform.localPosition = new Vector3(flWorldLength/2 - m_BaseLength / 2, raiseAmount, flWorldWidth/2 - m_BaseWidth / 2);
        }

        if (SpawnGrounds == null)
        {
            BaseParent.transform.localPosition = new Vector3(flWorldLength/2, raiseAmount, flWorldWidth/2);
        }

        // Calculate the offset to center the objects on the plane
        float offsetX = (flWorldWidth - (xAxis - 1) * m_PrefabDistance) / 2;
        float offsetZ = (flWorldLength - (zAxis - 1) * m_PrefabDistance) / 2;


        for (int i = 0; i < xAxis; i++)
        {
            for (int j = 0; j < zAxis; j++)
            {

                var prefabType = PrefabUtility.GetPrefabAssetType(SpawnPrefab);
                if (prefabType != PrefabAssetType.NotAPrefab)
                {
                    GameObject newObject;

                    Vector3 spawnPosition = new Vector3(i * m_PrefabDistance - flWorldWidth / 2 + offsetX, 0, j * m_PrefabDistance - flWorldLength / 2 + offsetZ);
                    newObject = PrefabUtility.InstantiatePrefab(SpawnPrefab) as GameObject;
                    newObject.transform.SetParent(BaseParent.transform, true);
                    newObject.transform.localPosition = spawnPosition;
                    newObject.transform.localRotation = Quaternion.identity;

                }
            }
        }
    }
}
