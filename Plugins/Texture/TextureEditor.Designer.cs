namespace Plugins.Texture
{
    partial class TextureEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureEditor));
            this.lblTextureName = new System.Windows.Forms.Label();
            this.tboxTextureName = new System.Windows.Forms.TextBox();
            this.lblTextureFormat = new System.Windows.Forms.Label();
            this.cboxTextureFormat = new System.Windows.Forms.ComboBox();
            this.lblHasMipMaps = new System.Windows.Forms.Label();
            this.chboxHasMipMaps = new System.Windows.Forms.CheckBox();
            this.chboxIsReadable = new System.Windows.Forms.CheckBox();
            this.lblIsReadable = new System.Windows.Forms.Label();
            this.cboxFilterMode = new System.Windows.Forms.ComboBox();
            this.lblFilterMode = new System.Windows.Forms.Label();
            this.tboxAniso = new System.Windows.Forms.TextBox();
            this.lblAniso = new System.Windows.Forms.Label();
            this.tboxMipMapBias = new System.Windows.Forms.TextBox();
            this.lblMipMapBias = new System.Windows.Forms.Label();
            this.cboxWrapU = new System.Windows.Forms.ComboBox();
            this.lblWrapU = new System.Windows.Forms.Label();
            this.cboxWrapV = new System.Windows.Forms.ComboBox();
            this.lblWrapV = new System.Windows.Forms.Label();
            this.tboxLightmapFormat = new System.Windows.Forms.TextBox();
            this.lblLightmapFormat = new System.Windows.Forms.Label();
            this.cboxColorSpace = new System.Windows.Forms.ComboBox();
            this.lblColorSpace = new System.Windows.Forms.Label();
            this.lblTexture = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.cboxTextureQuality = new System.Windows.Forms.ComboBox();
            this.lblTextureQuality = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.lblWriteMethod = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTextureName
            // 
            this.lblTextureName.AutoSize = true;
            this.lblTextureName.Location = new System.Drawing.Point(11, 27);
            this.lblTextureName.Name = "lblTextureName";
            this.lblTextureName.Size = new System.Drawing.Size(39, 15);
            this.lblTextureName.TabIndex = 0;
            this.lblTextureName.Text = "Name";
            // 
            // tboxTextureName
            // 
            this.tboxTextureName.Location = new System.Drawing.Point(141, 23);
            this.tboxTextureName.Name = "tboxTextureName";
            this.tboxTextureName.Size = new System.Drawing.Size(143, 23);
            this.tboxTextureName.TabIndex = 1;
            // 
            // lblTextureFormat
            // 
            this.lblTextureFormat.AutoSize = true;
            this.lblTextureFormat.Location = new System.Drawing.Point(11, 56);
            this.lblTextureFormat.Name = "lblTextureFormat";
            this.lblTextureFormat.Size = new System.Drawing.Size(84, 15);
            this.lblTextureFormat.TabIndex = 2;
            this.lblTextureFormat.Text = "Texture format";
            // 
            // cboxTextureFormat
            // 
            this.cboxTextureFormat.FormattingEnabled = true;
            this.cboxTextureFormat.Location = new System.Drawing.Point(141, 52);
            this.cboxTextureFormat.Name = "cboxTextureFormat";
            this.cboxTextureFormat.Size = new System.Drawing.Size(143, 23);
            this.cboxTextureFormat.TabIndex = 3;
            // 
            // lblHasMipMaps
            // 
            this.lblHasMipMaps.AutoSize = true;
            this.lblHasMipMaps.Location = new System.Drawing.Point(11, 80);
            this.lblHasMipMaps.Name = "lblHasMipMaps";
            this.lblHasMipMaps.Size = new System.Drawing.Size(83, 15);
            this.lblHasMipMaps.TabIndex = 4;
            this.lblHasMipMaps.Text = "Has mip maps";
            // 
            // chboxHasMipMaps
            // 
            this.chboxHasMipMaps.AutoSize = true;
            this.chboxHasMipMaps.Location = new System.Drawing.Point(141, 81);
            this.chboxHasMipMaps.Name = "chboxHasMipMaps";
            this.chboxHasMipMaps.Size = new System.Drawing.Size(15, 14);
            this.chboxHasMipMaps.TabIndex = 5;
            this.chboxHasMipMaps.UseVisualStyleBackColor = true;
            // 
            // chboxIsReadable
            // 
            this.chboxIsReadable.AutoSize = true;
            this.chboxIsReadable.Location = new System.Drawing.Point(141, 102);
            this.chboxIsReadable.Name = "chboxIsReadable";
            this.chboxIsReadable.Size = new System.Drawing.Size(15, 14);
            this.chboxIsReadable.TabIndex = 7;
            this.chboxIsReadable.UseVisualStyleBackColor = true;
            // 
            // lblIsReadable
            // 
            this.lblIsReadable.AutoSize = true;
            this.lblIsReadable.Location = new System.Drawing.Point(11, 101);
            this.lblIsReadable.Name = "lblIsReadable";
            this.lblIsReadable.Size = new System.Drawing.Size(63, 15);
            this.lblIsReadable.TabIndex = 6;
            this.lblIsReadable.Text = "Is readable";
            // 
            // cboxFilterMode
            // 
            this.cboxFilterMode.FormattingEnabled = true;
            this.cboxFilterMode.Location = new System.Drawing.Point(141, 122);
            this.cboxFilterMode.Name = "cboxFilterMode";
            this.cboxFilterMode.Size = new System.Drawing.Size(143, 23);
            this.cboxFilterMode.TabIndex = 9;
            // 
            // lblFilterMode
            // 
            this.lblFilterMode.AutoSize = true;
            this.lblFilterMode.Location = new System.Drawing.Point(11, 127);
            this.lblFilterMode.Name = "lblFilterMode";
            this.lblFilterMode.Size = new System.Drawing.Size(67, 15);
            this.lblFilterMode.TabIndex = 8;
            this.lblFilterMode.Text = "Filter mode";
            // 
            // tboxAniso
            // 
            this.tboxAniso.Location = new System.Drawing.Point(141, 151);
            this.tboxAniso.Name = "tboxAniso";
            this.tboxAniso.Size = new System.Drawing.Size(143, 23);
            this.tboxAniso.TabIndex = 11;
            // 
            // lblAniso
            // 
            this.lblAniso.AutoSize = true;
            this.lblAniso.Location = new System.Drawing.Point(11, 155);
            this.lblAniso.Name = "lblAniso";
            this.lblAniso.Size = new System.Drawing.Size(114, 15);
            this.lblAniso.TabIndex = 10;
            this.lblAniso.Text = "Anisotropic Filtering";
            // 
            // tboxMipMapBias
            // 
            this.tboxMipMapBias.Location = new System.Drawing.Point(141, 180);
            this.tboxMipMapBias.Name = "tboxMipMapBias";
            this.tboxMipMapBias.Size = new System.Drawing.Size(143, 23);
            this.tboxMipMapBias.TabIndex = 13;
            // 
            // lblMipMapBias
            // 
            this.lblMipMapBias.AutoSize = true;
            this.lblMipMapBias.Location = new System.Drawing.Point(11, 184);
            this.lblMipMapBias.Name = "lblMipMapBias";
            this.lblMipMapBias.Size = new System.Drawing.Size(79, 15);
            this.lblMipMapBias.TabIndex = 12;
            this.lblMipMapBias.Text = "Mip map bias";
            // 
            // cboxWrapU
            // 
            this.cboxWrapU.FormattingEnabled = true;
            this.cboxWrapU.Location = new System.Drawing.Point(141, 209);
            this.cboxWrapU.Name = "cboxWrapU";
            this.cboxWrapU.Size = new System.Drawing.Size(143, 23);
            this.cboxWrapU.TabIndex = 15;
            // 
            // lblWrapU
            // 
            this.lblWrapU.AutoSize = true;
            this.lblWrapU.Location = new System.Drawing.Point(11, 215);
            this.lblWrapU.Name = "lblWrapU";
            this.lblWrapU.Size = new System.Drawing.Size(88, 15);
            this.lblWrapU.TabIndex = 14;
            this.lblWrapU.Text = "Wrap mode (U)";
            // 
            // cboxWrapV
            // 
            this.cboxWrapV.FormattingEnabled = true;
            this.cboxWrapV.Location = new System.Drawing.Point(141, 238);
            this.cboxWrapV.Name = "cboxWrapV";
            this.cboxWrapV.Size = new System.Drawing.Size(143, 23);
            this.cboxWrapV.TabIndex = 17;
            // 
            // lblWrapV
            // 
            this.lblWrapV.AutoSize = true;
            this.lblWrapV.Location = new System.Drawing.Point(11, 244);
            this.lblWrapV.Name = "lblWrapV";
            this.lblWrapV.Size = new System.Drawing.Size(87, 15);
            this.lblWrapV.TabIndex = 16;
            this.lblWrapV.Text = "Wrap mode (V)";
            // 
            // tboxLightmapFormat
            // 
            this.tboxLightmapFormat.Location = new System.Drawing.Point(141, 267);
            this.tboxLightmapFormat.Name = "tboxLightmapFormat";
            this.tboxLightmapFormat.Size = new System.Drawing.Size(143, 23);
            this.tboxLightmapFormat.TabIndex = 19;
            // 
            // lblLightmapFormat
            // 
            this.lblLightmapFormat.AutoSize = true;
            this.lblLightmapFormat.Location = new System.Drawing.Point(11, 272);
            this.lblLightmapFormat.Name = "lblLightmapFormat";
            this.lblLightmapFormat.Size = new System.Drawing.Size(97, 15);
            this.lblLightmapFormat.TabIndex = 18;
            this.lblLightmapFormat.Text = "Lightmap format";
            // 
            // cboxColorSpace
            // 
            this.cboxColorSpace.FormattingEnabled = true;
            this.cboxColorSpace.Location = new System.Drawing.Point(141, 296);
            this.cboxColorSpace.Name = "cboxColorSpace";
            this.cboxColorSpace.Size = new System.Drawing.Size(143, 23);
            this.cboxColorSpace.TabIndex = 21;
            // 
            // lblColorSpace
            // 
            this.lblColorSpace.AutoSize = true;
            this.lblColorSpace.Location = new System.Drawing.Point(11, 300);
            this.lblColorSpace.Name = "lblColorSpace";
            this.lblColorSpace.Size = new System.Drawing.Size(69, 15);
            this.lblColorSpace.TabIndex = 20;
            this.lblColorSpace.Text = "Color space";
            // 
            // lblTexture
            // 
            this.lblTexture.AutoSize = true;
            this.lblTexture.Location = new System.Drawing.Point(11, 331);
            this.lblTexture.Name = "lblTexture";
            this.lblTexture.Size = new System.Drawing.Size(45, 15);
            this.lblTexture.TabIndex = 22;
            this.lblTexture.Text = "Texture";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(140, 325);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(72, 25);
            this.btnLoad.TabIndex = 23;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(213, 325);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(72, 25);
            this.btnView.TabIndex = 24;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.BtnView_Click);
            // 
            // cboxTextureQuality
            // 
            this.cboxTextureQuality.FormattingEnabled = true;
            this.cboxTextureQuality.Items.AddRange(new object[] {
            "Very Fast",
            "Normal",
            "Slow"});
            this.cboxTextureQuality.Location = new System.Drawing.Point(141, 355);
            this.cboxTextureQuality.Name = "cboxTextureQuality";
            this.cboxTextureQuality.Size = new System.Drawing.Size(143, 23);
            this.cboxTextureQuality.TabIndex = 26;
            // 
            // lblTextureQuality
            // 
            this.lblTextureQuality.AutoSize = true;
            this.lblTextureQuality.Location = new System.Drawing.Point(11, 359);
            this.lblTextureQuality.Name = "lblTextureQuality";
            this.lblTextureQuality.Size = new System.Drawing.Size(84, 15);
            this.lblTextureQuality.TabIndex = 25;
            this.lblTextureQuality.Text = "Texture quality";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Coming soon..."});
            this.comboBox2.Location = new System.Drawing.Point(141, 384);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(143, 23);
            this.comboBox2.TabIndex = 28;
            // 
            // lblWriteMethod
            // 
            this.lblWriteMethod.AutoSize = true;
            this.lblWriteMethod.Location = new System.Drawing.Point(11, 388);
            this.lblWriteMethod.Name = "lblWriteMethod";
            this.lblWriteMethod.Size = new System.Drawing.Size(80, 15);
            this.lblWriteMethod.TabIndex = 27;
            this.lblWriteMethod.Text = "Write method";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(201, 418);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 25);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(11, 418);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 25);
            this.btnOK.TabIndex = 29;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // TextureEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(296, 455);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.lblWriteMethod);
            this.Controls.Add(this.cboxTextureQuality);
            this.Controls.Add(this.lblTextureQuality);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lblTexture);
            this.Controls.Add(this.cboxColorSpace);
            this.Controls.Add(this.lblColorSpace);
            this.Controls.Add(this.tboxLightmapFormat);
            this.Controls.Add(this.lblLightmapFormat);
            this.Controls.Add(this.cboxWrapV);
            this.Controls.Add(this.lblWrapV);
            this.Controls.Add(this.cboxWrapU);
            this.Controls.Add(this.lblWrapU);
            this.Controls.Add(this.tboxMipMapBias);
            this.Controls.Add(this.lblMipMapBias);
            this.Controls.Add(this.tboxAniso);
            this.Controls.Add(this.lblAniso);
            this.Controls.Add(this.cboxFilterMode);
            this.Controls.Add(this.lblFilterMode);
            this.Controls.Add(this.chboxIsReadable);
            this.Controls.Add(this.lblIsReadable);
            this.Controls.Add(this.chboxHasMipMaps);
            this.Controls.Add(this.lblHasMipMaps);
            this.Controls.Add(this.cboxTextureFormat);
            this.Controls.Add(this.lblTextureFormat);
            this.Controls.Add(this.tboxTextureName);
            this.Controls.Add(this.lblTextureName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TextureEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Texture2D";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTextureName;
        private System.Windows.Forms.TextBox tboxTextureName;
        private System.Windows.Forms.Label lblTextureFormat;
        private System.Windows.Forms.ComboBox cboxTextureFormat;
        private System.Windows.Forms.Label lblHasMipMaps;
        private System.Windows.Forms.CheckBox chboxHasMipMaps;
        private System.Windows.Forms.CheckBox chboxIsReadable;
        private System.Windows.Forms.Label lblIsReadable;
        private System.Windows.Forms.ComboBox cboxFilterMode;
        private System.Windows.Forms.Label lblFilterMode;
        private System.Windows.Forms.TextBox tboxAniso;
        private System.Windows.Forms.Label lblAniso;
        private System.Windows.Forms.TextBox tboxMipMapBias;
        private System.Windows.Forms.Label lblMipMapBias;
        private System.Windows.Forms.ComboBox cboxWrapU;
        private System.Windows.Forms.Label lblWrapU;
        private System.Windows.Forms.ComboBox cboxWrapV;
        private System.Windows.Forms.Label lblWrapV;
        private System.Windows.Forms.TextBox tboxLightmapFormat;
        private System.Windows.Forms.Label lblLightmapFormat;
        private System.Windows.Forms.ComboBox cboxColorSpace;
        private System.Windows.Forms.Label lblColorSpace;
        private System.Windows.Forms.Label lblTexture;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.ComboBox cboxTextureQuality;
        private System.Windows.Forms.Label lblTextureQuality;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label lblWriteMethod;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}