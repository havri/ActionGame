﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Extensions
{
    static class ModelExtensions
    {
        private static readonly Dictionary<Model, Vector3> sizeCache = new Dictionary<Model, Vector3>();
        private static Matrix cachedWorldTransform = Matrix.Identity;

        /// <summary>
        /// Creates BoundingBox of this model.
        /// From: http://gamedev.stackexchange.com/questions/2438/how-do-rx-create-bounding-boxes-with-xna-4-0
        /// </summary>
        /// <param name="worldTransform">World transform matrix</param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this Model model, Matrix worldTransform)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            return new BoundingBox(min, max);
        }


        public static Vector3 GetSize(this Model model, Matrix worldTransform)
        {
            if (cachedWorldTransform == worldTransform)
            {
                if (sizeCache.ContainsKey(model))
                {
                    return sizeCache[model];
                }
            }
            else
            {
                cachedWorldTransform = worldTransform;
                sizeCache.Clear();
            }
            BoundingBox box = model.GetBoundingBox(worldTransform);
            
            Vector3 size = (box.Max - box.Min);
            sizeCache.Add(model, size);
            return size;
        }
    }
}
