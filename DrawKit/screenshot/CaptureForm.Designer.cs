namespace DrawKit.Screenshot
{
	partial class CaptureForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panel_operation = new System.Windows.Forms.Panel();
			this.pic_Save = new System.Windows.Forms.PictureBox();
			this.pic_cancel = new System.Windows.Forms.PictureBox();
			this.pic_OK = new System.Windows.Forms.PictureBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panel_operation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pic_Save)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic_cancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic_OK)).BeginInit();
			this.SuspendLayout();
			// 
			// panel_operation
			// 
			this.panel_operation.Controls.Add(this.pic_Save);
			this.panel_operation.Controls.Add(this.pic_cancel);
			this.panel_operation.Controls.Add(this.pic_OK);
			this.panel_operation.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.panel_operation.Location = new System.Drawing.Point(388, 187);
			this.panel_operation.Name = "panel_operation";
			this.panel_operation.Size = new System.Drawing.Size(113, 43);
			this.panel_operation.TabIndex = 0;
			this.panel_operation.Visible = false;
			// 
			// pic_Save
			// 
			this.pic_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pic_Save.BackgroundImage = global::DrawKit.Properties.Resources.savescreen_32;
			this.pic_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pic_Save.Location = new System.Drawing.Point(8, 6);
			this.pic_Save.Name = "pic_Save";
			this.pic_Save.Size = new System.Drawing.Size(30, 30);
			this.pic_Save.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pic_Save.TabIndex = 0;
			this.pic_Save.TabStop = false;
			this.toolTip1.SetToolTip(this.pic_Save, "保存");
			this.pic_Save.Click += new System.EventHandler(this.pic_Save_Click);
			// 
			// pic_cancel
			// 
			this.pic_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pic_cancel.BackgroundImage = global::DrawKit.Properties.Resources.ScreenshotCancellation_16;
			this.pic_cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pic_cancel.Location = new System.Drawing.Point(44, 6);
			this.pic_cancel.Name = "pic_cancel";
			this.pic_cancel.Size = new System.Drawing.Size(30, 30);
			this.pic_cancel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pic_cancel.TabIndex = 0;
			this.pic_cancel.TabStop = false;
			this.toolTip1.SetToolTip(this.pic_cancel, "退出");
			this.pic_cancel.Click += new System.EventHandler(this.pic_cancel_Click);
			// 
			// pic_OK
			// 
			this.pic_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pic_OK.BackgroundImage = global::DrawKit.Properties.Resources.ScreenshotCompleted_16;
			this.pic_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pic_OK.Location = new System.Drawing.Point(80, 6);
			this.pic_OK.Name = "pic_OK";
			this.pic_OK.Size = new System.Drawing.Size(30, 30);
			this.pic_OK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pic_OK.TabIndex = 0;
			this.pic_OK.TabStop = false;
			this.toolTip1.SetToolTip(this.pic_OK, "完成");
			this.pic_OK.Click += new System.EventHandler(this.pic_OK_Click);
			// 
			// CaptureForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.panel_operation);
			this.Name = "CaptureForm";
			this.Text = "CaptureForm";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CaptureForm_KeyDown);
			this.panel_operation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pic_Save)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic_cancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic_OK)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel_operation;
		private System.Windows.Forms.PictureBox pic_OK;
		private System.Windows.Forms.PictureBox pic_cancel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pic_Save;
	}
}