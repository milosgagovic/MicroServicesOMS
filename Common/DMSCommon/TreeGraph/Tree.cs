using DMSCommon.Model;
using DMSCommon.TreeGraph.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.TreeGraph
{
    [CollectionDataContract]
    public class Tree<T> : Dictionary<long, Element>
    {
        public  Dictionary<long, T> Data;
        [DataMember]
        public readonly Dictionary<long, NodeLink> Links;

        public Tree()
        {
            Data = new Dictionary<long, T>();
            Links = new Dictionary<long, NodeLink>();
        }
        [DataMember]
        public List<long> Roots { get; } = new List<long>();
        [DataMember]
        public List<long> Leaves { get; } = new List<long>();
        [DataMember]
        public TreeNode<T> this[long id] => new TreeNode<T> { Link = Links[id], Data = Data[id] };

        public bool Contains(long id) => Links.ContainsKey(id);

        public void AddChild(long? parentId, long id, T data)
        {
            if (Data.ContainsKey(id))
            {
                return;
            }

            if (parentId != null && !Data.ContainsKey((long)parentId))
            {
                return;
            }

            var link = new NodeLink { Id = id, Parent = parentId };
            var depth = 0;

            if (parentId == null)
            {
                if (Roots.Any())
                    Links[Roots[Roots.Count - 1]].Next = id;

                Roots.Add(id);
            }
            else
            {
                var parentLink = Links[(long)parentId];

                

                if (parentLink.Child != null)
                {
                    link.Next = parentLink.Child;
                }
                else
                {
                    Leaves.Remove((long)parentId);
                }
                parentLink.Child = id;
                if (!(Data[parentLink.Id] is Node))
                {
                    depth = parentLink.Depth + 1;
                }
                else
                {
                    depth = parentLink.Depth;
                }
            }

            // Saves data

            Data.Add(id, data);

            // Saves link

            link.Depth = depth;
            Links.Add(id, link);

            // Registers as leaf

            Leaves.Add(id);
        }

        public void AddRoot(long id, T data)
        {
            Roots.Add(id);

            Data.Add(id, data);

            Links.Add(id, new NodeLink { Id = id });

            // set next of the previous last root to the current one id
            Links[Roots[Roots.Count - 1]].Next = id;
        }

        public IEnumerator GetEnumerator()
        {
            return Links.Select(x => x.Value).GetEnumerator();
        }
    }
}
