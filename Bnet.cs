﻿namespace BELIEF_NET
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Collections;
	using System.Diagnostics;

	class BNode
	{
		public BNode(string name, int id, int range)
		{
			m_name = name;
			m_id = id;
			m_range = range;
			Parents = new ArrayList();
			children = new ArrayList();
			Clear();
		}

		public string Name
		{
			get { return m_name; }
		}

		public int ID
		{
			get { return m_id; }
		}

		public int Range
		{
			get { return m_range; }
		}

		public void Clear()
		{
			Evidence = -1;
		}

		public ArrayList Parents;
		public ArrayList children;
		public double[,] CPT;
		public int Evidence;

		private string m_name;
		private int m_id;
		private int m_range;
	}

	class BNet
	{
		public BNet()
		{
			m_bnNodes = new ArrayList();
		}

		public ArrayList Nodes
		{
			get { return m_bnNodes; }
		}

		public void ResetNodes()
		{
			foreach (BNode node in m_bnNodes)
				node.Clear();
		}

		public void SetNodes(string s)
		{
			string[] obs = s.Split(';');
			foreach (string ob in obs)
			{
				string[] pair = ob.Split('=');
				foreach (BNode node in m_bnNodes)
					if (pair.Length == 2 &&
						node.Name == pair[0].Trim().ToLower())
					{
						node.Evidence = Convert.ToInt32(pair[1]);
						break;
					}
			}
		}

		public void Build(string xmlfile)//文件读取并以/BNetNodes/*为分割 将xml数据文件分割成节点
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlfile);
			XmlElement root = doc.DocumentElement;

			XmlNodeList nodeList = root.SelectNodes("/BNetNodes/*");

			foreach (XmlNode node in nodeList)
				CreateNode(node);
		}

		private void CreateNode(XmlNode theXmlNode)//对每个节点的数据进行储存操作
		{
			//Attributes是对于每个节点的属性和值进行获取的函数
			XmlAttributeCollection attr = theXmlNode.Attributes;
			int range = Convert.ToInt32(attr.GetNamedItem("Range").Value);

			// 新建节点并添加值
			int nid = m_bnNodes.Count;
			BNode newBNode = new BNode(theXmlNode.Name.ToLower(), nid, range);

			//标注出所有节点的父节点以及子节点
			XmlNodeList xmlNodes = theXmlNode.SelectNodes("Parents/*");
			foreach (XmlNode xml_node in xmlNodes)
				foreach (BNode bn_node in m_bnNodes)
					if (xml_node.Name.ToLower() == bn_node.Name)
					{
						bn_node.children.Add(newBNode);
						newBNode.Parents.Add(bn_node);
						break;
					}

			// 对每个节点新建一个条件概率表
			int table_rows = 1;
			xmlNodes = theXmlNode.SelectNodes("CPT_Col");
			if (range != xmlNodes.Count + 1) throw new Exception("CPT cols mismatch");
			foreach (BNode bn_node in newBNode.Parents)
				table_rows *= bn_node.Range;

			newBNode.CPT = new double[table_rows, range];

			//为条件概率表添加通过学习已得到的参数
			for (int i = 0; i < xmlNodes.Count; ++i)
			{
				XmlNodeList cpNodes = xmlNodes[i].SelectNodes("CP");

				if (cpNodes.Count != table_rows)
					throw new Exception("CPT Rows mismatch");

				for (int j = 0; j < table_rows; ++j)
					newBNode.CPT[j, i] = Convert.ToDouble(cpNodes[j].InnerText);
			}

			for (int i = 0; i < table_rows; ++i)
			{
				double pr = 1.0;
				for (int j = 0; j < range - 1; ++j)
					pr -= newBNode.CPT[i, j];
				if (pr < 0) throw new Exception("Probability does not normalize");
				newBNode.CPT[i, range - 1] = pr;
			}

			m_bnNodes.Add(newBNode);
		}

		public void PrintNet(string fileName)
		{
			StreamWriter w = new StreamWriter(fileName);

			w.WriteLine("The BeliefNet Layout");

			foreach (BNode node in m_bnNodes)
			{
				w.WriteLine("");
				w.WriteLine("Node: {0}", node.Name);
				w.WriteLine("    Total {0} Parents", node.Parents.Count);
				if (node.Parents.Count > 0) w.Write("    Parents:");
				foreach (BNode pnode in node.Parents)
					w.Write("   {0}", pnode.Name);
				if (node.Parents.Count > 0) w.WriteLine("");

				w.WriteLine("    CPT");
				int rows = node.CPT.GetUpperBound(0) + 1;
				int cols = node.CPT.GetUpperBound(1) + 1;
				for (int r = 0; r < rows; ++r)
				{
					string cpt = "";
					for (int c = 0; c < cols; ++c)
						cpt += "   " + node.CPT[r, c].ToString();
					w.WriteLine("    {0}", cpt);
				}
			}
			w.Close();
		}

		private ArrayList m_bnNodes;
	}
} // end namespace
