﻿namespace WebContrall_Selenium
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
            dateLable = new Label();
            button1 = new Button();
            dateTimePicker1 = new DateTimePicker();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new Font("맑은 고딕", 13F, FontStyle.Regular, GraphicsUnit.Point, 129);
            label1.Location = new Point(12, 9);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(784, 80);
            label1.TabIndex = 0;
            label1.Text = "날자 : 선택 날자로 고정\r\n시간 : 선택 시간 이후 시간대중 가장 가까운시간으로 예약됨";
            label1.Click += label1_Click;
            // 
            // dateLable
            // 
            dateLable.AutoSize = true;
            dateLable.Location = new Point(330, 110);
            dateLable.Name = "dateLable";
            dateLable.Size = new Size(219, 32);
            dateLable.TabIndex = 3;
            dateLable.Text = "날자 및 시간 선택 :";
            dateLable.Click += dateLable_Click;
            // 
            // button1
            // 
            button1.Location = new Point(12, 110);
            button1.Name = "button1";
            button1.Size = new Size(286, 125);
            button1.TabIndex = 1;
            button1.Text = "실행";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.Location = new Point(566, 110);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Size = new Size(230, 39);
            dateTimePicker1.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(825, 446);
            Controls.Add(dateLable);
            Controls.Add(dateTimePicker1);
            Controls.Add(button1);
            Controls.Add(label1);
            Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label dateLable;
        private Button button1;
        private DateTimePicker dateTimePicker1;
    }
}
