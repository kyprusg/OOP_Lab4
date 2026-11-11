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
  
}