using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObjectsPositionManager : MonoBehaviour, IDataLoader
{
    [SerializeField] private Transform[] globalTransforms;

    private GridManager gridManager;

    public void InitializeData()
    {
        gridManager = InterfaceManager.Instance?.GetInterfaceInstance<GridManager>();

        var gridCenterPoint = gridManager.GetGridCenterPoint();
        Debug.Log($"GridCenterPoint: {gridCenterPoint}");
        foreach (var obj in globalTransforms)
        {
            obj.position = new Vector3(gridCenterPoint.x, obj.position.y, obj.position.z);
        }
    }
}
