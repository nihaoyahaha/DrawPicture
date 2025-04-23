namespace DrawPicture
{
	partial class Form2
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
			this.transparentRichTextBox1 = new DrawPicture.UserControl.TransparentRichTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// transparentRichTextBox1
			// 
			this.transparentRichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.transparentRichTextBox1.EmptyTextTip = null;
			this.transparentRichTextBox1.Location = new System.Drawing.Point(165, 87);
			this.transparentRichTextBox1.Name = "transparentRichTextBox1";
			this.transparentRichTextBox1.Size = new System.Drawing.Size(123, 137);
			this.transparentRichTextBox1.TabIndex = 0;
			this.transparentRichTextBox1.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(536, 196);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::DrawPicture.Properties.Resources._22;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.transparentRichTextBox1);
			this.Name = "Form2";
			this.Text = "Form2";
			this.ResumeLayout(false);

		}

		#endregion

		private UserControl.TransparentRichTextBox transparentRichTextBox1;
		private System.Windows.Forms.Button button1;
	}
}