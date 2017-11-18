using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace OneCache
{
    public class CacheManager
    {
        private Dictionary<TypeCache, List<Cache>> Caches = new Dictionary<TypeCache, List<Cache>>();
        private Action<Cache> DeleteCache = delegate(Cache cache) { Directory.Delete(cache.Path, true); };

        public bool Verbose { get; set; } = false;
        
        private static string PrettySize(long value)
        {
            double sizeInKb = value / 1024d;

            if (value == 0)
                return string.Format("{0:0.0} B", value);
            else if (sizeInKb < 1000)
                return string.Format("{0:0.0} KB", sizeInKb);
            else if (sizeInKb < 1000000)
                return string.Format("{0:0.0} MB", sizeInKb / 1024d);
            else
                return string.Format("{0:0.0} GB", sizeInKb / (1024d * 1024d));
        }

        private static void PrintTable(Dictionary<TypeCache, long> stat)
        {
            Console.WriteLine("Cache\tSize");
           
            foreach (var KeyValue in stat)
            {
                Console.WriteLine("{0}\t{1}", KeyValue.Key, PrettySize(KeyValue.Value));
            }
        }

        List<Cache> ReadCache(string directory)
        {
            List<Cache> cache = new List<Cache>();

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(string.Format("Каталог {0} не найден", directory));
            }

            Regex reg = new Regex(@"[0-9a-zA-F]{8}-[0-9a-zA-F]{4}-[0-9a-zA-F]{4}-[0-9a-zA-F]{4}-[0-9a-zA-F]{12}$");
            foreach(var cur_path in Directory.GetDirectories(directory, "1cv8*"))
            {
                var dirs = Directory.GetDirectories(cur_path).Where(p => reg.IsMatch(p)).ToList();

                foreach(var dir in dirs)
                {
                    cache.Add(new Cache() { Path = dir, UUID = Path.GetFileName(dir), Size = Cache.GetDirectorySize(dir), Age = Cache.GetAge(dir) });
                }
            }
            
            return cache;
        }

        public List<Cache> GetCache(TypeCache typecache)
        {
            return Caches[typecache];
        }

        public void FindCache()
        {
            var values = Enum.GetValues(typeof(TypeCache)).Cast<TypeCache>();
                      
            foreach (var typecache in values)
            {
                var env = typecache.GetAttribute<LocationAttribute>();
                var local_data = Environment.GetEnvironmentVariable(env.Location);
                var directory = Path.Combine(local_data, "1C");

                Caches.Add(typecache, ReadCache(directory));
            }

            if (Verbose)
            {
                Dictionary<TypeCache, long> stat = new Dictionary<TypeCache, long>();

                foreach(var KeyValue in Caches)
                {
                    stat.Add(KeyValue.Key, KeyValue.Value.Sum<Cache>(c => c.Size));
                }

                PrintTable(stat);
            }
        }
       
        public void Remove(TypeCache typecache)
        {
            var current = Caches[typecache];
            current.ForEach(DeleteCache);
            current.Clear();
        }

        public void RemoveAll()
        {
            foreach(var KeyValue in Caches)
            {
                Remove(KeyValue.Key);
            }
        }

        public void RemoveToAge(int age)
        {
            foreach (var KeyValue in Caches)
            {
                RemoveToAge(KeyValue.Key, age);
            }
        }

        public void RemoveToAge(TypeCache typecache, int age)
        {
            var current = Caches[typecache];

            var find_caches = current.Where(p => p.Age > age).ToList();
            find_caches.ForEach(DeleteCache);          
        }
    }
}
