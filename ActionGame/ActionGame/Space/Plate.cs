using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.Extensions;
using ActionGame.World;
using System.Drawing;

namespace ActionGame.Space
{
    public class Plate : Quadrangle, IDrawableObject, IDisposable
    {
        readonly VertexPositionNormalTexture[] frontVertices, backVertices;
        readonly short[] indexes;
        BasicEffect frontQuadEffect, backQuadEffect;
        readonly GraphicsDevice graphicsDevice;
        Vector3 ul, ur, ll, lr;
        Texture2D front, back;

        public Plate(TownQuarter homeQuarter, Vector3 upperLeft, Vector3 upperRight, Vector3 lowerLeft, Vector3 lowerRight, Texture2D front, Texture2D back)
            :base(upperLeft.XZToVector2(), upperRight.XZToVector2(), lowerLeft.XZToVector2(), lowerRight.XZToVector2())
        {
            ul = upperLeft;
            ur = upperRight;
            ll = lowerLeft;
            lr = lowerRight;
            this.back = back;
            this.front = front;
            graphicsDevice = front.GraphicsDevice;
            frontVertices = new VertexPositionNormalTexture[4];
            backVertices = new VertexPositionNormalTexture[4];
            indexes = new short[6];
            BuildEffects();
        }

        public void SetFront(Texture2D front)
        {
            frontQuadEffect.Dispose();
            backQuadEffect.Dispose();
            this.front = front;
            BuildEffects();
        }

        private void BuildEffects()
        {
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            Plane plane = new Plane(ul, ur, ll);

            for (int i = 0; i < frontVertices.Length; i++)
            {
                frontVertices[i].Normal = plane.Normal;
                backVertices[i].Normal = -plane.Normal;
            }

            frontVertices[0].Position = ll;
            frontVertices[0].TextureCoordinate = textureLowerLeft;
            frontVertices[1].Position = ul;
            frontVertices[1].TextureCoordinate = textureUpperLeft;
            frontVertices[2].Position = lr;
            frontVertices[2].TextureCoordinate = textureLowerRight;
            frontVertices[3].Position = ur;
            frontVertices[3].TextureCoordinate = textureUpperRight;

            backVertices[0].Position = lr;
            backVertices[0].TextureCoordinate = textureLowerRight;
            backVertices[1].Position = ur;
            backVertices[1].TextureCoordinate = textureUpperRight;
            backVertices[2].Position = ll;
            backVertices[2].TextureCoordinate = textureLowerLeft;
            backVertices[3].Position = ul;
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
