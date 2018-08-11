using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace WindowsFormsApp1
{
    class E2graph
    {
        // -------------------------------------------------------- Prerequisites --------------------------------------------------------
        #region Define prereq for the Latex Font
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private System.Drawing.Text.PrivateFontCollection fonts = new System.Drawing.Text.PrivateFontCollection();
        private Font latexFont;
        #endregion

        // -------------------------------------------------------- Fields and Properties --------------------------------------------------------
        public Graphics Graph;
        private E2data E2data;

        public int HeightGraph;
        public int WidthGraph;

        public int MaxXCoordGraph;
        public int MaxYCoordGraph;

        public float IntervalGridInPixelsPerUnit;
        private int IntervalGridInUnits;

        public float RatioInputToPixelOnBitmap; // this is defined as the nbr of pixel per interval (of length 1) / 50 (where 50 was chosen to make the range of scales for input better)

        public PointF Origin;
        public PointF AxisXend;
        public PointF AxisYend;

        private Color[] ColorTauTorsion; // put it here because we use it both in DrawPoints and DrawExtensions
        private StringFormat formatCentered;

        public bool HadToRescale;

        // -------------------------------------------------------- Constructors --------------------------------------------------------
        public E2graph(Graphics graphics, E2data e2Data, int width, int height, int maxX, int maxY, int intervalOnGrid, int marginToGraphLeft, int marginToGraphRight, int marginToGraphTop, int marginToGraphBottom, int axisXspaceToLabel, int axisYspaceToLabel, int deltaBetweenAdjacentPoints, float pointsize, bool goodQuality, Color[] colorTauTorsion)
        {
            #region Initiliaze the Latex Font
            byte[] fontData = Properties.Resources.pala;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.pala.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.pala.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            latexFont = new System.Drawing.Font(fonts.Families[0], 16.0F);
            #endregion

            E2data = e2Data;

            if (Graph != null)
                Graph.Dispose();

            Graph = graphics;
            HeightGraph = height;
            WidthGraph = width;

            ColorTauTorsion = colorTauTorsion;

            // so that the labels are drawn by specifiying the center of the string, like it is the case in Latex
            formatCentered = new StringFormat();
            formatCentered.LineAlignment = StringAlignment.Center;
            formatCentered.Alignment = StringAlignment.Center;

            // just making sure that these are multiples of the interval on the grid, so that it ends nicely
            MaxXCoordGraph = maxX + (maxX % intervalOnGrid);
            MaxYCoordGraph = maxY + (maxY % intervalOnGrid);
            
            #region Setting quality of Graph
            if(goodQuality)
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
            else
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            }

            #endregion 

            #region Calculating coordinates of Origin and Endpoints of axis, length of interval, etc
            
            float l_ratioForMargin = (float)width / (MaxXCoordGraph * 50); // the *width is so that margins are respected on different resolutions, the /2000 is just a rescalling so that the input is reasonable

            float marginToOriginX = marginToGraphLeft * l_ratioForMargin; 
            float marginToOriginY = marginToGraphBottom * l_ratioForMargin;
            float marginToEndAxisX = marginToGraphRight * l_ratioForMargin;
            float marginToEndAxisY = marginToGraphTop * l_ratioForMargin;


            Origin = new PointF(marginToOriginX + axisYspaceToLabel * l_ratioForMargin, height - marginToOriginY - axisXspaceToLabel * l_ratioForMargin); // the margin is already multiplied by this ratio

            if ((float)width / height <= (float)MaxXCoordGraph / MaxYCoordGraph)
            {
                AxisXend = new PointF(width - marginToEndAxisX, height - marginToOriginY - axisXspaceToLabel * l_ratioForMargin);
                AxisYend = new PointF(marginToOriginX + axisYspaceToLabel * l_ratioForMargin, height - marginToOriginY - (float)maxY * width / maxX);
            }
            else
            {
                AxisYend = new PointF(marginToEndAxisX + axisYspaceToLabel * l_ratioForMargin, marginToEndAxisY);
                AxisXend = new PointF(width - marginToOriginX + (float)maxX * height - maxY, height - marginToOriginY - axisXspaceToLabel * l_ratioForMargin);
            }
            
            IntervalGridInUnits = intervalOnGrid;
            float stepX = (AxisXend.X - Origin.X) / ((float)MaxXCoordGraph / IntervalGridInUnits);
            float stepY = (Origin.Y - AxisYend.Y) / ((float)MaxYCoordGraph / IntervalGridInUnits);
            IntervalGridInPixelsPerUnit = Math.Min(stepX, stepY) / IntervalGridInUnits;

            AxisXend.X = Origin.X + MaxXCoordGraph * IntervalGridInPixelsPerUnit;
            AxisYend.Y = Origin.Y - MaxYCoordGraph * IntervalGridInPixelsPerUnit;

            RatioInputToPixelOnBitmap = IntervalGridInPixelsPerUnit / 50;

            // rescale if the delta is too big to make sure that all points at the same (s,f) fit in the same box
            HadToRescale = false;
            while (deltaBetweenAdjacentPoints * RatioInputToPixelOnBitmap * E2data.MaxNbrElemPerStemFilt > IntervalGridInPixelsPerUnit)
            {
                RatioInputToPixelOnBitmap -= RatioInputToPixelOnBitmap / 10;
                HadToRescale = true;
                //PointsSize -= PointsSize / 5;
                //DeltaBetweenAdjacentPoints -= DeltaBetweenAdjacentPoints / 5;
            }

            #endregion

        }

        // -------------------------------------------------------- Methods --------------------------------------------------------
        public float GetIntervalBitmapPixelPerUnit()
        {
            return IntervalGridInPixelsPerUnit;
        }

        private Font ConstructLatexFont(float sizeOfFont)
        {
            return new Font(fonts.Families[0], sizeOfFont);
        } // what is the 2nd param when creating a FONT ? is it really the size ? or precision or something ?

        public void InitiatePoints(int deltaBetweenAdjacentPoints)
        {
            for (int x = 0; x <= E2data.MaxStem; x++)
            {
                for (int y = 0; y <= E2data.MaxFilt; y++)
                {
                    if (E2data.Elements[x][y] != null)
                    {
                        int count = 0;

                        PointF currentPoint = Origin;
                        currentPoint.X += x * IntervalGridInPixelsPerUnit;
                        currentPoint.Y -= y * IntervalGridInPixelsPerUnit;

                        for (int i = 0; i < E2data.Elements[x][y].Count; i++)
                        {
                            bool isvisible = !E2data.Elements[x][y][i].IsPeriodicAndLastPoint && E2data.Elements[x][y][i].IsVisible;

                            E2data.Elements[x][y][i].SetGeometricPoint(currentPoint);

                            if (isvisible)
                                count++;
                        }

                        float delta = deltaBetweenAdjacentPoints * RatioInputToPixelOnBitmap;

                        // now shift the points if there are more than 1 per StemFilt intersection
                        for (int i = 0; i < count; i++)
                        {
                            if(!(E2data.Elements[x][y][i].IsPeriodicAndLastPoint))
                                E2data.Elements[x][y][i].ShiftXGeomPoint((2 * i - count + 1) * delta / 2);
                        }
                    }
                }
            }
        }
        public void InitiateExtensions(float arrowInLocalizationFactor)
        {
            for (int k = 0; k < E2data.Extensions.Count; k++)
            {
                for (int i = 0; i < E2data.Extensions[k].IndicesExtensions.Count; i++)
                {
                    if (E2data.Extensions[k].IndicesExtensions[i][1][2] != -1) // NOT GOOD
                    {
                        #region If the element is the end of a localization, then move it a bit
                        if (E2data.GetElement(E2data.Extensions[k].IndicesExtensions[i][1]).IsPeriodicAndLastPoint == true)
                        {
                            E2data.GetElement(E2data.Extensions[k].IndicesExtensions[i][1]).ShiftXGeomPoint((arrowInLocalizationFactor - 1) * E2data.Extensions[k].StemElem * IntervalGridInPixelsPerUnit);
                            E2data.GetElement(E2data.Extensions[k].IndicesExtensions[i][1]).ShiftYGeomPoint((1 - arrowInLocalizationFactor) * E2data.Extensions[k].FiltElem * IntervalGridInPixelsPerUnit);
                        }
                        #endregion

                        if (E2data.GetElement(E2data.Extensions[k].IndicesExtensions[i][1]).LatexLabel == null)
                            E2data.GetElement(E2data.Extensions[k].IndicesExtensions[i][1]).IsLabelVisible = false;
                    }
                }
            }
        }

        public void DrawFrame(Color colorFrame, int widthFrame)
        {
            using (Pen FramePen = new Pen(colorFrame, widthFrame * RatioInputToPixelOnBitmap))
            {
                Point point1 = new Point(0, 0);
                Point point2 = new Point(WidthGraph, 0);
                Point point3 = new Point(WidthGraph, HeightGraph);
                Point point4 = new Point(0, HeightGraph);

                Graph.DrawLine(FramePen, point1, point2);
                Graph.DrawLine(FramePen, point2, point3);
                Graph.DrawLine(FramePen, point3, point4);
                Graph.DrawLine(FramePen, point4, point1);
            }
        }
        public void DrawAxis(Color colorAxis, float widthAxis)
        {
            using (Pen AxisPen = new Pen(colorAxis, widthAxis * RatioInputToPixelOnBitmap))
            {
                Graph.DrawLine(AxisPen, Origin, AxisXend);
                Graph.DrawLine(AxisPen, Origin, AxisYend);
            }
        }
        public void DrawGrid(Color colorGrid, float widthGrid)
        {
            using (Pen GridPen = new Pen(colorGrid, widthGrid * RatioInputToPixelOnBitmap))
            {
                PointF pointX1 = Origin, pointX2 = AxisYend;
                PointF pointY1 = Origin, pointY2 = AxisXend;

                for (int i = 1; i < (float)MaxXCoordGraph / 2 + 1; i++)
                {
                    pointX1.X += IntervalGridInPixelsPerUnit * IntervalGridInUnits;
                    pointX2.X += IntervalGridInPixelsPerUnit * IntervalGridInUnits;
                    Graph.DrawLine(GridPen, pointX1, pointX2);
                }

                for (int i = 1; i < (float)MaxYCoordGraph / 2 + 1; i++)
                {
                    pointY1.Y -= IntervalGridInPixelsPerUnit * IntervalGridInUnits;
                    pointY2.Y -= IntervalGridInPixelsPerUnit * IntervalGridInUnits;
                    Graph.DrawLine(GridPen, pointY1, pointY2);
                }
            }
        }
        public void DrawAxisLabel(int axisXspaceToLabel, int axisYspaceToLabel, int fontSizeAxis)
        {
            using (Brush axisBrush = new SolidBrush(Color.Black))
            {
                Font axisFont = ConstructLatexFont(fontSizeAxis * RatioInputToPixelOnBitmap);

                PointF pointLabelX = Origin;
                PointF pointLabelY = Origin;

                pointLabelX.X -= 8 * RatioInputToPixelOnBitmap;
                pointLabelX.Y += axisXspaceToLabel * RatioInputToPixelOnBitmap;
                for (int i = 0; i < (float)MaxXCoordGraph / 2 + 1; i++)
                {
                    string s = (2 * i).ToString();
                    Graph.DrawString(s, axisFont, axisBrush, pointLabelX);
                    pointLabelX.X += IntervalGridInPixelsPerUnit * IntervalGridInUnits;
                }

                pointLabelY.X -= axisYspaceToLabel * RatioInputToPixelOnBitmap;
                pointLabelY.Y -= 10 * RatioInputToPixelOnBitmap;
                for (int i = 0; i < (float)MaxYCoordGraph / 2 + 1; i++)
                {
                    string s = (2 * i).ToString();
                    Graph.DrawString(s, axisFont, axisBrush, pointLabelY);
                    pointLabelY.Y -= IntervalGridInPixelsPerUnit * IntervalGridInUnits;

                    if (i == 4)
                        pointLabelY.X -= 2 * RatioInputToPixelOnBitmap;
                }
            }

        }

        private void DrawPoint(PointF point, Pen pen)
        {
            float pw = pen.Width * RatioInputToPixelOnBitmap / 2;
            using (var brush = new SolidBrush(pen.Color))
                Graph.FillEllipse(brush, point.X - pw, point.Y - pw, pen.Width * RatioInputToPixelOnBitmap, pen.Width * RatioInputToPixelOnBitmap);
        }
        public void DrawAllPoints(float pointSize)
        {
            using (Pen currentPen = new Pen(Color.Black, pointSize))
            {
                for (int x = 0; x <= E2data.MaxStem; x++)
                {
                    for (int y = 0; y <= E2data.MaxFilt; y++)
                    {
                        if (E2data.Elements[x][y] != null)
                        {
                            for (int i = 0; i < E2data.Elements[x][y].Count; i++)
                            {
                                currentPen.Color = ColorTauTorsion[E2data.Elements[x][y][i].TauTorsion];
                                if (E2data.Elements[x][y][i].IsVisible)
                                {
                                    PointF currentPoint = E2data.Elements[x][y][i].GetPointF();
                                    currentPoint.X += E2data.Elements[x][y][i].OffsetX;
                                    currentPoint.Y += E2data.Elements[x][y][i].OffsetY;
                                    DrawPoint(currentPoint, currentPen);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawExtensions(int nbrOfExt, float extPenSize, int arrowOpensAngle, int arrowLength, int arrowInsideDiagonal)
        {
            using (Pen currentPen = new Pen(Color.Red, extPenSize * RatioInputToPixelOnBitmap))
            {    
                //currentPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

                if (E2data.Extensions.Count <= nbrOfExt)
                {
                    // problem as it doesn't exist... throw exception + check if the thing is not null also...
                }
                else
                {
                    for (int i = 0; i < E2data.Extensions[nbrOfExt].IndicesExtensions.Count; i++)
                    {
                        if (E2data.Extensions[nbrOfExt].IndicesExtensions[i][1][2] != -1) // NOT GOOD
                        {
                            currentPen.Color = E2data.Extensions[nbrOfExt].ExtDefaultColor;
                            currentPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                            if (E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][1]).TauTorsion != 0)
                                currentPen.Color = ColorTauTorsion[E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][1]).TauTorsion];

                            List<string> extensionInfo = E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][0]).PropertyExtTarget[nbrOfExt];
                            if (extensionInfo != null)
                            {
                                int powerOfTau = 0;
                                for (int j = 0; j < extensionInfo.Count; j++)
                                {
                                    string extinfo = extensionInfo[j].Replace(" ", "");

                                    if (extinfo.Contains("h"))
                                    {
                                        currentPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                                        extinfo = extinfo.Replace("h", "");
                                    }

                                    if (extinfo.Contains("t"))
                                    {
                                        extinfo = extinfo.Replace("t", "");
                                        if (extinfo == "")
                                        {
                                            powerOfTau = 1;
                                            break;
                                        }
                                        powerOfTau = Convert.ToInt32(extinfo); // NEED TO CHECK THAT THis doesn't fail.... 
                                        break;
                                    }
                                }

                                switch (powerOfTau)
                                {
                                    case 1:
                                        currentPen.Color = Color.Magenta;
                                        break;
                                    case 2:
                                        currentPen.Color = Color.Orange;
                                        break;
                                    case 0:
                                    default:
                                        break; // AND THROW EXCEPTION TOO.....
                                }
                            }

                            PointF sourcePoint = E2data.GetPointF(E2data.Extensions[nbrOfExt].IndicesExtensions[i][0]);
                            PointF targetPoint = E2data.GetPointF(E2data.Extensions[nbrOfExt].IndicesExtensions[i][1]);

                            // If the element is the end of a localization, draw an arrow towards it
                            if (E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][1]).IsPeriodicAndLastPoint == true)
                            {
                                DrawArrowEndOfLineExtension(currentPen, ref sourcePoint, ref targetPoint, arrowOpensAngle * (Math.PI / 180), arrowLength, arrowInsideDiagonal);
                            }
                            else
                            {
                                targetPoint.X += E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][1]).OffsetX;
                                targetPoint.Y += E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][1]).OffsetY;

                                sourcePoint.X += E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][0]).OffsetX;
                                sourcePoint.Y += E2data.GetElement(E2data.Extensions[nbrOfExt].IndicesExtensions[i][0]).OffsetY;
                            }

                            Graph.DrawLine(currentPen, sourcePoint, targetPoint);

                        }
                        else
                        {

                        }
                    }
                }
            }


        }
        private void DrawArrowEndOfLineExtension(Pen currentPen, ref PointF sourcePoint, ref PointF targetPoint, double arrowOpensAngle, int arrowLength, int arrowInsideDiagonal)
        {
            PointF endLeftWing = new PointF(targetPoint.X, targetPoint.Y);
            PointF endRightWing = new PointF(targetPoint.X, targetPoint.Y);
            PointF concaveInArrow = new PointF(targetPoint.X, targetPoint.Y);

            double angleOfExtension = Math.Atan((sourcePoint.Y - targetPoint.Y) / (targetPoint.X - sourcePoint.X));

            endLeftWing.X -= arrowLength * (float)Math.Cos(angleOfExtension - arrowOpensAngle / 2) * RatioInputToPixelOnBitmap;
            endLeftWing.Y += arrowLength * (float)Math.Sin(angleOfExtension - arrowOpensAngle / 2) * RatioInputToPixelOnBitmap;

            endRightWing.X -= arrowLength * (float)Math.Cos(angleOfExtension + arrowOpensAngle / 2) * RatioInputToPixelOnBitmap;
            endRightWing.Y += arrowLength * (float)Math.Sin(angleOfExtension + arrowOpensAngle / 2) * RatioInputToPixelOnBitmap;

            concaveInArrow.X -= arrowInsideDiagonal * (float)Math.Cos(angleOfExtension) * RatioInputToPixelOnBitmap;
            concaveInArrow.Y += arrowInsideDiagonal * (float)Math.Sin(angleOfExtension) * RatioInputToPixelOnBitmap;

            Graph.DrawLines(currentPen, new PointF[] { targetPoint, endLeftWing, concaveInArrow, endRightWing, targetPoint, endLeftWing });

        }
        
        public void DrawLabels(int labelSize, int labelDist)
        {
            E2data.Elements[0][0][0].IsLabelVisible = false; // hide the label of the element 1
                 

            for (int x = 0; x <= E2data.MaxStem; x++)
            {
                for (int y = 0; y <= E2data.MaxFilt; y++)
                {
                    if (E2data.Elements[x][y] != null)
                    {
                        for (int i = 0; i < E2data.Elements[x][y].Count; i++)
                        {
                            if (E2data.Elements[x][y][i].IsLabelVisible)
                            {
                                PointF currentPoint = E2data.Elements[x][y][i].GetPointF();

                                currentPoint.X += RatioInputToPixelOnBitmap * labelDist * (float)Math.Cos((float)E2data.Elements[x][y][i].LabelAngle * Math.PI / 180);
                                currentPoint.Y -= RatioInputToPixelOnBitmap * labelDist * (float)Math.Sin((float)E2data.Elements[x][y][i].LabelAngle * Math.PI / 180);

                                currentPoint.X += E2data.Elements[x][y][i].OffsetX + E2data.Elements[x][y][i].OffsetXlabel;
                                currentPoint.Y += E2data.Elements[x][y][i].OffsetY + E2data.Elements[x][y][i].OffsetYlabel;


                                Graph.DrawString(E2data.Elements[x][y][i].LatexLabel, ConstructLatexFont(labelSize * RatioInputToPixelOnBitmap), Brushes.Black, currentPoint, formatCentered);

                                //using(Bitmap bp = fmath.controls.MathMLFormulaControl.generateBitmapFromLatex("$c_0$"))
                                  //  Graph.DrawImage(bp, currentPoint.X, currentPoint.Y, bp.Width, bp.Height);


                            }
                        }
                    }
                }
            }

        }

        public void DrawRectangle(Color color, int stemMiddlePoint, int filtMiddlePoint, int stemMaxExt, int filtMaxExt)
        {
            float topLeftX = Origin.X + (stemMiddlePoint - stemMaxExt) * IntervalGridInPixelsPerUnit - IntervalGridInPixelsPerUnit / 2;
            float topleftY = Origin.Y - (filtMiddlePoint + filtMaxExt) * IntervalGridInPixelsPerUnit + IntervalGridInPixelsPerUnit / 2;

            float widthX = (2 * stemMaxExt + 1) * IntervalGridInPixelsPerUnit;
            float heightY = (2 * filtMaxExt + 1) * IntervalGridInPixelsPerUnit;

            using( Pen localPen = new Pen(Color.Black, 10))
                Graph.DrawRectangle(new Pen(Color.Black, 10), topLeftX, topleftY, widthX, heightY);

            using (Brush brush = new SolidBrush(color))
                Graph.FillRectangle(brush, topLeftX, topleftY, widthX, heightY);

        }
    }

}
