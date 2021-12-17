using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeteorSwathTestApp
{
    class SwathGeometryModel
    {
        /// <summary>
        /// Is it defined in the config.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// The name assigned in the config.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The index number within the config.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Get a string to represent this geometry.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Exists)
            {
                return String.Format("{0}: {1}", Index, Name);
            }
            else
            {
                return String.Format("{0}: -----", Index);
            }
        }
    }
}
