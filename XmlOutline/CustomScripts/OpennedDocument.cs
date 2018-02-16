using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlOutline.CustomScripts
{
    public class OpennedDocument
    {
        public string DocName { get; set; }
        public List<string> ExpandedNodes;
        public bool IsRefreshing { get; set; }

        public void AddExpandedNode(string path)
        {
            if (IsRefreshing) return;
            ExpandedNodes.RemoveAll(path.Contains);
            ExpandedNodes.Add(path);
        }

        public void RemoveExpandedNode(string path)
        {
            if(IsRefreshing)return;
            ExpandedNodes.RemoveAll(x => x.Contains(path));
        }
    }
}
