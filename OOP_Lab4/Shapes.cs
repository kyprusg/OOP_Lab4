using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OOP_Lab4_VisualEditor
{
    public enum ShapeType
    {
        Circle,
        Square,
        Ellipse,
        Rectangle,
        Triangle,
        Line
    }

    public abstract class Shape
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private Color _color;
        private bool _isSelected;

        private const int MIN_SIZE = 16;

        public Shape(int x, int y, int width, int height, Color color)
        {
            _x = x;
            _y = y;

            if (width < MIN_SIZE) _width = MIN_SIZE;
            else _width = width;

            if (height < MIN_SIZE) _height = MIN_SIZE;
            else _height = height;

            _color = color;
            _isSelected = false;
        }

        public bool GetSelected()
        {
            return _isSelected;
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
        }

        public Color GetColor()
        {
            if (_isSelected)
            {
                return Color.Red;
            }
            return _color;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        protected int GetX() { return _x; }
        protected int GetY() { return _y; }
        protected int GetWidth() { return _width; }
        protected int GetHeight() { return _height; }

        protected int GetLeft() { return _x - _width / 2; }
        protected int GetRight() { return _x + _width / 2; }
        protected int GetTop() { return _y - _height / 2; }
        protected int GetBottom() { return _y + _height / 2; }

        public bool IsInBounds(int maxWidth, int maxHeight)//выход за границы экрана
        {
            if (GetLeft() < 0) return false;
            if (GetTop() < 0) return false;
            if (GetRight() > maxWidth) return false;
            if (GetBottom() > maxHeight) return false;

            return true;
        }

        public void ClampToBounds(int maxWidth, int maxHeight)//принуждаем фигуру возвратиться в границы
        {
            if (maxWidth < MIN_SIZE || maxHeight < MIN_SIZE) return;

            if (_width > maxWidth) _width = maxWidth;
            if (_height > maxHeight) _height = maxHeight;

            if (GetLeft() < 0) _x = _width / 2;
            if (GetTop() < 0) _y = _height / 2;
            if (GetRight() > maxWidth) _x = maxWidth - _width / 2;
            if (GetBottom() > maxHeight) _y = maxHeight - _height / 2;
        }

        public bool Move(int dx, int dy, int maxWidth, int maxHeight)
        {
            _x = _x + dx;
            _y = _y + dy;

            if (!IsInBounds(maxWidth, maxHeight))
            {
                _x = _x - dx;
                _y = _y - dy;
                return false;
            }
            return true;
        }

        public virtual bool Resize(int dw, int dh, int maxWidth, int maxHeight)
        {
            int oldWidth = _width;
            int oldHeight = _height;

            _width = _width + dw;
            _height = _height + dh;

            if (_width < MIN_SIZE) _width = MIN_SIZE;
            if (_height < MIN_SIZE) _height = MIN_SIZE;

            if (!IsInBounds(maxWidth, maxHeight))
            {
                _width = oldWidth;
                _height = oldHeight;
                return false;
            }
            return true;
        }

        public void Draw(Graphics g)
        {
            DrawShape(g);
        }


        protected abstract void DrawShape(Graphics g);
        public abstract bool Contains(int px, int py);
    }

    //=======================================================================================================
    // прямоугольник и квадрат
    public class RectangleShape : Shape
    {
        public RectangleShape(int x, int y, int width, int height, Color color)
            : base(x, y, width, height, color)
        {
        }

        protected override void DrawShape(Graphics g)
        {
            SolidBrush brush = new SolidBrush(GetColor());
            Pen borderPen = new Pen(Color.Black, 2);

            g.FillRectangle(brush, GetLeft(), GetTop(), GetWidth(), GetHeight());
            g.DrawRectangle(borderPen, GetLeft(), GetTop(), GetWidth(), GetHeight());
        }

        public override bool Contains(int px, int py)
        {
            return (px >= GetLeft() && px <= GetRight() && py >= GetTop() && py <= GetBottom());
        }
    }


    public class SquareShape : RectangleShape
    {
        public SquareShape(int x, int y, int size, Color color)
            : base(x, y, size, size, color)
        {
        }
        public override bool Resize(int dw, int dh, int maxWidth, int maxHeight)
        {
            int delta;
            if (dw != 0)
            {
                delta = dw;
            }
            else
            {
                delta = dh;
            }

            return base.Resize(delta, delta, maxWidth, maxHeight);
        }
    }
    //=========================================================================================================
    // эллипс и круг
    public class EllipseShape : Shape
    {
        public EllipseShape(int x, int y, int width, int height, Color color)
            : base(x, y, width, height, color)
        {
        }

        protected override void DrawShape(Graphics g)
        {
            SolidBrush brush = new SolidBrush(GetColor());
            Pen borderPen = new Pen(Color.Black, 2);

            g.FillEllipse(brush, GetLeft(), GetTop(), GetWidth(), GetHeight());
            g.DrawEllipse(borderPen, GetLeft(), GetTop(), GetWidth(), GetHeight());
        }

        public override bool Contains(int px, int py)
        {
            double rx = GetWidth() / 2.0;
            double ry = GetHeight() / 2.0;
            if (rx <= 0 || ry <= 0) return false;

            double dx = px - GetX();
            double dy = py - GetY();
            return ((dx * dx) / (rx * rx) + (dy * dy) / (ry * ry)) <= 1.0;
        }
    }

    public class CircleShape : EllipseShape
    {
        public CircleShape(int x, int y, int diameter, Color color)
            : base(x, y, diameter, diameter, color)
        {
        }

        public override bool Resize(int dw, int dh, int maxWidth, int maxHeight)
        {
            int delta;
            if (dw != 0)
            {
                delta = dw;
            }
            else
            {
                delta = dh;
            }

            return base.Resize(delta, delta, maxWidth, maxHeight);
        }
    }
    //======================================================================================
    // треугольник
    public class TriangleShape : Shape
    {
        public TriangleShape(int x, int y, int width, int height, Color color)
            : base(x, y, width, height, color)
        {
        }

        protected override void DrawShape(Graphics g)
        {
            Point[] vertices = new Point[3];
            vertices[0] = new Point(GetX(), GetTop());
            vertices[1] = new Point(GetLeft(), GetBottom());
            vertices[2] = new Point(GetRight(), GetBottom());

            SolidBrush brush = new SolidBrush(GetColor());
            Pen borderPen = new Pen(Color.Black, 2);

            g.FillPolygon(brush, vertices);
            g.DrawPolygon(borderPen, vertices);
        }

        public override bool Contains(int px, int py)
        {
            Point p = new Point(px, py);
            Point v0 = new Point(GetX(), GetTop());
            Point v1 = new Point(GetLeft(), GetBottom());
            Point v2 = new Point(GetRight(), GetBottom());

            float d1 = Sign(p, v0, v1);
            float d2 = Sign(p, v1, v2);
            float d3 = Sign(p, v2, v0);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }

        private float Sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }

    // =========================================================================
    // отрезок
    public class LineShape : Shape
    {
        public LineShape(int x, int y, int width, int height, Color color)
            : base(x, y, width, height, color)
        {
        }

        protected override void DrawShape(Graphics g)
        {
            Pen linePen = new Pen(GetColor(), 4);
            g.DrawLine(linePen, GetLeft(), GetTop(), GetRight(), GetBottom());
        }

        public override bool Contains(int px, int py)
        {
            double x1 = GetLeft();
            double y1 = GetTop();
            double x2 = GetRight();
            double y2 = GetBottom();

            double A = px - x1;
            double B = py - y1;
            double C = x2 - x1;
            double D = y2 - y1;

            double dot = A * C + B * D;
            double lenSq = C * C + D * D;

            double param;
            if (lenSq != 0)
            {
                param = dot / lenSq;
            }
            else
            {
                param = -1;
            }

            double xx, yy;
            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            double dx = px - xx;
            double dy = py - yy;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            return distance <= 7.0;
        }
    }
}