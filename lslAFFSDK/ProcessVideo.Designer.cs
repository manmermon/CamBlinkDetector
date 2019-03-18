/*
* Modified by Manuel Merino Monge <manmermon@dte.us.es>
*/
using System;

namespace lslAFFSDK
{
    partial class ProcessVideo
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
            // 
            // ProcessVideo
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6.5F, 13.5F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.ProcessVideo_FormClosing );
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler( this.ProcessVideo_KeyPress );


            this.btnStart = new System.Windows.Forms.CheckBox();
            this.btnStart.Appearance = System.Windows.Forms.Appearance.Button;
            this.comboWlinkEvents = new System.Windows.Forms.ComboBox();
            this.comboMultiBlinkEvents = new System.Windows.Forms.ComboBox();
            this.labelWlinkEvent = new System.Windows.Forms.Label();
            this.labelMultiBlinkEvent = new System.Windows.Forms.Label();

            // 
            // btnStart
            //             
            //this.btnStart.Location = new System.Drawing.Point( 0, 0 );
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(85, 26);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Empezar";
            this.btnStart.UseMnemonic = false;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler( this.btStart_Click );

            // 
            // comboWlinkEvents
            // 
            this.comboWlinkEvents.FormattingEnabled = true;
            this.comboWlinkEvents.Items.AddRange(new object[] {
            "Click derecho",
            "Click izquierdo",
            "Doble click derecho",
            "Doble click izquierdo",
            "Intro",
            "Espacio",
            "Nada"});
            //this.comboEvents.Location = new System.Drawing.Point(430, 460);
            this.comboWlinkEvents.Name = "comboWlinkEvents";
            this.comboWlinkEvents.Size = new System.Drawing.Size(134, 21);
            this.comboWlinkEvents.TabIndex = 2;
            this.comboWlinkEvents.SelectedIndex = 0;
            this.comboWlinkEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // 
            // comboMultiBlinkEvents
            // 
            this.comboMultiBlinkEvents.FormattingEnabled = true;
            this.comboMultiBlinkEvents.Items.AddRange(new object[] {
            "Click derecho",
            "Click izquierdo",
            "Doble click derecho",
            "Doble click izquierdo",
            "Intro",
            "Espacio",
            "Nada"});
            //this.comboEvents.Location = new System.Drawing.Point(430, 460);
            this.comboMultiBlinkEvents.Name = "comboMultiBlinkEvents";
            this.comboMultiBlinkEvents.Size = new System.Drawing.Size(134, 21);
            this.comboMultiBlinkEvents.TabIndex = 2;
            this.comboMultiBlinkEvents.SelectedIndex = 6;
            this.comboMultiBlinkEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // 
            // labelWlinkEvent
            // 
            this.labelWlinkEvent.AutoSize = true;
            //this.labelEvent.Location = new System.Drawing.Point(380, 463);
            this.labelWlinkEvent.Name = "labelWlinkEvent";
            this.labelWlinkEvent.Size = new System.Drawing.Size(44, 13);
            this.labelWlinkEvent.TabIndex = 3;
            this.labelWlinkEvent.Text = "Parpadeo largo:";

            // 
            // labelMultiBlinkEvent
            // 
            this.labelMultiBlinkEvent.AutoSize = true;
            //this.labelEvent.Location = new System.Drawing.Point(380, 463);
            this.labelMultiBlinkEvent.Name = "labelMultiBlinkEvent";
            this.labelMultiBlinkEvent.Size = new System.Drawing.Size(44, 13);
            this.labelMultiBlinkEvent.TabIndex = 3;
            this.labelMultiBlinkEvent.Text = "Parpadeo multiple:";



            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new VideoPanel( this );
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add( this.panel1 );
            this.flowLayoutPanel5.Location = new System.Drawing.Point(5, 5);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size( 600, 500 );
            this.flowLayoutPanel5.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel2";
            this.panel1.Size = new System.Drawing.Size(600, 650);
            this.panel1.TabIndex = 0;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Controls.Add(this.btnStart);
            //this.flowLayoutPanel6.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel6.Location = new System.Drawing.Point( 5, 5 );
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size( 690, 50 );
            this.flowLayoutPanel6.TabIndex = 1;
                                    
            this.flowLayoutPanel6.Controls.Add(this.btnStart);
            this.flowLayoutPanel6.Controls.Add(this.labelWlinkEvent);
            this.flowLayoutPanel6.Controls.Add(this.comboWlinkEvents);
            this.flowLayoutPanel6.Controls.Add(this.labelMultiBlinkEvent);
            this.flowLayoutPanel6.Controls.Add(this.comboMultiBlinkEvents);


            // 
            // ProcessVideo
            // 
            this.ClientSize = new System.Drawing.Size( 625, 500 );
            this.Controls.Add(this.flowLayoutPanel6);
            this.Controls.Add(this.flowLayoutPanel5);
            
            

            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);

            this.Name = "TaisBlinks";
            this.Text = "TAIS Blinks";            

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            ExcCommander excWlink = new ExcCommander();
            ExcCommander excMultiBlink = new ExcCommander();

            if ( btnStart.Checked )
            {
                btnStart.Text = "Parar";
                this.comboWlinkEvents.Enabled = false;
                this.comboMultiBlinkEvents.Enabled = false;

                if (this.comboWlinkEvents.SelectedItem.ToString().ToLower().Equals("intro"))
                {
                    excWlink.addCommand("{ENTER}");
                }
                else if (this.comboWlinkEvents.SelectedItem.ToString().ToLower().Equals("espacio"))
                {
                    excWlink.addCommand(" ");
                }
                else if (this.comboWlinkEvents.SelectedItem.ToString().ToLower().Equals("click derecho"))
                {
                    excWlink.addCommand("mouseRight");
                }
                else if (this.comboWlinkEvents.SelectedItem.ToString().ToLower().Equals("doble click derecho"))
                {
                    excWlink.addCommand("mouseDoubleRight");
                }
                else if (this.comboWlinkEvents.SelectedItem.ToString().ToLower().Equals("click izquierdo"))
                {
                    excWlink.addCommand("mouseLeft");
                }
                else if (this.comboWlinkEvents.SelectedItem.ToString().ToLower().Equals("doble click izquierdo"))
                {
                    excWlink.addCommand("mouseDoubleLeft");
                }

                if (this.comboMultiBlinkEvents.SelectedItem.ToString().ToLower().Equals("intro"))
                {
                    excMultiBlink.addCommand("{ENTER}");
                }
                else if (this.comboMultiBlinkEvents.SelectedItem.ToString().ToLower().Equals("espacio"))
                {
                    excMultiBlink.addCommand(" ");
                }
                else if (this.comboMultiBlinkEvents.SelectedItem.ToString().ToLower().Equals("click derecho"))
                {
                    excMultiBlink.addCommand("mouseRight");
                }
                else if (this.comboMultiBlinkEvents.SelectedItem.ToString().ToLower().Equals("doble click derecho"))
                {
                    excMultiBlink.addCommand("mouseDoubleRight");
                }
                else if (this.comboMultiBlinkEvents.SelectedItem.ToString().ToLower().Equals("click izquierdo"))
                {
                    excMultiBlink.addCommand("mouseLeft");
                }
                else if (this.comboMultiBlinkEvents.SelectedItem.ToString().ToLower().Equals("doble click izquierdo"))
                {
                    excMultiBlink.addCommand("mouseDoubleLeft");
                }

            }
            else
            {
                btnStart.Text = "Empezar";
                this.comboWlinkEvents.Enabled = true;
                this.comboMultiBlinkEvents.Enabled = true;
            }
            
            this.panel1.setStart( excWlink, excMultiBlink );
        }

        #endregion
    }
}