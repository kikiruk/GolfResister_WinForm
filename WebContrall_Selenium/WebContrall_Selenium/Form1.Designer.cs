using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WebContrall_Selenium
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
            statusLabe = new Label();
            TotalBrowservolumeLable = new Label();
            button1 = new System.Windows.Forms.Button();
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
            label1.Text = "날자 : 선택 날자로 고정\r\n시간 : 선택 시간과 같거나 이후 가장 가까운시간으로 예약됨";
            // 
            // dateLable
            // 
            dateLable.AutoSize = true;
            dateLable.Location = new Point(330, 110);
            dateLable.Name = "dateLable";
            dateLable.Size = new Size(219, 32);
            dateLable.TabIndex = 3;
            dateLable.Text = "날자 및 시간 선택 :";
            // 
            // statusLabe
            // 
            statusLabe.AutoSize = true;
            statusLabe.Location = new Point(12, 265);
            statusLabe.Name = "statusLabe";
            statusLabe.Size = new Size(83, 32);
            statusLabe.TabIndex = 4;
            statusLabe.Text = "상태 : ";
            // 
            // TotalBrowservolumeLable
            // 
            TotalBrowservolumeLable.AutoSize = true;
            TotalBrowservolumeLable.Location = new Point(17, 314);
            TotalBrowservolumeLable.Name = "TotalBrowservolumeLable";
            TotalBrowservolumeLable.Size = new Size(78, 32);
            TotalBrowservolumeLable.TabIndex = 5;
            TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : 0";
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
            Controls.Add(label1);
            Controls.Add(statusLabe);
            Controls.Add(dateLable);
            Controls.Add(TotalBrowservolumeLable);
            Controls.Add(button1);
            Controls.Add(dateTimePicker1);
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
        private Label statusLabe;
        private Label TotalBrowservolumeLable;
        private System.Windows.Forms.Button button1;
        private DateTimePicker dateTimePicker1;
    }
}
