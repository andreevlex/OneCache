using System;
using System.Collections.Generic;
using Utility.CommandLine;

namespace OneCache
{
    class Program
    {
        #region Private Properties

        [Argument('d', "remove")]
        private static string Remove { get; set; }

        [Argument('o', "older")]
        private static int Age { get; set; }

        [Operands]
        private static string[] Operands { get; set; }

        #endregion Private Properties

        static void Main(string[] args)
        {
           if(args.Length == 0)
            {
                ShowUsage();                
            }

            Arguments.Populate();
                       
            CacheManager plat = new CacheManager();

            if(Remove == "all" || string.IsNullOrEmpty(Remove))
            {
                if(Age != 0)
                {
                    plat.RemoveToAge(Age);
                }

                plat.RemoveAll();
            } else if(Remove == "metadata")
            {
                if (Age != 0)
                {
                    plat.RemoveToAge(TypeCache.Metadata, Age);
                }

                plat.Remove(TypeCache.Metadata);
            } else if (Remove == "settings")
            {
                if (Age != 0)
                {
                    plat.RemoveToAge(TypeCache.Settings, Age);
                }

                plat.Remove(TypeCache.Settings);
            } else
            {
                NoCommand();
            }           
        }

        static void ShowUsage()
        {
            Console.WriteLine("onecache <команда>");
            Console.WriteLine("-d, --remove [all, metadata, settings] - удалить временные файлы платформы в профиле");
            Console.WriteLine("\tall - удалить все виды временных файлов");
            Console.WriteLine("\tmetadata - удалить временные файлы метаданных");
            Console.WriteLine("\tsettings - удалить временные файлы настроек");
            Console.WriteLine("-o, --older <days> - удалить времменные файлы платформы старше значения параметра days");
            Environment.Exit(0);
        }

        static void NoCommand()
        {
            Console.WriteLine("Неверная команда");
            Environment.Exit(1);
        }
    }
}
