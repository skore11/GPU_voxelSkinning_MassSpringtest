/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class VoxelizationTest : MonoBehaviour
{

    public bool drawMeshShell = true;
    public bool drawMeshInside = true;
    public bool drawEmptyCube = false;
    public bool includeChildren = true;
    public bool createMultipleGrids = true;
    public Vector3 meshShellPositionFromObject = Vector3.zero;
    public float cubeSide = 0.1f;
    private int[] aABCGrids;//check

    void Awake()
    {

    }

    void Start()
    {
        //	TestTriangleIntersectionAABC();
        if (createMultipleGrids && includeChildren)
        {
            aABCGrids = VoxelizationSource.CreateMultipleGridsWithGameObjectMesh(gameObject, cubeSide, drawMeshInside);
        }
        else
        {
            VoxelizationSource.AABCGrid thisObjGrid;
            aABCGrids = new Array();
            thisObjGrid = VoxelizationSource.CreateGridWithGameObjectMesh(gameObject, cubeSide, includeChildren, drawMeshInside);
            aABCGrids.Push(thisObjGrid);
        }
    }

    void FixedUpdate()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0, 0, 0.5f);
        //Gizmos.color = Color.red;
        DrawMeshShell();
    }

    void TestTriangleIntersectionAABC()
    {
        Vector3[] triangleA;
        Vector3[] triangleB;
        Vector3[] triangleC;
        FIXME_VAR_TYPE aABCGrid = VoxelizationSource.AABCGrid(1, 1, 1, 1, Vector3(1, 1, 1));
        Vector3[] aABCVertices;

        triangleA = new Vector3[3];
        triangleA[0] = new Vector3(1, 1, 1);
        triangleA[1] = new Vector3(1, 2, 1);
        triangleA[2] = new Vector3(2, 1, 2);

        triangleB = new Vector3[3];
        triangleB[0] = new Vector3(2, 1, 1);
        triangleB[1] = new Vector3(1, 2, 1);
        triangleB[2] = new Vector3(1, 1, 2);

        triangleC = new Vector3[3];
        triangleC[0] = new Vector3(-1, -1, -2);
        triangleC[1] = new Vector3(-1, -1, -2);
        triangleC[2] = new Vector3(-1, -1, -2);

        print("aabc vertices:");
        aABCVertices = aABCGrid.GetAABCCorners(0, 0, 0);
        for (int i = 0; i < 8; ++i)
        {
            print("Vertex " + i + ": " + aABCVertices[i]);
        }

        if (aABCGrid.TriangleIntersectAABC(triangleA, 0, 0, 0))
        {
            print("Triangle A intersect the AABC, Test is OK");
        }
        else
        {
            print("Triangle A doesn't intersect the AABC, Test is NOT OK");
        }
        if (aABCGrid.TriangleIntersectAABC(triangleB, 0, 0, 0))
        {
            print("Triangle B intersect the AABC, Test is OK");
        }
        else
        {
            print("Triangle B doesn't intersect the AABC, Test is NOT OK");
        }
        if (aABCGrid.TriangleIntersectAABC(triangleC, 0, 0, 0))
        {
            print("Triangle C intersect the AABC, Test is NOT OK");
        }
        else
        {
            print("Triangle C doesn't intersect the AABC, Test is OK");
        }
    }

    void DrawMeshShell()
    {
        foreach (VoxelizationSource.AABCGrid aABCGrid in aABCGrids)
        {
            if (drawMeshShell && aABCGrid)
            {
                Vector3 cubeSize = new Vector3(cubeSide, cubeSide, cubeSide);
                FIXME_VAR_TYPE gridSize = aABCGrid.GetSize();
                for (int x = 0; x < gridSize.x; ++x)
                {
                    for (int y = 0; y < gridSize.y; ++y)
                    {
                        for (int z = 0; z < gridSize.z; ++z)
                        {
                            FIXME_VAR_TYPE cubeCenter = aABCGrid.GetAABCCenterFromGridCenter(x, y, z) +
                                    aABCGrid.GetCenter() +
                                        meshShellPositionFromObject;
                            if (aABCGrid.IsAABCSet(x, y, z))
                            {
                                Gizmos.color = new Color(1, 0, 0, 0.5f);
                                Gizmos.DrawCube(cubeCenter, cubeSize);
                            }
                            else if (drawEmptyCube)
                            {
                                Gizmos.color = new Color(0, 1, 0, 1f);
                                Gizmos.DrawCube(cubeCenter, cubeSize);
                            }
                        }
                    }
                }
            }
        }
    }

}*/