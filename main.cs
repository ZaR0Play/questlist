using System;
using System.IO;
using System.Text;

class Program
{
    // ===== МЕТОДЫ ДЛЯ РАБОТЫ СО СПИСКОМ (строки) =====
    
    static void File_read(string[] list_tz)
    {
        if (list_tz.Length == 0)
        {
            Console.WriteLine("Список пуст!");
            return;
        }
        
        for (int i = 0; i < list_tz.Length; i++)
        {
            Console.WriteLine($"{i + 1}) {list_tz[i]}");
        }
    }

    static string[] File_wr(string[] list_t, string new_word)
    {
        string[] list_tt = new string[list_t.Length + 1];
        for (int i = 0; i < list_t.Length; i++)
        {
            list_tt[i] = list_t[i];
        }
        list_tt[list_t.Length] = "[False] " + new_word;
        return list_tt;
    }

    static string[] File_del(string[] list_z, int num)
    {
        if (num < 0 || num >= list_z.Length)
        {
            Console.WriteLine($"Ошибка: индекс {num} вне диапазона (0-{list_z.Length - 1})!");
            return list_z;
        }
        
        string[] new_list = new string[list_z.Length - 1];
        int j = 0;
        for (int i = 0; i < list_z.Length; i++)
        {
            if (i != num)
            {
                new_list[j] = list_z[i];
                j++;
            }
        }
        return new_list;
    }

    // ===== МЕТОД: ОТМЕТКА ВЫПОЛНЕНИЯ (ПЕРЕКЛЮЧЕНИЕ TRUE/FALSE) =====
    static string[] File_complete(string[] list_tz, int index)
    {
        if (index < 0 || index >= list_tz.Length)
        {
            Console.WriteLine($"Ошибка: индекс {index} вне диапазона!");
            return list_tz;
        }

        string task = list_tz[index];
        
        // Проверяем, есть ли отметка [True]
        if (task.StartsWith("[True] "))
        {
            // Если [True] - меняем на [False] (снимаем отметку)
            list_tz[index] = "[False] " + task.Substring(7);
            Console.WriteLine($"Статус изменён на False: '{task.Substring(7)}'");
        }
        else if (task.StartsWith("[False] "))
        {
            // Если [False] - меняем на [True] (отмечаем выполненной)
            list_tz[index] = "[True] " + task.Substring(8);
            Console.WriteLine($"Статус изменён на True: '{task.Substring(8)}'");
        }
        else
        {
            // Если нет отметки - добавляем [True]
            list_tz[index] = "[True] " + task;
            Console.WriteLine($"Добавлен статус True: '{task}'");
        }
        
        return list_tz;
    }

    // ===== МЕТОД: СТАТИСТИКА =====
    static void ShowStatistics(string[] list_tz)
    {
        if (list_tz.Length == 0)
        {
            Console.WriteLine("Список пуст!");
            return;
        }

        int completed = 0;
        foreach (string task in list_tz)
        {
            if (task.StartsWith("[True] "))
                completed++;
        }

        int total = list_tz.Length;
        int pending = total - completed;
        double percent = total > 0 ? (completed * 100.0 / total) : 0;

        Console.WriteLine("\n=== СТАТИСТИКА ===");
        Console.WriteLine($"Всего задач: {total}");
        Console.WriteLine($"Выполнено (True): {completed} ({percent:F1}%)");
        Console.WriteLine($"Не выполнено (False): {pending}");
        Console.WriteLine("==================\n");
    }

    // ===== МЕТОД: ПОИСК ПО СТАТУСУ =====
    static void ShowByStatus(string[] list_tz)
    {
        Console.WriteLine("\nВыберите статус для отображения:");
        Console.WriteLine("1) True - Выполненные");
        Console.WriteLine("2) False - Не выполненные");
        Console.Write("Ваш выбор: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Ошибка! Введите число!");
            return;
        }

        string status = choice == 1 ? "[True] " : "[False] ";
        bool found = false;

        Console.WriteLine($"\n=== Задачи со статусом {status.Trim()} ===");
        for (int i = 0; i < list_tz.Length; i++)
        {
            if (list_tz[i].StartsWith(status))
            {
                Console.WriteLine($"{i + 1}) {list_tz[i]}");
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine($"Нет задач со статусом {status.Trim()}");
        }
        Console.WriteLine("==============================\n");
    }

