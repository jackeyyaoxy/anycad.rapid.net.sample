﻿using AnyCAD.Exchange;
using AnyCAD.Forms;
using AnyCAD.Foundation;
using System;
using System.Windows.Forms;

namespace AnyCAD.Demo
{
    public partial class MainForm : Form
    {
        RenderControl mRenderView;
        public MainForm()
        {
            InitializeComponent();

            mRenderView = new RenderControl(this.splitContainer1.Panel2);

            TestCase.Register(this.treeView1);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mRenderView.SetSelectCallback((PickedItem item) =>
            {
                this.listBox1.Items.Clear();
                if (item.IsNull())
                    return;

                var ssn = BrepSceneNode.Cast(item.GetNode());
                if (ssn != null)
                {
                    this.listBox1.Items.Add(ssn.GetType().Name);
                }
                else
                {
                    this.listBox1.Items.Add(item.GetNode().GetType().Name);
                }
                this.listBox1.Items.Add(String.Format("NodeId: {0}", item.GetNodeId()));
                this.listBox1.Items.Add(item.GetPoint().GetPosition().ToString());
                this.listBox1.Items.Add(item.GetShapeType().ToString());
                this.listBox1.Items.Add(String.Format("SubIndex: {0}", item.GetShapeIndex()));
            });

            mRenderView.SetAnimationCallback((float timer)=>{
                TestCase.RunAnimation(mRenderView, timer);
            });



        }
        private void sTEPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "STEP (*.stp;*.step)|*.stp;*.step|IGES (*.igs;*.iges)|*.igs;*.iges";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            CADReader doc = new CADReader();
            doc.Open(dialog.FileName, (XdeNode xn, TopoShape shape, GTrsf trf, Vector3 color) =>
            {
                
                mRenderView.ShowShape(shape.Transformed(trf), color);
            });

            mRenderView.ZoomAll();
        }

        private void iGESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "IGES (*.igs;*.iges)|*.igs;*.iges|STEP (*.stp;*.step)|*.stp;*.step";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            CADReader doc = new CADReader();
            doc.Open(dialog.FileName, (XdeNode xn, TopoShape shape, GTrsf trf, Vector3 color) =>
            {
                mRenderView.ShowShape(shape.Transformed(trf), color);
            });

            mRenderView.ZoomAll();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.ClearAll();
        }

        private void captureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Bitmap (*.bmp)|*.bmp";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            mRenderView.CaptureScreenShot(dialog.FileName);
        }

        private void stepToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "STEP (*.stp;*.step)|*.stp;*.step";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var shape = StepIO.Open(dialog.FileName);
            if (shape == null)
                return;

            mRenderView.ShowShape(shape, new Vector3(0.8f));

            mRenderView.ZoomAll();
            StlIO.Save(shape, dialog.FileName + ".stl");
        }
        private void igesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "IGES (*.igs;*.iges)|*.igs;*.iges";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var shape = IgesIO.Open(dialog.FileName);
            if (shape == null)
                return;

            mRenderView.ShowShape(shape, new Vector3(0.8f));

            mRenderView.ZoomAll();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = this.treeView1.SelectedNode;
            if (node == null)
                return;
            TestCase.CreateTest(node.Tag, mRenderView);
        }

        private void zoomAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.ZoomAll(1.2f);
        }

        private void projectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SwitchProjectionType();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = SceneIO.FormatFilters();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

             var node = SceneIO.Load(dlg.FileName);
            if (node == null)
                return;
            mRenderView.ShowSceneNode(node);
            mRenderView.ZoomAll();
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.DefaultView);
        }

        private void frontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.Front);
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.Back);
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.Top);
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.Bottom);
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.Right);
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.SetStandardView(EnumStandardView.Left);
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            mRenderView.SetBackgroundColor(dlg.Color.R / 255.0f, dlg.Color.G / 255.0f, dlg.Color.B / 255.0f, 1);
        }
        private void backgroundImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            var texture = ImageTexture2D.Create(dlg.FileName);
            var background = new ImageBackground(texture);

            mRenderView.GetViewer().SetBackground(background);

        }

        private void backgroundSkyBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var background = SkyboxBackground.Create("cloudy");
            mRenderView.GetViewer().SetBackground(background);
        }
        private void mouseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ctx = mRenderView.GetContext();
            // change orbit  to Left and middle buttons
            ctx.SetOrbitButton(EnumMouseButton.LeftMiddle);
            // change pan operation to right button
            ctx.SetPanButton(EnumMouseButton.Right);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewForm3D dlg = new NewForm3D();
            dlg.ShowDialog();
            dlg = null;
        }

        bool bShowTooltip = false;
        private void toolTipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(bShowTooltip)
            {
                mRenderView.SetHilightingCallback(null);
            }
            else
            {
                mRenderView.SetHilightingCallback((PickedItem item) =>
                {
                    var text = item.GetPoint().GetPosition().ToString();
                    this.mRenderView.SetToolTip(text);
                    return true;
                });
            }

            bShowTooltip = !bShowTooltip;
        }

        private void filterEdgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ctx = mRenderView.GetContext();
            ctx.ClearDisplayFilter(EnumShapeFilter.All);
            ctx.AddDisplayFilter(EnumShapeFilter.Edge);
        }

        private void filterFaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ctx = mRenderView.GetContext();
            ctx.ClearDisplayFilter(EnumShapeFilter.All);
            ctx.AddDisplayFilter(EnumShapeFilter.Face);
        }

        private void filterResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ctx = mRenderView.GetContext();
            ctx.ResetDisplayFilters();
        }

        bool bShowCoordinateGrid = false;
        private void coordinateGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bShowCoordinateGrid = !bShowCoordinateGrid;
            mRenderView.ShowCoordinateGrid(bShowCoordinateGrid);
        }

        bool bEnableDepathTest = false;
        private void depthTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bEnableDepathTest = !bEnableDepathTest;
            mRenderView.GetContext().GetSelection().SetDepthTest(bEnableDepathTest);
            mRenderView.RequestDraw(EnumUpdateFlags.Camera);
        }

        private void clipBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mRenderView.ExecuteCommand("ClipBox");
        }
    }
}
