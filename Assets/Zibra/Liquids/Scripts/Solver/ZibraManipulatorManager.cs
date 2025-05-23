﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using com.zibra.liquid.Solver;
using com.zibra.liquid.SDFObjects;
using com.zibra.liquid.Utilities;

namespace com.zibra.liquid.Manipulators
{
    internal class ZibraManipulatorManager : MonoBehaviour
    {
        [HideInInspector]
        [StructLayout(LayoutKind.Sequential)]
        internal struct ManipulatorParam
        {
            public Int32 Enabled;
            public Int32 SDFObjectID;
            public Int32 ParticleSpecies;
            public Int32 IntParameter;

            public Vector4 AdditionalData0;
            public Vector4 AdditionalData1;
        }

        [HideInInspector]
        [StructLayout(LayoutKind.Sequential)]
        internal struct SDFObjectParams
        {
            public Vector3 Position;
            public Single NormalSmooth;

            public Vector3 Velocity;
            public Single SurfaceValue;

            public Vector3 Scale;
            public Single DistanceScale;

            public Vector3 AnguralVelocity;
            public Int32 Type;

            public Quaternion Rotation;

            public Vector3 BBoxSize;
            public Single BBoxVolume;

            public Int32 EmbeddingTextureBlocks;
            public Int32 SDFTextureBlocks;
            public Int32 ObjectID;
            public Single TotalGroupVolume;

            public Vector3 UnusedPadding;
            public Int32 ManipulatorID;
        };

        [HideInInspector]
        [StructLayout(LayoutKind.Sequential)]
        internal struct ManipulatorIndices
        {
            public Int32 EmitterIndexBegin;
            public Int32 EmitterIndexEnd;
            public Int32 VoidIndexBegin;
            public Int32 VoidIndexEnd;

            public Int32 ForceFieldIndexBegin;
            public Int32 ForceFieldIndexEnd;
            public Int32 AnalyticColliderIndexBegin;
            public Int32 AnalyticColliderIndexEnd;

            public Int32 NeuralColliderIndexBegin;
            public Int32 NeuralColliderIndexEnd;
            public Int32 GroupColliderIndexBegin;
            public Int32 GroupColliderIndexEnd;

            public Int32 DetectorIndexBegin;
            public Int32 DetectorIndexEnd;
            public Int32 SpeciesModifierIndexBegin;
            public Int32 SpeciesModifierIndexEnd;

            public Int32 PortalIndexBegin;
            public Int32 PortalIndexEnd;
            public Vector2Int IndexPadding;
        }

        private int[] TypeIndex = new int[(int)Manipulator.ManipulatorType.TypeNum + 1];

        public ManipulatorIndices Indices = new ManipulatorIndices();

        // All data together
        [HideInInspector]
        public int Elements = 0;
        [HideInInspector]
        public List<ManipulatorParam> ManipulatorParams = new List<ManipulatorParam>();
        [HideInInspector]
        public List<SDFObjectParams> SDFObjectList = new List<SDFObjectParams>();
        [HideInInspector]
        public Color32[] Embeddings;
        [HideInInspector]
        public byte[] SDFGrid;
        [HideInInspector]
        public List<int> ConstDataID = new List<int>();

        [HideInInspector]
        public int TextureCount = 0;
        [HideInInspector]
        public int SDFTextureSize = 0;
        [HideInInspector]
        public int EmbeddingTextureSize = 0;

        [HideInInspector]
        public int SDFTextureBlocks = 0;
        [HideInInspector]
        public int EmbeddingTextureBlocks = 0;

        [HideInInspector]
        public int SDFTextureDimension = 0;
        [HideInInspector]
        public int EmbeddingTextureDimension = 0;

#if ZIBRA_LIQUID_PAID_VERSION
        [HideInInspector]
        public Dictionary<ZibraHash128, NeuralSDF> NeuralSDFs = new Dictionary<ZibraHash128, NeuralSDF>();
        [HideInInspector]
        public Dictionary<ZibraHash128, int> TextureHashMap = new Dictionary<ZibraHash128, int>();
#endif

        private List<Manipulator> Manipulators;

