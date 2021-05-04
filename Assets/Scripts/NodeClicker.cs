using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class NodeClicker
    {
        MapGenerator m;

        public NodeClicker(MapGenerator m)
        {
            this.m = m;
        }

        public void ClickR()
        {
            m.ClickRandomNode();
        }

        public void ClickB()
        {
            m.ClickRandomNodeBatch();
        }
    }
}
