using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Extensions
{
    static class ModelExtensions
    {
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
            BoundingBox box = model.GetBoundingBox(worldTransform);
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3[] corners = box.GetCorners();
            foreach (Vector3 corner in corners)
            {
                if (corner.X > max.X)
                    max.X = corner.X;
                if (corner.X < min.X)
                    min.X = corner.X;
                if (corner.Y > max.Y)
                    max.Y = corner.Y;
                if (corner.Y < min.Y)
                    min.Y = corner.Y;
                if (corner.Z > max.Z)
                    max.Z = corner.Z;
                if (corner.Z < min.Z)
                    min.Z = corner.Z;
            }
            return (max - min);
        }
    }
}
