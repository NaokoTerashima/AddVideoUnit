namespace WinFormsAddVideoUnit
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_CSV = new System.Windows.Forms.TextBox();
            this.btn_CSV = new System.Windows.Forms.Button();
            this.btn_Execute = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox_CSV
            // 
            this.textBox_CSV.Location = new System.Drawing.Point(182, 14);
            this.textBox_CSV.Name = "textBox_CSV";
            this.textBox_CSV.Size = new System.Drawing.Size(606, 19);
            this.textBox_CSV.TabIndex = 0;
            // 
            // btn_CSV
            // 
            this.btn_CSV.Location = new System.Drawing.Point(12, 12);
            this.btn_CSV.Name = "btn_CSV";
            this.btn_CSV.Size = new System.Drawing.Size(164, 23);
            this.btn_CSV.TabIndex = 1;
            this.btn_CSV.Text = "[1] CSVファイル選択";
            this.btn_CSV.UseVisualStyleBackColor = true;
            this.btn_CSV.Click += new System.EventHandler(this.btn_CSV_Click);
            // 
            // btn_Execute
            // 
            this.btn_Execute.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Execute.Location = new System.Drawing.Point(12, 41);
            this.btn_Execute.Name = "btn_Execute";
            this.btn_Execute.Size = new System.Drawing.Size(164, 23);
            this.btn_Execute.TabIndex = 2;
            this.btn_Execute.Text = "[2] 開始";
            this.btn_Execute.UseVisualStyleBackColor = true;
            this.btn_Execute.Click += new System.EventHandler(this.btn_Execute_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(12, 66);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(776, 372);
            this.richTextBox.TabIndex = 3;
            this.richTextBox.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.btn_Execute);
            this.Controls.Add(this.btn_CSV);
            this.Controls.Add(this.textBox_CSV);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "VideoUnit自動取込";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_CSV;
        private System.Windows.Forms.Button btn_CSV;
        private System.Windows.Forms.Button btn_Execute;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}

