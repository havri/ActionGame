using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ActionGame.World;
using ActionGame.Extensions;

namespace ActionGame.Space
{
    /// <summary>
    /// Class describing flat objects lying on the ground. Adds texture and drawing capabilites for GameObject class.
    /// </summary>
    class FlatObject : GameObject, IDisposable, ITransformedDrawable
    {
        readonly Texture2D texture;
        readonly VertexPositionNormalTexture[] vertices;
        readonly short[] indexes;
        readonly BasicEffect quadEffect;

        /// <summary>
        /// Creates a new flat object.
        /// </summary>
        /// <param name="position">Position of the object</param>
        /// <param name="azimuth">Azimuth</param>
        /// <param name="size">Size (width, lenght)</param>
        /// <param name="texture">Used texture</param>
        public FlatObject(PositionInTown position, double azimuth, Vector2 size, Texture2D texture)
            : base(position, azimuth, size)
        {
            this.texture = texture;
            vertices = new VertexPositionNormalTexture[4];
            indexes = new short[6];

            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal = Vector3.Up;
            }

            vertices[0].Position = LowerLeftCorner.ToVector3(verticalPosition);
            vertices[0].TextureCoordinate = textureLowerLeft;
            vertices[1].Position = UpperLeftCorner.ToVector3(verticalPosition);
            vertices[1].TextureCoordinate = textureUpperLeft;
            vertices[2].Position = LowerRightCorner.ToVector3(verticalPosition);
            vertices[2].TextureCoordinate = textureLowerRight;
            vertices[3].Position = UpperRightCorner.ToVector3(verticalPosition);
            vertices[3].TextureCoordinate = textureUpperRight;

            indexes[0] = 0;
            indexes[1] = 1;
            indexes[2] = 2;
            indexes[3] = 2;
            indexes[4] = 1;
            indexes[5] = 3;

            quadEffect = new BasicEffect(texture.GraphicsDevice);
            quadEffect.EnableDefaultLighting();
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = texture;
        }

        /// <summary>
        /// Draws the ground object
        /// </summary>
        /// <param name="view">View transformation matrix</param>
        /// <param name="projection">Projection transformation matrix</param>
        /// <param name="world">World transformation matrix</param>
        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            quadEffect.World = world;
            quadEffect.View = view;
            quadEffect.Projection = projection;
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                texture.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, 4, indexes, 0, 2);
            }
        }

        public void Dispose()
        {
            texture.Dispose();
            quadEffect.Dispose();
        }

        private float verticalPosition
        {
            get
            {
                return 0;
            }
        }
    }
}
