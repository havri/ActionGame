﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace ActionGame
{
    class TownQuarter
    {
        /// <summary>
        /// Block without road width. In road square count.
        /// </summary>
        const int BlockWidth = 5;

        /// <summary>
        /// Width of road or sidewalk in pictured map. In pixels.
        /// </summary>
        const int PictureMapRoadWidth = 15;

        const float BetweenBuildingSpace = 4f;

        /// <summary>
        /// Object what makes ground textures.
        /// </summary>
        LinkedList<SpatialObject> groundObjects = new LinkedList<SpatialObject>();

        /// <summary>
        /// Really spatial objects - buildings, etc.
        /// </summary>
        LinkedList<SpatialObject> solidObjects = new LinkedList<SpatialObject>();

        /// <summary>
        /// Picture map of this quarter.
        /// </summary>
        Texture2D map;

        /// <summary>
        /// Creates new town quarter as map fragment. Generates roads, buildings, etc.
        /// </summary>
        /// <param name="size">Size of quarter without joining interface</param>
        /// <param name="degree">Number of quarter's interfaces (joining streets)</param>
        /// <param name="content">ContentManager for loading objects</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="graphicsDevice">Graphics device for creating textures</param>
        public TownQuarter(Vector2 size, int degree, ContentManager content, Matrix worldTransform, GraphicsDevice graphicsDevice)
        {
            ///TODO: Use bitmap to deny X crossroads.
            
            ///TODO: Load models from central repository
            Model roadModel = content.Load<Model>("Objects/Flat/road0");
            float roadModelWidth = roadModel.GetSize(worldTransform).X;
            Model sidewalkModel = content.Load<Model>("Objects/Flat/sidewalk0");
            float sidewalkModelWidth = sidewalkModel.GetSize(worldTransform).X;

            if (sidewalkModelWidth != roadModelWidth)
            {
                throw new InvalidOperationException("Sidewalk and road models must have same size.");
            }

            int xSize = (int) Math.Floor(size.X / roadModelWidth);
            int ySize = (int) Math.Floor(size.Y / roadModelWidth);

            if (xSize < 4 * BlockWidth || ySize < 4 * BlockWidth)
            {
                throw new ArgumentOutOfRangeException("Specified size is to small.");
            }

            MapFillType[] mapBitmap = new MapFillType[xSize*ySize];
            for (int i = 0; i < mapBitmap.Length; i++)
                mapBitmap[i] = MapFillType.Empty;

            GenerateInterfaces(degree, mapBitmap, xSize, ySize, roadModel, roadModelWidth, sidewalkModel, sidewalkModelWidth, worldTransform, content);

            Rectangle emptyRectangleInBorderRoadCyrcle = GenerateBorderRoads(ref size, ref worldTransform, roadModel, roadModelWidth, mapBitmap, xSize, ySize);
            List<Rectangle> emptyRectangles = GenerateInnerRoadNetwork(ref worldTransform, roadModel, roadModelWidth, emptyRectangleInBorderRoadCyrcle, mapBitmap, xSize, ySize);
            List<Rectangle> emptyRectaglesInsideSidewalks = new List<Rectangle>(emptyRectangles.Count);
            foreach (Rectangle emptyRect in emptyRectangles)
            {
                emptyRectaglesInsideSidewalks.Add(
                    GenerateSidewalks(emptyRect, sidewalkModel, sidewalkModelWidth, ref worldTransform, mapBitmap, xSize, ySize)
                );
            }

            GenerateMapPicture(graphicsDevice, xSize, ySize, mapBitmap);

            GenerateBuildings(emptyRectaglesInsideSidewalks, content, worldTransform, ref roadModelWidth);
        }


        private void GenerateBuildings(List<Rectangle> emptyRectaglesInsideSidewalks, ContentManager content, Matrix worldTransofrm, ref float roadSquareWidth)
        {
            ///TODO: Build central model repository
            Model[] buildingModels = new Model[]
            {
                content.Load<Model>("Objects/Buildings/panelak"),
                content.Load<Model>("Objects/Buildings/panelak2"),
                content.Load<Model>("Objects/Buildings/house1"),
                content.Load<Model>("Objects/Decorations/bin")
            };

            foreach (Rectangle emptyRect in emptyRectaglesInsideSidewalks)
            {
                float realWidth = emptyRect.Width * roadSquareWidth,
                    realHeight = emptyRect.Height * roadSquareWidth;

                RectangleF realEmptyRectangle = new RectangleF(emptyRect.X * roadSquareWidth, emptyRect.Y * roadSquareWidth, realWidth, realHeight);
                FillByBuildings(worldTransofrm, ref roadSquareWidth, buildingModels, realEmptyRectangle);
            }
        }

        private void FillByBuildings(Matrix worldTransofrm, ref float roadSquareWidth, Model[] buildingModels, RectangleF target)
        {
            Random rand = new Random();
            float horizontalSpace = (float)(rand.NextDouble() * 0.7 + 0.3) * BetweenBuildingSpace;
            float verticalSpace = (float)(rand.NextDouble() * 0.7 + 0.3) * BetweenBuildingSpace;

            
            IEnumerable<Model> modelCandidates = from model in buildingModels
                               where model.GetSize(worldTransofrm).X + horizontalSpace <= target.Width && model.GetSize(worldTransofrm).Z + verticalSpace <= target.Height
                               orderby rand.Next()
                               select model;
            if(modelCandidates.Any())
            {
                Model usedModel = modelCandidates.First();

                SpatialObject building = new SpatialObject(usedModel, new Vector3(target.X + horizontalSpace/2f, 0, target.Y + verticalSpace/2f), 0, worldTransofrm);
                solidObjects.AddLast(building);
                RectangleF nextRect = new RectangleF(
                    target.X + usedModel.GetSize(worldTransofrm).X + horizontalSpace,
                    target.Y,
                    target.Width - usedModel.GetSize(worldTransofrm).X - horizontalSpace,
                    usedModel.GetSize(worldTransofrm).Z + verticalSpace);

                RectangleF downRect = new RectangleF(
                    target.X,
                    target.Y + usedModel.GetSize(worldTransofrm).Z + verticalSpace,
                    target.Width,
                    target.Height - usedModel.GetSize(worldTransofrm).Z - verticalSpace);

                FillByBuildings(worldTransofrm, ref roadSquareWidth, buildingModels, nextRect);
                FillByBuildings(worldTransofrm, ref roadSquareWidth, buildingModels, downRect);
            }
        }

        /// <summary>
        /// Generates road and sidewalks interfaces - street for joining with other quarters.
        /// </summary>
        /// <param name="degree">Number of'quarters neigbors - degree of vertex</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Width of quarter bitmap</param>
        /// <param name="ySize">Height of quarter bitmap</param>
        /// <param name="roadModel">Used road model</param>
        /// <param name="roadModelWidth">Road model width</param>
        /// <param name="sidewalkModel">Used sidewalk model</param>
        /// <param name="sidewalkModelWidth">Sidewalk model width</param>
        /// <param name="worldTransform">World transform matrix</param>
        private void GenerateInterfaces(int degree, MapFillType[] mapBitmap, int xSize, int ySize, Model roadModel, float roadModelWidth, Model sidewalkModel, float sidewalkModelWidth, Matrix worldTransform, ContentManager content)
        {
            Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges = new Dictionary<TownQuarterInterfacePosition, List<Range>>(4);
            emptyRanges.Add(TownQuarterInterfacePosition.Top, new List<Range>(new Range[] { new Range(BlockWidth + 1, xSize - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Bottom,  new List<Range>(new Range[] {new Range(BlockWidth + 1, xSize - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Left,  new List<Range>(new Range[] { new Range(BlockWidth + 1, ySize - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Right,  new List<Range>(new Range[] { new Range(BlockWidth + 1, ySize - BlockWidth - 1) }));

            Random rand = new Random();
            for (int i = 0; i < degree; i++)
            {
                TownQuarterInterfacePosition side;
                List<TownQuarterInterfacePosition> possibleSides = new List<TownQuarterInterfacePosition>(
                    from key in emptyRanges.Keys where emptyRanges[key].Count(rangeItem => rangeItem.Length > 2 * BlockWidth + 1) > 0 select key
                    );

                if (possibleSides.Count < 1)
                {
                    throw new ArgumentException("This quarter has already full all sides of interfaces. Degree argument is too big.");
                }

                side = possibleSides[rand.Next(0, possibleSides.Count - 1)];

                List<Range> possibleRanges = emptyRanges[side].FindAll(rangeItem => rangeItem.Length > 2 * BlockWidth + 1);

                int rangeIndex = rand.Next(0, possibleRanges.Count - 1);
                Range range = possibleRanges[rangeIndex];
                emptyRanges[side].Remove(range);

                int position = range.Begin
                    + (int)( (0.35 + rand.NextDouble() * 0.3) // percentage position in rangle
                    * range.Length);

                emptyRanges[side].Add(new Range(range.Begin, position - 1));
                emptyRanges[side].Add(new Range(position + 1, range.End));

                AxisDirection direction
                    = (side == TownQuarterInterfacePosition.Right || side == TownQuarterInterfacePosition.Left) ? AxisDirection.Horizontal : AxisDirection.Vertical;

                for (int p = 0; p < BlockWidth; p++)
                {
                    int rx, ry, slx, sly, srx, sry;
                    switch (direction)
                    {
                        case AxisDirection.Horizontal:
                            rx = (p + (side == TownQuarterInterfacePosition.Left ? 0 : xSize - BlockWidth));
                            ry = position;
                            slx = rx;
                            srx = rx;
                            sly = ry - 1;
                            sry = ry + 1;
                            break;
                        case AxisDirection.Vertical:
                            rx = position;
                            ry = (p + (side == TownQuarterInterfacePosition.Top ? 0 : ySize - BlockWidth));
                            slx = rx - 1;
                            srx = rx + 1;
                            sly = ry;
                            sry = ry;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown AxisDirection value.");
                            break;
                    }

                    mapBitmap[rx*ySize + ry] = MapFillType.StraightRoad;
                    mapBitmap[slx * ySize + sly] = MapFillType.Sidewalk;
                    mapBitmap[srx * ySize + sry] = MapFillType.Sidewalk;
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(rx * roadModelWidth, 0, ry * roadModelWidth), 0, worldTransform);
                    SpatialObject sidewalkL = new SpatialObject(sidewalkModel, new Vector3(slx * sidewalkModelWidth, 0, sly * sidewalkModelWidth), 0, worldTransform);
                    SpatialObject sidewalkR = new SpatialObject(sidewalkModel, new Vector3(srx * sidewalkModelWidth, 0, sry * sidewalkModelWidth), 0, worldTransform);
                    groundObjects.AddLast(road);
                    groundObjects.AddLast(sidewalkL);
                    groundObjects.AddLast(sidewalkR);
                }
            }

            GenerateRestOfBorderSidewalks(emptyRanges, mapBitmap, xSize, ySize, sidewalkModel, sidewalkModelWidth, worldTransform);

            GenerateBorderBuildings(content, emptyRanges, roadModelWidth, worldTransform, xSize, ySize);
        }

        private void GenerateBorderBuildings(ContentManager content, Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges, float roadModelWidth, Matrix worldTransform, int bitmapSizeX, int bitmapSizeY)
        {
            //expand corner ranges
            foreach (var ranges in emptyRanges)
            {
                for (int i = 0; i < ranges.Value.Count; i++ )
                {
                    if (ranges.Value[i].Begin == BlockWidth + 1)
                    {
                        Range r = ranges.Value[i];
                        ranges.Value.RemoveAt(i);
                        r.Begin = BlockWidth - 2; // road corner, sidewlak
                        ranges.Value.Insert(i, r);
                    }
                    if (
                        (ranges.Value[i].End == bitmapSizeX - BlockWidth - 1 && ( ranges.Key == TownQuarterInterfacePosition.Top || ranges.Key == TownQuarterInterfacePosition.Bottom) )
                        || (ranges.Value[i].End == bitmapSizeY - BlockWidth - 1 && (ranges.Key == TownQuarterInterfacePosition.Left || ranges.Key == TownQuarterInterfacePosition.Right))
                        )
                    {
                        Range r = ranges.Value[i];
                        ranges.Value.RemoveAt(i);
                        if (ranges.Key == TownQuarterInterfacePosition.Top || ranges.Key == TownQuarterInterfacePosition.Bottom)
                        {
                            r.End = bitmapSizeX - BlockWidth + 1;
                        }
                        else
                        {
                            r.End = bitmapSizeY - BlockWidth + 1;
                        }
                        ranges.Value.Insert(i, r);
                    }
                }
            }

            ///TODO: Build central model repository
            Model[] buildingModels = new Model[]
            {
                content.Load<Model>("Objects/Buildings/borderBuilding"),
                content.Load<Model>("Objects/Buildings/borderBuilding10"),
                content.Load<Model>("Objects/Buildings/borderBuilding8"),
                content.Load<Model>("Objects/Buildings/borderBuilding4"),
                content.Load<Model>("Objects/Buildings/borderBuilding1"),
                content.Load<Model>("Objects/Buildings/fence1")
            };
            Random rand = new Random();
            foreach (var ranges in emptyRanges)
            {
                foreach (var range in ranges.Value)
                {
                    FillEmptyBorderRange(roadModelWidth, worldTransform, buildingModels, rand, ranges.Key, range, 0f, bitmapSizeX, bitmapSizeY);
                }
            }
        }

        private void FillEmptyBorderRange(float roadModelWidth, Matrix worldTransform, Model[] buildingModels, Random rand, TownQuarterInterfacePosition borderPosition, Range range, float offset, int bitmapSizeX, int bitmapSizeY)
        {
            float emptySpace = (range.Length - 1) * roadModelWidth - offset;
            IEnumerable<Model> modelCandidates = from model in buildingModels
                                                 where Math.Max(model.GetSize(worldTransform).X, model.GetSize(worldTransform).Z) <= emptySpace
                                                 orderby rand.Next()
                                                 select model;
            if (modelCandidates.Any())
            {
                Model usedModel = modelCandidates.First();
                Vector3 usedModelSize = usedModel.GetSize(worldTransform);
                float angle = 0;
                if (
                    ((borderPosition == TownQuarterInterfacePosition.Left || borderPosition == TownQuarterInterfacePosition.Right)
                    && usedModelSize.X > usedModelSize.Z)
                    ||
                    ((borderPosition == TownQuarterInterfacePosition.Top || borderPosition == TownQuarterInterfacePosition.Bottom)
                    && usedModelSize.X < usedModelSize.Z)
                    )
                {
                    angle = 0;
                }

                Vector3 position = Vector3.Zero;
                float newOffset = offset + usedModelSize.X;
                switch (borderPosition)
                {
                    case TownQuarterInterfacePosition.Top:
                        position = new Vector3(range.Begin * roadModelWidth + roadModelWidth + offset, 0, BlockWidth * roadModelWidth - roadModelWidth - usedModelSize.Z);
                        break;
                    case TownQuarterInterfacePosition.Right:
                        position = new Vector3(BlockWidth * roadModelWidth - roadModelWidth - usedModelSize.X / 2 + usedModelSize.Z / 2 + +(bitmapSizeX - 2 * (BlockWidth - 1)) * roadModelWidth, 0, range.Begin * roadModelWidth + roadModelWidth + offset + usedModelSize.X / 2 - usedModelSize.Z / 2);
                        angle = MathHelper.PiOver2;
                        break;
                    case TownQuarterInterfacePosition.Bottom:
                        position = new Vector3(range.Begin * roadModelWidth + roadModelWidth + offset, 0, BlockWidth * roadModelWidth - roadModelWidth + (bitmapSizeY - 2 * (BlockWidth - 1)) * roadModelWidth);
                        break;
                    case TownQuarterInterfacePosition.Left:
                        position = new Vector3(BlockWidth * roadModelWidth - roadModelWidth - usedModelSize.X / 2 - usedModelSize.Z / 2, 0, range.Begin * roadModelWidth + roadModelWidth + offset + usedModelSize.X / 2 - usedModelSize.Z / 2);
                        angle = MathHelper.PiOver2;
                        break;
                    default:
                        break;
                }

                SpatialObject borderBuilding = new SpatialObject(usedModel, position, angle, worldTransform);
                solidObjects.AddLast(borderBuilding);

                FillEmptyBorderRange(roadModelWidth, worldTransform, buildingModels, rand, borderPosition, range, newOffset, bitmapSizeX, bitmapSizeY);
            }
            else 
            { }
        }

        /// <summary>
        /// Generates sidewalks what surround quarter border road and weren't generated by interfaces.
        /// </summary>
        /// <param name="emptyRanges">Ranges for sidewalks placement</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Width of quarter bitmap</param>
        /// <param name="ySize">Height of quarter bitmap</param>
        /// <param name="sidewalkModel">Used sidewalk model</param>
        /// <param name="sidewalkModelWidth">Sidewalk model width</param>
        /// <param name="worldTransform">World transform matrix</param>
        private void GenerateRestOfBorderSidewalks(Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges, MapFillType[] mapBitmap, int xSize, int ySize, Model sidewalkModel, float sidewalkModelWidth, Matrix worldTransform)
        {
            // Corners
            foreach (Tuple<int, int> p in new Tuple<int, int>[]
            {
                new Tuple<int, int>(BlockWidth - 1, BlockWidth),
                new Tuple<int, int>(BlockWidth - 1, BlockWidth - 1),
                new Tuple<int, int>(BlockWidth, BlockWidth - 1),

                new Tuple<int, int>(xSize - BlockWidth - 1, BlockWidth - 1),
                new Tuple<int, int>(xSize - BlockWidth, BlockWidth - 1),
                new Tuple<int, int>(xSize - BlockWidth, BlockWidth),

                new Tuple<int, int>(xSize - BlockWidth, ySize - BlockWidth - 1),
                new Tuple<int, int>(xSize - BlockWidth, ySize - BlockWidth),
                new Tuple<int, int>(xSize - BlockWidth - 1, ySize - BlockWidth),

                new Tuple<int, int>(BlockWidth, ySize - BlockWidth),
                new Tuple<int, int>(BlockWidth-1, ySize - BlockWidth),
                new Tuple<int, int>(BlockWidth-1, ySize - BlockWidth - 1)
            })
            {
                int x = p.Item1, y = p.Item2;
                mapBitmap[x * ySize + y] = MapFillType.Sidewalk;
                SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(x * sidewalkModelWidth, 0, y * sidewalkModelWidth), 0, worldTransform);
                groundObjects.AddLast(sidewalk);
            }

            // Ranges
            foreach (KeyValuePair<TownQuarterInterfacePosition, List<Range>> sideRanges in emptyRanges)
            {
                foreach (Range range in sideRanges.Value)
                {
                    for (int i = 0; i < range.Length;i++ )
                    {
                        int x, y;
                        switch (sideRanges.Key)
                        {
                            case TownQuarterInterfacePosition.Top:
                                x = range.Begin + i;
                                y = BlockWidth - 1;
                                break;
                            case TownQuarterInterfacePosition.Right:
                                x = xSize - BlockWidth;
                                y = range.Begin + i;
                                break;
                            case TownQuarterInterfacePosition.Bottom:
                                x = range.Begin + i;
                                y = ySize - BlockWidth;
                                break;
                            case TownQuarterInterfacePosition.Left:
                                x = BlockWidth - 1;
                                y = range.Begin + i;
                                break;
                            default:
                                throw new InvalidOperationException("Unknown TownQuarterInterfacePosition value.");
                        }

                        mapBitmap[x * ySize + y] = MapFillType.Sidewalk;
                        SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(x * sidewalkModelWidth, 0 , y * sidewalkModelWidth), 0, worldTransform);
                        groundObjects.AddLast(sidewalk);
                    }
                }
            }
        }

        /// <summary>
        /// Creates picture of quarter map and saves as Texture2D
        /// </summary>
        /// <param name="graphicsDevice">Device for texture store</param>
        /// <param name="xSize">Quarter bitmap width</param>
        /// <param name="ySize">Quarter bitmap height</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        private void GenerateMapPicture(GraphicsDevice graphicsDevice, int xSize, int ySize, MapFillType[] mapBitmap)
        {
            System.Drawing.Image mapPicture = (System.Drawing.Image)(new System.Drawing.Bitmap((2 * BlockWidth + xSize) * PictureMapRoadWidth, (2 * BlockWidth + ySize) * PictureMapRoadWidth));
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(mapPicture);
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    System.Drawing.Color c = System.Drawing.Color.Green;
                    switch (mapBitmap[x * ySize + y])
                    {
                        case MapFillType.Empty:
                            c = System.Drawing.Color.Green;
                            break;
                        case MapFillType.StraightRoad:
                            c = System.Drawing.Color.Gray;
                            break;
                        case MapFillType.Sidewalk:
                            c = System.Drawing.Color.Silver;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown MapFillType given.");
                            break;
                    }
                    g.FillRectangle(new System.Drawing.SolidBrush(c), (BlockWidth + x) * PictureMapRoadWidth, (BlockWidth + y) * PictureMapRoadWidth, PictureMapRoadWidth, PictureMapRoadWidth);
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                mapPicture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                map = Texture2D.FromStream(graphicsDevice, ms);
            }
        }

        /// <summary>
        /// Generates inner road network into target rectangle. Runs recursively. Exponential.
        /// </summary>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="roadModel">Road square model</param>
        /// <param name="roadModelWidth">Road square size</param>
        /// <param name="emptyRectangle">Target rectangle</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Quarter bitmap width</param>
        /// <param name="ySize">Quarter bitmap height</param>
        /// <returns>Another list of empty rectangles for next filling</returns>
        private List<Rectangle> GenerateInnerRoadNetwork(ref Matrix worldTransform, Model roadModel, float roadModelWidth, Rectangle emptyRectangle, MapFillType[] mapBitmap, int xSize, int ySize)
        {
            List<Rectangle> result = new List<Rectangle>();
            int erSize = Math.Max(emptyRectangle.Width, emptyRectangle.Height);
            Random rand = new Random();
            double nextSplittingProbability = rand.NextDouble();
            if (nextSplittingProbability * erSize > 2*BlockWidth + 1) // Splitting isn't sure when it's possible.
            {
                Rectangle[] newEmptyRectangles = AddSplittingRoad(ref emptyRectangle, roadModel, roadModelWidth, ref worldTransform, mapBitmap, xSize, ySize);
                foreach (Rectangle emptyRect in newEmptyRectangles)
                {
                    ///TODO: This can be faster. Using linked list simply join results...
                    result.AddRange(
                        GenerateInnerRoadNetwork(ref worldTransform, roadModel, roadModelWidth, emptyRect, mapBitmap, xSize, ySize)
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
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Quarter bitmap width</param>
        /// <param name="ySize">Quarter bitmap height</param>
        /// <returns>Array of new empty rectangles inside the splited rectangle</returns>
        private Rectangle[] AddSplittingRoad(ref Rectangle target, Model roadModel, float roadModelWidth,ref Matrix worldTransform, MapFillType[] mapBitmap, int xSize, int ySize)
        {
            AxisDirection direction = AxisDirection.Vertical;
            if (target.Width < target.Height)
                direction = AxisDirection.Horizontal;

            int size = (direction == AxisDirection.Horizontal ? target.Height : target.Width);
            int secondDimensionSize = (direction == AxisDirection.Horizontal ? target.Width : target.Height);

            Random rand = new Random();
            int splitPosition = (int) (
                (0.35 + rand.NextDouble() * 0.3) //percentage side
                * (size - 1) );

            for (int i = 0; i < secondDimensionSize; i++)
            { 
                Vector3 position = Vector3.Zero;
                int bitmapIndex = -1;
                switch (direction)
                {
                    case AxisDirection.Horizontal:
                        position = new Vector3((i + target.X) * roadModelWidth, 0, (splitPosition + target.Y) * roadModelWidth);
                        bitmapIndex = (i + target.X) * ySize + (splitPosition + target.Y);
                        break;
                    case AxisDirection.Vertical:
                        position = new Vector3((splitPosition + target.X) * roadModelWidth, 0, (i + target.Y) * roadModelWidth);
                        bitmapIndex = (splitPosition + target.X) * ySize + (i + target.Y);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown AxisDirection for splitting.");
                        break;
                }

                mapBitmap[bitmapIndex] = MapFillType.StraightRoad;
                SpatialObject road = new SpatialObject(roadModel, position, 0, worldTransform);
                groundObjects.AddLast(road);
            }


            Rectangle[] emptyRectangles = new Rectangle[2];
            if(direction == AxisDirection.Horizontal)
            {
                emptyRectangles[0] = new Rectangle(target.X, target.Y, target.Width, splitPosition);
                emptyRectangles[1] = new Rectangle(target.X, target.Y + splitPosition + 1, target.Width, target.Height - splitPosition - 1);
            }
            else if(direction == AxisDirection.Vertical)
            {
                emptyRectangles[0] = new Rectangle(target.X, target.Y, splitPosition, target.Height);
                emptyRectangles[1] = new Rectangle(target.X + splitPosition + 1, target.Y, target.Width - splitPosition - 1, target.Height);
            }
            return emptyRectangles;
        }

        /// <summary>
        /// Generates border road network.
        /// </summary>
        /// <param name="size">Size of quarter</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="roadModel">Road model</param>
        /// <param name="roadModelWidth">Road square size</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Width of the quarter</param>
        /// <param name="ySize">Height of the quarter</param>
        /// <returns>Empty rectangle inside the bitmap</returns>
        private Rectangle GenerateBorderRoads(ref Vector2 size, ref Matrix worldTransform, Model roadModel, float roadModelWidth, MapFillType[] mapBitmap, int xSize, int ySize)
        {
            int xOffset = BlockWidth;
            int yOffset = BlockWidth;
            int xCount = xSize - 2*xOffset;
            int yCount = ySize - 2*yOffset;

            for (int x = 0; x < xCount; x++)
            {
                int X = x + xOffset;
                foreach (int y in new int[] { 0, yCount - 1 })
                {
                    int Y = y + yOffset;
                    mapBitmap[X * ySize + Y] = MapFillType.StraightRoad;
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(X*roadModelWidth, 0, Y*roadModelWidth), 0, worldTransform);
                    groundObjects.AddLast(road);
                }
            }
            for (int y = 1; y < yCount-1; y++)
            {
                int Y = y + yOffset;
                foreach (int x in new int[] { 0, xCount - 1 })
                {
                    int X = x + xOffset;
                    mapBitmap[X * ySize + Y] = MapFillType.StraightRoad;
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(X * roadModelWidth, 0, Y * roadModelWidth), 0, worldTransform);
                    groundObjects.AddLast(road);
                }
            }
            return new Rectangle(xOffset + 1, yOffset + 1, xCount - 2, yCount - 2);
        }

        /// <summary>
        /// Generates sidewalk border inside the target rectangle.
        /// </summary>
        /// <param name="target">Target side rectangle</param>
        /// <param name="sidewalkModel">Used model</param>
        /// <param name="sidewalkModelWidth">Used sidewalk size</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Quarter bitmap width</param>
        /// <param name="ySize">Quarter bitmap height</param>
        /// <returns>Rest of the target rectangle what's empty</returns>
        private Rectangle GenerateSidewalks(Rectangle target, Model sidewalkModel, float sidewalkModelWidth, ref Matrix worldTransform, MapFillType[] mapBitmap, int xSize, int ySize)
        {
            for (int x = 0; x < target.Width; x++)
            {
                foreach(int y in new int[]{0, target.Height - 1})
                {
                    int X = target.X + x;
                    int Y = target.Y + y;
                    mapBitmap[X * ySize + Y] = MapFillType.Sidewalk;
                    SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(X * sidewalkModelWidth, 0, Y * sidewalkModelWidth), 0, worldTransform);
                    groundObjects.AddFirst(sidewalk);
                }
            }
            for (int y = 1; y < (target.Height -1); y++)
            {
                foreach (int x in new int[] { 0, target.Width - 1 })
                {
                    int X = target.X + x;
                    int Y = target.Y + y;
                    mapBitmap[X * ySize + Y] = MapFillType.Sidewalk;
                    SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(X * sidewalkModelWidth, 0, Y * sidewalkModelWidth), 0, worldTransform);
                    groundObjects.AddFirst(sidewalk);
                }
            }

            return new Rectangle(target.X + 1, target.Y + 1, target.Width - 2, target.Height - 2);
        }

        /// <summary>
        /// Configures drawer for drawing this quarter.
        /// </summary>
        /// <param name="drawer">Display drawer</param>
        public void FillDrawer(Drawer drawer)
        {
            foreach (SpatialObject o in groundObjects)
            {
                drawer.StartDrawingObject(o, true);
            }
            foreach (SpatialObject o in solidObjects)
            {
                drawer.StartDrawingObject(o, false);
            }
            drawer.QuarterMapPicture = map;
        }
    }
}
