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
            this.VideoPanel1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.VideoPanel1)).BeginInit();
            this.SuspendLayout();
            // 
            // VideoPanel1
            // 
            this.VideoPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.VideoPanel1.Image = ((System.Drawing.Image)(resources.GetObject("VideoPanel1.Image")));
            this.VideoPanel1.Location = new System.Drawing.Point(0, 0);
            this.VideoPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.VideoPanel1.Name = "VideoPanel1";
            this.VideoPanel1.Size = new System.Drawing.Size(100, 50);
            this.VideoPanel1.TabIndex = 0;
            this.VideoPanel1.TabStop = false;
            this.VideoPanel1.Click += new System.EventHandler(this.VideoPanel_Click);
            // 
            // VideoContainer1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.VideoPanel1);
            this.Name = "VideoContainer1";
            this.Size = new System.Drawing.Size(200, 100);
            this.Load += new System.EventHandler(this.VideoContainer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.VideoPanel1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox VideoPanel1;
    }
}
