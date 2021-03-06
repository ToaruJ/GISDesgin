﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace simpleGIS
{
    /// <summary>
    /// 符号标记类
    /// </summary>
    [Serializable]
    public abstract class Symbol
    {
        /// <summary>
        /// 将Symbol的属性保存到可序列化的字段中
        /// </summary>
        public virtual void SaveToStruct() { }

        /// <summary>
        /// 生成Symbol的副本
        /// </summary>
        /// <returns>新的Symbol</returns>
        public abstract Symbol Clone();

        /// <summary>
        /// 以调用对象为原型，生成随机符号
        /// </summary>
        /// <param name="num">生成的随机符号个数</param>
        /// <returns>生成的Symbol</returns>
        public abstract Symbol[] RandomSymbolFromSelf(int num);
    }

    /// <summary>
    /// 点标记类
    /// </summary>
    [Serializable]
    public class PointSymbol : Symbol
    {
        #region 字段

        private int pointType;
        private Color color;
        private float size;

        private static readonly float sqrt3 = (float)Math.Sqrt(3);

        #endregion

        #region 属性

        /// <summary>
        /// 点符号的具体形状
        /// </summary>
        public int PointType { get => pointType; set => pointType = value; }
        
        /// <summary>
        /// 点的颜色
        /// </summary>
        public Color Color { get => color; set => color = value; }
        
        /// <summary>
        /// 点的大小（单位为像素）
        /// </summary>
        public float Size { get => size; set => size = value; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造点符号
        /// </summary>
        /// <param name="_pointType">点的符号类型，1~8</param>
        /// <param name="_color">点的颜色</param>
        /// <param name="_size">点的大小，单位像素</param>
        public PointSymbol(int _pointType, Color _color, float _size)
        {
            pointType = _pointType;
            color = _color;
            size = _size;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 生成PointSymbol的副本
        /// </summary>
        /// <returns>新的PointSymbol</returns>
        public override Symbol Clone()
        {
            return new PointSymbol(pointType, color, size);
        }

        /// <summary>
        /// 以调用对象为原型，生成随机点符号
        /// </summary>
        /// <param name="num">生成的随机符号个数</param>
        /// <returns>生成的PointSymbol</returns>
        public override Symbol[] RandomSymbolFromSelf(int num)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            PointSymbol[] result = new PointSymbol[num];
            for (int i = 0; i < num; i++)
            {
                result[i] = new PointSymbol(random.Next(1, 9),
                    Color.FromKnownColor((KnownColor)random.Next(28, 168)), size);
            }
            return result;
        }

        /// <summary>
        /// 在绘图上绘制该点符号
        /// </summary>
        /// <param name="g">GDI绘图对象</param>
        /// <param name="pointF">点中心的屏幕坐标PointF</param>
        public void DrawPoint(Graphics g, PointF pointF)
        {
            Pen pen = new Pen(color);
            SolidBrush brush = new SolidBrush(color);
            switch (pointType)
            {
                // 空心圆
                case 1:
                    g.DrawEllipse(pen, pointF.X - size / 2, pointF.Y - size / 2, size, size);
                    break;
                // 实心圆
                case 2:
                    g.FillEllipse(brush, pointF.X - size / 2, pointF.Y - size / 2, size, size);
                    break;
                // 空心矩形
                case 3:
                    g.DrawRectangle(pen, pointF.X - size / 2, pointF.Y - size / 2, size, size);
                    break;
                // 实心矩形
                case 4:
                    g.FillRectangle(brush, pointF.X - size / 2, pointF.Y - size / 2, size, size);
                    break;
                // 空心三角
                case 5:
                    PointF[] pointFs = new PointF[3]
                        { new PointF(pointF.X, pointF.Y - size / sqrt3),
                            new PointF(pointF.X - size / 2, pointF.Y + size / 2 / sqrt3),
                            new PointF(pointF.X + size / 2, pointF.Y + size / 2 / sqrt3) };
                    g.DrawPolygon(pen, pointFs);
                    break;
                // 实心三角
                case 6:
                    PointF[] points = new PointF[3]
                        { new PointF(pointF.X, pointF.Y - size / sqrt3),
                            new PointF(pointF.X - size / 2, pointF.Y + size / 2 / sqrt3),
                            new PointF(pointF.X + size / 2, pointF.Y + size / 2 / sqrt3) };
                    g.FillPolygon(brush, points);
                    break;
                // 圈点
                case 7:
                    g.DrawEllipse(pen, pointF.X - size / 2, pointF.Y - size / 2, size, size);
                    g.FillEllipse(brush, pointF.X - size / 6, pointF.Y - size / 6, size / 3, size / 3);
                    break;
                // 双空心圈
                case 8:
                    g.DrawEllipse(pen, pointF.X - size / 2, pointF.Y - size / 2, size, size);
                    g.DrawEllipse(pen, pointF.X - size / 4, pointF.Y - size / 4, size / 2, size / 2);
                    break;
            }
        }

        #endregion

    }

    /// <summary>
    /// 线标记类
    /// </summary>
    [Serializable]
    public class LineSymbol : Symbol
    {
        #region 字段

        [NonSerialized]
        private Pen style;

        private Color color;
        private float width;
        private DashStyle dashStyle;

        #endregion

        #region 属性

        /// <summary>
        /// 线的符号类型，.Net自带，里面有线型，颜色，宽度
        /// </summary>
        public Pen Style
        {
            get
            {
                if (style == null) {
                    style = new Pen(color, width);
                    style.DashStyle = dashStyle;
                }
                return style;
            }
            set
            {
                style = value;
                color = style.Color;
                width = style.Width;
                dashStyle = style.DashStyle;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造线符号
        /// </summary>
        /// <param name="_style">线的样式</param>
        public LineSymbol(Pen _style)
        {
            style = _style;
            color = _style.Color;
            width = _style.Width;
            dashStyle = _style.DashStyle;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 生成LineSymbol的副本
        /// </summary>
        /// <returns>新的LineSymbol</returns>
        public override Symbol Clone()
        {
            return new LineSymbol((Pen)style.Clone());
        }

        /// <summary>
        /// 以调用对象为原型，生成随机线符号
        /// </summary>
        /// <param name="num">生成的随机符号个数</param>
        /// <returns>生成的LineSymbol</returns>
        public override Symbol[] RandomSymbolFromSelf(int num)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            LineSymbol[] result = new LineSymbol[num];
            for (int i = 0; i < num; i++)
            {
                Pen _pen = new Pen(Color.FromKnownColor((KnownColor)random.Next(28, 168)), width);
                _pen.DashStyle = style.DashStyle;
                    result[i] = new LineSymbol(_pen);
            }
            return result;
        }

        /// <summary>
        /// 在绘图上绘制该线符号
        /// </summary>
        /// <param name="g">GDI绘图对象</param>
        /// <param name="pointFs">pointF[]</param>
        public void DrawLine(Graphics g, PointF[] pointFs)
        {
            g.DrawLines(Style, pointFs);
        }
        
        public override void SaveToStruct()
        {
            color = style.Color;
            width = style.Width;
            dashStyle = style.DashStyle;
        }

        #endregion
    }

    /// <summary>
    /// 面标记类
    /// </summary>
    [Serializable]
    public class PolygonSymbol : Symbol
    {
        #region 字段

        [NonSerialized]
        private Pen outLine;
        [NonSerialized]
        private SolidBrush fill;
        
        private Color outLineColor;
        private Color fillColor;

        #endregion

        #region 属性

        /// <summary>
        /// 多边形的边界样式
        /// </summary>
        public Pen OutLine
        {
            get
            {
                if (outLine == null)
                {
                    outLine = new Pen(outLineColor);
                }
                return outLine;
            }
            set
            {
                outLine = value;
                outLineColor = outLine.Color;
            }
        }

        /// <summary>
        /// 多边形内部填充样式
        /// </summary>
        public SolidBrush Fill
        {
            get
            {
                if (fill == null)
                {
                    fill = new SolidBrush(fillColor);
                }
                return fill;
            }
            set
            {
                fill = value;
                fillColor = fill.Color;
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_outline">多边形的边界样式</param>
        /// <param name="_fill">多边形内部填充样式</param>
        public PolygonSymbol(Pen _outline, SolidBrush _fill)
        {
            outLine = _outline;
            fill = _fill;

            outLineColor = _outline.Color;
            fillColor = _fill.Color;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 生成PolygonSymbol的副本
        /// </summary>
        /// <returns>新的PolygonSymbol</returns>
        public override Symbol Clone()
        {
            return new PolygonSymbol((Pen)outLine.Clone(), (SolidBrush)fill.Clone());
        }

        /// <summary>
        /// 以调用对象为原型，生成随机面符号
        /// </summary>
        /// <param name="num">生成的随机符号个数</param>
        /// <returns>生成的PolygonSymbol</returns>
        public override Symbol[] RandomSymbolFromSelf(int num)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            PolygonSymbol[] result = new PolygonSymbol[num];
            for (int i = 0; i < num; i++)
            {
                Pen _pen = new Pen(Color.FromKnownColor((KnownColor)random.Next(28, 168)));
                SolidBrush _brush = new SolidBrush(Color.FromKnownColor((KnownColor)random.Next(28, 168)));
                result[i] = new PolygonSymbol(_pen, _brush);
            }
            return result;
        }

        /// <summary>
        /// 在绘图上绘制该面符号
        /// </summary>
        /// <param name="g">GDI绘图对象</param>
        /// <param name="pointFs">PointF[]</param>
        public void DrawPolygon(Graphics g, PointF[] pointFs)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
           
            g.FillPolygon(Fill, pointFs);
            g.DrawPolygon(OutLine, pointFs);
        }
        
        public override void SaveToStruct()
        {
            outLineColor = outLine.Color;
            fillColor = fill.Color;
        }

        #endregion
    }
}