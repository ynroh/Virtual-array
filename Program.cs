using System;

namespace Virtual_array
{
    internal class Program
    {
       
        static void Main(string[] args) 
        {
            Console.WriteLine("Введите размер страницы в байтах");
            var pageSize = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите количество страниц в буфере");
            var pageCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите кол-во элементов");
            var arrayLength = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите имя файла");
            var fileName = Console.ReadLine();

            var fm = new FileManager();
            var va = new VirtualArray(arrayLength, pageSize, pageCount, fm, fileName);
            Menu(va);
        }

        static void Menu(VirtualArray va)
        {
            while (true)
            {
                Console.WriteLine("1 - установить значение элемента по индексу\n2 - прочитать значение элемента по индексу\n0 - завершить\n");
                int menu = Convert.ToInt32(Console.ReadLine());
                if (menu == 0)
                {
                    //va.SaveAllPages();
                    break;
                }
                while (true)
                {
                    if (menu != 1 && menu != 2 && menu != 3 && menu != 0)
                    {
                        Console.WriteLine("Некорректный ввод! Попробуйте еще раз");
                        menu = Convert.ToInt32(Console.ReadLine());
                    }
                    else
                        break;
                }

                switch (menu)
                {
                    case 1:
                        WriteElementByIndex(va);
                        break;
                    case 2:
                        ReadElementByIndex(va);
                        break;
                }
            }

        }
        static void WriteElementByIndex(VirtualArray va)
        {
            Console.WriteLine("Введите индекс");
            int index = Convert.ToInt32(Console.ReadLine());
            while (true)
            {
                if (index < 0)
                {
                    Console.WriteLine("Некорректный ввод! Попробуйте еще раз");
                    index = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            Console.WriteLine("Введите значение");
            int value = Convert.ToInt32(Console.ReadLine());
            va.WriteElementByIndex(index, value);
        }

        static void ReadElementByIndex(VirtualArray va)
        {
            Console.WriteLine("Введите индекс");
            int index = Convert.ToInt32(Console.ReadLine());
            while (true)
            {
                if (index < 0)
                {
                    Console.WriteLine("Некорректный ввод! Попробуйте еще раз");
                    index = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            va.ReadElementByIndex(index);
        }
    }
}
