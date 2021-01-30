using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlot.Dark.Wpf.Dark.Annotations
{
    /// <summary>
    /// 
    /// </summary>
    public class AnnotationTag
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public AnnotationTag(string name, string description = "")
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
