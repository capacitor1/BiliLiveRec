namespace BiliLiveRec
{
    partial class Input
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
            Content = new TextBox();
            Ok = new Button();
            SuspendLayout();
            // 
            // Content
            // 
            Content.Dock = DockStyle.Top;
            Content.Location = new Point(0, 0);
            Content.Name = "Content";
            Content.Size = new Size(384, 23);
            Content.TabIndex = 0;
            Content.KeyDown += Content_KeyDown;
            // 
            // Ok
            // 
            Ok.Location = new Point(309, 26);
            Ok.Name = "Ok";
            Ok.Size = new Size(75, 23);
            Ok.TabIndex = 1;
            Ok.Text = "Confirm";
            Ok.UseVisualStyleBackColor = true;
            Ok.Click += Ok_Click;
            // 
            // Input
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 52);
            Controls.Add(Ok);
            Controls.Add(Content);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Input";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Input";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Content;
        private Button Ok;
    }
}