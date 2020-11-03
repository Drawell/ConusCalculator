using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConusCalculator
{
    public partial class Form1 : Form
    {
        public delegate void PaintFunc(Graphics g, float scale, float centerX, float centerY);

        Bitmap img = new Bitmap(1000, 1000);
        string filename = "ConusCalculator.cfg";
        double Rb;
        double Rk;
        double alfa;
        double Ha;
        double Hb;
        double L1;
        double L2;
        double lenL1;
        double lenL2;
        double A;

        bool isCalculate = false;
        bool isIgnoreCheck = false;

        CustomLineCap arrowCap;
        Pen blackPen;
        Pen bluePen;
        Pen redAnchorPen;
        Pen redAnchorBothPen;
        Font textFont;
        Brush textBrush;
        Brush cleanBrush;

        Dictionary<CheckBox, PaintFunc> checkBoxsAndFuncs;

        public Form1()
        {
            InitializeComponent();

            using (GraphicsPath capPath = new GraphicsPath())
            {
                // A triangle
                capPath.AddLine(-3, -6, 3, -6);
                capPath.AddLine(-3, -6, 0, 0);
                capPath.AddLine(0, 0, 3, -6);
                arrowCap = new System.Drawing.Drawing2D.CustomLineCap(null, capPath);
            }

            blackPen = new Pen(Color.Black, 3);
            bluePen = new Pen(Color.Blue, 1);
            redAnchorPen = new Pen(Color.Red, 3);
            redAnchorPen.CustomEndCap = arrowCap;

            redAnchorBothPen = new Pen(Color.Red, 3);
            redAnchorBothPen.CustomEndCap = arrowCap;
            redAnchorBothPen.CustomStartCap = arrowCap;


            textFont = new Font("Arial", 20);
            textBrush = new SolidBrush(Color.Black);
            cleanBrush = new SolidBrush(Color.White);

            checkBoxsAndFuncs = new Dictionary<CheckBox, PaintFunc>();
            checkBoxsAndFuncs.Add(checkBoxL1, PaintL1);
            checkBoxsAndFuncs.Add(checkBoxL2, PaintL2);
            checkBoxsAndFuncs.Add(checkBoxCurveL1, PaintCurveL1);
            checkBoxsAndFuncs.Add(checkBoxCurveL2, PaintCurveL2);
            checkBoxsAndFuncs.Add(checkBoxRb, PaintRb);
            checkBoxsAndFuncs.Add(checkBoxRk, PaintRk);
            checkBoxsAndFuncs.Add(checkBoxHa, PaintHa);
            checkBoxsAndFuncs.Add(checkBoxHb, PaintHb);
            checkBoxsAndFuncs.Add(checkBoxAlf, PaintAlfa);

        }

        private void textBoxInput_TextChanged(object sender, EventArgs e)
        {
            double D1 = 0;
            double D2 = 0;
            double H = 0;
            isCalculate = true;
            try
            {
                string strD1 = textBoxD1.Text.Trim().Replace(".", ",");
                string strD2 = textBoxD2.Text.Trim().Replace(".", ",");
                string strH = textBoxH.Text.Trim().Replace(".", ",");

                if (strD1.Length != 0)
                    D1 = Convert.ToDouble(strD1);
                else
                    isCalculate = false;
                if (strD2.Length != 0)
                    D2 = Convert.ToDouble(strD2);
                else
                    isCalculate = false;
                if (strH.Length != 0)
                    H = Convert.ToDouble(strH);
                else
                    isCalculate = false;

                if (isCalculate && D2 >= D1)
                {
                    isCalculate = false;
                }
            }
            catch
            {
                MessageBox.Show("Недопустимые символы ввода");
                isCalculate = false;
            }

            if (isCalculate)
            {
                Calculate(D1, D2, H);
                SaveConfig();
            }
            SetTextBoxText();
            Repaint();
        }

        private void Calculate(double D1, double D2, double H)
        {
            double tmp = (2.0 * H) / (D1 - D2);
            double k = Math.Sqrt(1.0 + tmp * tmp);

            Rb = k * D1 / 2.0;
            Rk = k * D2 / 2.0;
            alfa = 360.0 / k;

            Hb = Math.Sqrt(H * H + (D1 / 2.0 - D2 / 2.0) * (D1 / 2.0 - D2 / 2.0));
            L1 = Math.Sqrt(2 * Rk * Rk - (2 * Rk * Rk * Math.Cos(Math.PI * alfa / 180.0)));

            lenL1 = Math.PI * D2;
            lenL2 = Math.PI * D1;

            A = Math.PI * (D1 + D2) / 2.0 * Hb;

            if (alfa > 180)
            {
                L2 = 2 * Rb;
                Ha = Rb + Rb * Math.Cos(ToRad(360d - alfa) / 2);
            }
            else
            {
                L2 = Math.Sqrt(2 * Rb * Rb - (2 * Rb * Rb * Math.Cos(Math.PI * alfa / 180.0)));
                tmp = Math.Sqrt(2 * Rk * Rk - (2 * Rk * Rk * Math.Cos(Math.PI * alfa / 360.0)));
                double tmp2 = Math.Sqrt(tmp * tmp - L1 * L1 / 4);
                Ha = Hb + tmp2;
            }
        }

        private void SetTextBoxText()
        {
            if (isCalculate)
            {

                textBoxRb.Text = Math.Round(Rb, 2).ToString();
                textBoxRk.Text = Math.Round(Rk, 2).ToString();
                textBoxAlfa.Text = Math.Round(alfa, 2).ToString();
                textBoxHb.Text = Math.Round(Hb, 2).ToString();
                textBoxHa.Text = Math.Round(Ha, 2).ToString();
                textBoxL1.Text = Math.Round(L1, 2).ToString();
                textBoxL2.Text = Math.Round(L2, 2).ToString();
                textBoxLenL1.Text = Math.Round(lenL1, 2).ToString();
                textBoxLenL2.Text = Math.Round(lenL2, 2).ToString();
                textBoxA.Text = Math.Round(A, 2).ToString();
            }
            else
            {
                textBoxRb.Text = "-";
                textBoxRk.Text = "-";
                textBoxAlfa.Text = "-";
                textBoxHb.Text = "-";
                textBoxHa.Text = "-";
                textBoxL1.Text = "-";
                textBoxL2.Text = "-";
                textBoxLenL1.Text = "-";
                textBoxLenL2.Text = "-";
                textBoxA.Text = "-";
            }
        }

        private void pictureBoxPaint_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (isIgnoreCheck)
                return;

            Repaint();
            SaveConfig();
        }

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            foreach (var item in checkBoxsAndFuncs)
            {
                CheckBox checkBox = item.Key;
                checkBox.Checked = true;
            }

            Repaint();
        }
        private void buttonUnchecAll_Click(object sender, EventArgs e)
        {
            foreach (var item in checkBoxsAndFuncs)
            {
                CheckBox checkBox = item.Key;
                checkBox.Checked = false;
            }

            Repaint();
        }
        private void buttonCopyText_Click(object sender, EventArgs e)
        {
            if (!isCalculate)
            {
                MessageBox.Show("Еще ничего не посчитано!");
                return;
            }

            StringBuilder strB = new StringBuilder();
            strB.Append("Наибольший диаметр D1: ");
            strB.Append(textBoxD1.Text.Trim());

            strB.Append("\nНаименьший диаметр D2: ");
            strB.Append(textBoxD2.Text.Trim());

            strB.Append("\nВысота H: ");
            strB.Append(textBoxH.Text.Trim());

            strB.Append("\nRb: "); strB.Append(Math.Round(Rb, 2));
            strB.Append("\nRk: "); strB.Append(Math.Round(Rk, 2));
            strB.Append("\nУгол α: "); strB.Append(Math.Round(alfa, 2));
            strB.Append("\nHa: "); strB.Append(Math.Round(Ha, 2));
            strB.Append("\nHb: "); strB.Append(Math.Round(Hb, 2));
            strB.Append("\nL1: "); strB.Append(Math.Round(L1, 2));
            strB.Append("\nL2: "); strB.Append(Math.Round(L2, 2));
            strB.Append("\nДлина дуги L1: "); strB.Append(Math.Round(lenL1, 2));
            strB.Append("\nДлина дуги L2: "); strB.Append(Math.Round(lenL2, 2));
            strB.Append("\nПлощадь А: "); strB.Append(Math.Round(A, 2));

            Clipboard.SetText(strB.ToString());
        }

        private void buttonCopyImg_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetImage(pictureBoxPaint.Image);
            }
            catch
            {
                MessageBox.Show("Еще ничего не нарисовано!");
            }
        }

        #region Paint

        private void Repaint()
        {
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);

            if (!isCalculate)
            {
                pictureBoxPaint.Image = img;
                return;
            }

            // Create start and sweep angles.
            float startAngle = 90f - (float)alfa / 2f;
            float sweepAngle = (float)alfa;

            float centerX;
            float centerY;

            float scale;
            if (L2 < Ha) // scale by Ha
            {
                scale = (img.Height - 200) / ((float)Ha);
                centerX = (float)Ha / 2 * scale + 100;
                centerY = (float)(Ha - Rb) * scale + 100;
            }
            else if (L2 >= Rb * 1.9) // scale by Rb
            {
                scale = (img.Width - 100) / ((float)Rb * 2);
                centerX = (float)Rb * scale + 50;
                centerY = (float)Rb * scale;
            }
            else // scale by L2
            {
                scale = (img.Width - 200) / ((float)L2);
                centerX = (float)L2 / 2 * scale + 100;
                centerY = (float)(L2 - Rb) * scale + 100;
            }

            // Draw big pie
            float w = 2 * (float)Rb * scale;
            float h = 2 * (float)Rb * scale;
            float x = centerX - w / 2f;
            float y = centerY - h / 2f;
            g.DrawPie(blackPen, x, y, w, h, startAngle, sweepAngle);

            // Draw small pie
            w = 2 * (float)Rk * scale;
            h = 2 * (float)Rk * scale;
            x = centerX - w / 2f;
            y = centerY - h / 2f;
            g.DrawPie(blackPen, x, y, w, h, startAngle, sweepAngle);

            g.FillEllipse(new SolidBrush(Color.White), x + 1, y + 1, w - 2, h - 2);

            foreach (var item in checkBoxsAndFuncs)
            {
                CheckBox checkBox = item.Key;
                PaintFunc paintFunc = item.Value;
                if (checkBox.Checked)
                    paintFunc(g, scale, centerX, centerY);
            }

            pictureBoxPaint.Image = img;
        }
        public static double ToRad(double val)
        {
            return (Math.PI / 180) * val;
        }

        private void PaintRb(Graphics g, float scale, float centerX, float centerY)
        {
            float r = (float)Rb * scale;
            float angle = (360 - (float)alfa) / 2f + (float)alfa;
            float angleAdd = Math.Min(180, (float)alfa) / 18f;

            float toX = centerX - r * (float)Math.Sin(ToRad(angle - angleAdd));
            float toY = centerY - r * (float)Math.Cos(ToRad(angle - angleAdd));
            string str = $"Rb = {Math.Round(Rb, 1)}";

            g.DrawLine(redAnchorPen, centerX, centerY, toX, toY);
            float fromX = Math.Max(0, centerX);
            float fromY = Math.Max(0, centerY);

            g.FillRectangle(cleanBrush, (toX + fromX) / 2, (toY + fromY) / 2, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, (toX + fromX) / 2, (toY + fromY) / 2);
        }
        private void PaintRk(Graphics g, float scale, float centerX, float centerY)
        {
            float r = (float)Rk * scale;
            float angle = (360 - (float)alfa) / 2f;
            float angleAdd = Math.Min(180, (float)alfa) / 18f;

            float toX = centerX - r * (float)Math.Sin(ToRad(angle + angleAdd));
            float toY = centerY - r * (float)Math.Cos(ToRad(angle + angleAdd));
            string str = $"Rk = {Math.Round(Rk, 1)}";

            g.DrawLine(redAnchorPen, centerX, centerY, toX, toY);
            float fromX = Math.Max(0, centerX);
            float fromY = Math.Max(0, centerY);

            g.FillRectangle(cleanBrush, (toX + fromX) / 2 - 50, (toY + fromY) / 2, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, (toX + fromX) / 2 - 50, (toY + fromY) / 2);
        }
        private void PaintAlfa(Graphics g, float scale, float centerX, float centerY)
        {
            string str = $"α = {Math.Round(alfa, 1)}";
            float toX = Math.Max(0, centerX) - 7 * str.Length;
            float toY = Math.Max(0, centerY) + 20;

            g.FillRectangle(cleanBrush, toX, toY, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, toX, toY);
        }
        private void PaintHa(Graphics g, float scale, float centerX, float centerY)
        {
            float rk = (float)Rk * scale;
            float rb = (float)Rb * scale;
            float botY = centerY + rb;
            float topY = botY - (float)Ha * scale;
            float rightX = centerX + (float)L2 / 2 * scale + 20;


            float angle = (360 - (float)alfa) / 2f + (float)alfa;

            float leftTopX;
            if (alfa > 180)
                leftTopX = centerX - rb * (float)Math.Sin(ToRad(angle));
            else
                leftTopX = centerX - rk * (float)Math.Sin(ToRad(angle));

            g.DrawLine(bluePen, centerX, botY, rightX, botY);
            g.DrawLine(bluePen, leftTopX, topY, rightX, topY);
            g.DrawLine(redAnchorBothPen, rightX, botY, rightX, topY);

            string str = $"Ha = {Math.Round(Ha, 1)}";
            g.FillRectangle(cleanBrush, rightX - 120, (topY + botY) / 2 - 20, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, rightX - 120, (topY + botY) / 2 - 20);
        }
        private void PaintHb(Graphics g, float scale, float centerX, float centerY)
        {
            float rb = (float)Rb * scale;
            float rk = (float)Rk * scale;
            float angleAdd = Math.Min(180, (float)alfa) / 36f;
            float angle = (360 - (float)alfa) / 2f - angleAdd;

            float toX = centerX - rb * (float)Math.Sin(ToRad(angle));
            float toY = centerY - rb * (float)Math.Cos(ToRad(angle));
            float fromX = centerX - rk * (float)Math.Sin(ToRad(angle));
            float fromY = centerY - rk * (float)Math.Cos(ToRad(angle));
            string str = $"Hb = {Math.Round(Hb, 1)}";

            g.DrawLine(redAnchorBothPen, fromX, fromY, toX, toY);
            g.FillRectangle(cleanBrush, (toX + fromX) / 2 - 50, (toY + fromY) / 2 - 20, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, (toX + fromX) / 2 - 50, (toY + fromY) / 2 - 20);
        }
        private void PaintL1(Graphics g, float scale, float centerX, float centerY)
        {
            float rk = (float)Rk * scale;
            float rb = (float)Rb * scale;
            float angle = (360 - (float)alfa) / 2f;
            float leftX = centerX - rk * (float)Math.Sin(ToRad(angle));
            float rightX = centerX + rk * (float)Math.Sin(ToRad(angle));
            float topY = centerY - rk * (float)Math.Cos(ToRad(angle));
            float botY = centerY + rb + 10;
            string str = $"L1 = {Math.Round(L1, 1)}";

            g.DrawLine(bluePen, leftX, topY, leftX, botY);
            g.DrawLine(bluePen, rightX, topY, rightX, botY);
            g.DrawLine(redAnchorBothPen, leftX, botY, rightX, botY);
            g.FillRectangle(cleanBrush, (leftX + rightX) / 2 - 7 * str.Length, botY + 5, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, (leftX + rightX) / 2 - 7 * str.Length, botY + 5);
        }
        private void PaintL2(Graphics g, float scale, float centerX, float centerY)
        {
            float rb = (float)Rb * scale;
            float angle = (360 - (float)alfa) / 2f;
            float leftX = centerX - (float)L2 * scale / 2f;
            float rightX = centerX + (float)L2 * scale / 2f;
            float topY;
            if (alfa > 180)
                topY = centerY;
            else
                topY = centerY - rb * (float)Math.Cos(ToRad(angle));


            float botY = centerY + rb + 55;
            string str = $"L2 = {Math.Round(L2, 1)}";

            g.DrawLine(bluePen, leftX, topY, leftX, botY);
            g.DrawLine(bluePen, rightX, topY, rightX, botY);
            g.DrawLine(redAnchorBothPen, leftX, botY, rightX, botY);
            g.FillRectangle(cleanBrush, (leftX + rightX) / 2 - 7 * str.Length, botY + 5, 15 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, (leftX + rightX) / 2 - 7 * str.Length, botY + 5);

        }
        private void PaintCurveL1(Graphics g, float scale, float centerX, float centerY)
        {
            float w = 2 * (float)Rk * scale;
            float h = 2 * (float)Rk * scale;
            float x = centerX - w / 2f;
            float y = centerY - h / 2f;
            float toY = centerY + (float)Rk * scale;
            string str = $"Дуга L1 = {Math.Round(lenL1, 1)}";
            float toX = centerX - 7 * str.Length;
            float startAngle = 90f - (float)alfa / 2f;
            float sweepAngle = (float)alfa;

            g.DrawArc(redAnchorBothPen, x, y, w, h, startAngle, sweepAngle);
            g.FillRectangle(cleanBrush, toX, toY + 10, 14 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, toX, toY + 10);
        }
        private void PaintCurveL2(Graphics g, float scale, float centerX, float centerY)
        {
            float w = 2 * (float)Rb * scale;
            float h = 2 * (float)Rb * scale;
            float x = centerX - w / 2f;
            float y = centerY - h / 2f;
            float toY = centerY + (float)Rb * scale - 100;
            string str = $"Дуга L2 = {Math.Round(lenL2, 1)}";
            float toX = centerX - 7 * str.Length;
            float startAngle = 90f - (float)alfa / 2f;
            float sweepAngle = (float)alfa;

            g.DrawArc(redAnchorBothPen, x, y, w, h, startAngle, sweepAngle);
            g.FillRectangle(cleanBrush, toX, toY + 44, 14 * str.Length, 30);
            g.DrawString(str, textFont, textBrush, toX, toY + 40);
        }

        #endregion

        private void LoadConfig()
        {
            try
            {
                isIgnoreCheck = true;
                if (!File.Exists(filename))
                    return;

                using (StreamReader fs = new StreamReader(filename))
                {
                    string[] strs = fs.ReadLine().Split();
                    int idx = 3;
                    foreach (var item in checkBoxsAndFuncs)
                    {
                        CheckBox checkBox = item.Key;
                        bool check = Convert.ToBoolean(strs[idx++]);
                        checkBox.Checked = check;
                    }

                    textBoxD1.Text = strs[0];
                    textBoxD2.Text = strs[1];
                    textBoxH.Text = strs[2];
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                isIgnoreCheck = false;
            }
        }

        private void SaveConfig()
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(filename))
                {
                    fs.Write(textBoxD1.Text);
                    fs.Write(" ");
                    fs.Write(textBoxD2.Text);
                    fs.Write(" ");
                    fs.Write(textBoxH.Text);
                    fs.Write(" ");

                    foreach (var item in checkBoxsAndFuncs)
                    {
                        CheckBox checkBox = item.Key;
                        
                        fs.Write(checkBox.Checked);
                        fs.Write(" ");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadConfig();
            Repaint();
        }
    }
}
