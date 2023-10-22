namespace AssetsUpdate
{
    partial class InputDialog
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
            uiOkButton = new Button();
            uiCancelButton = new Button();
            label1 = new Label();
            uiInputValua = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)uiInputValua).BeginInit();
            SuspendLayout();
            // 
            // uiOkButton
            // 
            uiOkButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            uiOkButton.Location = new Point(199, 62);
            uiOkButton.Name = "uiOkButton";
            uiOkButton.Size = new Size(85, 27);
            uiOkButton.TabIndex = 0;
            uiOkButton.Text = "OK";
            uiOkButton.UseVisualStyleBackColor = true;
            uiOkButton.Click += uiOkButton_Click;
            // 
            // uiCancelButton
            // 
            uiCancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            uiCancelButton.Location = new Point(300, 62);
            uiCancelButton.Name = "uiCancelButton";
            uiCancelButton.Size = new Size(85, 27);
            uiCancelButton.TabIndex = 1;
            uiCancelButton.Text = "Cancel";
            uiCancelButton.UseVisualStyleBackColor = true;
            uiCancelButton.Click += uiCancelButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 23);
            label1.Name = "label1";
            label1.Size = new Size(245, 15);
            label1.TabIndex = 2;
            label1.Text = "What's column index do you want override ? ";
            // 
            // uiInputValua
            // 
            uiInputValua.Location = new Point(278, 23);
            uiInputValua.Name = "uiInputValua";
            uiInputValua.Size = new Size(41, 23);
            uiInputValua.TabIndex = 3;
            // 
            // InputDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(397, 101);
            Controls.Add(uiInputValua);
            Controls.Add(label1);
            Controls.Add(uiCancelButton);
            Controls.Add(uiOkButton);
            MaximumSize = new Size(413, 140);
            MinimumSize = new Size(413, 140);
            Name = "InputDialog";
            Text = "Override Column Input";
            ((System.ComponentModel.ISupportInitialize)uiInputValua).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button uiOkButton;
        private Button uiCancelButton;
        private Label label1;
        private NumericUpDown uiInputValua;
    }
}