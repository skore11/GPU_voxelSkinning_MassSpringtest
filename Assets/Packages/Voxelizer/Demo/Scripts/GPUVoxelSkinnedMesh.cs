using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;
/*TODO:
 * 1. Should implement multiple compute shader kernels and structure data buffers of mass spring system to simulate particles' behaviors.

2. Have to get the vertices' actual world position and normal directions from the skinned mesh.

3. Need to make the particle's start color same or similar to mesh's materiel color.
Procedure:
1. In each frame bake the model space vertices' position and normal data into a mesh copy, and record the skinned mesh model-to-world matrix and pass it into compute shader's emit kernel to calculate the vertices' actual world position and normal direction.

2. Passing the UV coordinates into the compute shader with the original diffuse texture to get the particles' start colors.

3. I've tried using two buffers to separately pass particles' position and color data into the drawing shader however it lead to glitch result as follows. So I wrap the two float3 data into one struct data.

    Note:
    Comment out buildpoints() and Var pointmesh to stop reading vertices data to/from GPUVoxeldata of voxelizer.compute
 
     */
namespace VoxelSystem.Demo
{

    public class GPUVoxelSkinnedMesh : MonoBehaviour {

        enum MeshType {
            Volume, Surface
        };

        [SerializeField] MeshType type = MeshType.Volume;

        [SerializeField] new protected SkinnedMeshRenderer renderer;
        [SerializeField] protected ComputeShader voxelizer, particleUpdate, massSpring;
        [SerializeField] protected int count = 64;

        protected Kernel setupKernel, updateKernel;
        protected ComputeBuffer particleBuffer;

        protected Renderer _renderer;
        protected MaterialPropertyBlock block;

        #region Shader property keys //the keys are required by the GPU voxelizer to build the voxel shell from the data

        protected const string kSetupKernelKey = "Setup", kUpdateKernelKey = "Update";

        protected const string kVoxelBufferKey = "_VoxelBuffer";
        protected const string kParticleBufferKey = "_ParticleBuffer", kParticleCountKey = "_ParticleCount";
        protected const string kUnitLengthKey = "_UnitLength";

        #endregion

        GPUVoxelData data;

        void Start () {
            var mesh = Sample();//bakes out the given mesh
            data = GPUVoxelizer.Voxelize(voxelizer, mesh, count, (type == MeshType.Volume));//takes mesh data and voxelizes it 
            int count2 = data.Width * data.Height * data.Depth;// to get the vertex count from GPUVoxelData
            //var pointMesh = BuildPoints(data);
            // int indices = BuildPoints(data);
            Debug.Log("the o/P of GPUVoxelizer data" + data);
           
            //particleBuffer = new ComputeBuffer(pointMesh.vertexCount, Marshal.SizeOf(typeof(VParticle_t)));
         
            //GetComponent<MeshFilter>().sharedMesh = pointMesh;

            block = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            _renderer.GetPropertyBlock(block);

            setupKernel = new Kernel(particleUpdate, kSetupKernelKey);
            updateKernel = new Kernel(particleUpdate, kUpdateKernelKey);

            Setup(data);
        }
        
        void Update () {
            if (data == null) return;

            data.Dispose();

            var mesh = Sample();
            data = GPUVoxelizer.Voxelize(voxelizer, mesh, count, (type == MeshType.Volume));

            Compute(updateKernel, data, Time.deltaTime);
            block.SetBuffer(kParticleBufferKey, particleBuffer);
            _renderer.SetPropertyBlock(block);
            //Debug.Log(particleBuffer);
        }

        Mesh Sample()
        {
            var mesh = new Mesh();
            renderer.BakeMesh(mesh);//requies renderer to bake mesh first
            return mesh;
        }

        void OnDestroy ()
        {
            if(particleBuffer != null)
            {
                particleBuffer.Release();
                particleBuffer = null;
            }

            if(data != null)
            {
                data.Dispose();
                data = null;
            }
        }

        void Setup(GPUVoxelData data)
        {
            particleUpdate.SetBuffer(setupKernel.Index, kVoxelBufferKey, data.Buffer);
            particleUpdate.SetBuffer(setupKernel.Index, kParticleBufferKey, particleBuffer);
            particleUpdate.SetInt(kParticleCountKey, particleBuffer.count);
            particleUpdate.SetFloat(kUnitLengthKey, data.UnitLength);

            particleUpdate.Dispatch(setupKernel.Index, particleBuffer.count / (int)setupKernel.ThreadX + 1, (int)setupKernel.ThreadY, (int)setupKernel.ThreadZ);
        }

        void Compute (Kernel kernel, GPUVoxelData data, float dt)
        {
            particleUpdate.SetBuffer(kernel.Index, kVoxelBufferKey, data.Buffer);
            particleUpdate.SetFloat(kUnitLengthKey, data.UnitLength);

            particleUpdate.SetBuffer(kernel.Index, kParticleBufferKey, particleBuffer);
            particleUpdate.SetInt(kParticleCountKey, particleBuffer.count);

            particleUpdate.Dispatch(kernel.Index, particleBuffer.count / (int)kernel.ThreadX + 1, (int)kernel.ThreadY, (int)kernel.ThreadZ);
        }

       /* Mesh BuildPoints(GPUVoxelData data)
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
        }*/

    }

}


