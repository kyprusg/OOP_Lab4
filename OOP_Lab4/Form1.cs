using OOP_Lab4_VisualEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Lab4
{
    public partial class Form1 : Form
    {
        private MyStorage<Shape> _shapes = new MyStorage<Shape>();

        private ShapeType _currentShapeType = ShapeType.Circle;
        private Color _currentColor = Color.CornflowerBlue;

        private const int MOVE_STEP = 5;
        private const int RESIZE_STEP = 6;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Л.Р. 4 — Визуальный редактор (Стрелки: движение | Shift+Стрелки или +/-: размер | Del: удалить | Ctrl+Клик: мультивыбор)";
            this.MinimumSize = new Size(600, 450);
            this.KeyPreview = true;

            SetupMenusAndToolbar();
            SetupCanvasEvents();
        }

        private void SetupMenusAndToolbar()
        {
            menuStrip1.Items.Clear();
            toolStrip1.Items.Clear();

            ToolStripMenuItem shapesMenu = new ToolStripMenuItem("Фигуры");
            AddShapeOption(shapesMenu, "Круг", ShapeType.Circle);
            AddShapeOption(shapesMenu, "Квадрат", ShapeType.Square);
            AddShapeOption(shapesMenu, "Эллипс", ShapeType.Ellipse);
            AddShapeOption(shapesMenu, "Прямоугольник", ShapeType.Rectangle);
            AddShapeOption(shapesMenu, "Треугольник", ShapeType.Triangle);
            AddShapeOption(shapesMenu, "Отрезок", ShapeType.Line);

            ToolStripMenuItem actionsMenu = new ToolStripMenuItem("Действия");
            actionsMenu.DropDownItems.Add("Выбрать цвет...", null, OnChooseColorClick);
            actionsMenu.DropDownItems.Add("Выделить все (Ctrl+A)", null, OnSelectAllClick);
            actionsMenu.DropDownItems.Add("Удалить выбранные (Del)", null, OnDeleteClick);
            actionsMenu.DropDownItems.Add("Очистить всё", null, OnClearAllClick);

            menuStrip1.Items.Add(shapesMenu);
            menuStrip1.Items.Add(actionsMenu);

            toolStrip1.Items.Add(new ToolStripSeparator());

            ToolStripButton btnColor = new ToolStripButton("Цвет");
            btnColor.BackColor = _currentColor;
            btnColor.Click += OnChooseColorClick;
            toolStrip1.Items.Add(btnColor);

            ToolStripButton btnDelete = new ToolStripButton("Удалить (Del)");
            btnDelete.Click += OnDeleteClick;
            toolStrip1.Items.Add(btnDelete);

            UpdateToolbarSelection();
        }

        private void AddShapeOption(ToolStripMenuItem parentMenu, string title, ShapeType type)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(title);
            menuItem.Tag = type;
            menuItem.Click += OnShapeToolClick;
            parentMenu.DropDownItems.Add(menuItem);

            ToolStripButton toolBtn = new ToolStripButton(title);
            toolBtn.Tag = type;
            toolBtn.Click += OnShapeToolClick;
            toolStrip1.Items.Add(toolBtn);
        }

        private void OnShapeToolClick(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = (ToolStripItem)sender;
            _currentShapeType = (ShapeType)clickedItem.Tag;
            UpdateToolbarSelection();
        }

        private void OnChooseColorClick(object sender, EventArgs e)
        {
            ChooseColor();
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            SelectAllShapes();
        }

        private void OnDeleteClick(object sender, EventArgs e)
        {
            DeleteSelectedShapes();
        }

        private void OnClearAllClick(object sender, EventArgs e)
        {
            ClearAllShapes();
        }

        private void UpdateToolbarSelection()
        {
            for (int i = 0; i < toolStrip1.Items.Count; i++)
            {
                ToolStripButton btn = toolStrip1.Items[i] as ToolStripButton;
                if (btn != null && btn.Tag != null)
                {
                    ShapeType type = (ShapeType)btn.Tag;
                    btn.Checked = (type == _currentShapeType);
                }
            }
        }

        private void SetupCanvasEvents()
        {
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.Resize += PictureBox1_Resize;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < _shapes.GetCount(); i++)
            {
                _shapes.GetObject(i).Draw(e.Graphics);
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            bool isCtrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
            Shape clickedShape = null;

            for (int i = _shapes.GetCount() - 1; i >= 0; i--)
            {
                if (_shapes.GetObject(i).Contains(e.X, e.Y))
                {
                    clickedShape = _shapes.GetObject(i);
                    break;
                }
            }

            if (clickedShape != null)
            {
                if (isCtrlPressed)
                {
                    clickedShape.SetSelected(!clickedShape.GetSelected());
                }
                else
                {
                    DeselectAll();
                    clickedShape.SetSelected(true);
                }
            }
            else
            {
                if (!isCtrlPressed)
                {
                    DeselectAll();
                }

                Shape newShape = CreateShape(e.X, e.Y);
                newShape.SetSelected(true);
                _shapes.Add(newShape);
            }

            pictureBox1.Invalidate();
        }

        private Shape CreateShape(int x, int y)
        {
            Shape shape;
            switch (_currentShapeType)
            {
                case ShapeType.Circle:
                    shape = new CircleShape(x, y, 70, _currentColor);
                    break;
                case ShapeType.Square:
                    shape = new SquareShape(x, y, 70, _currentColor);
                    break;
                case ShapeType.Ellipse:
                    shape = new EllipseShape(x, y, 90, 55, _currentColor);
                    break;
                case ShapeType.Rectangle:
                    shape = new RectangleShape(x, y, 90, 55, _currentColor);
                    break;
                case ShapeType.Triangle:
                    shape = new TriangleShape(x, y, 80, 70, _currentColor);
                    break;
                case ShapeType.Line:
                    shape = new LineShape(x, y, 80, 60, _currentColor);
                    break;
                default:
                    shape = new CircleShape(x, y, 70, _currentColor);
                    break;
            }

            shape.ClampToBounds(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            return shape;
        }

        private void PictureBox1_Resize(object sender, EventArgs e)
        {
            int w = pictureBox1.ClientSize.Width;
            int h = pictureBox1.ClientSize.Height;

            for (int i = 0; i < _shapes.GetCount(); i++)
            {
                _shapes.GetObject(i).ClampToBounds(w, h);
            }
            pictureBox1.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int maxW = pictureBox1.ClientSize.Width;
            int maxH = pictureBox1.ClientSize.Height;
            bool handled = false;

            switch (keyData)
            {
                case Keys.Left:
                    MoveSelected(-MOVE_STEP, 0, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Right:
                    MoveSelected(MOVE_STEP, 0, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Up:
                    MoveSelected(0, -MOVE_STEP, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Down:
                    MoveSelected(0, MOVE_STEP, maxW, maxH);
                    handled = true;
                    break;

                case Keys.Shift | Keys.Right:
                    ResizeSelected(RESIZE_STEP, 0, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Shift | Keys.Left:
                    ResizeSelected(-RESIZE_STEP, 0, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Shift | Keys.Down:
                    ResizeSelected(0, RESIZE_STEP, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Shift | Keys.Up:
                    ResizeSelected(0, -RESIZE_STEP, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Add:
                case Keys.Oemplus:
                case Keys.Shift | Keys.Oemplus:
                    ResizeSelected(RESIZE_STEP, RESIZE_STEP, maxW, maxH);
                    handled = true;
                    break;
                case Keys.Subtract:
                case Keys.OemMinus:
                case Keys.Shift | Keys.OemMinus:
                    ResizeSelected(-RESIZE_STEP, -RESIZE_STEP, maxW, maxH);
                    handled = true;
                    break;

                case Keys.Delete:
                case Keys.Back:
                    DeleteSelectedShapes();
                    handled = true;
                    break;
                case Keys.Control | Keys.A:
                    SelectAllShapes();
                    handled = true;
                    break;
                case Keys.Escape:
                    DeselectAll();
                    pictureBox1.Invalidate();
                    handled = true;
                    break;
            }

            if (handled) return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveSelected(int dx, int dy, int maxW, int maxH)
        {
            for (int i = 0; i < _shapes.GetCount(); i++)
            {
                Shape shape = _shapes.GetObject(i);
                if (shape.GetSelected())
                {
                    shape.Move(dx, dy, maxW, maxH);
                }
            }
            pictureBox1.Invalidate();
        }

        private void ResizeSelected(int dw, int dh, int maxW, int maxH)
        {
            for (int i = 0; i < _shapes.GetCount(); i++)
            {
                Shape shape = _shapes.GetObject(i);
                if (shape.GetSelected())
                {
                    shape.Resize(dw, dh, maxW, maxH);
                }
            }
            pictureBox1.Invalidate();
        }

        private void DeleteSelectedShapes()
        {
            for (int i = _shapes.GetCount() - 1; i >= 0; i--)
            {
                if (_shapes.GetObject(i).GetSelected())
                {
                    _shapes.RemoveAt(i);
                }
            }
            pictureBox1.Invalidate();
        }

        private void ClearAllShapes()
        {
            for (int i = _shapes.GetCount() - 1; i >= 0; i--)
            {
                _shapes.RemoveAt(i);
            }
            pictureBox1.Invalidate();
        }

        private void ChooseColor()
        {
            colorDialog1.Color = _currentColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _currentColor = colorDialog1.Color;

                for (int i = 0; i < _shapes.GetCount(); i++)
                {
                    Shape shape = _shapes.GetObject(i);
                    if (shape.GetSelected())
                    {
                        shape.SetColor(_currentColor);
                    }
                }
                pictureBox1.Invalidate();
            }
        }

        private void SelectAllShapes()
        {
            for (int i = 0; i < _shapes.GetCount(); i++)
            {
                _shapes.GetObject(i).SetSelected(true);
            }
            pictureBox1.Invalidate();
        }

        private void DeselectAll()
        {
            for (int i = 0; i < _shapes.GetCount(); i++)
            {
                _shapes.GetObject(i).SetSelected(false);
            }
        }
    }
}
