using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MineSweeper
{
   public partial class MainPage : PhoneApplicationPage
   {
      Game game;
      List<Rectangle> covers;
      // Constructor
      public MainPage()
      {
         InitializeComponent();
         var height = LayoutRoot.ActualHeight;
         var width = LayoutRoot.ActualWidth;
         InitByLevel(level);

         var timer = new System.Windows.Threading.DispatcherTimer();
         timer.Interval = new TimeSpan(0, 0, 0, 0, 400); // 100 Milliseconds
         timer.Tick += (sender, args) =>
         {
            if (game != null && game.Status == GameStatus.InProgess && game.Begin.HasValue)
            {
               var secs = (int)Math.Round((DateTime.Now - game.Begin.Value).TotalSeconds);
               if (secs > 999)
                  secs = 999;
               elapsedTime.Text = secs.ToString("000");
            }            
         };
         timer.Start();
      }

      int level = 0;

      void InitByLevel(int lv)
      {
         switch(lv)
         {
            case 0:
               InitGame(9, 9, 10, 30);
               break;
            case 1:
               InitGame(16, 16, 40, 28);
               break;
            case 2:
               InitGame(16, 20, 66, 28);
               break;
         }
      }

      SolidColorBrush[] flagColors = new SolidColorBrush[]
        {
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Brown),
            new SolidColorBrush(Colors.Cyan),
            new SolidColorBrush(Colors.Yellow),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.Black),
        };

      void InitGame(int height, int width, int mines, int tileSize)
      {
         minePanel.Children.Clear();
         minePanel.Height = tileSize * height;
         minePanel.Width = tileSize * width;

         game = Game.RandomGame(width, height, mines);
         covers = new List<Rectangle>();
         for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
               var tile = game.GetTile(i, j);
               TextBlock txt = null;
               if (tile.IsMine)
               {
                  txt = new TextBlock();
                  txt.Text = "X";
               }
               else if (tile.SurroundMines > 0)
               {
                  txt = new TextBlock();
                  txt.Foreground = flagColors[tile.SurroundMines - 1];
                  txt.Text = tile.SurroundMines.ToString();
               }

               if (txt != null)
               {
                  txt.Height = txt.Width = tileSize;
                  txt.TextAlignment = TextAlignment.Center;
                  Canvas.SetLeft(txt, j * tileSize);
                  Canvas.SetTop(txt, i * tileSize);
                  minePanel.Children.Add(txt);
               }

               var cover = new Rectangle();
               cover.Tag = i * width + j;
               cover.Fill = new SolidColorBrush(Colors.Blue);
               cover.Height = cover.Width = tileSize;
               cover.MouseLeftButtonDown += new MouseButtonEventHandler(cover_MouseLeftButtonDown);
               Canvas.SetLeft(cover, j * tileSize);
               Canvas.SetTop(cover, i * tileSize);
               minePanel.Children.Add(cover);
               covers.Add(cover);
            }

         //grid lines
         for (int x = 0; x <= width * tileSize; x += tileSize)
         {
            var line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.X1 = line.X2 = x;
            line.Y1 = 0;
            line.Y2 = height * tileSize;
            minePanel.Children.Add(line);
         }

         for (int y = 0; y <= height * tileSize; y += tileSize)
         {
            var line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.Y1 = line.Y2 = y;
            line.X1 = 0;
            line.X2 = width * tileSize;
            minePanel.Children.Add(line);
         }
      }

      void cover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         var cover = sender as Rectangle;
         var index = (int)cover.Tag;
         game.Dig(index);
         UpdateMinePanel();
      }

      void UpdateMinePanel()
      {
         foreach (var cover in covers)
         {
            var index = (int)cover.Tag;
            var tile = game.GetTile(index);
            if (tile.Visited)
               cover.Visibility = System.Windows.Visibility.Collapsed;
         }

         TextBlock txt = null;
         if (game.Status == GameStatus.LOSE)
         {
            txt = new TextBlock();
            txt.Text = "LOSE";
            txt.Foreground = new SolidColorBrush(Colors.Red);
         }
         else if (game.Status == GameStatus.WIN)
         {
            txt = new TextBlock();
            txt.Text = "WIN";
            txt.Foreground = new SolidColorBrush(Colors.Green);            
         }

         if (txt != null) 
         {
            txt.FontSize = 72;            
            var x = (minePanel.Width - txt.ActualWidth) / 2;
            var y = (minePanel.Height - txt.ActualHeight) / 2;
            Canvas.SetLeft(txt, x);
            Canvas.SetTop(txt, y);
            minePanel.Children.Add(txt);
         }
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         level = 0;
         InitByLevel(level);
      }

      private void Button_Click_1(object sender, RoutedEventArgs e)
      {
         level = 1;
         InitByLevel(level);
      }

      private void Button_Click_2(object sender, RoutedEventArgs e)
      {
         level = 2;
         InitByLevel(level);
      }

      private void Button_Click_3(object sender, RoutedEventArgs e)
      {
         InitByLevel(level);
      }
   }
}