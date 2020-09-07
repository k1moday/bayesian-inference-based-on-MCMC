using System;
using System.Collections;
using System.Windows.Forms;
using BELIEF_NET;

internal class App
{
	internal static void Main()
	{
		BNForm frmMain = new BNForm();
		Application.Run(frmMain);
	}
}

public class BNForm : Form
{
	public BNForm()
	{
		InitializeComponents();
	}

	private void InitializeComponents()
	{
		this.lbBNodes = new System.Windows.Forms.ListBox();
		this.lbResult = new System.Windows.Forms.ListBox();
		this.tbXmlFile = new System.Windows.Forms.TextBox();
		this.tbCondition = new System.Windows.Forms.TextBox();//条件文本输入框
		this.tbQuery = new System.Windows.Forms.TextBox();//查询文本输入框
		this.lblFile = new System.Windows.Forms.Label();
		this.lblNodes = new System.Windows.Forms.Label();
		this.lblResult = new System.Windows.Forms.Label();
		this.lblQuery = new System.Windows.Forms.Label();
		this.lblCond = new System.Windows.Forms.Label();
		this.btnQuery = new System.Windows.Forms.Button();
		this.btnFile = new System.Windows.Forms.Button();
		this.SuspendLayout();

		// UI Layout
		int formW = 450;
		int formH = 320;
		int x1 = 10;
		int x2 = 140;
		System.Drawing.Point boxSize1 = new System.Drawing.Point(x2 - x1 - 20, 40);
		System.Drawing.Point boxSize2 = new System.Drawing.Point(300, 40);
		System.Drawing.Point lblSize = new System.Drawing.Point(60, 20);
		System.Drawing.Point txtSize = new System.Drawing.Point(300, 20);

		// label lblNodes
		this.lblNodes.Location = new System.Drawing.Point(x1, 60);
		this.lbBNodes.Name = "Nodes";
		this.lbBNodes.Size = new System.Drawing.Size(lblSize);
		this.lbBNodes.TabIndex = 0;
		this.lblNodes.Text = "Nodes:";

		// listBox lbBNodes
		this.lbBNodes.Location = new System.Drawing.Point(x1, 80);
		this.lbBNodes.Name = "ExtToDelBox";
		this.lbBNodes.Size = new System.Drawing.Size(x2 - x1 - 20, 185);
		this.lbBNodes.TabIndex = 0;

		// textBox lbXmlFile
		this.tbXmlFile.Location = new System.Drawing.Point(x1, 20);
		this.tbXmlFile.Name = "XML File";
		this.tbXmlFile.Size = new System.Drawing.Size(boxSize1);
		this.tbXmlFile.TabIndex = 0;
		this.tbXmlFile.Text = "BN_WetGrass.xml";

		// label lblCond
		this.lblCond.Location = new System.Drawing.Point(x2, 10);
		this.lblCond.Name = "Evidence";
		this.lblCond.Size = new System.Drawing.Size(txtSize);
		this.lblCond.TabIndex = 0;
		this.lblCond.Text = "Type evidence. ";

		// textBox tbCondition
		this.tbCondition.Location = new System.Drawing.Point(x2, 30);
		this.tbCondition.Name = "Contition";
		this.tbCondition.Size = new System.Drawing.Size(boxSize2);
		this.tbCondition.TabIndex = 0;

		// label lblQuery
		this.lblQuery.Location = new System.Drawing.Point(x2, 80);
		this.lblQuery.Name = "Query";
		this.lblQuery.Size = new System.Drawing.Size(txtSize);
		this.lblQuery.TabIndex = 0;
		this.lblQuery.Text = "Type query. ";

		// textBox tbQuery
		this.tbQuery.Location = new System.Drawing.Point(x2, 100);
		this.tbQuery.Name = "Query";
		this.tbQuery.Size = new System.Drawing.Size(boxSize2);
		this.tbQuery.TabIndex = 0;

		// label lblResult
		this.lblResult.Location = new System.Drawing.Point(x2, 175);
		this.lblResult.Name = "Result";
		this.lblResult.Size = new System.Drawing.Size(lblSize);
		this.lblResult.TabIndex = 0;
		this.lblResult.Text = "Result:";

		// listBox lbResult
		this.lbResult.Location = new System.Drawing.Point(x2, 195);
		this.lbResult.Name = "ResultBox";
		this.lbResult.Size = new System.Drawing.Size(boxSize2.X, 60);
		this.lbResult.TabIndex = 0;

		// btnFile
		int btnW = 80, btnH = 30;
		int btnX1 = boxSize1.X - btnW;
		this.btnFile.Location = new System.Drawing.Point(btnX1 / 2 + x1, 270);
		this.btnFile.Name = "File";
		this.btnFile.Size = new System.Drawing.Size(btnW, btnH);
		this.btnFile.TabIndex = 0;
		this.btnFile.Text = "Load Net";
		this.btnFile.Enabled = true;
		this.btnFile.Click += new System.EventHandler(this.btnLoad_Click);

		// btnQuery
		this.btnQuery.Location = new System.Drawing.Point(240, 270);
		this.btnQuery.Name = "Query";
		this.btnQuery.Size = new System.Drawing.Size(btnW, btnH);
		this.btnQuery.TabIndex = 1;
		this.btnQuery.Text = "Get Result";
		this.btnQuery.Enabled = false;
		this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);

