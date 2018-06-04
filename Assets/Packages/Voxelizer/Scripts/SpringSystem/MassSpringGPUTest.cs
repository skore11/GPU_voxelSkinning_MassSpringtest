using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;
// Perosnal script to parse vertices from baked mesh to compute shader to render out masses of mass-spring system

namespace VoxelSystem.Demo
{
    public class MassSpringGPUTest: MonoBehaviour
    {
        public GameObject MassPrefab;

        private float MassUnitSize;

        private ArrayList Primitives = new ArrayList();
        private Vector3[] positions;
        protected ComputeBuffer particleBuffer;

        [SerializeField] new protected SkinnedMeshRenderer renderer;
        enum MeshType
        {
            Volume, Surface
        };

        [SerializeField] MeshType type = MeshType.Volume;


        [SerializeField] protected ComputeShader voxelizer;
        [SerializeField] protected int count = 64;

        GPUVoxelData data;
        // Use this for initialization
        void Start()
        {

            var mesh = Sample();
            data = GPUVoxelizer.Voxelize(voxelizer, mesh, count, (type == MeshType.Volume));
            var pointMesh = BuildPoints(data);
            particleBuffer = new ComputeBuffer(pointMesh.vertexCount, Marshal.SizeOf(typeof(VParticle_t)));
            GetComponent<MeshFilter>().sharedMesh = pointMesh;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (data == null) return;

            data.Dispose();

            var mesh = Sample();
            data = GPUVoxelizer.Voxelize(voxelizer, mesh, count, (type == MeshType.Volume));
            int count2 = data.Width * data.Height * data.Depth;
            //Debug.Log(count2);
            //count2 = Primitives.Count;
            print(Primitives.Count);
            //int numPositions = Primitives.Count;
            int numPositions = count2;
            print(numPositions);
            for (int i = 0; i < numPositions; ++i)
                ((GameObject)Primitives[i]).transform.position = TranslateToUnityWorldSpace(positions[i]);
        }

        public void SetMassUnitSize(float length) { MassUnitSize = length; print(length); }

        public void SpawnPrimitives(Vector3[] p)
        {
            foreach (GameObject obj in Primitives)
            {
                //print(obj);
                Destroy(obj.gameObject);
            }

            Primitives.Clear();

            positions = p;
            //print(positions);
            int numPositions = positions.Length;
            Primitives.Clear();
            foreach (Vector3 massPosition in positions)
            {
                // print(massPosition);
                //translate y to z so we can use Unity's in-built gravity on the y axis.
                Vector3 worldPosition = TranslateToUnityWorldSpace(massPosition);
                //print(worldPosition);
                object springUnit = Instantiate(MassPrefab, worldPosition, Quaternion.identity);
                GameObject springMassObject = (GameObject)springUnit;

                springMassObject.transform.localScale = Vector3.one * MassUnitSize;
                Primitives.Add(springUnit);
            }
        }
        Mesh Sample()
        {
            var mesh = new Mesh();
            renderer.BakeMesh(mesh);//requies renderer to bake mesh first
            return mesh;
        }
        void OnDestroy()
        {
            if (particleBuffer != null)
            {
                particleBuffer.Release();
                particleBuffer = null;
            }

            if (data != null)
            {
                data.Dispose();
                data = null;
            }
        }


        Mesh BuildPoints(GPUVoxelData data)
        {
            var count = data.Width * data.Height * data.Depth;
            var mesh = new Mesh();
            mesh.indexFormat = (count > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;//16bit representation for color pallette
            mesh.vertices = new Vector3[count];
            Debug.Log("data count" + count);
            var indices = new int[count];

            for (int i = 0; i < count; i++) indices[i] = i;
            mesh.SetIndices(indices, MeshTopology.Points, 0);//setting an index in 3D volume for all the count points
            mesh.RecalculateBounds();

            return mesh;//required to create new voxelized mesh
                        //return indices;
        }


        public void UpdatePositions(Vector3[] p)
        {
            positions = p;
            //Debug.Log(p);
        }

        //===========================================================================================
        // Helper Functions
        //===========================================================================================

        private Vector3 TranslateToUnityWorldSpace(Vector3 gridPosition)
        {
            return new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
        }
    }
}