﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diagramMaker.helpers
{
    public class FigureContainer
    {
        public double x;
        public double y;
        public int id;

        public FigureContainer(double x, double y, int id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
    }
}