        private Vector3 Abs(Vector3 x)
        {
            return new Vector3(Mathf.Abs(x.x), Mathf.Abs(x.y), Mathf.Abs(x.z));
        }

        protected SDFObjectParams GetSDF(SDFObject obj, Manipulator manipulator)
        {
            SDFObjectParams sdf = new SDFObjectParams();

            if (obj == null)
            {
                throw new Exception("Missing SDF on manipulator");
            }

            sdf.Rotation = manipulator.transform.rotation;
            sdf.Scale = manipulator.transform.lossyScale;
            sdf.Position = manipulator.transform.position;
            sdf.BBoxSize = 2.0f * sdf.Scale;

            sdf.NormalSmooth = 0.01f;
            sdf.Velocity = Vector3.zero;
            sdf.SurfaceValue = 0.0f;
            SDFObject main = manipulator.GetComponent<SDFObject>();
            if (main != null)
            {
                sdf.SurfaceValue += main.SurfaceDistance;
            }
            sdf.SurfaceValue += obj.SurfaceDistance;
            sdf.DistanceScale = 1.0f;
            sdf.AnguralVelocity = Vector3.zero;
            sdf.Type = 0;
            sdf.TotalGroupVolume = 0.0f;
            sdf.BBoxSize = 0.5f * manipulator.transform.lossyScale;

#if ZIBRA_LIQUID_PAID_VERSION
            if (manipulator is ZibraLiquidEmitter || manipulator is ZibraLiquidVoid)
#else
            if (manipulator is ZibraLiquidEmitter)
#endif
            {
                // use Box as default
                sdf.Type = 1;
            }

            if (obj is AnalyticSDF)
            {
                AnalyticSDF analyticSDF = obj as AnalyticSDF;
                sdf.Type = (int)analyticSDF.ChosenSDFType;
                sdf.DistanceScale = analyticSDF.InvertSDF ? -1.0f : 1.0f;
                sdf.BBoxSize = analyticSDF.GetBBoxSize();
            }

#if ZIBRA_LIQUID_PAID_VERSION
            if (obj is NeuralSDF)
            {
                NeuralSDF neuralSDF = obj as NeuralSDF;
                Matrix4x4 transf = obj.transform.localToWorldMatrix * neuralSDF.ObjectRepresentation.ObjectTransform;

                sdf.Rotation = transf.rotation;
                sdf.Scale = Abs(transf.lossyScale);
                sdf.Position = transf.MultiplyPoint(Vector3.zero);
                sdf.Type = -1;
                sdf.ObjectID = TextureHashMap[neuralSDF.ObjectRepresentation.GetHash()];
                sdf.EmbeddingTextureBlocks = EmbeddingTextureBlocks;
                sdf.SDFTextureBlocks = SDFTextureBlocks;
                sdf.DistanceScale = neuralSDF.InvertSDF ? -1.0f : 1.0f;
                sdf.BBoxSize = sdf.Scale;
            }
#endif

            sdf.BBoxVolume = sdf.BBoxSize.x * sdf.BBoxSize.y * sdf.BBoxSize.z;
            return sdf;
        }

#if ZIBRA_LIQUID_PAID_VERSION
        protected void AddTexture(NeuralSDF neuralSDF)
        {
            ZibraHash128 curHash = neuralSDF.ObjectRepresentation.GetHash();

            if (TextureHashMap.ContainsKey(curHash))
                return;

            SDFTextureSize +=
                neuralSDF.ObjectRepresentation.GridResolution / NeuralSDFRepresentation.BLOCK_SDF_APPROX_DIMENSION;
            EmbeddingTextureSize += NeuralSDFRepresentation.EMBEDDING_SIZE *
                                    neuralSDF.ObjectRepresentation.EmbeddingResolution /
                                    NeuralSDFRepresentation.BLOCK_EMBEDDING_GRID_DIMENSION;
            NeuralSDFs[curHash] = neuralSDF;

            int sdfID = TextureCount;
            TextureHashMap[curHash] = sdfID;

            TextureCount++;
        }

