namespace LKRC_Tool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AllowDrop = true;
            label1.Location = new Point(4, 88);
            label1.Name = "label1";
            label1.Size = new Size(282, 84);
            label1.TabIndex = 0;
            label1.Text = "把酷狗音乐歌词拖动到本程序即可转换对应的格式，本程序自动识别格式\r\n（LRC转KRC，KRC转LRC）\r\n蔴球视频 2025.11.11";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.DragDrop += Form1_DragDrop;
            label1.DragEnter += Form1_DragEnter;
            // 
            // label2
            // 
            label2.AllowDrop = true;
            label2.AutoSize = true;
            label2.Location = new Point(28, 185);
            label2.Name = "label2";
            label2.Size = new Size(226, 21);
            label2.TabIndex = 1;
            label2.Text = "成功 X 个文件，失败 X 个文件";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.DragDrop += Form1_DragDrop;
            label2.DragEnter += Form1_DragEnter;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(291, 276);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Microsoft YaHei UI", 12F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LKRC_Tool by：蔴球视频";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
    }
}
