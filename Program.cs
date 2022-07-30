using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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
            var numberOfblackedTiles = PaintItBlackIfNecessary(initialTiles.rectangleCoordinates, initialTiles.tiles);
            //N 日間のそれぞれの日において、その日の作業が終了した時点で黒く塗りつぶされたタイルが何枚あるかを出力せよ。
            ShowOutput(numberOfblackedTiles);
        }

        static void ShowOutput(IEnumerable<ulong> numberOfBlackedTiles)
        {
            foreach(var numberOfBlackedTile in numberOfBlackedTiles)
            {
                Console.WriteLine(numberOfBlackedTile);
            }

            //Console.ReadKey();
        }

        static  IReadOnlyCollection<ulong> PaintItBlackIfNecessary( IReadOnlyCollection<RectangleCoordinate> rectangleCoordinates, CustomBitArray tiles)
        {
            var days = rectangleCoordinates.Count;
            var blackTiles = new List<ulong>(days);
            //var processedRectangleCoordinates = new List<RectangleCoordinate>(days);
            ulong inTotalBlackTiles = 0;

            var spanRects = new Span<RectangleCoordinate>(rectangleCoordinates.ToArray());
            foreach(var spanRect in spanRects)
            {
                //var processedSpanRect = new Span<RectangleCoordinate>(processedRectangleCoordinates.ToArray());
                //if (!FindBlackTiles(processedSpanRect, spanRect))
                if (!FindBlackTiles(tiles, spanRect))
                {
                    PaintItBlack(tiles, spanRect);
                    inTotalBlackTiles += spanRect.ArraySize;
                    //processedRectangleCoordinates.Add(spanRect);
                }

                blackTiles.Add(inTotalBlackTiles);
            }

            return blackTiles;
        }

        //static bool FindBlackTiles(ReadOnlySpan<RectangleCoordinate> processedRectangleCoordinatesInBlack, RectangleCoordinate rectangleCoordinate)
        static bool FindBlackTiles(CustomBitArray tiles, RectangleCoordinate rectangleCoordinate)
        {
            bool result = false;

            for(uint xindex = rectangleCoordinate.Vertex1.x; xindex <= rectangleCoordinate.Vertex2.x; xindex++)
            {
                for(uint yindex = rectangleCoordinate.Vertex1.y; yindex <= rectangleCoordinate.Vertex2.y; yindex++)
                {
                    var tileColor = tiles[xindex-1, yindex-1];
                    if (tileColor)
                    {
                        //PaintItWhite(tiles, rectangleCoordinate);
                        return true;
                    }
                }
            }
            // foreach(var processedRectangleCoordinateInBlack in processedRectangleCoordinatesInBlack)
            // {
            //     if (IsOverlapped(processedRectangleCoordinateInBlack, rectangleCoordinate))
            //     {
            //         return true;
            //     }
            // }


            return result;
        }

        static void PaintItWhite(CustomBitArray tiles, RectangleCoordinate rectangleCoordinate)
        {
            for(uint xindex = rectangleCoordinate.Vertex1.x; xindex <= rectangleCoordinate.Vertex2.x; xindex++)
            {
                for(uint yindex = rectangleCoordinate.Vertex1.y; yindex <= rectangleCoordinate.Vertex2.y; yindex++)
                {
                    tiles[xindex-1, yindex-1] = false;
                }
            }
        }

        static void PaintItBlack(CustomBitArray tiles, RectangleCoordinate rectangleCoordinate)
        {
            for(uint xindex = rectangleCoordinate.Vertex1.x; xindex <= rectangleCoordinate.Vertex2.x; xindex++)
            {
                for(uint yindex = rectangleCoordinate.Vertex1.y; yindex <= rectangleCoordinate.Vertex2.y; yindex++)
                {
                    tiles[xindex-1, yindex-1] = true;
                }
            }
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


        static (IReadOnlyCollection<RectangleCoordinate> rectangleCoordinates, CustomBitArray tiles) InitializeTiles()
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

            return (parsedInput.rectangleCoordinates, parsedInput.tiles);
        }

        static (uint W, uint H, IReadOnlyCollection<RectangleCoordinate> rectangleCoordinates, CustomBitArray tiles) ParseInputData( IReadOnlyCollection<string> input)
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

            var tiles = new CustomBitArray(W, H);

            return (W, H, rectangleCoordinates, tiles);
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
            var width = vertex2.x - vertex1.x + 1;
            var height = vertex2.y - vertex1.y + 1;
            ArraySize = width * height;
        }

        internal (uint x, uint y) Vertex1 { get; }
        internal (uint x, uint y) Vertex2 { get; }

        internal ulong ArraySize { get; }
    }

    internal class CustomBitArray
    {
        internal ulong[] RowBitArray {get;}

        internal CustomBitArray(uint width, uint height)
        {
            Width = width;
            Height = height;
            ulong logicalSize = (ulong) width * (ulong) height;
            ulong size = logicalSize/(ulong)(sizeof(ulong)*8) + 1;
            RowBitArray = new ulong[size];
        }

        internal uint Width {get;}
        internal uint Height {get;}
        internal bool this[uint x, uint y]
        {
            get
            {
                var offset = Width * y + x;
                var yindex = offset/(sizeof(ulong)*8);
                var xindex = offset%(sizeof(ulong)*8);
                int shiftCount = (int)xindex;
                var targetBit = GetBit(RowBitArray[yindex], shiftCount);
                return targetBit;
            }
            set
            {
                var offset = Width * y + x;
                var yindex = offset/(sizeof(ulong)*8);
                var xindex = offset%(sizeof(ulong)*8);
                int shiftCount = (int)xindex;
                SetBit(ref RowBitArray[yindex], shiftCount, value);
            }
        }

        private bool GetBit( ulong src, int bit)
        {
            ulong mask = (ulong) 0x8000000000000000 >> bit;
            return (src & mask) == mask;

        }
        private void SetBit(ref ulong src, int bit, bool val)
        {
            ulong mask = (ulong) 0x8000000000000000 >> bit;
            src = val ? src | mask : src & ~mask;
        }
        //internal Tile this[int xi, int yi] { get => this.RowTiles[yi * this.Width + xi]; set => this.RowTiles[yi * this.Width + xi] = value; }
    }
}