        protected void AddTextureData(NeuralSDF neuralSDF)
        {
            ZibraHash128 curHash = neuralSDF.ObjectRepresentation.GetHash();
            int sdfID = TextureHashMap[curHash];

            // Embedding texture
            for (int t = 0; t < NeuralSDFRepresentation.EMBEDDING_SIZE; t++)
            {
                int block = sdfID * NeuralSDFRepresentation.EMBEDDING_SIZE + t;
                Vector3Int blockPos = NeuralSDFRepresentation.BLOCK_EMBEDDING_GRID_DIMENSION *
                                      new Vector3Int(block % EmbeddingTextureBlocks,
                                                     (block / EmbeddingTextureBlocks) % EmbeddingTextureBlocks,
                                                     block / (EmbeddingTextureBlocks * EmbeddingTextureBlocks));
                int Size = neuralSDF.ObjectRepresentation.EmbeddingResolution;

                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        for (int k = 0; k < Size; k++)
                        {
                            Vector3Int pos = blockPos + new Vector3Int(i, j, k);
                            int id = pos.x + EmbeddingTextureDimension * (pos.y + EmbeddingTextureDimension * pos.z);
                            if (id >= EmbeddingTextureSize)
                            {
                                Debug.LogError(pos);
                            }
                            Embeddings[id] = neuralSDF.ObjectRepresentation.GetEmbedding(i, j, k, t);
                        }
                    }
                }
            }

            // SDF approximation texture
            {
                int block = sdfID;
                Vector3Int blockPos =
                    NeuralSDFRepresentation.BLOCK_SDF_APPROX_DIMENSION *
                    new Vector3Int(block % SDFTextureBlocks, (block / SDFTextureBlocks) % SDFTextureBlocks,
                                   block / (SDFTextureBlocks * SDFTextureBlocks));
                int Size = neuralSDF.ObjectRepresentation.GridResolution;
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        for (int k = 0; k < Size; k++)
                        {
                            Vector3Int pos = blockPos + new Vector3Int(i, j, k);
                            int id = pos.x + SDFTextureDimension * (pos.y + SDFTextureDimension * pos.z);
                            for (int t = 0; t < 2; t++)
                                SDFGrid[2 * id + t] = neuralSDF.ObjectRepresentation.GetSDGrid(i, j, k, t);
                        }
                    }
                }
            }
        }

        protected void CalculateTextureData()
        {
            SDFTextureBlocks = (int)Mathf.Ceil(Mathf.Pow(SDFTextureSize, (1.0f / 3.0f)));
            EmbeddingTextureBlocks = (int)Mathf.Ceil(Mathf.Pow(EmbeddingTextureSize, (1.0f / 3.0f)));

            SDFTextureDimension = NeuralSDFRepresentation.BLOCK_SDF_APPROX_DIMENSION * SDFTextureBlocks;
            EmbeddingTextureDimension = NeuralSDFRepresentation.BLOCK_EMBEDDING_GRID_DIMENSION * EmbeddingTextureBlocks;

            SDFTextureSize = SDFTextureDimension * SDFTextureDimension * SDFTextureDimension;
            EmbeddingTextureSize = EmbeddingTextureDimension * EmbeddingTextureDimension * EmbeddingTextureDimension;

            Array.Resize<Color32>(ref Embeddings, EmbeddingTextureSize);
            Array.Resize<byte>(ref SDFGrid, 2 * SDFTextureSize);

            foreach (var sdf in NeuralSDFs.Values)
            {
                AddTextureData(sdf);
            }
        }

