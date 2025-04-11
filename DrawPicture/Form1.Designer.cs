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
			this.btn_select = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btn_rotate = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btn_Erase = new System.Windows.Forms.ToolStripButton();
			this.cmb_size = new System.Windows.Forms.ToolStripComboBox();
			this.btn_Text = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.btn_Line = new System.Windows.Forms.ToolStripButton();
			this.btn_circle = new System.Windows.Forms.ToolStripButton();
			this.btn_rectangle = new System.Windows.Forms.ToolStripButton();
			this.btn_roundedRectangle = new System.Windows.Forms.ToolStripButton();
			this.btn_triangle = new System.Windows.Forms.ToolStripButton();
			this.btn_RightTriangle = new System.Windows.Forms.ToolStripButton();
			this.btn_rhombus = new System.Windows.Forms.ToolStripButton();
			this.btn_pentagon = new System.Windows.Forms.ToolStripButton();
			this.btn_hexagon = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.btn_showColor = new System.Windows.Forms.ToolStripDropDownButton();
			this.btn_BlackColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_greyColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_darkRedColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_RedColor = new System.Windows.Forms.ToolStripMenuItem();
			this.btn_OrangeColor = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.btn_save = new System.Windows.Forms.ToolStripButton();
			this.btn_open = new System.Windows.Forms.ToolStripButton();
			this.panel_main = new System.Windows.Forms.Panel();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.lb_Penposition = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
			this.toolStrip1.SuspendLayout();
			this.toolStrip2.SuspendLayout();
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
            this.btn_open});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(834, 56);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
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
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 56);
			// 
			// btn_rotate
			// 
			this.btn_rotate.Image = global::DrawPicture.Properties.Resources.右旋转;
			this.btn_rotate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_rotate.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_rotate.Name = "btn_rotate";
			this.btn_rotate.Size = new System.Drawing.Size(36, 53);
			this.btn_rotate.Text = "回転";
			this.btn_rotate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 56);
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
			// btn_Text
			// 
			this.btn_Text.Image = global::DrawPicture.Properties.Resources.文字颜色;
			this.btn_Text.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btn_Text.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btn_Text.Name = "btn_Text";
			this.btn_Text.Size = new System.Drawing.Size(36, 53);
			this.btn_Text.Text = "文字";
			this.btn_Text.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 56);
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
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 56);
			// 
			// btn_showColor
			// 
			this.btn_showColor.BackColor = System.Drawing.Color.Black;
			this.btn_showColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btn_showColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
			// btn_BlackColor
			// 
			this.btn_BlackColor.Name = "btn_BlackColor";
			this.btn_BlackColor.Size = new System.Drawing.Size(180, 22);
			this.btn_BlackColor.Text = "黑色";
			this.btn_BlackColor.Click += new System.EventHandler(this.btn_BlackColor_Click);
			// 
			// btn_greyColor
			// 
			this.btn_greyColor.Name = "btn_greyColor";
			this.btn_greyColor.Size = new System.Drawing.Size(180, 22);
			this.btn_greyColor.Text = "灰色-50%";
			this.btn_greyColor.Click += new System.EventHandler(this.btn_greyColor_Click);
			// 
			// btn_darkRedColor
			// 
			this.btn_darkRedColor.Name = "btn_darkRedColor";
			this.btn_darkRedColor.Size = new System.Drawing.Size(180, 22);
			this.btn_darkRedColor.Text = "深红色";
			this.btn_darkRedColor.Click += new System.EventHandler(this.btn_darkRedColor_Click);
			// 
			// btn_RedColor
			// 
			this.btn_RedColor.Name = "btn_RedColor";
			this.btn_RedColor.Size = new System.Drawing.Size(180, 22);
			this.btn_RedColor.Text = "红色";
			this.btn_RedColor.Click += new System.EventHandler(this.btn_RedColor_Click);
			// 
			// btn_OrangeColor
			// 
			this.btn_OrangeColor.Name = "btn_OrangeColor";
			this.btn_OrangeColor.Size = new System.Drawing.Size(180, 22);
			this.btn_OrangeColor.Text = "橙色";
			this.btn_OrangeColor.Click += new System.EventHandler(this.btn_OrangeColor_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 56);
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
			// panel_main
			// 
			this.panel_main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel_main.BackColor = System.Drawing.Color.White;
			this.panel_main.Location = new System.Drawing.Point(12, 59);
			this.panel_main.Name = "panel_main";
			this.panel_main.Size = new System.Drawing.Size(810, 444);
			this.panel_main.TabIndex = 1;
			this.panel_main.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_main_Paint);
			this.panel_main.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_main_MouseDown);
			this.panel_main.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_main_MouseMove);
			this.panel_main.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_main_MouseUp);
			// 
			// toolStrip2
			// 
			this.toolStrip2.AutoSize = false;
			this.toolStrip2.CanOverflow = false;
			this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lb_Penposition,
            this.toolStripSeparator6,
            this.toolStripComboBox1});
			this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.toolStrip2.Location = new System.Drawing.Point(0, 517);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(834, 25);
			this.toolStrip2.Stretch = true;
			this.toolStrip2.TabIndex = 2;
			this.toolStrip2.Text = "toolStrip2";
			// 
			// lb_Penposition
			// 
			this.lb_Penposition.Image = global::DrawPicture.Properties.Resources.坐标轴;
			this.lb_Penposition.Name = "lb_Penposition";
			this.lb_Penposition.Size = new System.Drawing.Size(112, 22);
			this.lb_Penposition.Text = "toolStripLabel1";
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripComboBox1
			// 
			this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
			this.toolStripComboBox1.Items.AddRange(new object[] {
            "12.5%",
            "25%",
            "50%",
            "100%",
            "200%",
            "300%",
            "400%",
            "500%",
            "600%",
            "700%",
            "800%"});
			this.toolStripComboBox1.Name = "toolStripComboBox1";
			this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(834, 542);
			this.Controls.Add(this.toolStrip2);
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
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripDropDownButton btn_select;
		private System.Windows.Forms.ToolStripButton btn_rotate;
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
		private System.Windows.Forms.ToolStripLabel lb_Penposition;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripComboBox cmb_size;
	}
}