		// MainForm
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.ClientSize = new System.Drawing.Size(formW, formH);
		this.Controls.AddRange(
			new System.Windows.Forms.Control[] {this.btnQuery,
						this.btnFile,
						this.tbXmlFile,
						this.tbCondition,
						this.tbQuery,
						this.lbBNodes,
						this.lblNodes,
						this.lblCond,
						this.lblQuery,
						this.lblResult,
						this.lbResult}
			);

		this.Name = "MainForm";
		this.Text = "Belief Network Inference";
		this.Load += new System.EventHandler(this.MainForm_Load);
		this.ResumeLayout(false);
	}


	private void MainForm_Load(object sender, System.EventArgs e)
	{
		m_net = new BNet();
		m_query = new ArrayList();
	}

	private void btnLoad_Click(object sender, System.EventArgs e)
	{
		m_net.Build(tbXmlFile.Text);
		m_net.PrintNet("BNet_layout.txt");

		m_infer = new BMcmc(m_net);//两种算法需要哪种更改类名即可更换调用

		lbBNodes.Items.Clear();
		foreach (BNode node in m_net.Nodes)
			lbBNodes.Items.Add(node.Name);

		this.btnFile.Enabled = false;
		this.btnQuery.Enabled = true;

		lblCond.Text += "  For example: " + lbBNodes.Items[0] + "=1; "
		+ lbBNodes.Items[1] + "=0";
		lblQuery.Text += "  For example: " + lbBNodes.Items[2] + "=1";
	}

	private void btnQuery_Click(object sender, System.EventArgs e)
	{
		tbCondition.Text = tbCondition.Text.TrimEnd(';');//对于输入进来的条件文本用；进行分割
		tbQuery.Text = tbQuery.Text.TrimEnd(';');//对于输入进来的查询文本用；进行分割
		string obs = tbCondition.Text.Trim();
		string x = tbQuery.Text.Trim();

		if (IsValid(x, obs))
		{
			double pr = m_infer.GetBelief(x, obs);//得到条件和查询目标后，进行推理计算，根据情况和需求进行更改

			string out1 = " P( " + x;
			out1 += (obs.Length > 0) ? " | " + obs + " )" : " )";
			string out2 = "  = " + pr.ToString("F4");
			lbResult.Items.Clear();
			lbResult.Items.Add("");
			lbResult.Items.Add(out1);
			lbResult.Items.Add(out2);
		}

		m_query.Clear();
	}

	private bool IsValid(string x, string o)
	{
		bool valid = true;
		if (o.Length > 0)
			valid = IsNameValid(o);
		if (x.Length == 0)
		{
			MessageBox.Show("Please provide query");
			valid = false;
		}
		else if (valid)
			valid = IsNameValid(x);
		return valid;
	}

	private bool IsNameValid(string s)
	{
		string[] items = s.Split(';');
		foreach (string item in items)
		{
			string[] pair = item.Split('=');
			if (pair.Length != 2 || pair[1].Length == 0)
			{
				MessageBox.Show("Value is missing!");
				return false;
			}
			else if (!lbBNodes.Items.Contains(pair[0].Trim().ToLower()))
			{
				MessageBox.Show("'" + pair[0] + "' is not a valid node");
				return false;
			}
			else if (m_query.Contains(pair[0].Trim().ToLower()))
			{
				MessageBox.Show("'" + pair[0] + "' has been used more than once");
				return false;
			}
			else
				m_query.Add(pair[0].Trim().ToLower());
		}
		return true;
	}

	private System.Windows.Forms.ListBox lbResult;
	private System.Windows.Forms.ListBox lbBNodes;
	private System.Windows.Forms.TextBox tbXmlFile;
	private System.Windows.Forms.TextBox tbCondition;
	private System.Windows.Forms.TextBox tbQuery;
	private System.Windows.Forms.Label lblFile;
	private System.Windows.Forms.Label lblQuery;
	private System.Windows.Forms.Label lblCond;
	private System.Windows.Forms.Label lblNodes;
	private System.Windows.Forms.Label lblResult;

	private System.Windows.Forms.Button btnQuery;
	private System.Windows.Forms.Button btnFile;

	private BNet m_net;
	private BNInfer m_infer;
	private ArrayList m_query;
}
