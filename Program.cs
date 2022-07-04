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

        static void ShowOutput(IEnumerable<int> numberOfBlackedTiles)
        {
            foreach(var numberOfBlackedTile in numberOfBlackedTiles)
            {
                Console.WriteLine(numberOfBlackedTile);
            }

            Console.ReadKey();
        }

        static IEnumerable<int> PaintItBlack((Tiles tiles, IEnumerable<RectangleCoordinate> rectanbleCoordinates) initialTiles)
        {
            var numberOfBlackTiles = new List<int>();
            int numberOfBlackTile = 0;
            foreach(var rectangleCoordinate in initialTiles.rectanbleCoordinates)
            {
                // var tiles = GetTargetTiles(initialTiles.tiles, rectangleCoordinate);
                // if( !tiles.Any(x => x.TileColor == TileColor.Black))
                // {
                //     numberOfBlackTile += PaintTargetInBlack(tiles);
                // }
                //本来は上が正しいが、パフォーマンスのためにタイルのコピーを取るのをやめる
                numberOfBlackTile += FindBlackTiles(initialTiles.tiles, rectangleCoordinate);
                numberOfBlackTiles.Add(numberOfBlackTile);
            }

            return numberOfBlackTiles;
        }

        static int FindBlackTiles(Tiles originalTiles, RectangleCoordinate rectangleCoordinate)
        {
            //var tiles = new List<Tile>();
            int width = rectangleCoordinate.Vertex2.Item1 - rectangleCoordinate.Vertex1.Item1 + 1;
            int height = rectangleCoordinate.Vertex2.Item2 - rectangleCoordinate.Vertex1.Item2 + 1;
            int arraySize = width*height;
            var tiles = new Tile[arraySize];
            int arrayIndex = 0;
            for(int xIndex = rectangleCoordinate.Vertex1.Item1; xIndex <= rectangleCoordinate.Vertex2.Item1; xIndex++)
            {
                for(int yIndex =rectangleCoordinate.Vertex1.Item2; yIndex <= rectangleCoordinate.Vertex2.Item2; yIndex++)
                {
                    var tile = originalTiles[xIndex-1, yIndex-1];
                    if (tile.TileColor == TileColor.Black)
                    {
                        return 0;
                    }

                    tiles[arrayIndex] = tile;
                    arrayIndex++;
                }
            }

            foreach(var tile in tiles)
            {
                tile.TileColor = TileColor.Black;
            }
            
            return arraySize;
        }

        static int CountTilesInBlack(Tiles tiles)
        {
            int numberOfBlackTiles = 0;
            for(int xIndex = 0; xIndex < tiles.Width; xIndex++)
            {
                for(int yIndex = 0; yIndex < tiles.Height; yIndex++)
                {
                    if (tiles[xIndex, yIndex].TileColor == TileColor.Black)
                    {
                        numberOfBlackTiles++;
                    }
                }
            }

            return numberOfBlackTiles;
        }
        static int PaintTargetInBlack(IEnumerable<Tile> tiles)
        {
            foreach(var tile in tiles)
            {
                tile.TileColor = TileColor.Black;
            }

            return tiles.Count();
        }

        static IEnumerable<Tile> GetTargetTiles(Tiles originalTiles, RectangleCoordinate rectangleCoordinate)
        {
            var gotTiles = new List<Tile>();
            for(int xIndex = rectangleCoordinate.Vertex1.Item1; xIndex <= rectangleCoordinate.Vertex2.Item1; xIndex++)
            {
                for(int yIndex =rectangleCoordinate.Vertex1.Item2; yIndex <= rectangleCoordinate.Vertex2.Item2; yIndex++)
                {
                    var tile = originalTiles[xIndex-1, yIndex-1];
                    //gotTiles.Add(tile);
                    yield return tile;
                }
            }

            //return gotTiles;
        }

        static (Tiles tiles, IEnumerable<RectangleCoordinate> rectanbleCoordinate) InitializeTiles()
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
            var tiles = CreateTiles(parsedInput.W, parsedInput.H);

            return (tiles, parsedInput.rectanbleCoordinates);
        }

        static Tiles CreateTiles(int W, int H)
        {
            var tiles = new Tiles(W, H);
            for(int xIndex = 0; xIndex < W; xIndex++)
            {
                for(int yIndex =0; yIndex < H; yIndex++)
                {
                    tiles[xIndex, yIndex] = new Tile();
                }
            }

            return tiles;
        }
        static (int W, int H, IEnumerable<RectangleCoordinate> rectanbleCoordinates) ParseInputData(IEnumerable<string> input)
        {
            var rectangleSize = input.ElementAt(0).Split(" ");
            int W = int.Parse(rectangleSize[1]);
            int H = int.Parse(rectangleSize[0]);
            var days = int.Parse(input.ElementAt(1));

            var rectanbleCoordinates = new List<RectangleCoordinate>();
            for (int dayIndex = 0; dayIndex < days; dayIndex++)
            {
                var rectangle = input.ElementAt(dayIndex+2).Split(" ");
                var vertex1 = (int.Parse(rectangle[0]), int.Parse(rectangle[1]));
                var vertex2 = (int.Parse(rectangle[2]), int.Parse(rectangle[3]));
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
                    if (line == null)
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

        static IEnumerable<string> CreateInputData
        {
            get
            {
                List<string> input = new List<string>();
                input.Add("5 4");
                input.Add("5");
                input.Add("1 1 3 3");
                input.Add("3 2 4 2");
                input.Add("4 3 5 4");
                input.Add("1 4 5 4");
                input.Add("4 1 4 1");

                return input;
            }
        }
    }

    internal class RectangleCoordinate
    {
        internal RectangleCoordinate((int, int) vertex1, (int, int) vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
        internal (int, int) Vertex1 {get; }
        internal (int, int) Vertex2 {get; }
    }
    internal class Tile
    {
        internal TileColor TileColor {get; set;}
    }

    internal enum TileColor
    {
        White,
        Black,
    }

    internal class Tiles
    {
        internal Tile[] RowTiles {get;}
        internal int Width {get;}
        internal int Height {get;}
 
        //internal Tile this[int xi, int yi] { get => this.RowTiles[xi, yi]; set => this.RowTiles[xi, yi] = value; }
        internal Tile this[int xi, int yi] { get => this.RowTiles[yi * this.Width + xi]; set => this.RowTiles[yi * this.Width + xi] = value; }

        internal Tiles(Tile[] tiles, int width, int height)
        {
            RowTiles = tiles;
            Width = width;
            Height = height;
        }

        internal Tiles(int width, int height)
        {
            Width = width;
            Height = height;
            RowTiles = new Tile[width*height];
        }
    }


}
