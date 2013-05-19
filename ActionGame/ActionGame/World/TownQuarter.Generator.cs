using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActionGame.Space;
using ActionGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ActionGame.Tasks;
using ActionGame.People;
using ActionGame.Exceptions;
using ActionGame.QSP;
using ActionGame.Tools;
using ActionGame.Objects;
using Microsoft.Xna.Framework.Audio;

namespace ActionGame.World
{
    public partial class TownQuarter : IDisposable
    {

        private void Generate(int degree)
        {
            ///TODO: Use bitmap to deny X crossroads.

            ///TODO: Load textures from central repository
            Texture2D roadTexture = game.Content.Load<Texture2D>("Textures/Ground/road0");
            Texture2D sidewalkTexture = game.Content.Load<Texture2D>("Textures/Ground/sidewalk0");
            Texture2D grassTexture = game.Content.Load<Texture2D>("Textures/Ground/grass0");


            if (bitmapSize.Width < 4 * BlockWidth || bitmapSize.Height < 4 * BlockWidth)
            {
                throw new ArgumentOutOfRangeException("Specified size is to small.");
            }

            GenerateInterfaces(degree, roadTexture, sidewalkTexture);

            Rectangle emptyRectangleInBorderRoadCyrcle = GenerateBorderRoads(roadTexture);

            List<Rectangle> emptyRectanglesInsideRoads = GenerateInnerRoadNetwork(roadTexture, emptyRectangleInBorderRoadCyrcle);
            List<Rectangle> emptyRectaglesInsideSidewalks = new List<Rectangle>(emptyRectanglesInsideRoads.Count);
            foreach (Rectangle emptyRect in emptyRectanglesInsideRoads)
            {
                emptyRectaglesInsideSidewalks.Add(
                    GenerateSidewalks(emptyRect, sidewalkTexture)
                );
            }
            IList<PathGraphVertex> pathVertecies = GeneratePathGraph(emptyRectanglesInsideRoads);

            ///TODO: This test cas slow generation process.
            if (!PathGraph.IsConnected(pathVertecies))
            {
                throw new PathGraphNotConnectedException("Path graph generated inside this town quarter has two or more components.");
            }
            
            foreach (PathGraphVertex v in pathVertecies)
            {
                spaceGrid.AddPathGraphVertex(v);
                //showing path graph
                /*
                const float pointHeight = 0.01f;
                Plate vplate = new Plate(
                        this,
                        v.Position.PositionInQuarter.Go(0.2f, 0).ToVector3(pointHeight),
                        v.Position.PositionInQuarter.Go(0.2f, MathHelper.PiOver2).ToVector3(pointHeight),
                        v.Position.PositionInQuarter.Go(0.2f, -MathHelper.PiOver2).ToVector3(pointHeight),
                        v.Position.PositionInQuarter.Go(0.2f, MathHelper.Pi).ToVector3(pointHeight),
                        game.Content.Load<Texture2D>("Textures/blue"),
                        game.Content.Load<Texture2D>("Textures/blue"));
                magicPlates.AddLast(vplate);
                foreach (var n in v.Neighbors)
                { 
                    const float height = 0.3f;//m
                    Plate plate = new Plate(
                        this,
                        v.Position.PositionInQuarter.ToVector3(height),
                        n.Position.PositionInQuarter.ToVector3(height),
                        v.Position.PositionInQuarter.ToVector3(0),
                        n.Position.PositionInQuarter.ToVector3(0),
                        game.Content.Load<Texture2D>("Textures/blue"),
                        game.Content.Load<Texture2D>("Textures/blue"));
                    magicPlates.AddLast(plate);
                }
                */
            }
            pathGraph = pathVertecies;

            

            GenerateGrass(grassTexture, emptyRectaglesInsideSidewalks);

            GenerateBuildings(emptyRectaglesInsideSidewalks);
            BuildFlag();
            GenerateMapPicture();
            GenerateRoadSignPicture();
            GenerateBoxes();
            pathGraph = pathVertecies;
            GenerateWalkers();
        }

        private Box[] GenerateBoxes()
        {
            Box[] addedBoxes = new Box[game.Settings.AmmoBoxCount + game.Settings.HealBoxCount];
            int ai = 0;
            HashSet<Point> occupiedPositions = new HashSet<Point>();
            if (game.BoxDefaultGuns.Count != 0)
            {
                for (int i = 0; i < game.Settings.AmmoBoxCount; i++)
                {
                    Point p = GetRandomSquare(pos => mapBitmap.Index2D(bitmapSize.Height, pos.X, pos.Y) == MapFillType.Sidewalk && !occupiedPositions.Contains(pos));
                    GunType gunType = game.BoxDefaultGuns[game.Random.Next(game.BoxDefaultGuns.Count)];
                    ToolBox tb = new ToolBox( new Gun(gunType, gunType.DefaultBulletCount),
                        game.Content.Load<SoundEffect>("Sounds/gunLoading"),
                        game.Content.Load<Model>("Objects/Decorations/ammoBox"),
                        new PositionInTown(this, new Vector2(p.X * SquareWidth, p.Y * SquareWidth)),
                        game.Drawer.WorldTransformMatrix
                        );
                    boxes.Add(tb);
                    occupiedPositions.Add(p);
                    addedBoxes[ai++] = tb;
                }
            }

            for (int i = 0; i < game.Settings.HealBoxCount; i++)
            {
                Point p = GetRandomSquare(pos => mapBitmap.Index2D(bitmapSize.Height, pos.X, pos.Y) == MapFillType.Sidewalk && !occupiedPositions.Contains(pos));
                HealBox hb = new HealBox(game.Random.Next(50,100),
                    game.Content.Load<SoundEffect>("Sounds/heal"),
                    game.Content.Load<Model>("Objects/Decorations/healthBox"),
                    new PositionInTown(this, new Vector2(p.X * SquareWidth, p.Y * SquareWidth)),
                    game.Drawer.WorldTransformMatrix
                    );
                boxes.Add(hb);
                occupiedPositions.Add(p);
                addedBoxes[ai++] = hb;
            }
            return addedBoxes;
        }

