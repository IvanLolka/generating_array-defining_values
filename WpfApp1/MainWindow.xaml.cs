using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Map _map;

        public MainWindow()
        {
            InitializeComponent();
            RegenerateMap();
        }


        private void RegenerateMap_Click(object sender, RoutedEventArgs e)
        {
            RegenerateMap();
            UpdateCoordinatesList();
        }
        private void UpdateCoordinatesList()
        {
            CoordinatesListBox.Items.Clear();
            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    if (_map.Cells[x, y].Biom == "Water")
                    {
                        CoordinatesListBox.Items.Add($"({x}, {y})");
                    }
                }
            }
        }

        private void RegenerateMap()
        {
            _map = new Map(10, 10); // Пример размера карты
            _map.GenerateMap(); // Генерация карты (замените на вашу реализацию)

            DrawMap();
        }

        private void DrawMap()
        {
            MapCanvas.Children.Clear();
            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 60; // Ширина ячейки
                    rect.Height = 60; // Высота ячейки
                    rect.Stroke = Brushes.Black;
                    rect.Fill = GetFillBrush(_map.Cells[x, y].Biom); // Получение цвета для ячейки
                    Canvas.SetLeft(rect, x * 60); // Установка позиции ячейки
                    Canvas.SetTop(rect, y * 60);
                    MapCanvas.Children.Add(rect); // Добавление ячейки на Canvas

                    if (_map.Cells[x, y].Biom == "Water")
                    {
                        DrawWaterBlock(x, y); // Отображение координат водных блоков
                    }
                }
            }
        }

        private void DrawWaterBlock(int x, int y)
        {
            CoordinatesListBox.Items.Add($"({x}, {y})");
        }

        private Brush GetFillBrush(string biom)
        {
            // Логика определения цвета для каждого биома
            // Здесь может быть ваша логика определения цвета...
            return biom == "Water" ? Brushes.Blue : Brushes.Green; // Просто для примера
        }
    }

    public class Map
    {
        public MapCell[,] Cells { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new MapCell[width, height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[x, y] = new MapCell(0, "Land"); // Инициализация всех ячеек значением по умолчанию
                }
            }
        }

        public void GenerateMap()
        {
            Random random = new Random();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Генерируем случайный тип блока
                    string biom = random.Next(0, 2) == 0 ? "Water" : "Land";

                    Cells[x, y] = new MapCell(random.Next(0, 100), biom);

                    if (x > 0 && x < Width - 1 && y > 0 && y < Height - 1)
                    {
                        if (Cells[x - 1, y].Biom == Cells[x + 1, y].Biom && Cells[x, y - 1].Biom == Cells[x, y + 1].Biom)
                        {
                            Cells[x, y].Biom = Cells[x - 1, y].Biom;
                        }
                        else if (Cells[x, y - 1].Biom == Cells[x, y].Biom && Cells[x, y + 1].Biom == Cells[x, y].Biom)
                        {
                            Cells[x, y].Biom = Cells[x, y - 1].Biom;
                        }
                        else if (Cells[x - 1, y].Biom == Cells[x, y].Biom && Cells[x + 1, y].Biom == Cells[x, y].Biom
                            && Cells[x, y - 1].Biom == Cells[x, y].Biom && Cells[x, y + 1].Biom == Cells[x, y].Biom)
                        {
                            Cells[x, y].Biom = Cells[x, y].Biom;
                        }
                    }
                }
            }

            bool hasSingleBlocks = true;
            while (hasSingleBlocks)
            {
                hasSingleBlocks = false;
                // Проверяем и удаляем оставшиеся одиночные блоки
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        if (Cells[x, y].Biom != Cells[x - 1, y].Biom && Cells[x, y].Biom != Cells[x + 1, y].Biom
                            && Cells[x, y].Biom != Cells[x, y - 1].Biom && Cells[x, y].Biom != Cells[x, y + 1].Biom)
                        {
                            Cells[x, y].Biom = Cells[x, y].Biom == "Water" ? "Land" : "Water";
                            hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                        }
                    }
                }

                // Проверка для блоков на верхней и нижней границах карты
                for (int x = 1; x < Width - 1; x++)
                {
                    if (Cells[x, 0].Biom != Cells[x - 1, 0].Biom && Cells[x, 0].Biom != Cells[x + 1, 0].Biom
                        && Cells[x, 0].Biom != Cells[x, 1].Biom)
                    {
                        Cells[x, 0].Biom = Cells[x, 0].Biom == "Water" ? "Land" : "Water";
                        hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                    }

                    if (Cells[x, Height - 1].Biom != Cells[x - 1, Height - 1].Biom && Cells[x, Height - 1].Biom != Cells[x + 1, Height - 1].Biom
                        && Cells[x, Height - 1].Biom != Cells[x, Height - 2].Biom)
                    {
                        Cells[x, Height - 1].Biom = Cells[x, Height - 1].Biom == "Water" ? "Land" : "Water";
                        hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                    }
                }

                // Проверка для блоков на левой и правой границах карты
                for (int y = 1; y < Height - 1; y++)
                {
                    if (Cells[0, y].Biom != Cells[1, y].Biom && Cells[0, y].Biom != Cells[0, y - 1].Biom
                        && Cells[0, y].Biom != Cells[0, y + 1].Biom)
                    {
                        Cells[0, y].Biom = Cells[0, y].Biom == "Water" ? "Land" : "Water";
                        hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                    }

                    if (Cells[Width - 1, y].Biom != Cells[Width - 2, y].Biom && Cells[Width - 1, y].Biom != Cells[Width - 1, y - 1].Biom
                        && Cells[Width - 1, y].Biom != Cells[Width - 1, y + 1].Biom)
                    {
                        Cells[Width - 1, y].Biom = Cells[Width - 1, y].Biom == "Water" ? "Land" : "Water";
                        hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                    }
                }

                // Проверка для угловых блоков
                if (Cells[0, 0].Biom != Cells[1, 0].Biom && Cells[0, 0].Biom != Cells[0, 1].Biom)
                {
                    Cells[0, 0].Biom = Cells[0, 0].Biom == "Water" ? "Land" : "Water";
                    hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                }

                if (Cells[0, Height - 1].Biom != Cells[1, Height - 1].Biom && Cells[0, Height - 1].Biom != Cells[0, Height - 2].Biom)
                {
                    Cells[0, Height - 1].Biom = Cells[0, Height - 1].Biom == "Water" ? "Land" : "Water";
                    hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                }

                if (Cells[Width - 1, 0].Biom != Cells[Width - 2, 0].Biom && Cells[Width - 1, 0].Biom != Cells[Width - 1, 1].Biom)
                {
                    Cells[Width - 1, 0].Biom = Cells[Width - 1, 0].Biom == "Water" ? "Land" : "Water";
                    hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                }

                if (Cells[Width - 1, Height - 1].Biom != Cells[Width - 2, Height - 1].Biom && Cells[Width - 1, Height - 1].Biom != Cells[Width - 1, Height - 2].Biom)
                {
                    Cells[Width - 1, Height - 1].Biom = Cells[Width - 1, Height - 1].Biom == "Water" ? "Land" : "Water";
                    hasSingleBlocks = true; // Устанавливаем флаг наличия одиночных блоков
                }
            }
        }



    }

    public class MapCell
    {
        public int Height { get; set; }
        public string Biom { get; set; }

        public MapCell(int height, string biom)
        {
            Height = height;
            Biom = biom;
        }
    }
}