#endif

        /// <summary>
        /// Update all arrays and lists with manipulator object data
        /// Should be executed every simulation frame
        /// </summary>
        ///
        public void UpdateDynamic(ZibraLiquid parent, float deltaTime = 0.0f)
        {
            int ID = 0;
            ManipulatorParams.Clear();
            SDFObjectList.Clear();
            // fill arrays

            foreach (var manipulator in Manipulators)
            {
                if (manipulator == null)
                    continue;

                ManipulatorParam manip = new ManipulatorParam();

                manip.Enabled = (manipulator.isActiveAndEnabled && manipulator.gameObject.activeInHierarchy) ? 1 : 0;
                manip.AdditionalData0 = manipulator.AdditionalData0;
                manip.AdditionalData1 = manipulator.AdditionalData1;

                SDFObjectParams sdf = GetSDF(manipulator.GetComponent<SDFObject>(), manipulator);

#if ZIBRA_LIQUID_PRO_VERSION
                if (manipulator.GetComponent<SkinnedMeshSDF>() != null)
                {
                    float TotalVolume = 0.0f;
                    Vector3 averageScale = Vector3.zero;
                    Vector3 averagePosition = Vector3.zero;
                    SkinnedMeshSDF skinnedMeshSDF = manipulator.GetComponent<SkinnedMeshSDF>();

                    sdf.Type = -2;
                    sdf.ObjectID = SDFObjectList.Count;
                    sdf.SDFTextureBlocks = skinnedMeshSDF.BoneSDFList.Count;

                    foreach (var bone in skinnedMeshSDF.BoneSDFList)
                    {
                        SDFObjectParams boneSDF = GetSDF(bone, manipulator);
                        TotalVolume += boneSDF.BBoxVolume;
                        averageScale += boneSDF.Scale;
                        averagePosition += boneSDF.Position;
                        boneSDF.ManipulatorID = ID;
                        SDFObjectList.Add(boneSDF);
                    }

                    sdf.Position = averagePosition / skinnedMeshSDF.BoneSDFList.Count;
                    sdf.Scale = averageScale / skinnedMeshSDF.BoneSDFList.Count;
                    sdf.TotalGroupVolume = TotalVolume;
                }

                if (manipulator is ZibraLiquidEmitter)
                    manipulator.CurrentInteractionMode = Manipulator.InteractionMode.OnlySelectedParticleSpecies;

                switch (manipulator.CurrentInteractionMode)
                {
                case Manipulator.InteractionMode.AllParticleSpecies:
                    manip.ParticleSpecies = 0;
                    break;
                case Manipulator.InteractionMode.OnlySelectedParticleSpecies:
                    manip.ParticleSpecies = 1 + manipulator.ParticleSpecies;
                    break;
                case Manipulator.InteractionMode.ExceptSelectedParticleSpecies:
                    manip.ParticleSpecies = -(1 + manipulator.ParticleSpecies);
                    break;
                }
#endif

                if (manipulator is ZibraLiquidEmitter)
                {
                    ZibraLiquidEmitter emitter = manipulator as ZibraLiquidEmitter;

                    float particlesPerSec = emitter.VolumePerSimTime / parent.NodeSize / parent.NodeSize /
                                            parent.NodeSize * parent.SolverParameters.ParticleDensity *
                                            parent.SimulationTimeScale;

                    manip.AdditionalData0.x = Mathf.Floor(particlesPerSec * deltaTime);

#if ZIBRA_LIQUID_PRO_VERSION
                    manip.ParticleSpecies = manipulator.ParticleSpecies;
#endif
                }

                manip.SDFObjectID = SDFObjectList.Count;
                sdf.ManipulatorID = ID;
                SDFObjectList.Add(sdf);
                ManipulatorParams.Add(manip);
                ID++;
            }

            Elements = Manipulators.Count;
        }

        private static float INT2Float(int a)
        {
            const float MAX_INT = 2147483647.0f;
            const float F2I_MAX_VALUE = 5000.0f;
            const float F2I_SCALE = (MAX_INT / F2I_MAX_VALUE);

            return a / F2I_SCALE;
        }

        private int GetStatIndex(int id, int offset)
        {
            return id * Solver.ZibraLiquid.STATISTICS_PER_MANIPULATOR + offset;
        }

