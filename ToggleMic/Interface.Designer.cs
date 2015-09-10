namespace ToggleMic
{
  partial class Interface
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
      this.Status = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // Status
      // 
      this.Status.AutoSize = true;
      this.Status.Location = new System.Drawing.Point(12, 9);
      this.Status.Name = "Status";
      this.Status.Size = new System.Drawing.Size(37, 13);
      this.Status.TabIndex = 0;
      this.Status.Text = "Status";
      this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.Status.Click += new System.EventHandler(this.label1_Click);
      // 
      // Interface
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(202, 33);
      this.Controls.Add(this.Status);
      this.Name = "Interface";
      this.Text = "CrappyPTT 1.0";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label Status;
  }
}