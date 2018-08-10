/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Copyright (c) 2012 Vincenzo Giovanni Comito
// See the file license.txt for copying permission.

// Voxelization.js
// Written by Clynamen, 23/08/2012
// Utility for voxelization of a mesh.
// Thanks to Mike Vandelay for the 
// AABB-Triangle SAT implementation in C++.

public class VoxelizationSource : MonoBehaviour
{

    static float MAX_FLOAT = 9999999999999999f;
    static float MIN_FLOAT = -9999999999999999f;

    public class GridSize
    {

        public int x;
        public int y;
        public int z;
        public float side;

        public void GetGridSize(GridSize gridSize)
        {
            this.x = gridSize.x;
            this.y = gridSize.y;
            this.z = gridSize.z;
            this.side = gridSize.side;
        }

        public void GetGridSize(int x, int y, int z, float side)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.side = side;
        }

    }


        public class Voxelization
        {
        public class AABCGrid {
            public float side;
            public short width;
            public short height;
            public short depth;
            public Vector3 origin;
            public bool[,,] cubeSet;
            public short[,,] cubeNormalSum;
            public bool debug = false;
        }
        public class AABCPosition
        {

            public int x;
            public int y;
            public int z;
        
                    public void GetAABCPosition(AABCPosition aABCPosition)
                    {
                        this.x = aABCPosition.x;
                        this.y = aABCPosition.y;
                        this.z = aABCPosition.z;
                    }

                    public void GetAABCPosition(int x, int y, int z)
                    {
                        this.x = x;
                        this.y = y;
                        this.z = z;
                    }

                

            





                public class AABC : AABCPosition
                {



                    private AABCGrid grid;

                    public void GetAABC(AABC aABC)
                    {
                        super(aABC.x, aABC.y, aABC.z);
                        this.grid = aABC.grid;
                    }




                    public void GetAABC(short x, short y, short z, AABCGrid grid)
                    {
                        super(x, y, z);
                        this.x = x;
                        this.y = y;
                        this.z = z;
                        this.grid = grid;
                    }


                    public void GetAABC(AABCPosition position, AABCGrid grid)
                    {
                        super(position.x, position.y, position.z);
                        this.grid = grid;
                    }

                    public Vector3 GetCenter()
                    {
                        return grid.GetAABCCenter(x, y, z);
                    }

                    public AABCGrid GetGrid()
                    {
                        return grid;
                    }

                    public bool IsSet()
                    {
                        return grid.IsAABCSet(x, y, z);
                    }

                    public Vector3[] GetCorners(short x, short y, short z)
                    {
                        return grid.AABCCorners(x, y, z);
                    }



                    public class AABCGridIteratorBase
                    {

                        protected AABCGrid grid;
                        protected AABCPosition position;

                        protected void GetAABCGridIteratorBase(AABCGrid grid)
                        {
                            this.grid = grid;
                        }

                        public bool HasNext;


                        public AABC Next;

                    }

                    public class AABCGridIterator : AABCGridIteratorBase
                    {


                        public void GetAABCGridIterator(AABCGrid grid)
                        {
                            super(grid);
                        }

                        public bool HasNext()
                        {
                            if (position.x == grid.width &&
                               position.y == grid.height &&
                               position.z == grid.depth)
                            {
                                return false;
                            }
                            return true;
                        }

                        public AABC Next()
                        {
                            position.z++;
                            if (position.z == grid.depth)
                            {
                                position.z = 0;
                                position.y++;
                                if (position.y == grid.height)
                                {
                                    position.y = 0;
                                    position.x++;
                                    if (position.x > grid.width)
                                    {
                                        throw new System.IndexOutOfRangeException();
                                    }
                                }
                            }
                            return AABC(position, grid);
                        }

                    }

                    public class AABCGridSetAABCIterator : AABCGridIteratorBase
                    {

                        private bool nextFound;
                        private AABCPosition nextPosition;

                        public void GetAABCGridSetAABCIterator(AABCGrid grid)
                        {
                            super(grid);
                            position = new AABCPosition(0, 0, 0);
                            if (grid.IsAABCSet(position))
                            {
                                nextPosition = position;
                            }
                            nextFound = true;
                        }

                        public bool HasNext()
                        {
                            if (!nextFound)
                            {
                                return FindNext();
                            }
                            return true;
                        }

                        public AABC Next()AABCPosition(position)
                        {
                            if (!nextFound)
                            {
                                FindNext();
                            }
                            position = nextPosition;
                            nextFound = false;
                            return AABC(position, grid);
                        }

                        private bool FindNext()
                        {
                            nextPosition = new AABCPosition(position);
                            nextPosition.z++;
                            for ( nextPosition.x < grid.width; nextPosition.x++)
                            {
                                for ( nextPosition.y < grid.height; nextPosition.y++)
                                {
                                    for ( nextPosition.z < grid.depth; nextPosition.z++)
                                    {
                                        if (grid.IsAABCSet(nextPosition.x, nextPosition.y, nextPosition.z))
                                        {
                                            nextFound = true;
                                            return true;
                                        }
                                    }
                                    nextPosition.z = 0;
                                }
                                nextPosition.y = 0;
                            }
                            nextFound = false;
                            return false;
                        }


                        public void AABCGrid(short x, short y, short z, float sideLength)
                        {
                            width = x;
                            height = y;
                            depth = z;
                            side = sideLength;
                            origin = new Vector3();
                            cubeSet = new bool[width, height, depth];
                        }

                        public void AABCGrid(int x, int y, int z, float sideLength, Vector3 ori)
                        {
                            width = x;
                            height = y;
                            depth = z;

                            side = sideLength;
                            origin = ori;
                            cubeSet = new bool[width, height, depth];
                        }

                        public void CleanGrid()
                        {
                            cubeSet = new bool[width, height, depth];
                        }

                        public void SetDebug(bool debug)
                        {
                            this.debug = debug;
                        }

                        public GridSize GetSize()
                        {
                            return GridSize(width, height, depth, side);
                        }

                        public void SetSize(GridSize dimension)
                        {
                            SetSize(dimension.x, dimension.y, dimension.z, dimension.side);
                        }

                        public void SetSize(short x, short y, short z, float sideLength)
                        {
                            width = x;
                            height = y;
                            depth = z;
                            side = sideLength;
                            CleanGrid();
                        }

                        public Vector3 GetCenter()
                        {
                            return origin + Vector3(width / 2 * side, height / 2 * side, depth / 2 * side);
                        }

                        public void SetCenter(Vector3 pos)
                        {
                            origin = pos - Vector3(width / 2 * side, height / 2 * side, depth / 2 * side);
                        }

                        public int GetSetAABCCount()
                        {
                            int count = 0;
                            for (int x = 0; x < width; ++x)
                            {
                                for (int y = 0; y < height; ++y)
                                {
                                    for (int z = 0; z < depth; ++z)
                                    {
                                        if (!IsAABCSet(x, y, z))
                                        {
                                            count++;
                                        }
                                    }
                                }
                            }
                            return count;
                        }

                        public Vector3[] GetAABCCorners(AABCPosition pos)
                        {
                            CheckBounds(pos.x, pos.y, pos.z);
                            return GetAABCCornersUnchecked(pos.x, pos.y, pos.z);
                        }

                        public Vector3[] GetAABCCorners(short x, short y, short z)
                        {
                            return GetAABCCornersUnchecked(x, y, z);
                        }

                        protected Vector3[] GetAABCCornersUnchecked(short x, short y, short z)
                        {
                            Vector3 center = new Vector3(x * side + side / 2, y * side + side / 2, z * side + side / 2);
                            Vector3[] corners = new Vector3[8];
                            corners[0] = Vector3(center.x + side, center.y - side, center.z + side) + origin;
                            corners[1] = Vector3(center.x + side, center.y - side, center.z - side) + origin;
                            corners[2] = Vector3(center.x - side, center.y - side, center.z - side) + origin;
                            corners[3] = Vector3(center.x - side, center.y - side, center.z + side) + origin;
                            corners[4] = Vector3(center.x + side, center.y + side, center.z + side) + origin;
                            corners[5] = Vector3(center.x + side, center.y + side, center.z - side) + origin;
                            corners[6] = Vector3(center.x - side, center.y + side, center.z - side) + origin;
                            corners[7] = Vector3(center.x - side, center.y + side, center.z + side) + origin;
                            return corners;
                        }

                        public Vector3 GetAABCCenter(AABCPosition pos)
                        {
                            CheckBounds(pos.x, pos.y, pos.z);
                            return GetAABCCenterUnchecked(pos.x, pos.y, pos.z);
                        }

                        public Vector3 GetAABCCenter(short x, short y, short z)
                        {
                            CheckBounds(x, y, z);
                            return GetAABCCenterUnchecked(x, y, z);
                        }

                        protected Vector3 GetAABCCenterUnchecked(short x, short y, short z)
                        {
                            return GetAABCCenterFromGridCenterUnchecked(x, y, z) + GetCenter();
                        }

                        public Vector3 GetAABCCenterFromGridOrigin(AABCPosition pos)
                        {
                            CheckBounds(pos.x, pos.y, pos.z);
                            return GetAABCCenterFromGridOriginUnchecked(pos.x, pos.y, pos.z);
                        }

                        public Vector3 GetAABCCenterFromGridOrigin(short x, short y, short z)
                        {
                            CheckBounds(x, y, z);
                            return GetAABCCenterFromGridOriginUnchecked(x, y, z);
                        }

                        protected Vector3 GetAABCCenterFromGridOriginUnchecked(short x, short y, short z)
                        {
                            return GetAABCCenterFromGridOriginUnchecked(x * side + side / 2, y * side + side / 2, z * side + side / 2);
                        }

                        public Vector3 GetAABCCenterFromGridCenter(AABCPosition pos)
                        {
                            CheckBounds(pos.x, pos.y, pos.z);
                            return GetAABCCenterFromGridCenterUnchecked(pos.x, pos.y, pos.z);
                        }

                        public Vector3 GetAABCCenterFromGridCenter(short x, short y, short z)
                        {
                            CheckBounds(x, y, z);
                            return GetAABCCenterFromGridCenterUnchecked(x, y, z);
                        }

                        protected Vector3 GetAABCCenterFromGridCenterUnchecked(short x, short y, short z)
                        {
                            return Vector3(side * (x + 1 / 2 - width / 2),
                                            side * (y + 1 / 2 - height / 2),
                                            side * (z + 1 / 2 - depth / 2));
                        }

                        public bool IsAABCSet(AABCPosition pos)
                        {
                            CheckBounds(pos.x, pos.y, pos.z);
                            return IsAABCSetUnchecked(pos.x, pos.y, pos.z);
                        }

                        public bool IsAABCSet(short x, short y, short z)
                        {
                            CheckBounds(x, y, z);
                            return IsAABCSetUnchecked(x, y, z);
                        }

                        protected bool IsAABCSetUnchecked(short x, short y, short z)
                        {
                            return cubeSet[x, y, z];
                        }

                        public bool TriangleIntersectAABC(Vector3[] triangle, AABCPosition pos)
                        {
                            CheckBounds(pos.x, pos.y, pos.z);
                            return TriangleIntersectAABCUnchecked(triangle, pos.x, pos.y, pos.z);
                        }

                        public bool TriangleIntersectAABC(Vector3[] triangle, short x, short y, short z)
                        {
                            CheckBounds(x, y, z);
                            return TriangleIntersectAABCUnchecked(triangle, x, y, z);
                        }

                        protected bool TriangleIntersectAABCUnchecked(Vector3[] triangle, short x, short y, short z)
                        {
                            Vector3[] aabcCorners;
                            Vector3 triangleEdgeA;
                            Vector3 triangleEdgeB;
                            Vector3 triangleEdgeC;
                            Vector3 triangleNormal;
                            Vector3 aabcEdgeA = new Vector3(1, 0, 0);
                            Vector3 aabcEdgeB = new Vector3(0, 1, 0);
                            Vector3 aabcEdgeC = new Vector3(0, 0, 1);

                            aabcCorners = GetAABCCornersUnchecked(x, y, z);

                            triangleEdgeA = triangle[1] - triangle[0];
                            triangleEdgeB = triangle[2] - triangle[1];
                            triangleEdgeC = triangle[0] - triangle[2];

                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeA, aabcEdgeA))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeA, aabcEdgeB))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeA, aabcEdgeC))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeB, aabcEdgeA))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeB, aabcEdgeB))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeB, aabcEdgeC))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeC, aabcEdgeA))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeC, aabcEdgeB))) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeC, aabcEdgeC))) return false;

                            triangleNormal = Vector3.Cross(triangleEdgeA, triangleEdgeB);
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, triangleNormal)) return false;

                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, aabcEdgeA)) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, aabcEdgeB)) return false;
                            if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, aabcEdgeC)) return false;

                            return true;
                        }

                        protected void CheckBounds(short x, short y, short z)
                        {
                            if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth)
                            {
                                throw new System.ArgumentOutOfRangeException("The requested AABC is out of the grid limits.");
                            }
                        }

                        public void FillGridWithGameObjectMeshShell(GameObject gameObj)
                        {
                            FillGridWithGameObjectMeshShell(gameObj, false);
                        }

                        public void FillGridWithGameObjectMeshShell(GameObject gameObj, bool storeNormalSum)
                        {
                            Mesh gameObjMesh = gameObj.GetComponent<MeshFilter>().mesh;
                            Transform gameObjTransf = gameObj.transform;
                            Vector3[] triangle = new Vector3[3];
                            float startTime = Time.realtimeSinceStartup;
                            Vector3[] meshVertices = gameObjMesh.vertices;
                            int[] meshTriangles = gameObjMesh.triangles;
                            int meshTrianglesCount = (meshTriangles.Length) / 3;
                            short x;
                            short y;
                            short z;
                            int ignoreNormalRange = 0;
                            // In this method we can also evaluate stores the normals of the triangles 
                            // that intersect the cube.
                            if (storeNormalSum)
                            {
                                cubeNormalSum = new short[width, height, depth];
                            }
                            if (debug)
                            {
                                Debug.Log("Start:");
                                Debug.Log("Time: " + startTime);
                                Debug.Log("		Mesh Description: ");
                                Debug.Log("Name: " + gameObjMesh.name);
                                Debug.Log("Triangles: " + meshTrianglesCount);
                                Debug.Log("Local AABB size: " + gameObjMesh.bounds.size);
                                Debug.Log("		AABCGrid Description:");
                                Debug.Log("Size: " + width + ',' + height + ',' + depth);
                            }

                            // For each triangle, perform SAT intersection check with the AABCs within the triangle AABB.
                            for (int i = 0; i < meshTrianglesCount; ++i)
                            {
                                triangle[0] = gameObjTransf.TransformPoint(meshVertices[meshTriangles[i * 3]]);
                                triangle[1] = gameObjTransf.TransformPoint(meshVertices[meshTriangles[i * 3 + 1]]);
                                triangle[2] = gameObjTransf.TransformPoint(meshVertices[meshTriangles[i * 3 + 2]]);
                                // Find the triangle AABB, select a sub grid.
                                int startX = Mathf.Floor((Mathf.Min(((triangle[0].x, triangle[1].x, triangle[2].x)) - origin.x) / side));
                                int startY = Mathf.Floor((Mathf.Min(([triangle[0].y, triangle[1].y, triangle[2].y]) - origin.y) / side));
                                int startZ = Mathf.Floor((Mathf.Min(([triangle[0].z, triangle[1].z, triangle[2].z]) - origin.z) / side));
                                int endX = Mathf.Ceil((Mathf.Max(([triangle[0].x, triangle[1].x, triangle[2].x]) - origin.x) / side));
                                int endY = Mathf.Ceil((Mathf.Max(([triangle[0].y, triangle[1].y, triangle[2].y]) - origin.y) / side));
                                int endZ = Mathf.Ceil((Mathf.Max(([triangle[0].z, triangle[1].z, triangle[2].z]) - origin.z) / side));
                                if (storeNormalSum)
                                {
                                    for (x = startX; x <= endX; ++x)
                                    {
                                        for (y = startY; y <= endY; ++y)
                                        {
                                            for (z = startZ; z <= endZ; ++z)
                                            {
                                                if (TriangleIntersectAABC(triangle, x, y, z))
                                                {
                                                    Vector3 triangleNormal = GetTriangleNormal(triangle);
                                                    cubeSet[x, y, z] = true;
                                                    if (triangleNormal.z < 0 - ignoreNormalRange)
                                                    {
                                                        cubeNormalSum[x, y, z]++;
                                                    }
                                                    else if (triangleNormal.z > 0 + ignoreNormalRange)
                                                    {
                                                        cubeNormalSum[x, y, z]--;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (x = startX; x < endX; ++x)
                                    {
                                        for (y = startY; y < endY; ++y)
                                        {
                                            for (z = startZ; z < endZ; ++z)
                                            {
                                                if (!IsAABCSet(x, y, z) && TriangleIntersectAABC(triangle, x, y, z))
                                                {
                                                    cubeSet[x, y, z] = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (debug) {
                                Debug.Log("Grid Evaluation Ended!");
                                Debug.Log("Time spent: " + (Time.realtimeSinceStartup - startTime) + "s");
                                Debug.Log("End: ");
                            }
                        }

                        public void FillGridWithGameObjectMesh(GameObject gameObj)
                        {
                            FillGridWithGameObjectMeshShell(gameObj, true);

                            for (int x = 0; x < width; ++x)
                            {
                                for (int y = 0; y < height; ++y)
                                {
                                    bool fill = false;
                                    int cubeToFill = 0;
                                    for (int z = 0; z < depth; ++z)
                                    {
                                        if (cubeSet[x, y, z])
                                        {
                                            int[] normalSum = cubeNormalSum[x, y, z];
                                            if (normalSum)
                                            {
                                                if (normalSum > 0)
                                                {
                                                    fill = true;
                                                }
                                                else
                                                {
                                                    fill = false;
                                                    while (cubeToFill > 1)
                                                    {
                                                        cubeToFill--;
                                                        cubeSet[x, y, z - cubeToFill] = true;
                                                    }
                                                }
                                                cubeToFill = 0;
                                            }
                                            continue;
                                        }
                                        if (fill)
                                        {
                                            cubeToFill++;
                                        }
                                    }
                                }
                            }
                            cubeNormalSum = null;
                        }

                        public ParticleSystem.Particle[] AddParticles(ParticleSystem.Particle[] particles, int particlesToAdd)
                        {
                            int settedAABCCount = GetSetAABCCount();
                            int particlesPerAABC;
                            float randMax = side / 2;
                            int addedParticles = 0;
                            AABC cube;
                            int i = 0;

                            if (particlesToAdd <= 0)
                            {
                                throw new System.ArgumentException("The number of particles to add is < 0");
                            }

                            particlesPerAABC = particlesToAdd / settedAABCCount;
                            if (particlesPerAABC <= 0)
                            {
                                particlesPerAABC = 1;
                            }

                            while (particlesToAdd > 0)
                            {
                                int iter = AABCGridSetAABCIterator(this);
                                int cubeFilledCount = 0;
                                while (iter.HasNext())
                                {
                                    cube = iter.Next();
                                    for (; i < addedParticles + particlesPerAABC && particlesToAdd > 0; ++i)
                                    {
                                        particles[i].position = cube.GetCenter() +
                                                        Vector3(Random.Range(-randMax, randMax),
                                                                Random.Range(-randMax, randMax),
                                                                Random.Range(-randMax, randMax)) / 100;
                                        particlesToAdd--;
                                    }
                                    addedParticles += particlesPerAABC;
                                    cubeFilledCount++;
                                    if (particlesToAdd <= 0)
                                    {
                                        break;
                                    }
                                }
                                if (particlesToAdd > 0)
                                {
                                    particlesPerAABC = 1;
                                }
                            }

                            return particles;
                        }
                        public bool ProjectionsIntersectOnAxis(Vector3[] solidA, Vector3[] solidB, Vector3 axis)
                        {
                            float minSolidA = MinOfProjectionOnAxis(solidA, axis);
                            float maxSolidA = MaxOfProjectionOnAxis(solidA, axis);
                            float minSolidB = MinOfProjectionOnAxis(solidB, axis);
                            float maxSolidB = MaxOfProjectionOnAxis(solidB, axis);

                            if (minSolidA > maxSolidB)
                            {
                                return false;
                            }
                            if (maxSolidA < minSolidB)
                            {
                                return false;
                            }

                            return true;
                        }
                        public float MinOfProjectionOnAxis(Vector3[] solid, Vector3 axis)
                        {
                            float min = MAX_FLOAT;
                            float dotProd;

                            for (int i = 0; i < solid.Length; ++i)
                            {
                                dotProd = Vector3.Dot(solid[i], axis);
                                if (dotProd < min)
                                {
                                    min = dotProd;
                                }
                            }
                            return min;
                        }

                        public float MaxOfProjectionOnAxis(Vector3[] solid, Vector3 axis)
                        {
                            float max = MIN_FLOAT;
                            float dotProd;

                            for (int i = 0; i < solid.Length; ++i)
                            {
                                dotProd = Vector3.Dot(solid[i], axis);
                                if (dotProd > max)
                                {
                                    max = dotProd;
                                }
                            }
                            return max;
                        }

                        // Return a vector with the minimum components
                        public Vector3 MinVectorComponents(Vector3 a, Vector3 b)
                        {
                            Vector3 ret = new Vector3();
                            ret.x = Mathf.Min(a.x, b.x);
                            ret.y = Mathf.Min(a.y, b.y);
                            ret.z = Mathf.Min(a.z, b.z);
                            return ret;
                        }

                        // Return a vector with the minimum components
                        public Vector3 MaxVectorComponents(Vector3 a, Vector3 b)
                        {
                            Vector3 ret = new Vector3();
                            ret.x = Mathf.Max(a.x, b.x);
                            ret.y = Mathf.Max(a.y, b.y);
                            ret.z = Mathf.Max(a.z, b.z);
                            return ret;
                        }

                        public Vector3 GetTriangleNormal(Vector3[] triangle)
                        {
                            return Vector3.Cross(triangle[1] - triangle[0], triangle[2] - triangle[0]).normalized;
                        }

                        // Return an AABB which include the meshes of the object itself and of its children
                        public Bounds GetTotalBoundsOfGameObject(GameObject gameObj)
                        {
                            Bounds totalBounds = new Bounds();
                            Vector3 min;
                            Vector3 max;
                            if (gameObj.GetComponent<Renderer>())
                            {
                                min = gameObj.GetComponent<Renderer>().bounds.min;
                                max = gameObj.GetComponent<Renderer>().bounds.max;
                            }

                            for (int i = 0; i < gameObj.transform.childCount; ++i)
                            {
                                GameObject childObj = gameObj.transform.GetChild(i).gameObject;
                                Bounds childTotalBounds = GetTotalBoundsOfGameObject(childObj);
                                min = gameObj.GetComponent<Renderer>().bounds.min;
                                max = gameObj.GetComponent<Renderer>().bounds.max;
                                min = MinVectorComponents(min, childTotalBounds.min);
                                max = MaxVectorComponents(max, childTotalBounds.max);
                                totalBounds.SetMinMax(min, max);
                            }


                            return totalBounds;
                        }



                        public List<GameObject> GetChildrenWithMesh(GameObject gameObj)
                        {
                            //FIXME_VAR_TYPE ret = new Array();
                            //int[] ret;

                            List<GameObject> ret = new List<GameObject>();
                            for (int i = 0; i < gameObj.transform.childCount; ++i)
                            {
                                GameObject childObj = gameObj.transform.GetChild(i).gameObject;
                                if (childObj.GetComponent<Renderer>())
                                {
                                    ret.Add(childObj);

                                }
                                //ret = ret.Add(GetChildrenWithMesh(childObj));
                            }
                            return ret;
                        }

                        // Warning: this method creates a grid at least as big as the total bounding box of the
                        // game object, if children are included there may be empty space. Consider to use 
                        // CreateMultipleGridsWithGameObjectMeshShell in order to save memory.
                        public AABCGrid CreateGridWithGameObjectMesh(GameObject gameObj,
                                                                  float cubeSide, bool includeChildren, bool includeInside)
                        {
                            AABCGrid aABCGrid;
                            short width;
                            short height;
                            short depth;
                            Vector3 origin = new Vector3();
                            List<GameObject> gameObjectsWithMesh;
                            Vector3 gridBoundsMin = new Vector3(MAX_FLOAT, MAX_FLOAT, MAX_FLOAT);
                            Vector3 gridBoundsMax = new Vector3(MIN_FLOAT, MIN_FLOAT, MIN_FLOAT);

                            if (includeChildren)
                            {
                                gameObjectsWithMesh = GetChildrenWithMesh(gameObj);
                            }
                            else
                            {
                                gameObjectsWithMesh = new List<GameObject>();
                            }
                            if (gameObj.GetComponent<Renderer>())
                            {
                                gameObjectsWithMesh.Add(gameObj);
                            }


                            foreach (GameObject gameObjectWithMesh in gameObjectsWithMesh)
                            {
                                gridBoundsMin = Voxelization.MinVectorComponents(gridBoundsMin,
                                                            gameObjectWithMesh.GetComponent<Renderer>().bounds.min);
                                gridBoundsMax = Voxelization.MaxVectorComponents(gridBoundsMax,
                                                            gameObjectWithMesh.GetComponent<Renderer>().bounds.max);
                            }
                            width = Mathf.Ceil((gridBoundsMax.x - gridBoundsMin.x) / cubeSide) + 2;
                            height = Mathf.Ceil((gridBoundsMax.y - gridBoundsMin.y) / cubeSide) + 2;
                            depth = Mathf.Ceil((gridBoundsMax.z - gridBoundsMin.z) / cubeSide) + 2;
                            origin = gridBoundsMin - Vector3(cubeSide, cubeSide, cubeSide);
                            aABCGrid = new AABCGrid(width, height, depth, cubeSide, origin);
                            foreach (GameObject gameObjectWithMesh in gameObjectsWithMesh)
                            {
                                if (includeInside)
                                {
                                    aABCGrid.FillGridWithGameObjectMesh(gameObjectWithMesh);
                                }
                                else
                                {
                                    aABCGrid.FillGridWithGameObjectMeshShell(gameObjectWithMesh);
                                }
                            }

                            return aABCGrid;
                        }

                        public List<GameObject> CreateMultipleGridsWithGameObjectMesh(GameObject gameObj,
                                                          float cubeSide, bool includeMeshInside)
                        {

                            List<GameObject> gameObjectsWithMesh = new List<GameObject>();
                            // FIXME_VAR_TYPE grids = new Array();
                            //int[] grids;
                            List<GameObject> grids = new List<GameObject>();

                            gameObjectsWithMesh = GetChildrenWithMesh(gameObj);
                            if (gameObj.GetComponent<Renderer>())
                            {
                                gameObjectsWithMesh.Add(gameObj);
                            }

                            foreach (GameObject gameObjWithMesh in gameObjectsWithMesh)
                            {
                                grids.Add(CreateGridWithGameObjectMesh(gameObjWithMesh, cubeSide, false, includeMeshInside));
                            }

                            return grids;
                        }
                    }
                }
            }
        }
    }

*/
    