#if ZIBRA_LIQUID_PAID_VERSION
        /// <summary>
        /// Update manipulator statistics
        /// </summary>
        public void UpdateStatistics(Int32[] data, List<Manipulator> curManipulators,
                                     DataStructures.ZibraLiquidSolverParameters solverParameters,
                                     List<ZibraLiquidCollider> sdfObjects)
        {
            int id = 0;
            foreach (var manipulator in Manipulators)
            {
                if (manipulator == null)
                    continue;

                Vector3 Force = Mathf.Exp(4.0f * solverParameters.ForceInteractionStrength) *
                                new Vector3(INT2Float(data[GetStatIndex(id, 0)]), INT2Float(data[GetStatIndex(id, 1)]),
                                            INT2Float(data[GetStatIndex(id, 2)]));
                Vector3 Torque = Mathf.Exp(4.0f * solverParameters.ForceInteractionStrength) *
                                 new Vector3(INT2Float(data[GetStatIndex(id, 3)]), INT2Float(data[GetStatIndex(id, 4)]),
                                             INT2Float(data[GetStatIndex(id, 5)]));

                switch (manipulator.GetManipulatorType())
                {
                default:
                    break;
                case Manipulator.ManipulatorType.Emitter:
                    ZibraLiquidEmitter emitter = manipulator as ZibraLiquidEmitter;
                    emitter.CreatedParticlesPerFrame = data[GetStatIndex(id, 0)];
                    emitter.CreatedParticlesTotal += emitter.CreatedParticlesPerFrame;
                    break;
                case Manipulator.ManipulatorType.Void:
                    ZibraLiquidVoid zibravoid = manipulator as ZibraLiquidVoid;
                    zibravoid.DeletedParticleCountPerFrame = data[GetStatIndex(id, 0)];
                    zibravoid.DeletedParticleCountTotal += zibravoid.DeletedParticleCountPerFrame;
                    break;
                case Manipulator.ManipulatorType.Detector:
                    ZibraLiquidDetector zibradetector = manipulator as ZibraLiquidDetector;
                    zibradetector.ParticlesInside = data[GetStatIndex(id, 0)];
                    break;
                case Manipulator.ManipulatorType.NeuralCollider:
                case Manipulator.ManipulatorType.AnalyticCollider:
                    ZibraLiquidCollider collider = manipulator as ZibraLiquidCollider;
                    collider.ApplyForceTorque(Force, Torque);
                    break;
                }
#if UNITY_EDITOR
                manipulator.NotifyChange();
#endif

                id++;
            }
        }
