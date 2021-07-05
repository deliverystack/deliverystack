namespace Deliverystack.DeliveryApi.Models
{
    using Deliverystack.Interfaces;
    using Deliverystack.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    //TODO: specify a field for sorting

    public class PathApiCache : ICanNormalizeUrlPaths
    {
        // not using concurrent collections because
        // sorted dictionary for paths can have advantages
        // and rebuilding (as implemented) is not synchronous 
        // and future access is read-only
        SortedDictionary<string, PathApiEntryModel> _paths;
        Dictionary<string, PathApiEntryModel> _ids;
        IDeliveryClient _client = null;

        private SortedDictionary<string, PathApiEntryModel> Paths
        {
            get
            {
                // wait for background threads to finish loading cache
                while (_paths == null)
                {
                    Thread.Sleep(0);
                }

                return _paths;
            }
        }

        private Dictionary<string, PathApiEntryModel> Ids
        {
            get
            {
                // wait for background threads to finish loading cache
                while (_ids == null)
                {
                    Thread.Sleep(0);
                }

                return _ids;
            }
        }



        public PathApiCache(IDeliveryClient client)
        {
            _client = client;
            new Thread((t) => { Reset(); }).Start();
        }

        public void Reset()
        {
            SortedDictionary<string, PathApiEntryModel> pathsToIds
                = new SortedDictionary<string, PathApiEntryModel>();
            Dictionary<string, PathApiEntryModel> idsToEntries
                = new Dictionary<string, PathApiEntryModel>();

            foreach (string contentType in _client.GetContentTypeIds(requireUrls: true))
            {
                PathApiEntryModel[] entries = _client.Entries<PathApiEntryModel>(contentType, "title,pagedata");

                for(int i = 0; i < entries.Length; i++)
                { 
                    entries[i].ContentType = contentType;
                    pathsToIds[(this as ICanNormalizeUrlPaths).NormalizeUrlPath(entries[i].PageData.Url)] = entries[i];
                    idsToEntries[entries[i].Uid] = entries[i];
                }
            }

            _paths = pathsToIds;
            _ids = idsToEntries;
        }

        public bool ContainsId(string id)
        {
            // wait for background threads to finish loading cache
            return Ids.ContainsKey(id);
        }

        public bool ContainsPath(string path)
        {
            return Paths.ContainsKey((this as ICanNormalizeUrlPaths).NormalizeUrlPath(path));
        }

//        private string NormalizeUrl(string url)
//        {
//            return '/' + (url??string.Empty).ToLower().TrimStart('/').TrimEnd('/');
//        }

        public PathApiEntryModel[] GetAncestors(string url)
        {
            List<PathApiEntryModel> ancestors = new List<PathApiEntryModel>();
            url = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(url);

            while (!String.IsNullOrWhiteSpace(url.TrimStart('/')))
            {
                url = url.Length - url.Replace("/", "").Length == 1 ? "/" : url.Substring(0, url.LastIndexOf('/'));

                if (Paths.ContainsKey(url))
                {
                    ancestors.Add(Paths[url]);
                }
                else
                {
                    Console.WriteLine($"Entry with URL {url} is missing.");
                    //TODO: warn - missing element in information archietcture (/hr/jobs exists but not /hr).
                }
            }

            ancestors.Reverse();
            return ancestors.ToArray();
        }

        public PathApiEntryModel GetEntry(string path)
        {
            return GetCurrentGeneration(path, excludeSelf: false, includeSiblings: false)[0];
        }

        public PathApiEntryModel[] GetCurrentGeneration(string url, bool excludeSelf, bool includeSiblings)
        {
            string path = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(url);
            List<PathApiEntryModel> siblings = new List<PathApiEntryModel>();

            if (path != "/")
            {
                // siblings paths start with the parent path
                // and have the same number of slashes
                // home is never a sibling
                // ensure caller wants self or siblings 
                foreach (string key in Paths.Keys.Where(check => check.StartsWith(path.Substring(0, path.LastIndexOf('/') + 1)) 
                    && check.Length - check.Replace("/", "").Length == path.Length - path.Replace("/", "").Length               
                    && (check != "/" || path == "/")
                    && ((includeSiblings && check != path) || (check == path && !excludeSelf))))                                 
                {
                    siblings.Add(Paths[key]);
                }
            }
            else if (!excludeSelf)
            {
                siblings.Add(Paths[path]);
            }

            return siblings.ToArray();
        }

        public PathApiEntryModel[] GetDescendants(string path, int levels)
        {
            path = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(path);
            List<PathApiEntryModel> descendants = new List<PathApiEntryModel>();

            if (path != "/")
            {
                path += '/';
            }

            foreach (string key in Paths.Keys.Where(check => check != "/" && check.StartsWith(path)
                && check.Length - check.Replace("/", "").Length <= levels + path.Length - path.Replace("/", "").Length))
            {
                descendants.Add(Paths[key]);
            }

            return descendants.ToArray();
        }

        public PathApiEntryModel[] GetChildren(string path)
        {
            path = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(path);
            List<PathApiEntryModel> children = new List<PathApiEntryModel>();

            if (path != "/")
            {
                path += '/';
            }

            foreach (string key in Paths.Keys.Where(check => check != "/" && check.StartsWith(path) 
                && check.Length - check.Replace("/", "").Length == path.Length - path.Replace("/", "").Length))
            {
                children.Add(Paths[key]);
            }

            return children.ToArray();
        }

        public string GetPath(string id)
        {
            return Ids[id].PageData.Url;
        }

        public string GetId(string path)
        {
            return Paths[(this as ICanNormalizeUrlPaths).NormalizeUrlPath(path)].Uid;
        }
    }
}
