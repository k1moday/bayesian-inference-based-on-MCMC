using System;
using BELIEF_NET;
using System.IO;
class Test
{
	/*static void Main(string[] args)
	{
		BNet net = new BNet();

		net.Build("nodenames(2).xml");

		//net.PrintNet("BNet_layout.txt");

		BNInfer infer = new BMcmc(net);

		//StreamWriter w = new StreamWriter("fout(100).txt");

		double pr = infer.GetBelief("and=0", "x1v=1;x2v=1");
		w.WriteLine("Y = 1 pr={0}",pr);
		pr = infer.GetBelief("xiv = 1", "Y	= 1");
		w.WriteLine("xiv = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xiiv = 1", "Y	= 1");
		w.WriteLine("xiiv = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xiiiv = 1", "Y = 1");
		w.WriteLine("xiiiv = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xivv = 1", "Y	= 1");
		w.WriteLine("xivv = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xis = 1", "Y	= 1");
		w.WriteLine("xis = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xiis = 1", "Y	= 1");
		w.WriteLine("xiis = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xiiis = 1", "Y	= 1");
		w.WriteLine("xiiis = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xivs = 1", "Y	= 1");
		w.WriteLine("xivs = 1|Y = 1 pr={0}", pr);
		pr = infer.GetBelief("xv = 1", "Y	= 1");
		w.WriteLine("xv = 1|Y = 1 pr={0}", pr);
		w.Close();

		//Console.WriteLine(" Result: {0}", pr);
	}*/
}