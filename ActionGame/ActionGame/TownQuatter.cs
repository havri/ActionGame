using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    class TownQuatter
    {
        /// <summary>
        /// Offset of border road cycle.
        /// </summary>
        const int BorderRoadOffset = 2;

        /// <summary>
        /// Block without road width. In road square count.
        /// </summary>
        const int BlockWidth = 8;

        LinkedList<SpatialObject> groundObjects = new LinkedList<SpatialObject>();

        /// <summary>
        /// Creates new town quatter as map fragment. Generates roads, buildings, etc.
        /// </summary>
        /// <param name="size">Size of quatter without joining interface</param>
        /// <param name="degree">Number of joining streets</param>
        /// <param name="content">ContentManager for loading objects</param>
        /// <param name="worldTransform">World transform matrix</param>
        public TownQuatter(Vector2 size, int degree, ContentManager content, Matrix worldTransform)
        {
            ///TODO: Make help bitmap for grid to recognize X crossroads.
            Model roadModel = content.Load<Model>("Objects/Flat/road0");
            float roadModelWidth = roadModel.GetSize(worldTransform).X;

            RectangleF emptyRectangle = GenerateBorderRoads(ref size, ref worldTransform, roadModel, roadModelWidth);

            GenerateInnerRoadNetwork(ref worldTransform, roadModel, roadModelWidth, emptyRectangle);
        }

        /// <summary>
        /// Generates inner road network into target rectangle. Runs recursively. Exponential.
        /// </summary>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="roadModel">Road square model</param>
        /// <param name="roadModelWidth">Road square size</param>
        /// <param name="emptyRectangle">Target rectangle</param>
        /// <returns>Another list of empty rectangles for next filling</returns>
        private List<RectangleF> GenerateInnerRoadNetwork(ref Matrix worldTransform, Model roadModel, float roadModelWidth, RectangleF emptyRectangle)
        {
            List<RectangleF> result = new List<RectangleF>();
            float erSize = Math.Max(emptyRectangle.Width, emptyRectangle.Height);
            Random rand = new Random();
            double nextSplittingProbability = rand.NextDouble();
            if (nextSplittingProbability * erSize > BlockWidth * roadModelWidth) // Splitting isn't sure when ít's possible.
            {
                RectangleF[] newEmptyRectangles = AddSplittingRoad(emptyRectangle, roadModel, roadModelWidth, ref worldTransform);
                foreach (RectangleF emptyRect in newEmptyRectangles)
                {
                    ///TODO: This can be faster. Using linked list simply join results...
                    result.AddRange(
                        GenerateInnerRoadNetwork(ref worldTransform, roadModel, roadModelWidth, emptyRect)
                        );
                }
            }
            else
            {
                result.Add(emptyRectangle);
            }
            return result;
        }


        /// <summary>
        /// Splits the given rectangle with a road.
        /// </summary>
        /// <param name="target">Splited rectangle</param>
        /// <param name="roadModel">Road model</param>
        /// <param name="roadModelWidth">Road model size</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <returns>Array of new empty rectangles inside the splited rectangle</returns>
        private RectangleF[] AddSplittingRoad(RectangleF target, Model roadModel, float roadModelWidth,ref Matrix worldTransform)
        {
            AxisDirection direction = AxisDirection.Vertical;
            if (target.Width < target.Height)
                direction = AxisDirection.Horizontal;

            float size = (direction == AxisDirection.Horizontal ? target.Height : target.Width);
            float secondDimensionSize = (direction == AxisDirection.Horizontal ? target.Width : target.Height);
            int lengthCount = (int)(secondDimensionSize / roadModelWidth);

            Random rand = new Random();
            float relativePosition = (float)(
                0.35 + rand.NextDouble() * 0.3 // percentage position
                * (size - roadModelWidth)); // position

            for (int i = 0; i < lengthCount; i++)
            { 
                Vector3 position = Vector3.Zero;
                if(direction == AxisDirection.Horizontal)
                    position = new Vector3(i * roadModelWidth + target.X, 0, relativePosition + target.Y);
                else if(direction == AxisDirection.Vertical)
                    position = new Vector3(relativePosition + target.X, 0, i * roadModelWidth + target.Y);

                SpatialObject road = new SpatialObject(roadModel, position, 0, worldTransform);
                groundObjects.AddLast(road);
            }


            RectangleF[] emptyRectangles = new RectangleF[2];
            if(direction == AxisDirection.Horizontal)
            {
                emptyRectangles[0] = new RectangleF(target.X, target.Y, target.Width, relativePosition);
                emptyRectangles[1] = new RectangleF(target.X, target.Y + relativePosition + roadModelWidth, target.Width, size - (relativePosition + roadModelWidth));
            }
            else if(direction == AxisDirection.Vertical)
            {
                emptyRectangles[0] = new RectangleF(target.X, target.Y, relativePosition, target.Height);
                emptyRectangles[1] = new RectangleF(target.X +  relativePosition + roadModelWidth, target.Y, size - (relativePosition + roadModelWidth), target.Height);
            }
            return emptyRectangles;
        }

        /// <summary>
        /// Generates border road network.
        /// </summary>
        /// <param name="size">Size of quatter</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="roadModel">Road model</param>
        /// <param name="roadModelWidth">Road square size</param>
        /// <returns>Empty rectangle inside</returns>
        private RectangleF GenerateBorderRoads(ref Vector2 size, ref Matrix worldTransform, Model roadModel, float roadModelWidth)
        {
            int xCount = (int)(size.X / roadModelWidth);
            int yCount = (int)(size.Y / roadModelWidth);
            for (int x = 0; x < xCount; x++)
            {
                foreach (float y in new float[] { BorderRoadOffset, BorderRoadOffset + ((yCount-1) * roadModelWidth) })
                {
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(BorderRoadOffset + x * roadModelWidth, 0, y), 0, worldTransform);
                    groundObjects.AddLast(road);
                }
            }
            for (int y = 1; y < yCount-1; y++)
            {
                foreach (float x in new float[] { BorderRoadOffset, BorderRoadOffset + ((xCount-1) * roadModelWidth) })
                {
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(x, 0, BorderRoadOffset + y * roadModelWidth), 0, worldTransform);
                    groundObjects.AddLast(road);
                }
            }
            return new RectangleF(
                BorderRoadOffset + roadModelWidth,
                BorderRoadOffset + roadModelWidth,
                (xCount - 2) * roadModelWidth,
                (yCount - 2) * roadModelWidth);
        }



        /// <summary>
        /// Configures drawer for drawing this quatter.
        /// </summary>
        /// <param name="drawer">Display drawer</param>
        public void FillDrawer(Drawer drawer)
        {
            foreach (SpatialObject o in groundObjects)
            {
                drawer.StartDrawingObject(o, true);
            }
        }
    }
}
