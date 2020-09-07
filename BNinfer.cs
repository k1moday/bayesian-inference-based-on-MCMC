namespace BELIEF_NET
{
	using System;
	using System.Collections;
	using System.Windows.Forms;

	abstract class BNInfer
	{
		protected BNInfer(BNet net)
		{
			m_net = net;
		}

		public abstract double GetBelief(string x, string o);
		public abstract double GetBelief_WithoutBup(string x);
		public abstract double GetBelief_Multistate(string x, string o);

		
		protected BNet m_net;
	}

	class BElim : BNInfer
	{
		public BElim(BNet net) : base(net)
		{
			m_buckets = new ArrayList();

			PrepareBuckets();
		}

		private class Bucket
		{
			public Bucket(int i)
			{
				id = i;
				parentNodes = new ArrayList();
				childBuckets = new ArrayList();
			}

			public int id;
			public ArrayList parentNodes;
			public ArrayList childBuckets;
		}

		public override double GetBelief(string x, string o)//求解数据的输入
		{

			m_net.ResetNodes();

			double norm = 1.0;

			if (o.Length > 0)
			{
				//将字符串输入，在SetNodes函数中将字符串分割后赋值到该节点
				m_net.SetNodes(o);
				norm = Sum(0, 0);
			}

			m_net.SetNodes(x);

			return Sum(0, 0) / norm;
		}

		public override double GetBelief_WithoutBup(string x)
        {
			m_net.ResetNodes();

			double norm = 1.0;

			m_net.SetNodes(x);
			return Sum(0, 0) / norm;
		}
		public override double GetBelief_Multistate(string x, string o)//求解数据的输入
		{

			m_net.ResetNodes();

			double norm = 1.0;

			if (o.Length > 0)
			{
				//将字符串输入，在SetNodes函数中将字符串分割后赋值到该节点
				m_net.SetNodes(o);
				norm = Sum_Multistate(0, 0);
			}

			m_net.SetNodes(x);

			return Sum_Multistate(0, 0) / norm;
		}
		private void PrepareBuckets()
		{
			ArrayList nodes = m_net.Nodes;

			for (int i = 0; i < nodes.Count; ++i)
				m_buckets.Add(new Bucket(i));

			for (int i = nodes.Count - 1; i >= 0; --i)
			{
				BNode theNode = (BNode)nodes[i];
				Bucket theBuck = (Bucket)m_buckets[i];

				foreach (BNode node in theNode.Parents)
					theBuck.parentNodes.Add(node);

				foreach (Bucket nxtBuck in theBuck.childBuckets)
				{
					foreach (BNode node in nxtBuck.parentNodes)
						if (node.ID != i && !theBuck.parentNodes.Contains(node))
							theBuck.parentNodes.Add(node);
				}

				int max_nid = FindMaxNodeId(theBuck.parentNodes);

				if (max_nid >= 0)
					((Bucket)m_buckets[max_nid]).childBuckets.Add(theBuck);
			}
		}

		protected double Sum(int nid, int para)
		{
			BNode theNode = (BNode)m_net.Nodes[nid];
			Bucket theBuck = (Bucket)m_buckets[nid];

			int p_cnt = theNode.Parents.Count;

			int cond = (para & ((1 << p_cnt) - 1));

			double pr = 0.0;

			for (int e = 0; e < 2; ++e)
			{
				if (theNode.Evidence != -1 && theNode.Evidence != e)
					continue;

				double tmpPr = theNode.CPT[cond, e];

				foreach (Bucket nxtBuck in theBuck.childBuckets)
				{
					int next_para = 0;

					for (int j = 0; j < nxtBuck.parentNodes.Count; ++j)
					{
						BNode pnode = (BNode)nxtBuck.parentNodes[j];

						int pos = theBuck.parentNodes.IndexOf(pnode);

						next_para += (pos >= 0) ? ((para >> pos & 1) << j) : (e << j);
					}

					tmpPr *= Sum(nxtBuck.id, next_para);
				}

				pr += tmpPr;
			}

			return pr;
		}

		protected double Sum_Multistate(int nid, int para)
		{
			BNode theNode = (BNode)m_net.Nodes[nid];
			Bucket theBuck = (Bucket)m_buckets[nid];

			int p_cnt = 1;
			foreach(BNode node in theNode.Parents)
			p_cnt *= node.Range ;

			int cond = 0;

			if (p_cnt != 1)
				cond = para % p_cnt ;//防止溢出
			else cond = 0;

			double pr = 0.0;	

			for (int e = 0; e < theNode.Range; ++e)
			{
				if (theNode.Evidence != -1 && theNode.Evidence != e)
					continue;

				double tmpPr = theNode.CPT[cond, e];

				foreach (Bucket nxtBuck in theBuck.childBuckets)
				{
					int next_para = 0;
					int cond_new = 1;
					int cond_pos = 1;
					for (int j = 0; j < nxtBuck.parentNodes.Count; ++j)
					{
						BNode pnode = (BNode)nxtBuck.parentNodes[j];

						int pos = theBuck.parentNodes.IndexOf(pnode);

						int k ;
						cond_new = 1;

						for (k = 0; k < j; ++k)
						{
							BNode panode = (BNode)nxtBuck.parentNodes[k];
							cond_new *= panode.Range;
						}

						if (pos >= 0) 
						{
							cond_pos = 1;
							for (k = 0; k < pos ; ++k)
							{
								
								BNode panode = (BNode)theBuck.parentNodes[k];
								cond_pos *= panode.Range;
							}
							next_para += para / cond_pos * cond_new;
						}
						else
						next_para += e * cond_new;
					}

					tmpPr *= Sum_Multistate(nxtBuck.id, next_para);
				}

				pr += tmpPr;
			}

			return pr;
		}
		private int FindMaxNodeId(ArrayList nodes)
		{
			int max = -1;
			foreach (BNode node in nodes)
				if (node.ID > max) max = node.ID;
			return max;
		}

		private ArrayList m_buckets;
	}

	class BMcmc : BNInfer
	{
		static int GetRandomSeedbyGuid()
		{
			return Guid.NewGuid().GetHashCode();
		}
		public BMcmc(BNet net) : base(net)
		{

		}

		public override double GetBelief_Multistate(string x, string o)
		{
			return 0;
		}
		public override double GetBelief(string x, string o)
		{
			m_net.ResetNodes();
	
			//在吉布斯抽样算法中，由于将对所求节点进行抽样，所以只对条件进行赋值
			if (o.Length > 0)
			{
				m_net.SetNodes(o);
			}

			int queryid = 0;
			int querycon;

			string[] pair = x.Split('=');
			foreach (BNode node in m_net.Nodes)
			{
				if (pair.Length == 2 && node.Name == pair[0].Trim().ToLower())
				{
					queryid = node.ID;
				}
			}

			querycon = Convert.ToInt32(pair[1]);

			return Sampling(queryid, querycon);
		}

		public override double GetBelief_WithoutBup(string x)
		{
			m_net.ResetNodes();

			int queryid = 0;
			int querycon;

			string[] pair = x.Split('=');
			foreach (BNode node in m_net.Nodes)
			{
				if (pair.Length == 2 && node.Name == pair[0].Trim().ToLower())
				{
					queryid = node.ID;
				}
			}

			querycon = Convert.ToInt32(pair[1]);

			return Sampling(queryid, querycon);
		}

		protected double Sampling(int prob, int probcon)
		{
			mq = 0;
			int T = 10000;//采样个数
			First_Sampling();//第一次随机抽样

			Random random = new Random(GetRandomSeedbyGuid());
			int sample = random.Next();

			if (the_sample[prob] == probcon)
				mq = mq + 1;

			double pr = 0.0;
			double this_pr, multipr1, multipr2;

			nxt_sample = the_sample;

			for (int i = 2; i < T; i++)
			{
				this_pr = 1;
				multipr1 = 1;
				multipr2 = 1;

				the_sample = nxt_sample;

				//确定抽样时所需要的近似概率
				for (int j = 0; j < m_net.Nodes.Count; j++)
				{
					BNode theNode = (BNode)m_net.Nodes[j];
					int cond = 0;
					multipr1 = 1;
					multipr2 = 1;
					int flag = 0;


					if (theNode.Evidence == -1)
					{
						
						if (theNode.Parents.Count != 0)
						{

							foreach (BNode node in theNode.Parents)
							{
								cond = cond * 2 + the_sample[node.ID];
							}
							multipr1 = multipr1 * theNode.CPT[cond, 0];
							multipr2 = multipr2 * theNode.CPT[cond, 1];
						}
						else {
							multipr1 = multipr1 * theNode.CPT[0, 0];
							multipr2 = multipr2 * theNode.CPT[0, 1];
							}

						foreach (string item in strs)
						{
							if (theNode.Name.Contains(item))
							{
								this_pr = theNode.CPT[cond, 0];
								random = new Random(GetRandomSeedbyGuid());
								sample = random.Next();
								if (sample % 10000000 > this_pr * 10000000)
									nxt_sample[j] = 1;
								else
									nxt_sample[j] = 0;
								flag = 1;
							}
						}
						if (flag == 1)
							continue;
						
						if (theNode.children.Count != 0)
						{
							int cond1 = 0;
							int cond2 = 0;
							foreach (BNode node in theNode.children)
							{
								cond1 = 0;
								cond2 = 0;
								foreach (BNode parent in node.Parents)
								{
									if (parent.ID != theNode.ID)
									{
										cond1 = cond1 * parent.Range + the_sample[parent.ID];
										cond2 = cond2 * parent.Range + the_sample[parent.ID];
									}
									else
									{
										cond1 = cond1 * theNode.Range + 0;
										cond2 = cond2 * theNode.Range + 1;
									}
								}
								multipr1 = multipr1 * node.CPT[cond1, the_sample[node.ID]];
								multipr2 = multipr2 * node.CPT[cond2, the_sample[node.ID]];
							}

						}
						this_pr = multipr1 / (multipr1 + multipr2);

						random = new Random(GetRandomSeedbyGuid());
						sample = random.Next();

						if (sample % 10000000  > this_pr * 10000000 )
							nxt_sample[j] = 1;
						else
							nxt_sample[j] = 0;
					}
				}

				if (nxt_sample[prob] == probcon)
					mq = mq + 1;
			}

			pr =(double) mq / T;
			return pr;

		}

		private string[] strs = { "an", "or" };

		private void First_Sampling()
		{

			ArrayList nodes = m_net.Nodes;
			the_sample = new int[nodes.Count];
			for (int i = nodes.Count - 1; i >= 0; --i)
			{
				 
				BNode theNode = (BNode)m_net.Nodes[i];
				if (theNode.Evidence == -1)
				{
					Random rd = new Random();
					int j = rd.Next();
					the_sample[i] = j % theNode.Range;
				}
				else 
					the_sample[i] = theNode.Evidence; 
			}
		}

		private int[] the_sample; 
		private int[] nxt_sample;
		private int mq = 0;
	}
}// end namespace
				 