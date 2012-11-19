using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame
{
    public partial class TownQuarter : IDisposable
    {

        private void Generate(ref Vector2 size, int degree, ContentManager content, ref Matrix worldTransform, GraphicsDevice graphicsDevice)
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

            int xSize = (int)Math.Floor(size.X / roadModelWidth);
            int ySize = (int)Math.Floor(size.Y / roadModelWidth);

            squareWidth = roadModelWidth;

            if (xSize < 4 * BlockWidth || ySize < 4 * BlockWidth)
            {
                throw new ArgumentOutOfRangeException("Specified size is to small.");
            }

            bitmapSize = new System.Drawing.Size(xSize, ySize);

            MapFillType[] mapBitmap = new MapFillType[xSize * ySize];
            for (int i = 0; i < mapBitmap.Length; i++)
                mapBitmap[i] = MapFillType.Empty;

            GenerateInterfaces(degree, mapBitmap, roadModel, sidewalkModel, worldTransform, content);

            Rectangle emptyRectangleInBorderRoadCyrcle = GenerateBorderRoads(ref size, ref worldTransform, roadModel, mapBitmap);
            List<Rectangle> emptyRectangles = GenerateInnerRoadNetwork(ref worldTransform, roadModel, emptyRectangleInBorderRoadCyrcle, mapBitmap);
            List<Rectangle> emptyRectaglesInsideSidewalks = new List<Rectangle>(emptyRectangles.Count);
            foreach (Rectangle emptyRect in emptyRectangles)
            {
                emptyRectaglesInsideSidewalks.Add(
                    GenerateSidewalks(emptyRect, sidewalkModel, ref worldTransform, mapBitmap)
                );
            }

            GenerateBuildings(emptyRectaglesInsideSidewalks, content, worldTransform);

            GenerateMapPicture(graphicsDevice, mapBitmap);
        }
        
        private void GenerateBuildings(List<Rectangle> emptyRectaglesInsideSidewalks, ContentManager content, Matrix worldTransofrm)
        {
            ///TODO: Build central model repository
            Model[] buildingModels = new Model[]
            {
                content.Load<Model>("Objects/Buildings/panelak"),
                content.Load<Model>("Objects/Buildings/panelak2"),
                content.Load<Model>("Objects/Buildings/house1")/*,
                content.Load<Model>("Objects/Decorations/bin")*/
            };

            foreach (Rectangle emptyRect in emptyRectaglesInsideSidewalks)
            {
                float realWidth = emptyRect.Width * squareWidth,
                    realHeight = emptyRect.Height * squareWidth;

                RectangleF realEmptyRectangle = new RectangleF(emptyRect.X * squareWidth, emptyRect.Y * squareWidth, realWidth, realHeight);
                FillByBuildings(worldTransofrm, buildingModels, realEmptyRectangle);
            }
        }

        private void FillByBuildings(Matrix worldTransofrm, Model[] buildingModels, RectangleF target)
        {
            Random rand = new Random();
            float horizontalSpace = (float)(rand.NextDouble() * 0.7 + 0.3) * BetweenBuildingSpace;
            float verticalSpace = (float)(rand.NextDouble() * 0.7 + 0.3) * BetweenBuildingSpace;


            IEnumerable<Model> modelCandidates = from model in buildingModels
                                                 where model.GetSize(worldTransofrm).X + horizontalSpace <= target.Width && model.GetSize(worldTransofrm).Z + verticalSpace <= target.Height
                                                 orderby rand.Next()
                                                 select model;
            if (modelCandidates.Any())
            {
                Model usedModel = modelCandidates.First();

                SpatialObject building = new SpatialObject(usedModel, new Vector3(target.X + horizontalSpace / 2f, 0, target.Y + verticalSpace / 2f), 0, worldTransofrm);
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

                FillByBuildings(worldTransofrm, buildingModels, nextRect);
                FillByBuildings(worldTransofrm, buildingModels, downRect);
            }
        }

        /// <summary>
        /// Generates road and sidewalks interfaces - street for joining with other quarters.
        /// </summary>
        /// <param name="degree">Number of'quarters neigbors - degree of vertex</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="roadModel">Used road model</param>
        /// <param name="roadModelWidth">Road model width</param>
        /// <param name="sidewalkModel">Used sidewalk model</param>
        /// <param name="sidewalkModelWidth">Sidewalk model width</param>
        /// <param name="worldTransform">World transform matrix</param>
        private void GenerateInterfaces(int degree, MapFillType[] mapBitmap, Model roadModel, Model sidewalkModel, Matrix worldTransform, ContentManager content)
        {
            Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges = new Dictionary<TownQuarterInterfacePosition, List<Range>>(4);
            emptyRanges.Add(TownQuarterInterfacePosition.Top, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Width - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Bottom, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Width - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Left, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Height - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Right, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Height - BlockWidth - 1) }));

            Random rand = new Random();
            for (int i = 0; i < degree; i++)
            {
                TownQuarterInterfacePosition side;
                List<TownQuarterInterfacePosition> possibleSides = new List<TownQuarterInterfacePosition>(
                    from key in emptyRanges.Keys where emptyRanges[key].Count(rangeItem => rangeItem.Length > 2 * BlockWidth + 1) > 0 select key
                    );

                if (possibleSides.Count < 1)
                {
                    throw new NoSpaceForInterfaceException("This quarter has already full all sides of interfaces. Degree argument is too big.");
                }

                side = possibleSides[rand.Next(0, possibleSides.Count - 1)];

                List<Range> possibleRanges = emptyRanges[side].FindAll(rangeItem => rangeItem.Length > 2 * BlockWidth + 1);

                int rangeIndex = rand.Next(0, possibleRanges.Count - 1);
                Range range = possibleRanges[rangeIndex];
                emptyRanges[side].Remove(range);

                int position = range.Begin
                    + (int)((0.35 + rand.NextDouble() * 0.3) // percentage position in range
                    * range.Length);

                emptyRanges[side].Add(new Range(range.Begin, position - 1));
                emptyRanges[side].Add(new Range(position + 1, range.End));

                AxisDirection direction
                    = (side == TownQuarterInterfacePosition.Right || side == TownQuarterInterfacePosition.Left) ? AxisDirection.Horizontal : AxisDirection.Vertical;

                TownQuarterInterface iface = new TownQuarterInterface { SidePosition = side, Quarter = this, OppositeInterface = null };
                for (int p = 0; p < BlockWidth; p++)
                {
                    int rx, ry, slx, sly, srx, sry;
                    switch (direction)
                    {
                        case AxisDirection.Horizontal:
                            rx = (p + (side == TownQuarterInterfacePosition.Left ? 0 : bitmapSize.Width - BlockWidth));
                            ry = position;
                            slx = rx;
                            srx = rx;
                            sly = ry - 1;
                            sry = ry + 1;
                            break;
                        case AxisDirection.Vertical:
                            rx = position;
                            ry = (p + (side == TownQuarterInterfacePosition.Top ? 0 : bitmapSize.Height - BlockWidth));
                            slx = rx - 1;
                            srx = rx + 1;
                            sly = ry;
                            sry = ry;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown AxisDirection value.");
                    }

                    if (direction == AxisDirection.Horizontal)
                        iface.BitmapPosition = ry;
                    if (direction == AxisDirection.Vertical)
                        iface.BitmapPosition = rx;

                    mapBitmap[rx * bitmapSize.Height + ry] = MapFillType.StraightRoad;
                    mapBitmap[slx * bitmapSize.Height + sly] = MapFillType.Sidewalk;
                    mapBitmap[srx * bitmapSize.Height + sry] = MapFillType.Sidewalk;
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(rx * squareWidth, 0, ry * squareWidth), 0, worldTransform);
                    SpatialObject sidewalkL = new SpatialObject(sidewalkModel, new Vector3(slx * squareWidth, 0, sly * squareWidth), 0, worldTransform);
                    SpatialObject sidewalkR = new SpatialObject(sidewalkModel, new Vector3(srx * squareWidth, 0, sry * squareWidth), 0, worldTransform);
                    groundObjects.AddLast(road);
                    groundObjects.AddLast(sidewalkL);
                    groundObjects.AddLast(sidewalkR);
                }
                this.interfaces.Add(iface);
            }

            GenerateRestOfBorderSidewalks(emptyRanges, mapBitmap, sidewalkModel, worldTransform);

            GenerateBorderBuildings(content, emptyRanges, worldTransform);
        }

        private void GenerateBorderBuildings(ContentManager content, Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges, Matrix worldTransform)
        {
            //expand corner ranges
            foreach (var ranges in emptyRanges)
            {
                for (int i = 0; i < ranges.Value.Count; i++)
                {
                    if (ranges.Value[i].Begin == BlockWidth + 1)
                    {
                        Range r = ranges.Value[i];
                        ranges.Value.RemoveAt(i);
                        r.Begin = BlockWidth - 2; // road corner, sidewlak
                        ranges.Value.Insert(i, r);
                    }
                    if (
                        (ranges.Value[i].End == bitmapSize.Width - BlockWidth - 1 && (ranges.Key == TownQuarterInterfacePosition.Top || ranges.Key == TownQuarterInterfacePosition.Bottom))
                        || (ranges.Value[i].End == bitmapSize.Height - BlockWidth - 1 && (ranges.Key == TownQuarterInterfacePosition.Left || ranges.Key == TownQuarterInterfacePosition.Right))
                        )
                    {
                        Range r = ranges.Value[i];
                        ranges.Value.RemoveAt(i);
                        if (ranges.Key == TownQuarterInterfacePosition.Top || ranges.Key == TownQuarterInterfacePosition.Bottom)
                        {
                            r.End = bitmapSize.Width - BlockWidth + 1;
                        }
                        else
                        {
                            r.End = bitmapSize.Height - BlockWidth + 1;
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
                    FillEmptyBorderRange(worldTransform, buildingModels, rand, ranges.Key, range, 0f);
                }
            }
        }

        /// <summary>
        /// Fills empty range of town quarter border by buildings. It takes it by dimension X.
        /// </summary>
        /// <param name="roadModelWidth">Width of road (sidewalk) square</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="buildingModels">Availible models</param>
        /// <param name="rand">Random generator</param>
        /// <param name="borderPosition">Position of border - specifies side of rectangle</param>
        /// <param name="range">Empty range for filling</param>
        /// <param name="offset">Already filled part of range</param>
        /// <param name="bitmapSizeX">Town quarter width in road squares</param>
        /// <param name="bitmapSizeY">Town quarter height in road squares</param>
        private void FillEmptyBorderRange(Matrix worldTransform, Model[] buildingModels, Random rand, TownQuarterInterfacePosition borderPosition, Range range, float offset)
        {
            float emptySpace = (range.Length - 1) * squareWidth - offset;
            IEnumerable<Model> modelCandidates = from model in buildingModels
                                                 where model.GetSize(worldTransform).X <= emptySpace
                                                 orderby rand.Next()
                                                 select model;
            if (modelCandidates.Any())
            {
                Model usedModel = modelCandidates.First();
                Vector3 usedModelSize = usedModel.GetSize(worldTransform);


                float angle = 0;
                Vector3 position = Vector3.Zero;
                float newOffset = offset + usedModelSize.X;
                switch (borderPosition)
                {
                    case TownQuarterInterfacePosition.Top:
                        position = new Vector3(range.Begin * squareWidth + squareWidth + offset, 0, BlockWidth * squareWidth - squareWidth - usedModelSize.Z);
                        break;
                    case TownQuarterInterfacePosition.Right:
                        position = new Vector3(BlockWidth * squareWidth - squareWidth - usedModelSize.X / 2 + usedModelSize.Z / 2 + +(bitmapSize.Width - 2 * (BlockWidth - 1)) * squareWidth, 0, range.Begin * squareWidth + squareWidth + offset + usedModelSize.X / 2 - usedModelSize.Z / 2);
                        angle = MathHelper.PiOver2;
                        break;
                    case TownQuarterInterfacePosition.Bottom:
                        position = new Vector3(range.Begin * squareWidth + squareWidth + offset, 0, BlockWidth * squareWidth - squareWidth + (bitmapSize.Height - 2 * (BlockWidth - 1)) * squareWidth);
                        break;
                    case TownQuarterInterfacePosition.Left:
                        position = new Vector3(BlockWidth * squareWidth - squareWidth - usedModelSize.X / 2 - usedModelSize.Z / 2, 0, range.Begin * squareWidth + squareWidth + offset + usedModelSize.X / 2 - usedModelSize.Z / 2);
                        angle = MathHelper.PiOver2;
                        break;
                    default:
                        break;
                }

                SpatialObject borderBuilding = new SpatialObject(usedModel, position, angle, worldTransform);
                solidObjects.AddLast(borderBuilding);

                FillEmptyBorderRange(worldTransform, buildingModels, rand, borderPosition, range, newOffset);
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
        private void GenerateRestOfBorderSidewalks(Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges, MapFillType[] mapBitmap, Model sidewalkModel, Matrix worldTransform)
        {
            // Corners
            foreach (Tuple<int, int> p in new Tuple<int, int>[]
            {
                new Tuple<int, int>(BlockWidth - 1, BlockWidth),
                new Tuple<int, int>(BlockWidth - 1, BlockWidth - 1),
                new Tuple<int, int>(BlockWidth, BlockWidth - 1),

                new Tuple<int, int>(bitmapSize.Width - BlockWidth - 1, BlockWidth - 1),
                new Tuple<int, int>(bitmapSize.Width - BlockWidth, BlockWidth - 1),
                new Tuple<int, int>(bitmapSize.Width - BlockWidth, BlockWidth),

                new Tuple<int, int>(bitmapSize.Width - BlockWidth, bitmapSize.Height - BlockWidth - 1),
                new Tuple<int, int>(bitmapSize.Width - BlockWidth, bitmapSize.Height - BlockWidth),
                new Tuple<int, int>(bitmapSize.Width - BlockWidth - 1, bitmapSize.Height - BlockWidth),

                new Tuple<int, int>(BlockWidth, bitmapSize.Height - BlockWidth),
                new Tuple<int, int>(BlockWidth-1, bitmapSize.Height - BlockWidth),
                new Tuple<int, int>(BlockWidth-1, bitmapSize.Height - BlockWidth - 1)
            })
            {
                int x = p.Item1, y = p.Item2;
                mapBitmap[x * bitmapSize.Height + y] = MapFillType.Sidewalk;
                SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(x * squareWidth, 0, y * squareWidth), 0, worldTransform);
                groundObjects.AddLast(sidewalk);
            }

            // Ranges
            foreach (KeyValuePair<TownQuarterInterfacePosition, List<Range>> sideRanges in emptyRanges)
            {
                foreach (Range range in sideRanges.Value)
                {
                    for (int i = 0; i < range.Length; i++)
                    {
                        int x, y;
                        switch (sideRanges.Key)
                        {
                            case TownQuarterInterfacePosition.Top:
                                x = range.Begin + i;
                                y = BlockWidth - 1;
                                break;
                            case TownQuarterInterfacePosition.Right:
                                x = bitmapSize.Width - BlockWidth;
                                y = range.Begin + i;
                                break;
                            case TownQuarterInterfacePosition.Bottom:
                                x = range.Begin + i;
                                y = bitmapSize.Height - BlockWidth;
                                break;
                            case TownQuarterInterfacePosition.Left:
                                x = BlockWidth - 1;
                                y = range.Begin + i;
                                break;
                            default:
                                throw new InvalidOperationException("Unknown TownQuarterInterfacePosition value.");
                        }

                        mapBitmap[x * bitmapSize.Height + y] = MapFillType.Sidewalk;
                        SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(x * squareWidth, 0, y * squareWidth), 0, worldTransform);
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
        private void GenerateMapPicture(GraphicsDevice graphicsDevice, MapFillType[] mapBitmap)
        {
            const int namePixelHeight = 14;
            System.Drawing.Bitmap mapPicture = new System.Drawing.Bitmap(bitmapSize.Width * PictureMapRoadWidth,bitmapSize.Height * PictureMapRoadWidth + namePixelHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(mapPicture);
            for (int x = 0; x < bitmapSize.Width; x++)
            {
                for (int y = 0; y < bitmapSize.Height; y++)
                {
                    System.Drawing.Color c = System.Drawing.Color.Green;
                    switch (mapBitmap[x * bitmapSize.Height + y])
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
                    }
                    g.FillRectangle(new System.Drawing.SolidBrush(c), x * PictureMapRoadWidth, y * PictureMapRoadWidth, PictureMapRoadWidth, PictureMapRoadWidth);
                }
            }

            System.Drawing.Font font = new System.Drawing.Font("Tahoma", namePixelHeight);
            g.DrawString(Name, font, System.Drawing.Brushes.Black, 0, 0);

            foreach (SpatialObject obj in solidObjects)
            {
                System.Drawing.PointF[] points = new System.Drawing.PointF[]
                {
                    (obj.UpperLeftCorner * ( PictureMapRoadWidth / squareWidth ) ).ToPointF(),
                    (obj.UpperRightCorner * ( PictureMapRoadWidth / squareWidth ) ).ToPointF(),
                    (obj.LowerRightCorner * ( PictureMapRoadWidth / squareWidth ) ).ToPointF(),
                    (obj.LowerLeftCorner * ( PictureMapRoadWidth / squareWidth ) ).ToPointF()
                };
                g.FillPolygon(System.Drawing.Brushes.SaddleBrown, points);
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
        private List<Rectangle> GenerateInnerRoadNetwork(ref Matrix worldTransform, Model roadModel, Rectangle emptyRectangle, MapFillType[] mapBitmap)
        {
            List<Rectangle> result = new List<Rectangle>();
            int erSize = Math.Max(emptyRectangle.Width, emptyRectangle.Height);
            Random rand = new Random();
            double nextSplittingProbability = rand.NextDouble();
            if (nextSplittingProbability * erSize > 2 * BlockWidth + 1) // Splitting isn't sure when it's possible.
            {
                Rectangle[] newEmptyRectangles = AddSplittingRoad(ref emptyRectangle, roadModel, ref worldTransform, mapBitmap);
                foreach (Rectangle emptyRect in newEmptyRectangles)
                {
                    ///TODO: This can be faster. Using linked list simply join results...
                    result.AddRange(
                        GenerateInnerRoadNetwork(ref worldTransform, roadModel, emptyRect, mapBitmap)
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
        private Rectangle[] AddSplittingRoad(ref Rectangle target, Model roadModel, ref Matrix worldTransform, MapFillType[] mapBitmap)
        {
            AxisDirection direction = AxisDirection.Vertical;
            if (target.Width < target.Height)
                direction = AxisDirection.Horizontal;

            int size = (direction == AxisDirection.Horizontal ? target.Height : target.Width);
            int secondDimensionSize = (direction == AxisDirection.Horizontal ? target.Width : target.Height);

            Random rand = new Random();
            int splitPosition = (int)(
                (0.35 + rand.NextDouble() * 0.3) //percentage side
                * (size - 1));

            for (int i = 0; i < secondDimensionSize; i++)
            {
                Vector3 position = Vector3.Zero;
                int bitmapIndex = -1;
                switch (direction)
                {
                    case AxisDirection.Horizontal:
                        position = new Vector3((i + target.X) * squareWidth, 0, (splitPosition + target.Y) * squareWidth);
                        bitmapIndex = (i + target.X) * bitmapSize.Height + (splitPosition + target.Y);
                        break;
                    case AxisDirection.Vertical:
                        position = new Vector3((splitPosition + target.X) * squareWidth, 0, (i + target.Y) * squareWidth);
                        bitmapIndex = (splitPosition + target.X) * bitmapSize.Height + (i + target.Y);
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
            if (direction == AxisDirection.Horizontal)
            {
                emptyRectangles[0] = new Rectangle(target.X, target.Y, target.Width, splitPosition);
                emptyRectangles[1] = new Rectangle(target.X, target.Y + splitPosition + 1, target.Width, target.Height - splitPosition - 1);
            }
            else if (direction == AxisDirection.Vertical)
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
        private Rectangle GenerateBorderRoads(ref Vector2 size, ref Matrix worldTransform, Model roadModel, MapFillType[] mapBitmap)
        {
            int xOffset = BlockWidth;
            int yOffset = BlockWidth;
            int xCount = bitmapSize.Width - 2 * xOffset;
            int yCount = bitmapSize.Height - 2 * yOffset;

            for (int x = 0; x < xCount; x++)
            {
                int X = x + xOffset;
                foreach (int y in new int[] { 0, yCount - 1 })
                {
                    int Y = y + yOffset;
                    mapBitmap[X * bitmapSize.Height + Y] = MapFillType.StraightRoad;
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(X * squareWidth, 0, Y * squareWidth), 0, worldTransform);
                    groundObjects.AddLast(road);
                }
            }
            for (int y = 1; y < yCount - 1; y++)
            {
                int Y = y + yOffset;
                foreach (int x in new int[] { 0, xCount - 1 })
                {
                    int X = x + xOffset;
                    mapBitmap[X * bitmapSize.Height + Y] = MapFillType.StraightRoad;
                    SpatialObject road = new SpatialObject(roadModel, new Vector3(X * squareWidth, 0, Y * squareWidth), 0, worldTransform);
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
        private Rectangle GenerateSidewalks(Rectangle target, Model sidewalkModel, ref Matrix worldTransform, MapFillType[] mapBitmap)
        {
            for (int x = 0; x < target.Width; x++)
            {
                foreach (int y in new int[] { 0, target.Height - 1 })
                {
                    int X = target.X + x;
                    int Y = target.Y + y;
                    mapBitmap[X * bitmapSize.Height + Y] = MapFillType.Sidewalk;
                    SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(X * squareWidth, 0, Y * squareWidth), 0, worldTransform);
                    groundObjects.AddFirst(sidewalk);
                }
            }
            for (int y = 1; y < (target.Height - 1); y++)
            {
                foreach (int x in new int[] { 0, target.Width - 1 })
                {
                    int X = target.X + x;
                    int Y = target.Y + y;
                    mapBitmap[X * bitmapSize.Height + Y] = MapFillType.Sidewalk;
                    SpatialObject sidewalk = new SpatialObject(sidewalkModel, new Vector3(X * squareWidth, 0, Y * squareWidth), 0, worldTransform);
                    groundObjects.AddFirst(sidewalk);
                }
            }

            return new Rectangle(target.X + 1, target.Y + 1, target.Width - 2, target.Height - 2);
        }

    }
}
