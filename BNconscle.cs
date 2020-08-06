using System;
using BELIEF_NET;

class Test
{
	static void Main(string[] args)
	{
		BNet net = new BNet();

		net.Build("bn_Alarm.xml");

		net.PrintNet("BNet_layout.txt");

		BNInfer infer = new BElim(net);

		double pr = infer.GetBelief("alarm= 1","");
			
		Console.WriteLine(" Result: {0}", pr);
	}
}
