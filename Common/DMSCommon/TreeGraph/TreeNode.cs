using DMSCommon.TreeGraph.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.TreeGraph
{
    [DataContract]
    public sealed class TreeNode<T>
    {
        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public NodeLink Link { get; set; }
    }
}
