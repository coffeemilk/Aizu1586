using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal static class Program
    {

        static void Main(string[] args)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //白い正方形のタイルが横方向に W 個、縦方向に H 個、合計 W × H 個敷き詰められている。
            var initialTiles = InitializeTiles();
            //太郎君は、 i 日目の朝に、左から axi 番目で上から ayi 番目のタイルを左上、 左から bxi 番目で上から byi 番目のタイルを右下にした長方形領域に存在しているタイルがすべて白いかどうかを確認する。 
            //もしすべてのタイルが白かった場合、それらのタイルをすべて黒く塗りつぶす。それ以外のときは何もしない。
            var numberOfblackedTiles = PaintItBlackIfNecessary(initialTiles);
            //N 日間のそれぞれの日において、その日の作業が終了した時点で黒く塗りつぶされたタイルが何枚あるかを出力せよ。
            ShowOutput(numberOfblackedTiles);
            sw.Stop();
            Console.WriteLine($"Elapsed time = {sw.ElapsedMilliseconds}");
        }

        static void ShowOutput(IEnumerable<ulong> numberOfBlackedTiles)
        {
            foreach(var numberOfBlackedTile in numberOfBlackedTiles)
            {
                Console.WriteLine(numberOfBlackedTile);
            }

            //Console.ReadKey();
        }

        static  IReadOnlyCollection<ulong> PaintItBlackIfNecessary( IReadOnlyCollection<RectangleCoordinate> rectangleCoordinates)
        {
            var days = rectangleCoordinates.Count;
            var blackTiles = new List<ulong>(days);
            var processedRectangleCoordinates = new List<RectangleCoordinate>(days);
            ulong inTotalBlackTiles = 0;

            var spanRects = new Span<RectangleCoordinate>(rectangleCoordinates.ToArray());
            var inBlackRect = new RectangleCoordinate(spanRects[0]);
            foreach(var spanRect in spanRects)
            {
                //var processedSpanRect = new Span<RectangleCoordinate>(processedRectangleCoordinates.ToArray());
                if (!FindBlackTiles(inBlackRect, processedRectangleCoordinates, spanRect))
                {
                    inTotalBlackTiles += PaintItBlack(spanRect);
                    processedRectangleCoordinates.Add(spanRect);
                    inBlackRect = UpdateInBlackRect(inBlackRect, spanRect);
                }

                blackTiles.Add(inTotalBlackTiles);
            }

            return blackTiles;
        }

        static RectangleCoordinate UpdateInBlackRect(RectangleCoordinate inBlackRect, RectangleCoordinate spanRect)
        {
            uint newX1 = Math.Min(inBlackRect.Vertex1.x, spanRect.Vertex1.x);
            uint newY1 = Math.Min(inBlackRect.Vertex1.y, spanRect.Vertex1.y);
            uint newX2 = Math.Max(inBlackRect.Vertex2.x, spanRect.Vertex2.x);
            uint newY2 = Math.Max(inBlackRect.Vertex2.y, spanRect.Vertex2.y);

            return new RectangleCoordinate((newX1, newY1), (newX2, newY2));
        }
        static bool FindBlackTiles(RectangleCoordinate inBlackRect, IReadOnlyCollection<RectangleCoordinate> processedRectangleCoordinatesInBlack, RectangleCoordinate rectangleCoordinate)
        {
            bool result = false;

            if (!IsOverlapped(inBlackRect, rectangleCoordinate))
            {
                return result;
            }

            foreach(var processedRectangleCoordinateInBlack in processedRectangleCoordinatesInBlack)
            {
                if (IsOverlapped(processedRectangleCoordinateInBlack, rectangleCoordinate))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private static ulong PaintItBlack(RectangleCoordinate rectangleCoordinate)
        {
            uint width = rectangleCoordinate.Vertex2.x - rectangleCoordinate.Vertex1.x + 1;
            uint height = rectangleCoordinate.Vertex2.y - rectangleCoordinate.Vertex1.y + 1;
            ulong arraySize = width * height;
            return arraySize;
        }

        static bool IsOverlapped(RectangleCoordinate processedRectangleCoordinate, RectangleCoordinate rectangleCoordinate)
        {
            var leftTop = rectangleCoordinate.Vertex1;
            var rightBottom = rectangleCoordinate.Vertex2;

            var processedLeftTop = processedRectangleCoordinate.Vertex1;
            var processedRightBottom = processedRectangleCoordinate.Vertex2;

            bool IsLeftLessRight = (leftTop.x <= processedRightBottom.x);
            if (IsLeftLessRight == false) return false;
            bool IsRightGreaterLeft = (rightBottom.x >= processedLeftTop.x);
            if (IsRightGreaterLeft == false) return false;
            bool IsTopLessBottom = (leftTop.y <= processedRightBottom.y);
            if (IsTopLessBottom == false) return false;
            bool IsBottomGreaterTop = (rightBottom.y >= processedLeftTop.y);
            if (IsBottomGreaterTop == false) return false;

            return true;
        }


        static  IReadOnlyCollection<RectangleCoordinate> InitializeTiles()
        {
            //入力形式
            //W H
            //N
            //ax1 ay1 bx1 by1
            //ax2 ay2 bx2 by2
            //axn ayn bxn byn

            //var input = CreateInputData;
            var input = ReadInputData;
            var parsedInput = ParseInputData(input);
            //var tiles = CreateTiles(parsedInput.W, parsedInput.H);

            return parsedInput.rectangleCoordinates;
        }

        static (uint W, uint H, IReadOnlyCollection<RectangleCoordinate> rectangleCoordinates) ParseInputData( IReadOnlyCollection<string> input)
        {
            var rectangleSize = input.ElementAt(0).Split(" ");
            uint W = uint.Parse(rectangleSize[1]);
            uint H = uint.Parse(rectangleSize[0]);
            //uint days = uint.Parse(input.ElementAt(1));
            int days = input.Count() - 2;

            var rectangleCoordinates = new List<RectangleCoordinate>(days);
            for (uint dayIndex = 0; dayIndex < days; dayIndex++)
            {
                var rectangle = input.ElementAt((int)dayIndex+2).Split(" ");
                var vertex1 = (uint.Parse(rectangle[0]), uint.Parse(rectangle[1]));
                var vertex2 = (uint.Parse(rectangle[2]), uint.Parse(rectangle[3]));
                rectangleCoordinates.Add(new RectangleCoordinate(vertex1, vertex2));
            }

            return (W, H, rectangleCoordinates);
        }

        static  IReadOnlyCollection<string> ReadInputData
        {
            get
            {
                int days = 0;
                List<string> input = new List<string>();

                while(true)
                {
                    var line = Console.ReadLine();
                    if ((line == null) || string.IsNullOrEmpty(line) )
                    {
                        break;
                    }
                    else
                    {
                        input.Add(line);
                    }

                    //Aizuシステム向けに逐次パースに変更
                    if (input.Count() == 2)
                    {
                        days = int.Parse(input.ElementAt(1));
                    }

                    if (input.Count() == 2 + days)
                    {
                        break;
                    }

                }
                
                return input;
            }
        }

    }

    internal struct RectangleCoordinate
    {
        internal RectangleCoordinate((uint x, uint y) vertex1, (uint x, uint y) vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }

        internal RectangleCoordinate(RectangleCoordinate clone)
        {
            Vertex1 = clone.Vertex1;
            Vertex2 = clone.Vertex2;
        }

        internal (uint x, uint y) Vertex1 {get; }
        internal (uint x, uint y) Vertex2 {get; }

    }




}
