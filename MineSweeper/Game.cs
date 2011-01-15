using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Threading;

namespace MineSweeper
{
   public class Tile
   {
      public bool Visited;
      public bool IsMine;
      public int SurroundMines;
   }

   public enum GameStatus
   {
      InProgess,
      LOSE,
      WIN,      
   }

   public class Game
   {
      Tile[] tiles;
      public GameStatus Status { get; private set; }
      int uncleared;
      public int SecondsElapsed { get; private set; }
      public DateTime? Begin { get; private set; }
      public Game(int width, int height, bool[] mines)
      {
         this.Width = width;
         this.Height = height;
         this.tiles = GenerateTiles(width, mines);
         this.Status = GameStatus.InProgess;
         this.uncleared = mines.Count(_=>!_);         
      }

      public int Width { get; private set; }

      public int Height { get; private set; }

      public Tile this[int y, int x]
      {
         get
         {
            return GetTile(y, x);
         }
      }

      public Tile GetTile(int y, int x)
      {
         return tiles[y * Width + x];
      }

      public Tile GetTile(int index)
      {
         return tiles[index];
      }

      public void Dig(int index)
      {
         if (Status != GameStatus.InProgess)
            return;

         Dig(index / Width, index % Width);
      }      

      public void Dig(int y, int x)
      {
         if (Begin == null)
            Begin = DateTime.Now;

         var tile = GetTile(y, x);

         if (tile.Visited)
            return;

         tile.Visited = true;
         if (tile.IsMine)
         {
            Status = GameStatus.LOSE;
            return;
         }
         
         if (--uncleared == 0)
         {
            Status = GameStatus.WIN;
            return;
         }
         if (tile.SurroundMines > 0 || tile.IsMine)
            return;

         if (y > 0)
            Dig(y - 1, x);

         if (y < Height - 1)
            Dig(y + 1, x);

         if (x > 0)
            Dig(y, x - 1);

         if (x < Width - 1)
            Dig(y, x + 1);

         if (y > 0 && x > 0)
            Dig(y - 1, x - 1);

         if (y > 0 && x < Width - 1)
            Dig(y - 1, x + 1);

         if (y < Height - 1 && x > 0)
            Dig(y + 1, x - 1);

         if (y < Height - 1 && x < Width - 1)
            Dig(y + 1, x + 1);
      }

      public static Game RandomGame(int width, int height, int mineCount)
      {
         bool[] tiles = GenerateRandomMines(width * height, mineCount);
         return new Game(width, height, tiles);
      }

      static Tile[] GenerateTiles(int width, bool[] mines)
      {
         int count = mines.Length;
         int height = mines.Length / width;
         var tiles = new Tile[count];
         for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new Tile { IsMine = mines[i] };

         for (int i = 0; i < tiles.Length; i++)
         {
            int x = i % width,
                y = i / width;

            if (y > 0 && mines[i - width])
               tiles[i].SurroundMines++;

            if (y < height - 1 && mines[i + width])
               tiles[i].SurroundMines++;

            if (x > 0 && mines[i - 1])
               tiles[i].SurroundMines++;

            if (x < width - 1 && mines[i + 1])
               tiles[i].SurroundMines++;

            if (y > 0 && x > 0 && mines[i - width - 1])
               tiles[i].SurroundMines++;

            if (y > 0 && x < width - 1 && mines[i - width + 1])
               tiles[i].SurroundMines++;

            if (y < height - 1 && x > 0 && mines[i + width - 1])
               tiles[i].SurroundMines++;

            if (y < height - 1 && x < width - 1 && mines[i + width + 1])
               tiles[i].SurroundMines++;
         }

         return tiles;
      }

      static bool[] GenerateRandomMines(int totalCount, int mineCount)
      {
         if (mineCount > totalCount)
            throw new ArgumentException();

         var rnd = new Random();
         var tiles = new bool[totalCount];

         for (int i = 0; i < mineCount; i++)
            tiles[i] = true;

         for (int i = 0; i < mineCount - 1; i++)
         {
            var pos = rnd.Next(i, totalCount);
            var tmp = tiles[i];
            tiles[i] = tiles[pos];
            tiles[pos] = tmp;
         }

         return tiles;
      }
   }
}