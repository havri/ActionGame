using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.Extensions;
using ActionGame.World;

namespace ActionGame.Space
{
    class Plate : Quadrangle, IDrawableObject, IDisposable
    {
        readonly VertexPositionNormalTexture[] frontVertices, backVertices;
        readonly short[] indexes;
        readonly BasicEffect frontQuadEffect, backQuadEffect;
        readonly GraphicsDevice graphicsDevice;

        public Plate(TownQuarter homeQuarter, Vector3 upperLeft, Vector3 upperRight, Vector3 lowerLeft, Vector3 lowerRight, Texture2D front, Texture2D back)
            :base(upperLeft.XZToVector2(), upperRight.XZToVector2(), lowerLeft.XZToVector2(), lowerRight.XZToVector2())
        {
            graphicsDevice = front.GraphicsDevice;

            frontVertices = new VertexPositionNormalTexture[4];
            backVertices = new VertexPositionNormalTexture[4];
            indexes = new short[6];

            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            Plane plane = new Plane(upperLeft, upperRight, lowerLeft);

            for (int i = 0; i < frontVertices.Length; i++)
            {
                frontVertices[i].Normal = plane.Normal;
                backVertices[i].Normal = -plane.Normal;
            }

            frontVertices[0].Position = lowerLeft;
            frontVertices[0].TextureCoordinate = textureLowerLeft;
            frontVertices[1].Position = upperLeft;
            frontVertices[1].TextureCoordinate = textureUpperLeft;
            frontVertices[2].Position = lowerRight;
            frontVertices[2].TextureCoordinate = textureLowerRight;
            frontVertices[3].Position = upperRight;
            frontVertices[3].TextureCoordinate = textureUpperRight;

            backVertices[0].Position = lowerRight;
            backVertices[0].TextureCoordinate = textureLowerRight;
            backVertices[1].Position = upperRight;
            backVertices[1].TextureCoordinate = textureUpperRight;
            backVertices[2].Position = lowerLeft;
            backVertices[2].TextureCoordinate = textureLowerLeft;
            backVertices[3].Position = upperLeft;
            backVertices[3].TextureCoordinate = textureUpperLeft;

            indexes[0] = 0;
            indexes[1] = 1;
            indexes[2] = 2;
            indexes[3] = 2;
            indexes[4] = 1;
            indexes[5] = 3;

            frontQuadEffect = new BasicEffect(graphicsDevice);
            frontQuadEffect.EnableDefaultLighting();
            frontQuadEffect.TextureEnabled = true;
            frontQuadEffect.Texture = front;

            backQuadEffect = new BasicEffect(graphicsDevice);
            backQuadEffect.EnableDefaultLighting();
            backQuadEffect.TextureEnabled = true;
            backQuadEffect.Texture = back;
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            frontQuadEffect.World = world;
            frontQuadEffect.View = view;
            frontQuadEffect.Projection = projection;
            backQuadEffect.World = world;
            backQuadEffect.View = view;
            backQuadEffect.Projection = projection;
            foreach (EffectPass pass in frontQuadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, frontVertices, 0, 4, indexes, 0, 2);
            }
            foreach (EffectPass pass in backQuadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, backVertices, 0, 4, indexes, 0, 2);
            }
        }

        public void Dispose()
        {
            frontQuadEffect.Dispose();
            backQuadEffect.Dispose();
        }
    }
}
