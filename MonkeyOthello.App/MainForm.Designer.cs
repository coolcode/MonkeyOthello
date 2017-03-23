namespace MonkeyOthello
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cvsCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useBookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pvsPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlOthelloBoard = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblNodes = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSpendTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEval = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSquare = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEmpties = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statMain = new System.Windows.Forms.StatusStrip();
            this.aboutAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pvsCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lsvMoves = new System.Windows.Forms.ListView();
            this.colNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMove = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEval = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.picMonkey = new System.Windows.Forms.PictureBox();
            this.picblackStone = new System.Windows.Forms.PictureBox();
            this.picWhiteStone = new System.Windows.Forms.PictureBox();
            this.Tips = new System.Windows.Forms.ToolTip(this.components);
            this.lblwhiteNum = new System.Windows.Forms.Label();
            this.lblblackNum = new System.Windows.Forms.Label();
            this.cvsPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameModetoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCrazy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExpert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMedium = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEasy = new System.Windows.Forms.ToolStripMenuItem();
            this.levelLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMonkey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picblackStone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWhiteStone)).BeginInit();
            this.menuMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cvsCToolStripMenuItem
            // 
            this.cvsCToolStripMenuItem.Name = "cvsCToolStripMenuItem";
            this.cvsCToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.cvsCToolStripMenuItem.Text = "Monkey Vs Monkey";
            // 
            // useBookToolStripMenuItem
            // 
            this.useBookToolStripMenuItem.Name = "useBookToolStripMenuItem";
            this.useBookToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.useBookToolStripMenuItem.Text = "Opening Book";
            this.useBookToolStripMenuItem.Visible = false;
            // 
            // pvsPToolStripMenuItem
            // 
            this.pvsPToolStripMenuItem.Name = "pvsPToolStripMenuItem";
            this.pvsPToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.pvsPToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.pvsPToolStripMenuItem.Text = "Human Vs Human";
            // 
            // pnlOthelloBoard
            // 
            this.pnlOthelloBoard.BackgroundImage = global::MonkeyOthello.Properties.Resources.background;
            this.pnlOthelloBoard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlOthelloBoard.Location = new System.Drawing.Point(6, 30);
            this.pnlOthelloBoard.Name = "pnlOthelloBoard";
            this.pnlOthelloBoard.Size = new System.Drawing.Size(400, 400);
            this.pnlOthelloBoard.TabIndex = 34;
            this.pnlOthelloBoard.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblMessage.Size = new System.Drawing.Size(16, 19);
            this.lblMessage.Text = "   ";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNodes
            // 
            this.lblNodes.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblNodes.Name = "lblNodes";
            this.lblNodes.Size = new System.Drawing.Size(17, 19);
            this.lblNodes.Text = "0";
            this.lblNodes.ToolTipText = "Nodes";
            // 
            // lblSpeed
            // 
            this.lblSpeed.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(42, 19);
            this.lblSpeed.Text = "0 NPS";
            this.lblSpeed.ToolTipText = "Search Speed";
            // 
            // lblSpendTime
            // 
            this.lblSpendTime.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblSpendTime.Name = "lblSpendTime";
            this.lblSpendTime.Size = new System.Drawing.Size(34, 19);
            this.lblSpendTime.Text = "0.0 s";
            this.lblSpendTime.ToolTipText = "Time";
            // 
            // lblEval
            // 
            this.lblEval.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblEval.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEval.ForeColor = System.Drawing.Color.Blue;
            this.lblEval.Name = "lblEval";
            this.lblEval.Size = new System.Drawing.Size(18, 19);
            this.lblEval.Text = "0";
            this.lblEval.ToolTipText = "Evaluation";
            // 
            // lblSquare
            // 
            this.lblSquare.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblSquare.Name = "lblSquare";
            this.lblSquare.Size = new System.Drawing.Size(17, 19);
            this.lblSquare.Text = "  ";
            this.lblSquare.ToolTipText = "Move";
            // 
            // lblEmpties
            // 
            this.lblEmpties.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblEmpties.Name = "lblEmpties";
            this.lblEmpties.Size = new System.Drawing.Size(23, 19);
            this.lblEmpties.Text = "60";
            this.lblEmpties.ToolTipText = "Empties";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(14, 19);
            this.toolStripStatusLabel4.Text = "#";
            // 
            // statMain
            // 
            this.statMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel4,
            this.lblEmpties,
            this.lblSquare,
            this.lblEval,
            this.lblSpendTime,
            this.lblSpeed,
            this.lblNodes,
            this.lblMessage});
            this.statMain.Location = new System.Drawing.Point(0, 439);
            this.statMain.Name = "statMain";
            this.statMain.ShowItemToolTips = true;
            this.statMain.Size = new System.Drawing.Size(569, 24);
            this.statMain.TabIndex = 37;
            // 
            // aboutAToolStripMenuItem
            // 
            this.aboutAToolStripMenuItem.Name = "aboutAToolStripMenuItem";
            this.aboutAToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.aboutAToolStripMenuItem.Text = "About(&A)";
            this.aboutAToolStripMenuItem.Click += new System.EventHandler(this.aboutAToolStripMenuItem_Click);
            // 
            // helpHToolStripMenuItem
            // 
            this.helpHToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutAToolStripMenuItem});
            this.helpHToolStripMenuItem.Name = "helpHToolStripMenuItem";
            this.helpHToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.helpHToolStripMenuItem.Text = "Help(&H)";
            // 
            // pvsCToolStripMenuItem
            // 
            this.pvsCToolStripMenuItem.Name = "pvsCToolStripMenuItem";
            this.pvsCToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.pvsCToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.pvsCToolStripMenuItem.Text = "Monkey Play White";
            // 
            // lsvMoves
            // 
            this.lsvMoves.AutoArrange = false;
            this.lsvMoves.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNum,
            this.colColor,
            this.colMove,
            this.colEval});
            this.lsvMoves.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lsvMoves.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvMoves.Location = new System.Drawing.Point(415, 130);
            this.lsvMoves.Name = "lsvMoves";
            this.lsvMoves.Size = new System.Drawing.Size(149, 300);
            this.lsvMoves.TabIndex = 38;
            this.Tips.SetToolTip(this.lsvMoves, "Moves History");
            this.lsvMoves.UseCompatibleStateImageBehavior = false;
            this.lsvMoves.View = System.Windows.Forms.View.Details;
            // 
            // colNum
            // 
            this.colNum.Text = "#";
            this.colNum.Width = 26;
            // 
            // colColor
            // 
            this.colColor.Text = "○";
            this.colColor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colColor.Width = 26;
            // 
            // colMove
            // 
            this.colMove.Text = "Move";
            this.colMove.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colMove.Width = 38;
            // 
            // colEval
            // 
            this.colEval.Text = "Eval";
            this.colEval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colEval.Width = 38;
            // 
            // picMonkey
            // 
            this.picMonkey.BackgroundImage = global::MonkeyOthello.Properties.Resources.Monkey;
            this.picMonkey.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picMonkey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picMonkey.Location = new System.Drawing.Point(8, 15);
            this.picMonkey.Name = "picMonkey";
            this.picMonkey.Size = new System.Drawing.Size(60, 60);
            this.picMonkey.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMonkey.TabIndex = 17;
            this.picMonkey.TabStop = false;
            this.Tips.SetToolTip(this.picMonkey, "Monkey Othello");
            // 
            // picblackStone
            // 
            this.picblackStone.BackgroundImage = global::MonkeyOthello.Properties.Resources.BlackStone;
            this.picblackStone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picblackStone.Location = new System.Drawing.Point(74, 15);
            this.picblackStone.Name = "picblackStone";
            this.picblackStone.Size = new System.Drawing.Size(24, 24);
            this.picblackStone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picblackStone.TabIndex = 8;
            this.picblackStone.TabStop = false;
            this.Tips.SetToolTip(this.picblackStone, "Black Pieces");
            // 
            // picWhiteStone
            // 
            this.picWhiteStone.BackgroundImage = global::MonkeyOthello.Properties.Resources.WhiteStone;
            this.picWhiteStone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picWhiteStone.Location = new System.Drawing.Point(74, 51);
            this.picWhiteStone.Name = "picWhiteStone";
            this.picWhiteStone.Size = new System.Drawing.Size(24, 24);
            this.picWhiteStone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWhiteStone.TabIndex = 9;
            this.picWhiteStone.TabStop = false;
            this.Tips.SetToolTip(this.picWhiteStone, "White Pieces");
            // 
            // lblwhiteNum
            // 
            this.lblwhiteNum.AutoSize = true;
            this.lblwhiteNum.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblwhiteNum.ForeColor = System.Drawing.Color.Red;
            this.lblwhiteNum.Location = new System.Drawing.Point(110, 56);
            this.lblwhiteNum.Name = "lblwhiteNum";
            this.lblwhiteNum.Size = new System.Drawing.Size(17, 16);
            this.lblwhiteNum.TabIndex = 11;
            this.lblwhiteNum.Text = " ";
            // 
            // lblblackNum
            // 
            this.lblblackNum.AutoSize = true;
            this.lblblackNum.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblblackNum.ForeColor = System.Drawing.Color.Red;
            this.lblblackNum.Location = new System.Drawing.Point(110, 20);
            this.lblblackNum.Name = "lblblackNum";
            this.lblblackNum.Size = new System.Drawing.Size(17, 16);
            this.lblblackNum.TabIndex = 10;
            this.lblblackNum.Text = " ";
            // 
            // cvsPToolStripMenuItem
            // 
            this.cvsPToolStripMenuItem.Name = "cvsPToolStripMenuItem";
            this.cvsPToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.cvsPToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.cvsPToolStripMenuItem.Text = "Monkey Play Black";
            // 
            // gameModetoolStripMenuItem
            // 
            this.gameModetoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cvsPToolStripMenuItem,
            this.pvsCToolStripMenuItem,
            this.pvsPToolStripMenuItem,
            this.cvsCToolStripMenuItem});
            this.gameModetoolStripMenuItem.Name = "gameModetoolStripMenuItem";
            this.gameModetoolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.gameModetoolStripMenuItem.Text = "Game Mode";
            // 
            // optionOToolStripMenuItem
            // 
            this.optionOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameModetoolStripMenuItem,
            this.useBookToolStripMenuItem});
            this.optionOToolStripMenuItem.Name = "optionOToolStripMenuItem";
            this.optionOToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.optionOToolStripMenuItem.Text = "Options(&O)";
            // 
            // menuCrazy
            // 
            this.menuCrazy.Name = "menuCrazy";
            this.menuCrazy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
            this.menuCrazy.Size = new System.Drawing.Size(159, 22);
            this.menuCrazy.Text = "Crazy";
            // 
            // menuExpert
            // 
            this.menuExpert.Name = "menuExpert";
            this.menuExpert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.menuExpert.Size = new System.Drawing.Size(159, 22);
            this.menuExpert.Text = "Expert";
            // 
            // menuHard
            // 
            this.menuHard.Name = "menuHard";
            this.menuHard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.menuHard.Size = new System.Drawing.Size(159, 22);
            this.menuHard.Text = "Hard";
            // 
            // menuMedium
            // 
            this.menuMedium.Name = "menuMedium";
            this.menuMedium.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.menuMedium.Size = new System.Drawing.Size(159, 22);
            this.menuMedium.Text = "Medium";
            // 
            // menuEasy
            // 
            this.menuEasy.Name = "menuEasy";
            this.menuEasy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.menuEasy.Size = new System.Drawing.Size(159, 22);
            this.menuEasy.Text = "Easy";
            // 
            // levelLToolStripMenuItem
            // 
            this.levelLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEasy,
            this.menuMedium,
            this.menuHard,
            this.menuExpert,
            this.menuCrazy});
            this.levelLToolStripMenuItem.Name = "levelLToolStripMenuItem";
            this.levelLToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.levelLToolStripMenuItem.Text = "Level(&L)";
            // 
            // exitEToolStripMenuItem
            // 
            this.exitEToolStripMenuItem.Name = "exitEToolStripMenuItem";
            this.exitEToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.exitEToolStripMenuItem.Text = "Exit(&E)";
            this.exitEToolStripMenuItem.Click += new System.EventHandler(this.exitEToolStripMenuItem_Click);
            // 
            // undoUToolStripMenuItem
            // 
            this.undoUToolStripMenuItem.Name = "undoUToolStripMenuItem";
            this.undoUToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoUToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.undoUToolStripMenuItem.Text = "Take Back(&U)";
            this.undoUToolStripMenuItem.Click += new System.EventHandler(this.undoUToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(180, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.saveAsToolStripMenuItem.Text = "Export";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // newNToolStripMenuItem
            // 
            this.newNToolStripMenuItem.Name = "newNToolStripMenuItem";
            this.newNToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.newNToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.newNToolStripMenuItem.Text = "New(&N)";
            this.newNToolStripMenuItem.Click += new System.EventHandler(this.newNToolStripMenuItem_Click);
            // 
            // gameGToolStripMenuItem
            // 
            this.gameGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newNToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.undoUToolStripMenuItem,
            this.exitEToolStripMenuItem});
            this.gameGToolStripMenuItem.Name = "gameGToolStripMenuItem";
            this.gameGToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.gameGToolStripMenuItem.Text = "Game(&G)";
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameGToolStripMenuItem,
            this.levelLToolStripMenuItem,
            this.optionOToolStripMenuItem,
            this.helpHToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(569, 24);
            this.menuMain.TabIndex = 35;
            this.menuMain.Text = "menuStrip1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picMonkey);
            this.groupBox1.Controls.Add(this.picblackStone);
            this.groupBox1.Controls.Add(this.picWhiteStone);
            this.groupBox1.Controls.Add(this.lblblackNum);
            this.groupBox1.Controls.Add(this.lblwhiteNum);
            this.groupBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox1.Location = new System.Drawing.Point(417, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 92);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 463);
            this.Controls.Add(this.pnlOthelloBoard);
            this.Controls.Add(this.statMain);
            this.Controls.Add(this.lsvMoves);
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monkey Othello v3.0 (Bruce Lee)";
            this.statMain.ResumeLayout(false);
            this.statMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMonkey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picblackStone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWhiteStone)).EndInit();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem cvsCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useBookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pvsPToolStripMenuItem;
        private System.Windows.Forms.Panel pnlOthelloBoard;
        private System.Windows.Forms.ToolStripStatusLabel lblMessage;
        private System.Windows.Forms.ToolStripStatusLabel lblNodes;
        private System.Windows.Forms.ToolStripStatusLabel lblSpeed;
        private System.Windows.Forms.ToolStripStatusLabel lblSpendTime;
        private System.Windows.Forms.ToolStripStatusLabel lblEval;
        private System.Windows.Forms.ToolStripStatusLabel lblSquare;
        private System.Windows.Forms.ToolStripStatusLabel lblEmpties;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.StatusStrip statMain;
        private System.Windows.Forms.ToolStripMenuItem aboutAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pvsCToolStripMenuItem;
        private System.Windows.Forms.ListView lsvMoves;
        private System.Windows.Forms.ColumnHeader colNum;
        private System.Windows.Forms.ColumnHeader colColor;
        private System.Windows.Forms.ColumnHeader colMove;
        private System.Windows.Forms.ColumnHeader colEval;
        private System.Windows.Forms.ToolTip Tips;
        private System.Windows.Forms.PictureBox picMonkey;
        private System.Windows.Forms.PictureBox picblackStone;
        private System.Windows.Forms.PictureBox picWhiteStone;
        private System.Windows.Forms.Label lblwhiteNum;
        private System.Windows.Forms.Label lblblackNum;
        private System.Windows.Forms.ToolStripMenuItem cvsPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameModetoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuCrazy;
        private System.Windows.Forms.ToolStripMenuItem menuExpert;
        private System.Windows.Forms.ToolStripMenuItem menuHard;
        private System.Windows.Forms.ToolStripMenuItem menuMedium;
        private System.Windows.Forms.ToolStripMenuItem menuEasy;
        private System.Windows.Forms.ToolStripMenuItem levelLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoUToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameGToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

