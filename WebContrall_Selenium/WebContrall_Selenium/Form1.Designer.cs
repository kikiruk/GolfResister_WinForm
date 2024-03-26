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
            startButton = new System.Windows.Forms.Button();
            stopButton = new System.Windows.Forms.Button();
            dateTimePicker = new DateTimePicker();
            selectBrowserVolume = new NumericUpDown();
            selectBrowserVolumeLable = new Label();
            exitBrowsersButton = new System.Windows.Forms.Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)selectBrowserVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new Font("Malgun Gothic", 10F);
            label1.Location = new Point(12, 9);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(390, 80);
            label1.TabIndex = 0;
            label1.Text = "날자 : 선택 날자로 고정\r\n시간 : 선택 시간과 같거나 이후 가장 가까운시간으로 예약됨";
            label1.Click += label1_Click;
            // 
            // dateLable
            // 
            dateLable.AutoSize = true;
            dateLable.Location = new Point(17, 177);
            dateLable.Name = "dateLable";
            dateLable.Size = new Size(150, 21);
            dateLable.TabIndex = 3;
            dateLable.Text = "날자 및 시간 선택 :";
            // 
            // statusLabe
            // 
            statusLabe.AutoSize = true;
            statusLabe.Font = new Font("Malgun Gothic", 8F);
            statusLabe.Location = new Point(17, 392);
            statusLabe.Name = "statusLabe";
            statusLabe.Size = new Size(39, 13);
            statusLabe.TabIndex = 4;
            statusLabe.Text = "상태 : ";
            // 
            // TotalBrowservolumeLable
            // 
            TotalBrowservolumeLable.AutoSize = true;
            TotalBrowservolumeLable.Font = new Font("Malgun Gothic", 8F);
            TotalBrowservolumeLable.Location = new Point(17, 413);
            TotalBrowservolumeLable.Name = "TotalBrowservolumeLable";
            TotalBrowservolumeLable.Size = new Size(156, 13);
            TotalBrowservolumeLable.TabIndex = 5;
            TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : 0";
            // 
            // startButton
            // 
            startButton.Location = new Point(12, 297);
            startButton.Name = "startButton";
            startButton.Size = new Size(228, 92);
            startButton.TabIndex = 1;
            startButton.Text = "실행";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(300, 297);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(217, 92);
            stopButton.TabIndex = 8;
            stopButton.Text = "일시정지";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += stopButton_Click;
            // 
            // dateTimePicker
            // 
            dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm";
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.Location = new Point(238, 177);
            dateTimePicker.Name = "dateTimePicker";
            dateTimePicker.ShowUpDown = true;
            dateTimePicker.Size = new Size(230, 29);
            dateTimePicker.TabIndex = 2;
            // 
            // selectBrowserVolume
            // 
            selectBrowserVolume.Location = new Point(238, 222);
            selectBrowserVolume.Name = "selectBrowserVolume";
            selectBrowserVolume.Size = new Size(180, 29);
            selectBrowserVolume.TabIndex = 9;
            // 
            // selectBrowserVolumeLable
            // 
            selectBrowserVolumeLable.AutoSize = true;
            selectBrowserVolumeLable.Location = new Point(17, 222);
            selectBrowserVolumeLable.Name = "selectBrowserVolumeLable";
            selectBrowserVolumeLable.Size = new Size(144, 21);
            selectBrowserVolumeLable.TabIndex = 10;
            selectBrowserVolumeLable.Text = "브라우저 창 개수 :";
            // 
            // exitBrowsersButton
            // 
            exitBrowsersButton.Location = new Point(579, 297);
            exitBrowsersButton.Name = "exitBrowsersButton";
            exitBrowsersButton.Size = new Size(217, 92);
            exitBrowsersButton.TabIndex = 11;
            exitBrowsersButton.Text = "브라우저 종료";
            exitBrowsersButton.UseVisualStyleBackColor = true;
            exitBrowsersButton.Click += exitBrowsersButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(409, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(387, 377);
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(825, 446);
            Controls.Add(label1);
            Controls.Add(exitBrowsersButton);
            Controls.Add(selectBrowserVolumeLable);
            Controls.Add(selectBrowserVolume);
            Controls.Add(stopButton);
            Controls.Add(statusLabe);
            Controls.Add(dateLable);
            Controls.Add(TotalBrowservolumeLable);
            Controls.Add(startButton);
            Controls.Add(dateTimePicker);
            Controls.Add(pictureBox1);
            Font = new Font("Malgun Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "석호형 선물 ★";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)selectBrowserVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label dateLable;
        private Label statusLabe;
        private Label TotalBrowservolumeLable;
        private System.Windows.Forms.Button startButton;
        private DateTimePicker dateTimePicker;
        private System.Windows.Forms.Button stopButton;
        private NumericUpDown selectBrowserVolume;
        private Label selectBrowserVolumeLable;
        private System.Windows.Forms.Button exitBrowsersButton;
        private PictureBox pictureBox1;
    }
}
