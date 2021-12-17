using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ttp.Meteor.MeteorSwath;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// Simple wrapper around MeteorSwath's AutoSwathGeometry class,
    /// which lets us define to ToString method.
    /// </summary>
    public class AutoGeometryModel
    {
        public AutoSwathGeometry Geometry { get; set; }

        public override string ToString()
        {
            return " • AutoGeometry";
        }
    }
}