#endif

        /// <summary>
        /// Update constant object data and generate and sort the current manipulator list
        /// Should be executed once
        /// </summary>
        public void UpdateConst(List<Manipulator> curManipulators, List<ZibraLiquidCollider> colliders)
        {
            Manipulators = new List<Manipulator>();

#if ZIBRA_LIQUID_PAID_VERSION
            NeuralSDFs = new Dictionary<ZibraHash128, NeuralSDF>();
            TextureHashMap = new Dictionary<ZibraHash128, int>();
#endif

            // add all colliders to the manipulator list
            foreach (var manipulator in curManipulators)
            {
                if (manipulator == null)
                    continue;

                var sdf = manipulator.GetComponent<SDFObject>();
                if (sdf == null)
                {
                    Debug.LogWarning("Manipulator " + manipulator.gameObject.name + " missing sdf and is disabled.");
                    continue;
                }

#if ZIBRA_LIQUID_PAID_VERSION
#if ZIBRA_LIQUID_PRO_VERSION
                if (sdf is NeuralSDF)
                {
                    NeuralSDF neuralSDF = sdf as NeuralSDF;
                    if (!neuralSDF.ObjectRepresentation.HasRepresentationV3)
                    {
                        Debug.LogWarning("NeuralSDF in " + manipulator.gameObject.name +
                                         " was not generated. Manipulator is disabled.");
                        continue;
                    }
                }

                if (sdf is SkinnedMeshSDF)
                {
                    SkinnedMeshSDF skinnedMeshSDF = sdf as SkinnedMeshSDF;
                    if (!skinnedMeshSDF.HasRepresentation())
                    {
                        Debug.LogWarning("SkinnedMeshSDF in " + manipulator.gameObject.name +
                                         " was not generated. Manipulator is disabled.");
                        continue;
                    }
                }
#else
                if (sdf is NeuralSDF)
                {
                    Debug.LogWarning("NeuralSDF was used in manipulator in " + manipulator.gameObject.name +
                                     " which is not supported in this version. Manipulator is disabled.");
                    continue;
                }
#endif
#endif

                Manipulators.Add(manipulator);
            }

            // add all colliders to the manipulator list
            foreach (var manipulator in colliders)
            {
                if (manipulator == null)
                    continue;

                var sdf = manipulator.GetComponent<SDFObject>();
                if (sdf == null)
                {
                    Debug.LogWarning("Collider " + manipulator.gameObject.name + " missing sdf and is disabled.");
                    continue;
                }

#if ZIBRA_LIQUID_PAID_VERSION
                if (sdf is NeuralSDF)
                {
                    NeuralSDF neuralSDF = sdf as NeuralSDF;
                    if (!neuralSDF.ObjectRepresentation.HasRepresentationV3)
                    {
                        Debug.LogWarning("NeuralSDF in " + manipulator.gameObject.name +
                                         " was not generated. Collider is disabled.");
                        continue;
                    }
                }

                if (sdf is SkinnedMeshSDF)
                {
                    SkinnedMeshSDF skinnedMeshSDF = sdf as SkinnedMeshSDF;
                    if (!skinnedMeshSDF.HasRepresentation())
                    {
                        Debug.LogWarning("SkinnedMeshSDF in " + manipulator.gameObject.name +
                                         " was not generated. Collider is disabled.");
                        continue;
                    }
                }
#endif

                Manipulators.Add(manipulator);
            }

            // first sort the manipulators
            Manipulators.Sort(new ManipulatorCompare());

            // compute prefix sum
            for (int i = 0; i < (int)Manipulator.ManipulatorType.TypeNum; i++)
            {
                int id = 0;
                foreach (var manipulator in Manipulators)
                {
                    if ((int)manipulator.GetManipulatorType() >= i)
                    {
                        TypeIndex[i] = id;
                        break;
                    }
                    id++;
                }

                if (id == Manipulators.Count)
                {
                    TypeIndex[i] = Manipulators.Count;
                }
            }

            // set last as the total number of manipulators
            TypeIndex[(int)Manipulator.ManipulatorType.TypeNum] = Manipulators.Count;

            Indices.EmitterIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.Emitter];
            Indices.EmitterIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.Emitter + 1];
            Indices.VoidIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.Void];
            Indices.VoidIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.Void + 1];
            Indices.ForceFieldIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.ForceField];
            Indices.ForceFieldIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.ForceField + 1];
            Indices.AnalyticColliderIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.AnalyticCollider];
            Indices.AnalyticColliderIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.AnalyticCollider + 1];
            Indices.NeuralColliderIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.NeuralCollider];
            Indices.NeuralColliderIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.NeuralCollider + 1];
            Indices.GroupColliderIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.GroupCollider];
            Indices.GroupColliderIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.GroupCollider + 1];
            Indices.DetectorIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.Detector];
            Indices.DetectorIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.Detector + 1];
            Indices.SpeciesModifierIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.SpeciesModifier];
            Indices.SpeciesModifierIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.SpeciesModifier + 1];
            Indices.PortalIndexBegin = TypeIndex[(int)Manipulator.ManipulatorType.Portal];
            Indices.PortalIndexEnd = TypeIndex[(int)Manipulator.ManipulatorType.Portal + 1];

            if (ConstDataID.Count != 0)
            {
                ConstDataID.Clear();
            }

#if ZIBRA_LIQUID_PAID_VERSION
            SDFTextureSize = 0;
            EmbeddingTextureSize = 0;
            TextureCount = 0;
            foreach (var manipulator in Manipulators)
            {
                if (manipulator == null)
                    continue;

                if (manipulator.GetComponent<NeuralSDF>() != null)
                {
                    AddTexture(manipulator.GetComponent<NeuralSDF>());
                }

#if ZIBRA_LIQUID_PRO_VERSION
                if (manipulator.GetComponent<SkinnedMeshSDF>() != null)
                {
                    SkinnedMeshSDF skinnedMeshSDF = manipulator.GetComponent<SkinnedMeshSDF>();

                    foreach (var bone in skinnedMeshSDF.BoneSDFList)
                    {
                        if (bone is NeuralSDF neuralBone)
                            AddTexture(neuralBone);
                    }
                }
#endif
            }

            CalculateTextureData();
#endif
        }
    }
}