    // ===== МЕТОД: СОХРАНЕНИЕ В ФАЙЛ =====
    static void SaveToFile(string filename, string[] list_tz)
    {
        try
        {
            File.WriteAllLines(filename, list_tz, Encoding.UTF8);
            Console.WriteLine($"Данные сохранены в файл '{filename}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    // ===== МЕТОД: ЗАГРУЗКА ИЗ ФАЙЛА =====
    static string[] LoadFromFile(string filename)
    {
        if (File.Exists(filename))
        {
            string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
            
            // Проверяем, есть ли статусы у задач, если нет - добавляем [False]
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith("[True] ") && !lines[i].StartsWith("[False] "))
                {
                    lines[i] = "[False] " + lines[i];
                }
            }
            
            Console.WriteLine($"Загружено {lines.Length} задач из файла.");
            return lines;
        }
        else
        {
            Console.WriteLine($"Файл '{filename}' не найден. Создан новый список.");
            return new string[0];
        }
    }

    // ===== ОЧИСТКА ВСЕХ ЗАДАЧ =====
    static string[] ClearAllTasks(string[] list_tz)
    {
        Console.Write("Вы уверены, что хотите удалить все задачи? (y/n): ");
        string confirm = Console.ReadLine();
        
        if (confirm?.ToLower() == "y")
        {
            Console.WriteLine($"Удалено {list_tz.Length} задач.");
            return new string[0];
        }
        else
        {
            Console.WriteLine("Операция отменена.");
            return list_tz;
        }
    }

    static void Main()
    {
        // ===== ЗАГРУЗКА ИЗ ФАЙЛА =====
        string[] list_tz = LoadFromFile("list.txt");

        // ===== ВЫБОР РЕЖИМА =====
        Console.WriteLine("\nДобрый день, пользователь. Выберите режим работы (только номер):");
        Console.WriteLine("1) Консольный");
        Console.WriteLine("2) Графический (в разработке)");

        string input = Console.ReadLine();
        if (!int.TryParse(input, out int screen))
        {
            Console.WriteLine("Ошибка: нужно ввести число 1 или 2!");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
            return;
        }

        if (screen == 1)
        {
            while (true)
            {
                Console.WriteLine("\nВыберите одно из действий (только номер):");
                Console.WriteLine("1) Показать список задач");
                Console.WriteLine("2) Добавить задачу");
                Console.WriteLine("3) Удалить дело");
                Console.WriteLine("4) Отметить выполнение (True/False)");  
                Console.WriteLine("5) Статистика");
                Console.WriteLine("6) Поиск по статусу (True/False)");
                Console.WriteLine("7) Очистить все задачи");
                Console.WriteLine("8) Сохранить в файл");
                Console.WriteLine("9) Выход");
                Console.Write("Ваш выбор: ");

                input = Console.ReadLine();

                if (!int.TryParse(input, out int numb))
                {
                    Console.WriteLine("Ошибка! Введите число!");
                    continue;
                }

                switch (numb)
                {
                    case 1: // ПОКАЗАТЬ СПИСОК
                        File_read(list_tz);
                        break;

                    case 2: // ДОБАВИТЬ ЗАДАЧУ
                        Console.Write("Напишите новую задачу: ");
                        string word = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            list_tz = File_wr(list_tz, word);
                            Console.WriteLine($"Задача '[False] {word}' добавлена!");
                        }
                        else
                        {
                            Console.WriteLine("Задача не может быть пустой!");
                        }
                        break;

                    case 3: // УДАЛИТЬ ЗАДАЧУ
                        if (list_tz.Length == 0)
                        {
                            Console.WriteLine("Список пуст, нечего удалять!");
                            break;
                        }

                        File_read(list_tz);
                        Console.Write("Введите номер удаляемой задачи: ");
                        
                        if (!int.TryParse(Console.ReadLine(), out int id))
                        {
                            Console.WriteLine("Ошибка! Введите число!");
                            break;
                        }

                        int indexToDelete = id - 1;
                        if (indexToDelete < 0 || indexToDelete >= list_tz.Length)
                        {
                            Console.WriteLine($"Ошибка! Нет задачи с номером {id}");
                            break;
                        }

                        string deletedTask = list_tz[indexToDelete];
                        // Убираем [True] или [False] для красоты
                        if (deletedTask.StartsWith("[True] "))
                            deletedTask = deletedTask.Substring(7);
                        else if (deletedTask.StartsWith("[False] "))
                            deletedTask = deletedTask.Substring(8);
                            
                        list_tz = File_del(list_tz, indexToDelete);
                        Console.WriteLine($"Задача '{deletedTask}' удалена!");
                        break;

                    case 4: // ОТМЕТИТЬ ВЫПОЛНЕНИЕ (TRUE/FALSE)
                        if (list_tz.Length == 0)
                        {
                            Console.WriteLine("Список пуст, нечего отмечать!");
                            break;
                        }

                        File_read(list_tz);
                        Console.Write("Введите номер задачи для изменения статуса: ");
                        
                        if (!int.TryParse(Console.ReadLine(), out int completeId))
                        {
                            Console.WriteLine("Ошибка! Введите число!");
                            break;
                        }

                        int completeIndex = completeId - 1;
                        if (completeIndex < 0 || completeIndex >= list_tz.Length)
                        {
                            Console.WriteLine($"Ошибка! Нет задачи с номером {completeId}");
                            break;
                        }

                        list_tz = File_complete(list_tz, completeIndex);
                        break;

                    case 5: // СТАТИСТИКА
                        ShowStatistics(list_tz);
                        break;

                    case 6: // ПОИСК ПО СТАТУСУ
                        ShowByStatus(list_tz);
                        break;

                    case 7: // ОЧИСТИТЬ ВСЕ ЗАДАЧИ
                        list_tz = ClearAllTasks(list_tz);
                        break;

                    case 8: // РУЧНОЕ СОХРАНЕНИЕ
                        SaveToFile("list.txt", list_tz);
                        break;

                    case 9: // ВЫХОД С СОХРАНЕНИЕМ
                        Console.WriteLine("Выход...");
                        SaveToFile("list.txt", list_tz);
                        Console.WriteLine("Данные сохранены. До свидания!");
                        return;

                    default:
                        Console.WriteLine("Неизвестная команда! Введите число от 1 до 9");
                        break;
                }
            }
        }
        else if (screen == 2)
        {
            Console.WriteLine("Графический режим пока не реализован");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Ошибка! Нужно ввести 1 или 2!");
        }

        // Сохраняем при выходе (если не вышли через case 9)
        SaveToFile("list.txt", list_tz);
    }
}
