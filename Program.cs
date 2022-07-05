using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //白い正方形のタイルが横方向に W 個、縦方向に H 個、合計 W × H 個敷き詰められている。
            var initialTiles = InitializeTiles();
            //太郎君は、 i 日目の朝に、左から axi 番目で上から ayi 番目のタイルを左上、 左から bxi 番目で上から byi 番目のタイルを右下にした長方形領域に存在しているタイルがすべて白いかどうかを確認する。 
            //もしすべてのタイルが白かった場合、それらのタイルをすべて黒く塗りつぶす。それ以外のときは何もしない。
            var numberOfblackedTiles = PaintItBlack(initialTiles);
            //N 日間のそれぞれの日において、その日の作業が終了した時点で黒く塗りつぶされたタイルが何枚あるかを出力せよ。
            ShowOutput(numberOfblackedTiles);
        }

        static void ShowOutput(IEnumerable<ulong> numberOfBlackedTiles)
        {
            foreach(var numberOfBlackedTile in numberOfBlackedTiles)
            {
                Console.WriteLine(numberOfBlackedTile);
            }

            Console.ReadKey();
        }

        static IEnumerable<ulong> PaintItBlack(IEnumerable<RectangleCoordinate> rectangleCoordinates)
        {
            var numberOfBlackTiles = new List<ulong>();
            var numberOfProcessedRectangleCoordinates = new List<RectangleCoordinate>();
            ulong numberOfBlackTile = 0;
            foreach(var rectangleCoordinate in rectangleCoordinates)
            {
                numberOfBlackTile += FindBlackTiles(numberOfProcessedRectangleCoordinates, rectangleCoordinate);
                numberOfBlackTiles.Add(numberOfBlackTile);
                numberOfProcessedRectangleCoordinates.Add(rectangleCoordinate);
            }

            return numberOfBlackTiles;
        }

        static ulong FindBlackTiles(IEnumerable<RectangleCoordinate> numberOfProcessedRectangleCoordinates, RectangleCoordinate rectangleCoordinate)
        {
            if (IsTargetInBlack(numberOfProcessedRectangleCoordinates, rectangleCoordinate) == true)
            {
                return 0L;
            }
            
            rectangleCoordinate.InBlack = true;

            uint width = rectangleCoordinate.Vertex2.Item1 - rectangleCoordinate.Vertex1.Item1 + 1;
            uint height = rectangleCoordinate.Vertex2.Item2 - rectangleCoordinate.Vertex1.Item2 + 1;
            ulong arraySize = width*height;

            return arraySize;
        }

        static bool IsTargetInBlack(IEnumerable<RectangleCoordinate> numberOfProcessedRectangleCoordinates, RectangleCoordinate rectangleCoordinate)
        {
            foreach(var numberOfProcessedRectangleCoordinate in numberOfProcessedRectangleCoordinates)
            {
                if (IsTargetInBlack(numberOfProcessedRectangleCoordinate, rectangleCoordinate))
                {
                    return true;
                }
            }

            return false;
        }

        static bool IsTargetInBlack(RectangleCoordinate processedRectangleCoordinate, RectangleCoordinate rectangleCoordinate)
        {
            if (processedRectangleCoordinate.InBlack == true)
            {
                if (IsOverlapped(processedRectangleCoordinate, rectangleCoordinate))
                {
                    return true;
                }
            }

            return false;

        }

        static bool IsOverlapped(RectangleCoordinate processedRectangleCoordinate, RectangleCoordinate rectangleCoordinate)
        {
            var leftTop = rectangleCoordinate.Vertex1;
            var rightBottom = rectangleCoordinate.Vertex2;

            var processedLeftTop = processedRectangleCoordinate.Vertex1;
            var processedRightBottom = processedRectangleCoordinate.Vertex2;

            bool IsLeftLessRight = false;
            if (leftTop.Item2 <= processedRightBottom.Item2)
            {
                IsLeftLessRight = true;
            }

            bool IsRightGreaterLeft = false;
            if (rightBottom.Item2 >= processedLeftTop.Item2)
            {
                IsRightGreaterLeft = true;
            }

            bool IsTopLessBottom = false;
            if (leftTop.Item1 <= processedRightBottom.Item1)
            {
                IsTopLessBottom = true;
            }

            bool IsBottomGreaterTop = false;
            if (rightBottom.Item1 >= processedLeftTop.Item1)
            {
                IsBottomGreaterTop = true;
            }

            if ((IsLeftLessRight) && (IsRightGreaterLeft) && (IsTopLessBottom) && (IsBottomGreaterTop))
            {
                return true;
            }

            return false;
        }
        static void PaintTargeçtInBlack(Tiles originalTiles, RectangleCoordinate rectangleCoordinate)
        {
           for(uint xIndex = rectangleCoordinate.Vertex1.Item1; xIndex <= rectangleCoordinate.Vertex2.Item1; xIndex++)
            {
                for(uint yIndex =rectangleCoordinate.Vertex1.Item2; yIndex <= rectangleCoordinate.Vertex2.Item2; yIndex++)
                {
                    originalTiles.SetTileColor(xIndex-1, yIndex-1, true);
                }
            }
        }


        static IEnumerable<RectangleCoordinate> InitializeTiles()
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

            return parsedInput.rectanbleCoordinates;
        }

        static Tiles CreateTiles(uint H, uint W)
        {
            var tiles = new Tiles(H, W);
            for(uint xIndex = 0; xIndex < H; xIndex++)
            {
                for(uint yIndex =0; yIndex < W; yIndex++)
                {
                    tiles.SetTileColor(xIndex, yIndex, false);
                }
            }

            return tiles;
        }
        static (uint W, uint H, IEnumerable<RectangleCoordinate> rectanbleCoordinates) ParseInputData(IEnumerable<string> input)
        {
            var rectangleSize = input.ElementAt(0).Split(" ");
            uint W = uint.Parse(rectangleSize[1]);
            uint H = uint.Parse(rectangleSize[0]);
            //uint days = uint.Parse(input.ElementAt(1));
            int days = input.Count() - 2;

            var rectanbleCoordinates = new List<RectangleCoordinate>();
            for (uint dayIndex = 0; dayIndex < days; dayIndex++)
            {
                var rectangle = input.ElementAt((int)dayIndex+2).Split(" ");
                var vertex1 = (uint.Parse(rectangle[0]), uint.Parse(rectangle[1]));
                var vertex2 = (uint.Parse(rectangle[2]), uint.Parse(rectangle[3]));
                rectanbleCoordinates.Add(new RectangleCoordinate(vertex1, vertex2));
            }

            return (W, H, rectanbleCoordinates);
        }

        static IEnumerable<string> ReadInputData
        {
            get
            {
                int days = 0;
                List<string> input = new List<string>();
                while(true)
                {
                    var line = Console.ReadLine();
                    if ((line == null) || (string.IsNullOrEmpty(line)))
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

    internal class RectangleCoordinate
    {
        internal RectangleCoordinate((uint, uint) vertex1, (uint, uint) vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
        internal (uint, uint) Vertex1 {get; }
        internal (uint, uint) Vertex2 {get; }

        internal bool InBlack {get; set;}
    }

    internal class Tiles
    {
        internal bool[] RowTiles {get;}
        internal uint Width {get;}
        internal uint Height {get;}
 
        internal bool GetTileColor(uint xi, uint yi)
        {
            ulong index = xi * this.Width + yi;
            return RowTiles[index];
        }

        internal void SetTileColor(uint xi, uint yi, bool value)
        {
            ulong index = xi * this.Width + yi;
            RowTiles[index] = value;
        }

        internal Tiles(uint height, uint width)
        {
            Width = width;
            Height = height;
            ulong arraySize = width * height;
            RowTiles = new bool[arraySize];
        }
    }




}