        public Point GetRandomSquare(Predicate<MapFillType> where)
        {
            return GetRandomSquare(pos => where(mapBitmap.Index2D(bitmapSize.Height, pos.X, pos.Y)));
        }

        public Point GetRandomSquare(Predicate<Point> where)
        {
            Random rand = new Random();
            int startX = rand.Next(bitmapSize.Width);
            bool firstX = true;
            for (int x = startX; x != startX || firstX; x = (x + 1) % bitmapSize.Width)
            {
                firstX = false;
                int startY = rand.Next(bitmapSize.Height);
                bool firstY = true;
                for (int y = startY; y != startY || firstY; y = (y + 1) % bitmapSize.Height)
                {
                    firstY = false;
                    Point point = new Point(x, y);
                    if (where(point))
                    {
                        return point;
                    }
                }
            }
            return Point.Zero;
        }

        private void GenerateRoadSignPicture()
        {
            //Stuff from Content Manager doesn't need manual disposing.
            Texture2D bgTex = owner.Content.RoadSignTexture;
            using (MemoryStream bgStream = new MemoryStream())
            {
                bgTex.SaveAsPng(bgStream, 768, 256);
                bgStream.Position = 0;
                using (System.Drawing.Image signImg = System.Drawing.Image.FromStream(bgStream))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(signImg))
                    {
                        float em = 14;
                        var font = new System.Drawing.Font("Tahoma", em);
                        while (g.MeasureString(Name, font).Width < 768 - 300)
                        {
                            font = new System.Drawing.Font("Tahoma", em);
                            em++;
                        }
                        g.DrawString(Name, font, System.Drawing.Brushes.White, new System.Drawing.RectangleF(150, 30, 768 - 150, 256 - 30));
                        g.Flush();
                    }
                    using (MemoryStream imgStream = new MemoryStream())
                    {
                        signImg.Save(imgStream, System.Drawing.Imaging.ImageFormat.Png);
                        imgStream.Position = 0;
                        roadSignTexture = Texture2D.FromStream(game.GraphicsDevice, imgStream);
                    }
                }
            }
        }

        private void GenerateGrass(Texture2D grassTexture, List<Rectangle> emptyRectaglesInsideSidewalks)
        {
            float grassWidth = 13.5f; //m
            foreach (Rectangle emptyRect in emptyRectaglesInsideSidewalks)
            {
                int xCount = (int)(emptyRect.Width * SquareWidth / grassWidth);
                int yCount = (int)(emptyRect.Height * SquareWidth / grassWidth);
                Vector2 size = new Vector2(emptyRect.Width * SquareWidth / xCount, emptyRect.Height * SquareWidth / yCount);
                for (int x = 0; x < xCount; x++)
                {
                    for (int y = 0; y < yCount; y++)
                    {
                        Vector2 position = new Vector2(emptyRect.X * SquareWidth + x * size.X, emptyRect.Y * SquareWidth + y * size.Y);
                        FlatObject grass = new FlatObject(new PositionInTown(this, position), 0, size, grassTexture);
                        groundObjects.AddFirst(grass);
                    }
                }
            }
        }

        public IEnumerable<PositionInTown> GetRandomWalkingWaypoints()
        {
            return pathGraph.OrderBy(x => game.Random.Next()).Take(WalkerWayPointCount).Select(x => x.Position);
        }

        void GenerateWalkers()
        {
            if (pathGraph.Count > 2)
            {
                HashSet<Point> points = new HashSet<Point>();
                Model[] models = new Model[]{
                    game.Content.Load<Model>("Objects\\Humans\\human0")
                };
                Point point= GetRandomSquare((Point p) => mapBitmap.Index2D(bitmapSize.Height, p.X, p.Y) == MapFillType.Sidewalk && !points.Contains(p));
                points.Add(point);
                for (int i = 0; i < WalkerCount; i++)
                {
                    IEnumerable<PositionInTown> waypoints = GetRandomWalkingWaypoints();
                    ///TODO: Take human model from central repository.
                    Human walker = new Human(game, models[game.Random.Next(models.Length)], waypoints.First(), 0, game.Drawer.WorldTransformMatrix);
                    InfinityWalkingTask task = new InfinityWalkingTask(walker, waypoints);
                    walker.AddTask(task);
                    walkers.Add(walker);
                }
            }
        }


        IList<PathGraphVertex> GeneratePathGraph(List<Rectangle> emptyRectanglesInsideRoads)
        {
            List<Point> pointsOfInterests = new List<Point>();
            foreach (Rectangle rect in emptyRectanglesInsideRoads)
            {
                pointsOfInterests.AddRange( new Point[] {
                    new Point(rect.X, rect.Y),
                    //new Point(rect.X + rect.Width - 1, rect.Y),
                    //new Point(rect.X, rect.Y + rect.Height - 1),
                    new Point(rect.X + rect.Width - 1, rect.Y + rect.Height - 1)
                    });
            }

            Dictionary<Point, Tuple<TownQuarterInterface, bool>> interfaceByPoint = new Dictionary<Point, Tuple<TownQuarterInterface, bool>>();//tuple: interface, isLeftPoint
            foreach (var iface in interfaces)
            {
                Point left, right;
                switch (iface.SidePosition)
                {
                    case TownQuarterInterfacePosition.Top:
                        left.X = iface.BitmapPosition - 1;
                        left.Y = 0;
                        right.X = iface.BitmapPosition + 1;
                        right.Y = 0;
                        break;
                    case TownQuarterInterfacePosition.Right:
                        left.X = bitmapSize.Width - 1;
                        left.Y = iface.BitmapPosition - 1;
                        right.X = bitmapSize.Width - 1;
                        right.Y = iface.BitmapPosition + 1;
                        break;
                    case TownQuarterInterfacePosition.Bottom:
                        left.X = iface.BitmapPosition + 1;
                        left.Y = bitmapSize.Height - 1;
                        right.X = iface.BitmapPosition - 1;
                        right.Y = bitmapSize.Height - 1;
                        break;
                    case TownQuarterInterfacePosition.Left:
                        left.X = 0;
                        left.Y = iface.BitmapPosition + 1;
                        right.X = 0;
                        right.Y = iface.BitmapPosition - 1;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown SidePosition value.");
                }
                pointsOfInterests.Add(left);
                pointsOfInterests.Add(right);
                interfaceByPoint.Add(left, new Tuple<TownQuarterInterface, bool>(iface, true));
                interfaceByPoint.Add(right, new Tuple<TownQuarterInterface, bool>(iface, false));
            }

            pointsOfInterests.AddRange(new Point[] {
                new Point(BlockWidth - 1, BlockWidth - 1),
                new Point(bitmapSize.Width - BlockWidth, bitmapSize.Height - BlockWidth)
            });
            
            SortedSet<int> xCoordinates = new SortedSet<int>();
            SortedSet<int> yCoordinates = new SortedSet<int>();
            foreach (Point point in pointsOfInterests)
            {
                if (!xCoordinates.Contains(point.X))
                    xCoordinates.Add(point.X);
                if (!yCoordinates.Contains(point.Y))
                    yCoordinates.Add(point.Y);
            }

            List<PathGraphVertex> innerVertices = new List<PathGraphVertex>();
            Dictionary<int, SortedDictionary<int, PathGraphVertex>> verticalIndexedPaths = new Dictionary<int, SortedDictionary<int, PathGraphVertex>>(yCoordinates.Count);
            Dictionary<int, SortedDictionary<int, PathGraphVertex>> horizontalIndexedPaths = new Dictionary<int, SortedDictionary<int, PathGraphVertex>>(xCoordinates.Count);
            foreach (int y in yCoordinates)
            {
                verticalIndexedPaths.Add(y, new SortedDictionary<int, PathGraphVertex>());
            }
            foreach (int x in xCoordinates)
            {
                horizontalIndexedPaths.Add(x, new SortedDictionary<int, PathGraphVertex>());
                foreach (int y in yCoordinates)
                {
                    if (mapBitmap.Index2D(bitmapSize.Height, x, y) == MapFillType.Sidewalk)
                    {
                        PathGraphVertex vertex = new PathGraphVertex(new PositionInTown(this, new Vector2(x + 0.5f, y + 0.5f) * SquareWidth));
                        innerVertices.Add(vertex);
                        verticalIndexedPaths[y].Add(x, vertex);
                        horizontalIndexedPaths[x].Add(y, vertex);

                        Point p = new Point(x, y);
                        if (interfaceByPoint.ContainsKey(p))
                        {
                            if (interfaceByPoint[p].Item2)
                            {
                                interfaceByPoint[p].Item1.LeftPathGraphVertex = vertex;
                            }
                            else
                            {
                                interfaceByPoint[p].Item1.RightPathGraphVertex = vertex;
                            }
                        }
                    }
                }
            }

            SweepPathVertices(verticalIndexedPaths, AxisDirection.Horizontal);
            SweepPathVertices(horizontalIndexedPaths,  AxisDirection.Vertical);

            return innerVertices;
        }

        private void SweepPathVertices(Dictionary<int, SortedDictionary<int, PathGraphVertex>> vertexLines, AxisDirection sweepDirection)
        { 
            foreach(KeyValuePair<int, SortedDictionary<int, PathGraphVertex>> vertexLine in vertexLines)
            {
                using (SortedDictionary<int, PathGraphVertex>.Enumerator enumerator = vertexLine.Value.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        int x1, y1, x2, y2;
                        x1 = x2 = y1 = y2 = 0;
                        switch (sweepDirection)
                        {
                            case AxisDirection.Horizontal:
                                y1 = y2 = vertexLine.Key;
                                x1 = enumerator.Current.Key;
                                break;
                            case AxisDirection.Vertical:
                                x1 = x2 = vertexLine.Key;
                                y1 = enumerator.Current.Key;
                                break;
                            default:
                                throw new InvalidOperationException("Unknown AxisDirection.");
                        }
                        PathGraphVertex vertex1 = enumerator.Current.Value;
                        while (enumerator.MoveNext())
                        {
                            PathGraphVertex vertex2 = enumerator.Current.Value;
                            switch (sweepDirection)
                            {
                                case AxisDirection.Horizontal:
                                    x2 = enumerator.Current.Key;
                                    break;
                                case AxisDirection.Vertical:
                                    y2 = enumerator.Current.Key;
                                    break;
                                default:
                                    throw new InvalidOperationException("Unknown AxisDirection.");
                            }
                            //Special foreach begin
                            bool clearForWalking = true;
                            int roadCount = 0;
                            switch (sweepDirection)
                            {
                                case AxisDirection.Horizontal:
                                    for (int i = x1 + 1; i < x2; i++)
                                    {
                                        if (mapBitmap.Index2D(bitmapSize.Height, i, y1) == MapFillType.Empty)
                                        {
                                            clearForWalking = false;
                                            break;
                                        }
                                        else if (mapBitmap.Index2D(bitmapSize.Height, i, y1) != MapFillType.Sidewalk)
                                        {
                                            roadCount++;
                                            if (roadCount > 1)
                                            {
                                                clearForWalking = false;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                case AxisDirection.Vertical:
                                    for (int i = y1 + 1; i < y2; i++)
                                    {
                                        if (mapBitmap.Index2D(bitmapSize.Height, x1, i) == MapFillType.Empty)
                                        {
                                            clearForWalking = false;
                                            break;
                                        }
                                        else if (mapBitmap.Index2D(bitmapSize.Height, x1, i) != MapFillType.Sidewalk)
                                        {
                                            roadCount++;
                                            if (roadCount > 1)
                                            {
                                                clearForWalking = false;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException("Unknown AxisDirection.");
                            }
                            if (clearForWalking)
                            {
                                vertex1.AddNeighborBothDirection(vertex2, vertex1.Position.MinimalDistanceTo(vertex2.Position));
                            }
                            //Special foreach end
                            vertex1 = vertex2;
                            switch (sweepDirection)
                            {
                                case AxisDirection.Horizontal:
                                    x1 = x2;
                                    break;
                                case AxisDirection.Vertical:
                                    y1 = y2;
                                    break;
                                default:
                                    throw new InvalidOperationException("Unknown AxisDirection.");
                            }
                        }
                    }
                }
            }
        }

        private void GenerateBuildings(List<Rectangle> emptyRectaglesInsideSidewalks)
        {
            Model[] buildingModels = game.ContentRepository.InnerBuildings;
            foreach (Rectangle emptyRect in emptyRectaglesInsideSidewalks)
            {
                float realWidth = emptyRect.Width * SquareWidth,
                    realHeight = emptyRect.Height * SquareWidth;

                RectangleF realEmptyRectangle = new RectangleF(emptyRect.X * SquareWidth, emptyRect.Y * SquareWidth, realWidth, realHeight);
                FillByBuildings(buildingModels, realEmptyRectangle);
            }
        }

        private void FillByBuildings(Model[] buildingModels, RectangleF target)
        {
            Matrix worldTransform = game.Drawer.WorldTransformMatrix;
            Random rand = game.Random;
            float horizontalSpace = (float)(rand.NextDouble() * 0.7 + 0.3) * BetweenBuildingSpace;
            float verticalSpace = (float)(rand.NextDouble() * 0.7 + 0.3) * BetweenBuildingSpace;


            IEnumerable<Model> modelCandidates = from model in buildingModels
                                                 where model.GetSize(worldTransform).X + horizontalSpace <= target.Width && model.GetSize(worldTransform).Z + verticalSpace <= target.Height
                                                 orderby -(rand.NextDouble(0.4,1.0) * model.GetSize(worldTransform).X * model.GetSize(worldTransform).Z)
                                                 select model;
            if (modelCandidates.Any())
            {
                Model usedModel = modelCandidates.First();

                SpatialObject building = new SpatialObject(
                    usedModel,
                    this,
                    new Vector3(target.X + horizontalSpace / 2f, 0, target.Y + verticalSpace / 2f),
                    0,
                    worldTransform);
                solidObjects.AddLast(building);
                RectangleF nextRect = new RectangleF(
                    target.X + usedModel.GetSize(worldTransform).X + horizontalSpace,
                    target.Y,
                    target.Width - usedModel.GetSize(worldTransform).X - horizontalSpace,
                    usedModel.GetSize(worldTransform).Z + verticalSpace);

                RectangleF downRect = new RectangleF(
                    target.X,
                    target.Y + usedModel.GetSize(worldTransform).Z + verticalSpace,
                    target.Width,
                    target.Height - usedModel.GetSize(worldTransform).Z - verticalSpace);

                FillByBuildings(buildingModels, nextRect);
                FillByBuildings(buildingModels, downRect);
            }
        }

        /// <summary>
        /// Generates road and sidewalks interfaces - street for joining with other quarters.
        /// </summary>
        /// <param name="degree">Number of'quarters neigbors - degree of vertex</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="roadTexture">Used road model</param>
        /// <param name="sidewalkTexture">Used sidewalk model</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="content">Content manager for loading models</param>
        private void GenerateInterfaces(int degree, Texture2D roadTexture, Texture2D sidewalkTexture)
        {
            Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges = new Dictionary<TownQuarterInterfacePosition, List<Range>>(4);
            emptyRanges.Add(TownQuarterInterfacePosition.Top, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Width - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Bottom, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Width - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Left, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Height - BlockWidth - 1) }));
            emptyRanges.Add(TownQuarterInterfacePosition.Right, new List<Range>(new Range[] { new Range(BlockWidth + 1, bitmapSize.Height - BlockWidth - 1) }));

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

                side = possibleSides[game.Random.Next(0, possibleSides.Count - 1)];

                List<Range> possibleRanges = emptyRanges[side].FindAll(rangeItem => rangeItem.Length > 2 * BlockWidth + 1);

                int rangeIndex = game.Random.Next(0, possibleRanges.Count - 1);
                Range range = possibleRanges[rangeIndex];
                emptyRanges[side].Remove(range);

                int position = range.Begin
                    + (int)((0.35 + game.Random.NextDouble() * 0.3) // percentage position in range
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
                    FlatObject road = new FlatObject(new PositionInTown(this, new Vector2(rx * SquareWidth, ry * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), roadTexture);
                    FlatObject sidewalkL = new FlatObject(new PositionInTown(this, new Vector2(slx * SquareWidth, sly * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), sidewalkTexture);
                    FlatObject sidewalkR = new FlatObject(new PositionInTown(this, new Vector2(srx * SquareWidth, sry * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), sidewalkTexture);
                    groundObjects.AddLast(road);
                    groundObjects.AddLast(sidewalkL);
                    groundObjects.AddLast(sidewalkR);
                }

                //wall generator
                {
                    ///TODO: load textures from central repository
                    Texture2D wallTexture = game.Content.Load<Texture2D>("Textures/Spatial/wall0");
                    float wallWidth = 6f; //m
                    const float wallHeight = 4.5f;
                    int count = (int)((BlockWidth - 1) * SquareWidth / wallWidth); //minus sidewalk
                    wallWidth = (BlockWidth - 1) * SquareWidth / count;
                    const float ifaceWallEpsilon = 0.006f; // 3mm
                    for (int p = 0; p < count; p++)
                    {
                        Vector2 beginL, endL, beginR, endR;
                        switch (direction)
                        {
                            case AxisDirection.Horizontal:
                                beginL.X = p * wallWidth + (side == TownQuarterInterfacePosition.Left ? 0 : bitmapSize.Width - BlockWidth + 1) * SquareWidth;
                                beginL.Y = (position - 1) * SquareWidth + ifaceWallEpsilon;
                                endL = beginL;
                                endL.X += wallWidth;

                                beginR.X = p * wallWidth + (side == TownQuarterInterfacePosition.Left ? 0 : bitmapSize.Width - BlockWidth + 1) * SquareWidth;
                                beginR.Y = (position + 2) * SquareWidth - ifaceWallEpsilon;
                                endR = beginR;
                                endR.X += wallWidth;
                                break;
                            case AxisDirection.Vertical:
                                beginL.Y = p * wallWidth + (side == TownQuarterInterfacePosition.Top ? 0 : bitmapSize.Height - BlockWidth + 1) * SquareWidth;
                                beginL.X = (position - 1) * SquareWidth + ifaceWallEpsilon;
                                endL = beginL;
                                endL.Y += wallWidth;

                                beginR.Y = p * wallWidth + (side == TownQuarterInterfacePosition.Top ? 0 : bitmapSize.Height - BlockWidth + 1) * SquareWidth;
                                beginR.X = (position + 2) * SquareWidth - ifaceWallEpsilon;
                                endR = beginR;
                                endR.Y += wallWidth;
                                break;
                            default:
                                throw new InvalidOperationException("Unknown AxisDirection value.");
                        }
                        Plate wallL = new Plate(this, beginL.ToVector3(wallHeight), endL.ToVector3(wallHeight), beginL.ToVector3(0), endL.ToVector3(0), wallTexture, wallTexture, true);
                        solidPlates.AddLast(wallL);
                        Plate wallr = new Plate(this, beginR.ToVector3(wallHeight), endR.ToVector3(wallHeight), beginR.ToVector3(0), endR.ToVector3(0), wallTexture, wallTexture, true);
                        solidPlates.AddLast(wallr);
                    }
                }

                interfaces.Add(iface);
            }

            GenerateRestOfBorderSidewalks(emptyRanges, sidewalkTexture);
            GenerateBorderBuildings(emptyRanges);
        }

        private void GenerateBorderBuildings(Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges)
        {
            //expand corner ranges
            foreach (KeyValuePair<TownQuarterInterfacePosition, List<Range>> ranges in emptyRanges)
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
                        (ranges.Value[i].End== bitmapSize.Width - BlockWidth - 1 && (ranges.Key == TownQuarterInterfacePosition.Top || ranges.Key == TownQuarterInterfacePosition.Bottom))
                        || (ranges.Value[i].End== bitmapSize.Height - BlockWidth - 1 && (ranges.Key == TownQuarterInterfacePosition.Left || ranges.Key == TownQuarterInterfacePosition.Right))
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

            Model[] buildingModels = game.ContentRepository.BorderBuildings;
            //sort by size X desc
            Array.Sort(buildingModels, (x, y) => -(x.GetSize(game.Drawer.WorldTransformMatrix).X.CompareTo(y.GetSize(game.Drawer.WorldTransformMatrix).X)));
            foreach (KeyValuePair<TownQuarterInterfacePosition, List<Range>> ranges in emptyRanges)
            {
                foreach (Range range in ranges.Value)
                {
                    FillEmptyBorderRange(buildingModels, ranges.Key, range, 0f, false);
                }
            }
        }

        /// <summary>
        /// Fills empty range of town quarter border by buildings. It takes it by dimension X - first fit.
        /// </summary>
        /// <param name="buildingModels">Availible models</param>
        /// <param name="borderPosition">Position of border - specifies side of rectangle</param>
        /// <param name="range">Empty range for filling</param>
        /// <param name="offset">Already filled part of range</param>
        private int FillEmptyBorderRange(Model[] buildingModels, TownQuarterInterfacePosition borderPosition, Range range, float offset, bool lastOne)
        {
            float emptySpace = (range.Length - 1) * SquareWidth - offset;
            foreach(Model model in buildingModels)
            {
                if (model.GetSize(game.Drawer.WorldTransformMatrix).X <= emptySpace)
                {
                    Model usedModel = model;
                    Vector3 usedModelSize = usedModel.GetSize(game.Drawer.WorldTransformMatrix);

                    float angle = 0;
                    Vector3 position = Vector3.Zero;
                    float newOffset = offset + usedModelSize.X;
                    switch (borderPosition)
                    {
                        case TownQuarterInterfacePosition.Top:
                            position = new Vector3(range.Begin * SquareWidth + SquareWidth + offset, 0, BlockWidth * SquareWidth - SquareWidth - usedModelSize.Z);
                            break;
                        case TownQuarterInterfacePosition.Right:
                            position = new Vector3(BlockWidth * SquareWidth - SquareWidth - usedModelSize.X / 2 + usedModelSize.Z / 2 + +(bitmapSize.Width - 2 * (BlockWidth - 1)) * SquareWidth, 0, range.Begin * SquareWidth + SquareWidth + offset + usedModelSize.X / 2 - usedModelSize.Z / 2);
                            angle = MathHelper.PiOver2;
                            break;
                        case TownQuarterInterfacePosition.Bottom:
                            position = new Vector3(range.Begin * SquareWidth + SquareWidth + offset, 0, BlockWidth * SquareWidth - SquareWidth + (bitmapSize.Height - 2 * (BlockWidth - 1)) * SquareWidth);
                            break;
                        case TownQuarterInterfacePosition.Left:
                            position = new Vector3(BlockWidth * SquareWidth - SquareWidth - usedModelSize.X / 2 - usedModelSize.Z / 2, 0, range.Begin * SquareWidth + SquareWidth + offset + usedModelSize.X / 2 - usedModelSize.Z / 2);
                            angle = MathHelper.PiOver2;
                            break;
                        default:
                            break;
                    }

                    SpatialObject borderBuilding = new SpatialObject(usedModel, this, position, angle, game.Drawer.WorldTransformMatrix);
                    solidObjects.AddLast(borderBuilding);
                    int recRes = 0;
                    if (!lastOne)
                    {
                        recRes = FillEmptyBorderRange(buildingModels, borderPosition, range, newOffset, false);
                    }
                    if (recRes == 0 && !lastOne)
                    { 
                        Range overflowedRange = range;
                        overflowedRange.End++;
                        recRes = FillEmptyBorderRange(new Model[] {buildingModels.Last()}, borderPosition, overflowedRange, newOffset, true);
                    }
                    return recRes + 1;
                    //break;
                }
            }
            return 0;
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
        private void GenerateRestOfBorderSidewalks(Dictionary<TownQuarterInterfacePosition, List<Range>> emptyRanges, Texture2D sidewalkTexture)
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
                FlatObject sidewalk = new FlatObject(new PositionInTown(this, new Vector2(x * SquareWidth, y * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), sidewalkTexture);
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
                        FlatObject sidewalk = new FlatObject(new PositionInTown(this, new Vector2(x * SquareWidth, y * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), sidewalkTexture);
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
        private void GenerateMapPicture()
        {
            const int namePixelHeight = 14;
            System.Drawing.Bitmap mapPicture = new System.Drawing.Bitmap(bitmapSize.Width * PictureMapRoadWidth,bitmapSize.Height * PictureMapRoadWidth + namePixelHeight);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(mapPicture))
            {
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
                    System.Drawing.PointF[] points = new System.Drawing.PointF[] { (obj.UpperLeftCorner * (PictureMapRoadWidth / SquareWidth)).ToPointF(), (obj.UpperRightCorner * (PictureMapRoadWidth / SquareWidth)).ToPointF(), (obj.LowerRightCorner * (PictureMapRoadWidth / SquareWidth)).ToPointF(), (obj.LowerLeftCorner * (PictureMapRoadWidth / SquareWidth)).ToPointF() };
                    g.FillPolygon(System.Drawing.Brushes.SaddleBrown, points);
                }

                System.Drawing.PointF flagPos = (flag.Pivot.PositionInQuarter * (PictureMapRoadWidth / SquareWidth)).ToPointF();
                g.FillEllipse(System.Drawing.Brushes.White, flagPos.X - 5, flagPos.Y - 5,10 ,10);
                g.DrawEllipse(System.Drawing.Pens.Black, flagPos.X - 5, flagPos.Y - 5, 10, 10);
                g.FillEllipse(System.Drawing.Brushes.Black, flagPos.X - 1, flagPos.Y - 1, 2, 2);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                mapPicture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                map = Texture2D.FromStream(game.GraphicsDevice, ms);
            }
        }

        /// <summary>
        /// Generates inner road network into target rectangle. Runs recursively. Exponential.
        /// </summary>
        /// <param name="roadTexture">Road square model</param>
        /// <param name="roadModelWidth">Road square size</param>
        /// <param name="emptyRectangle">Target rectangle</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Quarter bitmap width</param>
        /// <param name="ySize">Quarter bitmap height</param>
        /// <returns>Another list of empty rectangles for next filling</returns>
        private List<Rectangle> GenerateInnerRoadNetwork(Texture2D roadTexture, Rectangle emptyRectangle)
        {
            List<Rectangle> result = new List<Rectangle>();
            int erSize = Math.Max(emptyRectangle.Width, emptyRectangle.Height);
            Random rand = new Random();
            double nextSplittingProbability = rand.NextDouble();
            if (nextSplittingProbability * erSize > 2 * BlockWidth + 1) // Splitting isn't sure when it's possible.
            {
                Rectangle[] newEmptyRectangles = AddSplittingRoad(ref emptyRectangle, roadTexture);
                foreach (Rectangle emptyRect in newEmptyRectangles)
                {
                    ///TODO: This can be faster. Using linked list simply join results...
                    result.AddRange(
                        GenerateInnerRoadNetwork(roadTexture, emptyRect)
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
        /// <param name="roadTexture">Road model</param>
        /// <param name="roadModelWidth">Road model size</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <param name="xSize">Quarter bitmap width</param>
        /// <param name="ySize">Quarter bitmap height</param>
        /// <returns>Array of new empty rectangles inside the splited rectangle</returns>
        private Rectangle[] AddSplittingRoad(ref Rectangle target, Texture2D roadTexture)
        {
            AxisDirection direction;
            if (target.Width < target.Height)
                direction = AxisDirection.Horizontal;
            else
                direction = AxisDirection.Vertical;

            int size = (direction == AxisDirection.Horizontal ? target.Height : target.Width);
            int secondDimensionSize = (direction == AxisDirection.Horizontal ? target.Width : target.Height);

            Random rand = new Random();
            int splitPosition = (int)(
                (0.35 + rand.NextDouble() * 0.3) //percentage side
                * (size - 1));

            for (int i = 0; i < secondDimensionSize; i++)
            {
                Vector2 position = Vector2.Zero;
                int bitmapIndex = -1;
                switch (direction)
                {
                    case AxisDirection.Horizontal:
                        position = new Vector2((i + target.X) * SquareWidth, (splitPosition + target.Y) * SquareWidth);
                        bitmapIndex = (i + target.X) * bitmapSize.Height + (splitPosition + target.Y);
                        break;
                    case AxisDirection.Vertical:
                        position = new Vector2((splitPosition + target.X) * SquareWidth, (i + target.Y) * SquareWidth);
                        bitmapIndex = (splitPosition + target.X) * bitmapSize.Height + (i + target.Y);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown AxisDirection for splitting.");
                }

                mapBitmap[bitmapIndex] = MapFillType.StraightRoad;
                FlatObject road = new FlatObject(new PositionInTown(this, position), 0, new Vector2(SquareWidth, SquareWidth), roadTexture);
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
        /// <param name="roadTexture">Road texture</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <returns>Empty rectangle inside the bitmap</returns>
        private Rectangle GenerateBorderRoads(Texture2D roadTexture)
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
                    FlatObject road = new FlatObject(new PositionInTown(this, new Vector2(X * SquareWidth, Y * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), roadTexture);
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
                    FlatObject road = new FlatObject(new PositionInTown(this, new Vector2(X * SquareWidth, Y * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), roadTexture);
                    groundObjects.AddLast(road);
                }
            }
            return new Rectangle(xOffset + 1, yOffset + 1, xCount - 2, yCount - 2);
        }

        /// <summary>
        /// Generates sidewalk border inside the target rectangle.
        /// </summary>
        /// <param name="target">Target side rectangle</param>
        /// <param name="sidewalkTexture">Used texture</param>
        /// <param name="worldTransform">World transform matrix</param>
        /// <param name="mapBitmap">Quarter bitmap</param>
        /// <returns>Rest of the target rectangle what's empty</returns>
        private Rectangle GenerateSidewalks(Rectangle target, Texture2D sidewalkTexture)
        {
            for (int x = 0; x < target.Width; x++)
            {
                foreach (int y in new int[] { 0, target.Height - 1 })
                {
                    int X = target.X + x;
                    int Y = target.Y + y;
                    mapBitmap[X * bitmapSize.Height + Y] = MapFillType.Sidewalk;
                    FlatObject sidewalk = new FlatObject(new PositionInTown(this, new Vector2(X * SquareWidth, Y * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), sidewalkTexture);
                    groundObjects.AddLast(sidewalk);
                }
            }
            for (int y = 1; y < (target.Height - 1); y++)
            {
                foreach (int x in new int[] { 0, target.Width - 1 })
                {
                    int X = target.X + x;
                    int Y = target.Y + y;
                    mapBitmap[X * bitmapSize.Height + Y] = MapFillType.Sidewalk;
                    FlatObject sidewalk = new FlatObject(new PositionInTown(this, new Vector2(X * SquareWidth, Y * SquareWidth)), 0, new Vector2(SquareWidth, SquareWidth), sidewalkTexture);
                    groundObjects.AddLast(sidewalk);
                }
            }

            return new Rectangle(target.X + 1, target.Y + 1, target.Width - 2, target.Height - 2);
        }

        public void BuildInterfaceRoadSigns()
        {
            const float vPosition = 8;
            float height = 2f;
            const float width = 6f;
            const float halfWidth = width / 2;
            Model handleModel = game.Content.Load<Model>("Objects/roadSignHandle");
            Vector3 handleModelSize = handleModel.GetSize(game.Drawer.WorldTransformMatrix);
            foreach (TownQuarterInterface iface in interfaces)
            {
                Vector2 handlePos = Vector2.Zero;
                float handleAzimuth = 0f;
                Vector3 upperLeft = iface.Position().ToVector3(vPosition);
                Vector3 upperRight = upperLeft;
                const float handleEpsilon = 0.02f; //2cm
                switch (iface.SidePosition)
                {
                    case TownQuarterInterfacePosition.Top:
                        upperLeft.X -= halfWidth;
                        upperRight.X += halfWidth;
                        upperLeft.Z += (BlockWidth - 2) * SquareWidth;
                        upperRight.Z += (BlockWidth - 2) * SquareWidth;

                        handlePos = upperLeft.XZToVector2();
                        handlePos.X += halfWidth;
                        handlePos.X -= handleModelSize.X * 0.5f;
                        handlePos.Y -= handleModelSize.Z + handleEpsilon;
                        handleAzimuth = 0f;
                        break;
                    case TownQuarterInterfacePosition.Right:
                        upperLeft.Z -= halfWidth;
                        upperRight.Z += halfWidth;
                        upperLeft.X -= (BlockWidth - 2) * SquareWidth;
                        upperRight.X -= (BlockWidth - 2) * SquareWidth;

                        handlePos = upperLeft.XZToVector2();
                        handlePos.Y += halfWidth;
                        //handlePos.Y -= handleModelSize.X * 0.5f;
                        handlePos.X += handleModelSize.Z + handleEpsilon - handleModelSize.X * 0.5f - handleModelSize.Z * 0.5f;
                        handleAzimuth = MathHelper.PiOver2;
                        break;
                    case TownQuarterInterfacePosition.Bottom:
                        upperLeft.X += halfWidth;
                        upperRight.X -= halfWidth;
                        upperLeft.Z -= (BlockWidth - 2) * SquareWidth;
                        upperRight.Z -= (BlockWidth - 2) * SquareWidth;

                        handlePos = upperLeft.XZToVector2();
                        handlePos.X -= halfWidth;
                        handlePos.X -= handleModelSize.X * 0.5f;
                        handlePos.Y += handleEpsilon;
                        handleAzimuth = 0f;
                        break;
                    case TownQuarterInterfacePosition.Left:
                        upperLeft.Z += halfWidth;
                        upperRight.Z -= halfWidth;
                        upperLeft.X += (BlockWidth - 2) * SquareWidth;
                        upperRight.X += (BlockWidth - 2) * SquareWidth;

                        handlePos = upperLeft.XZToVector2();
                        handlePos.Y -= halfWidth;
                        //handlePos.Y -= handleModelSize.X * 0.5f;
                        handlePos.X -= handleModelSize.Z + handleEpsilon + handleModelSize.X * 0.5f - handleModelSize.Z * 0.5f;
                        handleAzimuth = MathHelper.PiOver2;
                        break;
                    default:
                        break;
                }
                Vector3 lowerLeft = upperLeft, lowerRight = upperRight;
                lowerLeft.Y -= height;
                lowerRight.Y -= height;
                Plate signPlate = new Plate(this, upperLeft, upperRight, lowerLeft, lowerRight, iface.OppositeInterface.Quarter.RoadSignTexture, game.Content.Load<Texture2D>("Textures/metal"), true);
                magicPlates.AddLast(signPlate);
                iface.OppositeInterface.Quarter.RegisterNewRoadSign(signPlate);
                SpatialObject handle = new SpatialObject(handleModel, new PositionInTown(this, handlePos), handleAzimuth, game.Drawer.WorldTransformMatrix);
                solidObjects.AddLast(handle);
            }

        }
    }
}
