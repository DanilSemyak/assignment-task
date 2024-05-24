using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace SemyakWork30Variant
{
    class SulurionOptimization
    {
        static void Main(string[] args)
        {

            List<double> cargoes = new List<double>(); // лист с грузами
            double containerCapacity = 0; // вместимость контейнера
            bool running = true;

            while (running)
            {
                Console.WriteLine("1. Ввести грузы");
                Console.WriteLine("2. Ввести вместимость контейнера");
                Console.WriteLine("3. Рассчитать распределение жадным алгоритмом");
                Console.WriteLine("4. Рассчитать распределение алгоритм по остаточной вместимости");
                Console.WriteLine("5. Сохранить данные");
                Console.WriteLine("6. Загрузить данные");
                Console.WriteLine("7. Выполнить распределение случайным образом");
                Console.WriteLine("8. Выход");
                Console.Write("Выберите опцию: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        cargoes = EnterCargoes();
                        break;
                    case "2":
                        containerCapacity = EnterContainerCapacity();
                        break;
                    case "3":
                        bool flag = true;
                        foreach (var result in cargoes)
                        {
                            if (result > containerCapacity)
                            {
                                Console.WriteLine("Распределение невозмжно, один из грузов превысил вместимость контейнера");
                                Console.WriteLine("В договоре указано, что вместимость контейнера всегда привышает вместимость грузов");
                                flag = false;
                                break;
                            }
                        } 
                        if (flag)
                            FirstMethod(cargoes, containerCapacity);
                        break;
                    case "4":
                        SecondMethod(cargoes, containerCapacity);
                        break;
                    case "5":
                        SaveData(cargoes, containerCapacity);
                        break;
                    case "6":
                        (cargoes, containerCapacity) = LoadData();
                        break;
                    case "7":
                        cargoes = RandomCargoes();
                        containerCapacity = RandomContainerCapacity();

                        bool flag2 = true;
                        foreach (var result in cargoes)
                        {
                            if (result > containerCapacity)
                            {
                                Console.WriteLine("Распределение невозмжно, один из грузов превысил вместимость контейнера");
                                Console.WriteLine("В договоре указано, что вместимость контейнера всегда привышает вместимость грузов");
                                flag2 = false;
                                break;
                            }
                        }
                        if (flag2)
                        {
                            Console.WriteLine("Каким методом распределить? Укажите 1 или 2");
                            int whatMethod = Int32.Parse(Console.ReadLine());
                            if (whatMethod == 1)
                            {
                                FirstMethod(cargoes, containerCapacity);
                            } else if (whatMethod == 2)
                            {
                                SecondMethod(cargoes, containerCapacity);
                            } else
                            {
                                Console.WriteLine("Неудалось распределить, введите корректное значение по запросу");
                            }
                        }
                        break;
                    case "8":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Некорректная опция.");
                        break;
                }
            }
        }

        static List<double> EnterCargoes() // метод для корректного добавления грузов
        {
            List<double> cargoesTest = new List<double>();
            Console.WriteLine("Введите веса грузов (введите 'готово' для завершения):");
            string input;
            while ((input = Console.ReadLine()) != "готово")
            {
                if (double.TryParse(input, out double weight))
                {
                    cargoesTest.Add(weight);
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите число.");
                }
            }
            return cargoesTest;
        }

        static double EnterContainerCapacity() // метод для ввода вместимости контейнера
        {
            Console.Write("Введите вместимость контейнера: ");
            double capacityTest;
            while (!double.TryParse(Console.ReadLine(), out capacityTest))
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, введите число.");
            }
            if (capacityTest < 0)
            {
                Console.WriteLine("Вместимость контейнера не может быть отрицательной");
                Console.WriteLine("Введите корректное значение еще раз");
                return 0;
            } else
            {
                return capacityTest;
            }
        }

        static void FirstMethod(List<double> cargoes, double containerCapacity) // 1 метод для распределения
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            cargoes.Sort((a, b) => b.CompareTo(a)); // сортировка по убыванию + создание списка списков
            var containers = new List<List<double>>();

            foreach (var cargo in cargoes)
            {
                bool placed = false;
                foreach (var container in containers)
                {
                    if (container.Sum() + cargo <= containerCapacity)
                    {
                        container.Add(cargo);
                        placed = true;
                        break;
                    }
                }
                if (!placed)
                {
                    containers.Add(new List<double> { cargo });
                }
            }
            Console.WriteLine("Минимальное количество требуемых контейнеров:" + containers.Count);
            for (int i = 0; i < containers.Count; i++)
            {
                Console.WriteLine($"Контейнер {i + 1}: {string.Join(", ", containers[i])}");
                Console.WriteLine($"Сумма контейнера {i + 1}: {containers[i].Sum()}");
            }
            sw.Stop();
            Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} мс");
        }
        
        static void SecondMethod(List<double> cargoes, double containerCapacity) // 2 метод для распределения
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            cargoes.Sort((a, b) => b.CompareTo(a));
            int containerCount = 0;
            double[] bin_rem = new double[cargoes.Count];

            List<List<double>> containers = new List<List<double>>();

            for (int i = 0; i < cargoes.Count; i++)
            {
                int j;
                for (j = 0; j < containerCount; j++)
                {
                    if (bin_rem[j] >= cargoes[i])
                    {
                        bin_rem[j] = bin_rem[j] - cargoes[i];
                        containers[j].Add(cargoes[i]); 
                        break;
                    }
                }
                if (j == containerCount)
                {
                    bin_rem[containerCount] = containerCapacity - cargoes[i];
                    containerCount++;
                    containers.Add(new List<double> { cargoes[i] }); 
                }
            }
            Console.WriteLine("Минимальное количество требуемых контейнеров: " + containerCount);
            for (int i = 0; i < containers.Count; i++)
            {
                Console.WriteLine($"Контейнер {i + 1}: {string.Join(", ", containers[i])}");
                Console.WriteLine($"Сумма контейнера {i + 1}: {containers[i].Sum()}");
            }
            sw.Stop();
            Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} мс");
        }

        static List<double> RandomCargoes()
        {
            Random random = new Random();
            List<double> randomCargoes = new List<double>();
            
            for (int i = 0; i < 10; i++) // Пример: 10 грузов
            {
                randomCargoes.Add(random.Next(10, 80));
            }

            Console.WriteLine("Случайные значения грузов: " + string.Join(", ", randomCargoes));
            return randomCargoes;
        }

        static double RandomContainerCapacity()
        {
            Random random = new Random();
            double containerCapacity = random.Next(50, 100);
            Console.WriteLine("Вместимость контейнера: " + containerCapacity);
            return containerCapacity;
        }

        static void SaveData(List<double> cargoes, double containerCapacity)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = "Semyal.txt";
            string filePath = Path.Combine(desktopPath, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(containerCapacity);
                foreach (var cargo in cargoes)
                {
                    writer.WriteLine(cargo);
                }
            }

            Console.WriteLine("Данные сохранены.");
        }


        static (List<double>, double) LoadData()
        {
            var cargoesTest = new List<double>();
            double containerCapacityTest = 0;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = "Semyal.txt";
            string filePath = Path.Combine(desktopPath, fileName);

            using (var reader = new StreamReader(filePath))
            {
                containerCapacityTest = double.Parse(reader.ReadLine());
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    cargoesTest.Add(double.Parse(line));
                }
            }

            Console.WriteLine("Данные загружены.");
            return (cargoesTest, containerCapacityTest);
        }

    }
}