namespace WindowsFormsVideoControl
{
    partial class VideoContainer1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoContainer1));
            this.SelectablePictureBox1 = new WindowsFormsVideoControl.SelectablePictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.SelectablePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SelectablePictureBox1
            // 
            this.SelectablePictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.SelectablePictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("SelectablePictureBox1.Image")));
            this.SelectablePictureBox1.Location = new System.Drawing.Point(0, 0);
            this.SelectablePictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.SelectablePictureBox1.Name = "SelectablePictureBox1";
            this.SelectablePictureBox1.Size = new System.Drawing.Size(400, 300);
            this.SelectablePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.SelectablePictureBox1.TabIndex = 0;
            // 
            // VideoContainer1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.SelectablePictureBox1);
            this.Name = "VideoContainer1";
            this.Size = new System.Drawing.Size(888, 549);
            this.Load += new System.EventHandler(this.VideoContainer1_Load);
            this.Resize += new System.EventHandler(this.VideoContainer1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.SelectablePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

       // public System.Windows.Forms.PictureBox VideoPanel;

        public SelectablePictureBox SelectablePictureBox1;
    }
}
