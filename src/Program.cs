﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Argument('v', "verbose")]
        private static bool Verbose { get; set; }

        [Operands]
        private static List<string> Operands { get; set; }

        #endregion Private Properties

        static void Main(string[] args)
        {
           if(args.Length == 0)
            {
                ShowUsage();                
            }

            CheckProcesses();

            Arguments.Populate();
                       
            CacheManager plat = new CacheManager();
            plat.Verbose = Verbose;
            plat.FindCache();
                        
            if(Remove == "all")
            {
                if(Age != 0)
                {
                    plat.RemoveToAge(Age);
                }
                else
                {
                    plat.RemoveAll();
                }
                
            } else if(Remove == "metadata")
            {
                RemoveCache(plat, TypeCache.Metadata);

            } else if (Remove == "settings")
            {
                RemoveCache(plat, TypeCache.Settings);

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

        static void CheckProcesses()
        {
            if(!ProcessInfo.NoRunningPlatformProcesses())
            {
                Console.WriteLine("Есть запущенные процессы платформы. Удаление невозможно");
                Environment.Exit(1);
            }
        }

        static void NoCommand()
        {
            Console.WriteLine("Неверная команда");
            Environment.Exit(1);
        }

        static void RemoveCache(CacheManager manager, TypeCache typecache)
        {
            if (Age != 0)
            {
                manager.RemoveToAge(typecache, Age);
            }
            else
            {
                manager.Remove(typecache);
            }
        }
    }
}
