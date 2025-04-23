namespace DrawPicture
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.cmb_size = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.panel_main = new System.Windows.Forms.Panel();
			this.lb_Penposition = new System.Windows.Forms.Label();
			this.lb_CanvasSize = new System.Windows.Forms.Label();
			this.lb_SelectionSize = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btn_select = new System.Windows.Forms.ToolStripDropDownButton();
			this.btn_rotate = new System.Windows.Forms.ToolStripDropDownButton();
			this.btn_RightRotate90 = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_LeftRotate90 = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_Rotate180 = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_FlipVertical = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_FlipHorizontal = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_Erase = new System.Windows.Forms.ToolStripButton();
			this.btn_Fill = new System.Windows.Forms.ToolStripButton();
			this.btn_Text = new System.Windows.Forms.ToolStripButton();
			this.btn_Line = new System.Windows.Forms.ToolStripButton();
			this.btn_circle = new System.Windows.Forms.ToolStripButton();
			this.btn_rectangle = new System.Windows.Forms.ToolStripButton();
			this.btn_roundedRectangle = new System.Windows.Forms.ToolStripButton();
			this.btn_triangle = new System.Windows.Forms.ToolStripButton();
			this.btn_RightTriangle = new System.Windows.Forms.ToolStripButton();
			this.btn_rhombus = new System.Windows.Forms.ToolStripButton();
			this.btn_pentagon = new System.Windows.Forms.ToolStripButton();
			this.btn_hexagon = new System.Windows.Forms.ToolStripButton();
			this.btn_showColor = new System.Windows.Forms.ToolStripDropDownButton();
			this.btn_WhileColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_BlackColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_greyColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_darkRedColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_RedColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_OrangeColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_save = new System.Windows.Forms.ToolStripButton();
			this.btn_open = new System.Windows.Forms.ToolStripButton();
			this.btn_ClearAll = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.Color.White;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_select,
            this.toolStripSeparator1,
            this.btn_rotate,
            this.toolStripSeparator2,
            this.btn_Erase,
            this.btn_Fill,
            this.cmb_size,
            this.btn_Text,
            this.toolStripSeparator4,
            this.btn_Line,
            this.btn_circle,
            this.btn_rectangle,
            this.btn_roundedRectangle,
            this.btn_triangle,
            this.btn_RightTriangle,
            this.btn_rhombus,
            this.btn_pentagon,
            this.btn_hexagon,
            this.toolStripSeparator3,
            this.btn_showColor,
            this.toolStripSeparator5,
            this.btn_save,
            this.btn_open,
            this.btn_ClearAll});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(940, 56);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 56);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 56);
			// 
			// cmb_size
			// 
			this.cmb_size.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmb_size.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cmb_size.Items.AddRange(new object[] {
            "1px",
            "3px",
            "5px",
            "8px"});
			this.cmb_size.Name = "cmb_size";
			this.cmb_size.Size = new System.Drawing.Size(75, 56);
			this.cmb_size.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmb_size.ToolTipText = "太さ";
			this.cmb_size.SelectedIndexChanged += new System.EventHandler(this.cmb_size_SelectedIndexChanged);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 56);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 56);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 56);
			// 
			// panel_main
			// 
			this.panel_main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel_main.BackColor = System.Drawing.Color.White;
			this.panel_main.Location = new System.Drawing.Point(0, 59);
			this.panel_main.Name = "panel_main";
			this.panel_main.Size = new System.Drawing.Size(940, 447);
			this.panel_main.TabIndex = 1;
			this.panel_main.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_main_Paint);
			this.panel_main.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_main_MouseDown);
			this.panel_main.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_main_MouseMove);
			this.panel_main.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_main_MouseUp);
			// 
			// lb_Penposition
			// 
			this.lb_Penposition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lb_Penposition.AutoSize = true;
			this.lb_Penposition.Location = new System.Drawing.Point(44, 521);
			this.lb_Penposition.Name = "lb_Penposition";
			this.lb_Penposition.Size = new System.Drawing.Size(41, 12);
			this.lb_Penposition.TabIndex = 2;
			this.lb_Penposition.Text = "label1";
			// 
			// lb_CanvasSize
			// 
			this.lb_CanvasSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lb_CanvasSize.AutoSize = true;
			this.lb_CanvasSize.Location = new System.Drawing.Point(416, 521);
			this.lb_CanvasSize.Name = "lb_CanvasSize";
			this.lb_CanvasSize.Size = new System.Drawing.Size(41, 12);
			this.lb_CanvasSize.TabIndex = 2;
			this.lb_CanvasSize.Text = "label1";
			// 
			// lb_SelectionSize
			// 
			this.lb_SelectionSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lb_SelectionSize.AutoSize = true;
			this.lb_SelectionSize.Location = new System.Drawing.Point(218, 521);
			this.lb_SelectionSize.Name = "lb_SelectionSize";
			this.lb_SelectionSize.Size = new System.Drawing.Size(41, 12);
			this.lb_SelectionSize.TabIndex = 2;
			this.lb_SelectionSize.Text = "label1";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox2.BackgroundImage = global::DrawPicture.Properties.Resources.尺寸1;
			this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pictureBox2.Location = new System.Drawing.Point(380, 512);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(30, 26);
			this.pictureBox2.TabIndex = 0;
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox3
			// 
			this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox3.BackgroundImage = global::DrawPicture.Properties.Resources.尺寸__1_;
			this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pictureBox3.Location = new System.Drawing.Point(186, 512);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(26, 26);
			this.pictureBox3.TabIndex = 0;
			this.pictureBox3.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox1.BackgroundImage = global::DrawPicture.Properties.Resources.坐标轴;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pictureBox1.Location = new System.Drawing.Point(12, 512);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(26, 26);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// btn_select
			// 
			this.btn_select.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
			this.btn_select.Image = global::DrawPicture.Properties.Resources.拓展;
			this.btn_select.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_select.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_select.Name = "btn_select";
			this.btn_select.RightToLeftAutoMirrorImage = true;
			this.btn_select.ShowDropDownArrow = false;
			this.btn_select.Size = new System.Drawing.Size(60, 53);
			this.btn_select.Text = "せんたく";
			this.btn_select.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_select.ToolTipText = "せんたく";
			this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
			// 
			// btn_rotate
			// 
			this.btn_rotate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_RightRotate90,
            this.btn_LeftRotate90,
            this.btn_Rotate180,
            this.btn_FlipVertical,
            this.btn_FlipHorizontal});
			this.btn_rotate.Image = global::DrawPicture.Properties.Resources.右旋转1;
			this.btn_rotate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_rotate.Name = "btn_rotate";
			this.btn_rotate.Size = new System.Drawing.Size(45, 53);
			this.btn_rotate.Text = "回転";
			this.btn_rotate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// btn_RightRotate90
			// 
			this.btn_RightRotate90.Name = "btn_RightRotate90";
			this.btn_RightRotate90.Size = new System.Drawing.Size(150, 22);
			this.btn_RightRotate90.Text = "向右旋转90度";
			this.btn_RightRotate90.Click += new System.EventHandler(this.btn_RightRotate90_Click);
			// 
			// btn_LeftRotate90
			// 
			this.btn_LeftRotate90.Name = "btn_LeftRotate90";
			this.btn_LeftRotate90.Size = new System.Drawing.Size(150, 22);
			this.btn_LeftRotate90.Text = "向左旋转90度";
			this.btn_LeftRotate90.Click += new System.EventHandler(this.btn_LeftRotate90_Click);
			// 
			// btn_Rotate180
			// 
			this.btn_Rotate180.Name = "btn_Rotate180";
			this.btn_Rotate180.Size = new System.Drawing.Size(150, 22);
			this.btn_Rotate180.Text = "旋转180度";
			this.btn_Rotate180.Click += new System.EventHandler(this.btn_Rotate180_Click);
			// 
			// btn_FlipVertical
			// 
			this.btn_FlipVertical.Name = "btn_FlipVertical";
			this.btn_FlipVertical.Size = new System.Drawing.Size(150, 22);
			this.btn_FlipVertical.Text = "垂直翻转";
			this.btn_FlipVertical.Click += new System.EventHandler(this.btn_FlipVertical_Click);
			// 
			// btn_FlipHorizontal
			// 
			this.btn_FlipHorizontal.Name = "btn_FlipHorizontal";
			this.btn_FlipHorizontal.Size = new System.Drawing.Size(150, 22);
			this.btn_FlipHorizontal.Text = "水平翻转";
			this.btn_FlipHorizontal.Click += new System.EventHandler(this.btn_FlipHorizontal_Click);
			// 
			// btn_Erase
			// 
			this.btn_Erase.Image = global::DrawPicture.Properties.Resources.擦除;
			this.btn_Erase.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_Erase.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_Erase.Name = "btn_Erase";
			this.btn_Erase.Size = new System.Drawing.Size(36, 53);
			this.btn_Erase.Text = "消去";
			this.btn_Erase.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_Erase.Click += new System.EventHandler(this.btn_Erase_Click);
			// 
			// btn_Fill
			// 
			this.btn_Fill.Image = global::DrawPicture.Properties.Resources.填充颜色;
			this.btn_Fill.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_Fill.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_Fill.Name = "btn_Fill";
			this.btn_Fill.Size = new System.Drawing.Size(60, 53);
			this.btn_Fill.Text = "ドラム缶";
			this.btn_Fill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_Fill.Click += new System.EventHandler(this.btn_Fill_Click);
			// 
			// btn_Text
			// 
			this.btn_Text.Image = global::DrawPicture.Properties.Resources.文字颜色;
			this.btn_Text.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_Text.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_Text.Name = "btn_Text";
			this.btn_Text.Size = new System.Drawing.Size(36, 53);
			this.btn_Text.Text = "文字";
			this.btn_Text.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_Text.Visible = false;
			// 
			// btn_Line
			// 
			this.btn_Line.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_Line.Image = global::DrawPicture.Properties.Resources.直线;
			this.btn_Line.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_Line.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_Line.Name = "btn_Line";
			this.btn_Line.Size = new System.Drawing.Size(36, 53);
			this.btn_Line.Text = "toolStripButton4";
			this.btn_Line.ToolTipText = "直線";
			this.btn_Line.Click += new System.EventHandler(this.btn_Line_Click);
			// 
			// btn_circle
			// 
			this.btn_circle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_circle.Image = global::DrawPicture.Properties.Resources.圆圈;
			this.btn_circle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_circle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_circle.Name = "btn_circle";
			this.btn_circle.Size = new System.Drawing.Size(36, 53);
			this.btn_circle.Text = "円";
			this.btn_circle.Click += new System.EventHandler(this.btn_circle_Click);
			// 
			// btn_rectangle
			// 
			this.btn_rectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_rectangle.Image = global::DrawPicture.Properties.Resources.形状_矩形;
			this.btn_rectangle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_rectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_rectangle.Name = "btn_rectangle";
			this.btn_rectangle.Size = new System.Drawing.Size(36, 53);
			this.btn_rectangle.Text = "長方形";
			this.btn_rectangle.Click += new System.EventHandler(this.btn_rectangle_Click);
			// 
			// btn_roundedRectangle
			// 
			this.btn_roundedRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_roundedRectangle.Image = global::DrawPicture.Properties.Resources.圆角矩形;
			this.btn_roundedRectangle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_roundedRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_roundedRectangle.Name = "btn_roundedRectangle";
			this.btn_roundedRectangle.Size = new System.Drawing.Size(42, 53);
			this.btn_roundedRectangle.Text = "フィレット長方形";
			this.btn_roundedRectangle.Click += new System.EventHandler(this.btn_roundedRectangle_Click);
			// 
			// btn_triangle
			// 
			this.btn_triangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_triangle.Image = global::DrawPicture.Properties.Resources.形状_三角形;
			this.btn_triangle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_triangle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_triangle.Name = "btn_triangle";
			this.btn_triangle.Size = new System.Drawing.Size(36, 53);
			this.btn_triangle.Text = "三角形";
			this.btn_triangle.Click += new System.EventHandler(this.btn_triangle_Click);
			// 
			// btn_RightTriangle
			// 
			this.btn_RightTriangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_RightTriangle.Image = global::DrawPicture.Properties.Resources.直角三角形;
			this.btn_RightTriangle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_RightTriangle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_RightTriangle.Name = "btn_RightTriangle";
			this.btn_RightTriangle.Size = new System.Drawing.Size(36, 53);
			this.btn_RightTriangle.Text = "直角三角形";
			this.btn_RightTriangle.Click += new System.EventHandler(this.btn_RightTriangle_Click);
			// 
			// btn_rhombus
			// 
			this.btn_rhombus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_rhombus.Image = global::DrawPicture.Properties.Resources.菱形框;
			this.btn_rhombus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_rhombus.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_rhombus.Name = "btn_rhombus";
			this.btn_rhombus.Size = new System.Drawing.Size(36, 53);
			this.btn_rhombus.Text = "ひし形";
			this.btn_rhombus.Click += new System.EventHandler(this.btn_rhombus_Click);
			// 
			// btn_pentagon
			// 
			this.btn_pentagon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_pentagon.Image = global::DrawPicture.Properties.Resources.五边形;
			this.btn_pentagon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_pentagon.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_pentagon.Name = "btn_pentagon";
			this.btn_pentagon.Size = new System.Drawing.Size(36, 53);
			this.btn_pentagon.Text = "五角形";
			this.btn_pentagon.Click += new System.EventHandler(this.btn_pentagon_Click);
			// 
			// btn_hexagon
			// 
			this.btn_hexagon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btn_hexagon.Image = global::DrawPicture.Properties.Resources.六边形;
			this.btn_hexagon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_hexagon.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_hexagon.Name = "btn_hexagon";
			this.btn_hexagon.Size = new System.Drawing.Size(36, 53);
			this.btn_hexagon.Text = "六角形";
			this.btn_hexagon.Click += new System.EventHandler(this.btn_hexagon_Click);
			// 
			// btn_showColor
			// 
			this.btn_showColor.BackColor = System.Drawing.Color.Black;
			this.btn_showColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btn_showColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_WhileColor,
            this.btn_BlackColor,
            this.btn_greyColor,
            this.btn_darkRedColor,
            this.btn_RedColor,
            this.btn_OrangeColor});
			this.btn_showColor.Image = global::DrawPicture.Properties.Resources.透明;
			this.btn_showColor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_showColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_showColor.Name = "btn_showColor";
			this.btn_showColor.Size = new System.Drawing.Size(57, 53);
			this.btn_showColor.Text = "カラー";
			this.btn_showColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btn_showColor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// btn_WhileColor
			// 
			this.btn_WhileColor.Name = "btn_WhileColor";
			this.btn_WhileColor.Size = new System.Drawing.Size(130, 22);
			this.btn_WhileColor.Text = "白色";
			this.btn_WhileColor.Click += new System.EventHandler(this.btn_WhileColor_Click);
			// 
			// btn_BlackColor
			// 
			this.btn_BlackColor.Name = "btn_BlackColor";
			this.btn_BlackColor.Size = new System.Drawing.Size(130, 22);
			this.btn_BlackColor.Text = "黑色";
			this.btn_BlackColor.Click += new System.EventHandler(this.btn_BlackColor_Click);
			// 
			// btn_greyColor
			// 
			this.btn_greyColor.Name = "btn_greyColor";
			this.btn_greyColor.Size = new System.Drawing.Size(130, 22);
			this.btn_greyColor.Text = "灰色-50%";
			this.btn_greyColor.Click += new System.EventHandler(this.btn_greyColor_Click);
			// 
			// btn_darkRedColor
			// 
			this.btn_darkRedColor.Name = "btn_darkRedColor";
			this.btn_darkRedColor.Size = new System.Drawing.Size(130, 22);
			this.btn_darkRedColor.Text = "深红色";
			this.btn_darkRedColor.Click += new System.EventHandler(this.btn_darkRedColor_Click);
			// 
			// btn_RedColor
			// 
			this.btn_RedColor.Name = "btn_RedColor";
			this.btn_RedColor.Size = new System.Drawing.Size(130, 22);
			this.btn_RedColor.Text = "红色";
			this.btn_RedColor.Click += new System.EventHandler(this.btn_RedColor_Click);
			// 
			// btn_OrangeColor
			// 
			this.btn_OrangeColor.Name = "btn_OrangeColor";
			this.btn_OrangeColor.Size = new System.Drawing.Size(130, 22);
			this.btn_OrangeColor.Text = "橙色";
			this.btn_OrangeColor.Click += new System.EventHandler(this.btn_OrangeColor_Click);
			// 
			// btn_save
			// 
			this.btn_save.Image = global::DrawPicture.Properties.Resources.保存__1_;
			this.btn_save.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_save.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_save.Name = "btn_save";
			this.btn_save.Size = new System.Drawing.Size(36, 53);
			this.btn_save.Text = "保存";
			this.btn_save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
			// 
			// btn_open
			// 
			this.btn_open.Image = global::DrawPicture.Properties.Resources.打开;
			this.btn_open.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_open.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_open.Name = "btn_open";
			this.btn_open.Size = new System.Drawing.Size(36, 53);
			this.btn_open.Text = "開く";
			this.btn_open.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
			// 
			// btn_ClearAll
			// 
			this.btn_ClearAll.Image = global::DrawPicture.Properties.Resources.Clearup;
			this.btn_ClearAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_ClearAll.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_ClearAll.Name = "btn_ClearAll";
			this.btn_ClearAll.Size = new System.Drawing.Size(48, 53);
			this.btn_ClearAll.Text = "クリア";
			this.btn_ClearAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_ClearAll.Click += new System.EventHandler(this.btn_ClearAll_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(940, 542);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.lb_SelectionSize);
			this.Controls.Add(this.lb_CanvasSize);
			this.Controls.Add(this.lb_Penposition);
			this.Controls.Add(this.panel_main);
			this.Controls.Add(this.toolStrip1);
			this.MinimumSize = new System.Drawing.Size(800, 500);
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripDropDownButton btn_select;
		private System.Windows.Forms.ToolStripButton btn_Erase;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton btn_Line;
		private System.Windows.Forms.ToolStripButton btn_circle;
		private System.Windows.Forms.ToolStripButton btn_rectangle;
		private System.Windows.Forms.ToolStripButton btn_roundedRectangle;
		private System.Windows.Forms.ToolStripButton btn_triangle;
		private System.Windows.Forms.ToolStripButton btn_RightTriangle;
		private System.Windows.Forms.ToolStripButton btn_rhombus;
		private System.Windows.Forms.ToolStripButton btn_pentagon;
		private System.Windows.Forms.ToolStripButton btn_hexagon;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripDropDownButton btn_showColor;
		private System.Windows.Forms.ToolStripMenuItem btn_BlackColor;
		private System.Windows.Forms.ToolStripMenuItem btn_greyColor;
		private System.Windows.Forms.ToolStripMenuItem btn_darkRedColor;
		private System.Windows.Forms.ToolStripMenuItem btn_RedColor;
		private System.Windows.Forms.ToolStripMenuItem btn_OrangeColor;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton btn_save;
		private System.Windows.Forms.ToolStripButton btn_open;
		private System.Windows.Forms.Panel panel_main;
		private System.Windows.Forms.ToolStripButton btn_Text;
		private System.Windows.Forms.ToolStripComboBox cmb_size;
		private System.Windows.Forms.ToolStripMenuItem btn_WhileColor;
		private System.Windows.Forms.ToolStripButton btn_Fill;
		private System.Windows.Forms.ToolStripDropDownButton btn_rotate;
		private System.Windows.Forms.ToolStripMenuItem btn_RightRotate90;
		private System.Windows.Forms.ToolStripMenuItem btn_LeftRotate90;
		private System.Windows.Forms.ToolStripMenuItem btn_Rotate180;
		private System.Windows.Forms.ToolStripMenuItem btn_FlipVertical;
		private System.Windows.Forms.ToolStripMenuItem btn_FlipHorizontal;
		private System.Windows.Forms.ToolStripButton btn_ClearAll;
		private System.Windows.Forms.Label lb_Penposition;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label lb_CanvasSize;
		private System.Windows.Forms.Label lb_SelectionSize;
		private System.Windows.Forms.PictureBox pictureBox3;
	}
}

