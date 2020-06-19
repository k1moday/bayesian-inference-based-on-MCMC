using System;
using BELIEF_NET;

/*class Test
{
	static void Main(string[] args)
	{
		BNet net = new BNet();
		net.Build("bn_wetGrass.xml");

		net.PrintNet("BNet_layout.txt");

		BNInfer infer = new BMcmc(net);

		//double pr = infer.GetBelief("Rain=1","WetGrass=1");
		double pr = infer.GetBelief("Sprinkler=1", "WetGrass=1");

		Console.WriteLine(" Result: {0}", pr);
	}
}*/
