//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Generic;

namespace ProjectX_V3_Game.Calculations
{
	// Thanks to albetros for this.
	
    public class Sector
    {
        private int attackerX, attackerY, attackX, attackY;
        private int degree, sectorsize, leftside, rightside;
        private int distance;
        private bool addextra;

        public Sector(int attackerX, int attackerY, int attackX, int attackY)
        {
            this.attackerX = attackerX;
            this.attackerY = attackerY;
            this.attackX = attackX;
            this.attackY = attackY;
            this.degree = Core.Screen.GetDegree(attackerX, attackX, attackerY, attackY);
            this.addextra = false;
        }

        public void Arrange(int sectorsize, int distance)
        {
            this.distance = Math.Min(distance, 14);
            this.sectorsize = sectorsize;
            this.leftside = this.degree - (sectorsize / 2);
            if (this.leftside < 0)
                this.leftside += 360;
            this.rightside = this.degree + (sectorsize / 2);
            if (this.leftside < this.rightside || this.rightside - this.leftside != this.sectorsize)
            {
                this.rightside += 360;
                this.addextra = true;
            }
        }


        public bool Inside(int X, int Y)
        {
            if (Core.Screen.GetDistance(X, Y, attackerX, attackerY) <= distance)
            {
                int degree = Core.Screen.GetDegree(attackerX, X, attackerY, Y);
                if (this.addextra)
                    degree += 360;
                if (degree >= this.leftside && degree <= this.rightside)
                    return true;
            }
            return false;
        }
    }
    public class InLineAlgorithm
    {
        public enum Algorithm
        {
            DDA,
            SomeMath
        }
        public struct coords
        {
            public int X;
            public int Y;

            public coords(double x, double y)
            {
                this.X = (int)x;
                this.Y = (int)y;
            }
        }
        bool Contains(List<coords> Coords, coords Check)
        {
            foreach (coords Coord in Coords)
                if (Coord.X == Check.X && Check.Y == Coord.Y)
                    return true;
            return false;
        }
        List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty)
        {
            return linedda(userx, usery, shotx, shoty);
        }
        void Add(List<coords> Coords, int x, int y)
        {
            coords add = new coords((ushort)x, (ushort)y);
            if (!Coords.Contains(add))
                Coords.Add(add);
        }
        List<coords> linedda(int xa, int ya, int xb, int yb)
        {
            int dx = xb - xa, dy = yb - ya, steps, k;
            float xincrement, yincrement, x = xa, y = ya;

            if (Math.Abs(dx) > Math.Abs(dy)) steps = Math.Abs(dx);
            else steps = Math.Abs(dy);

            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            List<coords> ThisLine = new List<coords>();
            ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));

            for (k = 0; k < MaxDistance; k++)
            {
                x += xincrement;
                y += yincrement;
                ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));
            }
            return ThisLine;
        }
        List<coords> lcoords;
        public byte MaxDistance = 10;
        public InLineAlgorithm(ushort X1, ushort X2, ushort Y1, ushort Y2, byte MaxDistance, Algorithm algo)
        {
            algorithm = algo;
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;
            if (algo == Algorithm.DDA)
                lcoords = LineCoords(X1, Y1, X2, Y2);

            this.MaxDistance = MaxDistance;
            Direction = (byte)Core.Screen.GetAngle(X1, Y1, X2, Y2); ;
        }
        private Algorithm algorithm;
        public ushort X1 { get; set; }
        public ushort Y1 { get; set; }
        public ushort X2 { get; set; }
        public ushort Y2 { get; set; }
        public byte Direction { get; set; }

        public bool InLine(ushort X, ushort Y)
        {
            int mydst = Core.Screen.GetDistance((ushort)X1, (ushort)Y1, X, Y);
            byte dir = (byte)Core.Screen.GetAngle(X1, Y1, X, Y);

            if (mydst <= MaxDistance)
            {
                if (algorithm == Algorithm.SomeMath)
                {

                    if (dir != Direction)
                        return false;
                    //calculate line eq
                    if (X2 - X1 == 0)
                    {
                        //=> X - X1 = 0
                        //=> X = X1
                        return X == X1;
                    }
                    else if (Y2 - Y1 == 0)
                    {
                        //=> Y - Y1 = 0
                        //=> Y = Y1
                        return Y == Y1;
                    }
                    else
                    {
                        double val1 = ((double)(X - X1)) / ((double)(X2 - X1));
                        double val2 = ((double)(Y + Y1)) / ((double)(Y2 + Y1));
                        bool works = Math.Floor(val1) == Math.Floor(val2);
                        return works;
                    }
                }
                else
                    if (algorithm == Algorithm.DDA)
                        return Contains(lcoords, new coords(X, Y));
            }
            return false;
        }
    }
}
