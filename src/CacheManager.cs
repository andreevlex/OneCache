using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace OneCache
{
    public class CacheManager
    {
        Dictionary<TypeCache, List<Cache>> Caches;

        public CacheManager()
        {
            Caches = new Dictionary<TypeCache, List<Cache>>();
            FindCache();
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

        private void FindCache()
        {
            var values = Enum.GetValues(typeof(TypeCache)).Cast<TypeCache>();

            foreach (var value in values)
            {
                var env = value.GetAttribute<LocationAttribute>();
                var local_data = Environment.GetEnvironmentVariable(env.Location);
                var directory = Path.Combine(local_data, "1C");

                Caches.Add(value, ReadCache(directory));
            }                       
        }
        
        private void RemoveInner(List<Cache> list)
        {
            foreach (var cache in list)
            {
                Directory.Delete(cache.Path, true);
            }
        }

        public void Remove(TypeCache typecache)
        {
            var current = Caches[typecache];
            RemoveInner(current);
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
            RemoveInner(find_caches);           
        }
    }
}
