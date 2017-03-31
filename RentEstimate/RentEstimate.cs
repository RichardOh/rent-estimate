using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RentEstimate
{
	
	public class Estimator
	{
		// Stores data information from the txt file 
		public List<int> houseSizes = new List<int>();
		public List<int> houseRent = new List<int>();
		public int numHouses;


		public Estimator()
		{
			numHouses = 0;
		}

		public void storeHouse(int size, int rent)
		{
			houseSizes.Add(size);
			houseRent.Add(rent);
			numHouses++;
		}

		public double averageCost(double slope)
		{
			// Average cost function used is (Sum of (expected - obs)^2) / 2 * (# houses)
			// Calculates how far each data point is from the
			// regression line specified by the slope parameter
			double sumCost = 0;
			for (int i = 0; i < houseSizes.Count; i++)
			{
				sumCost += Math.Pow((houseSizes[i] * slope) - houseRent[i], 2);
			}
			sumCost /= 2 * numHouses;
			return sumCost;
		}
	}
	class MainClass
	{
		public static void Main(string[] args)
		{
			// file path of data file 
			string path = @"/Users/Oh/Projects/RentEstimate/RentEstimate/data.txt";

			Console.WriteLine("Welcome to RentEstimate! This simple program " +
						  "will give you an estimate of rent prices based off " +
						  "apartment size in the La Jolla area");
			Console.WriteLine("Enter 'n' to add new housing data, " +
							  "'e' to get a rent estimate, " +
							  "and 'q' to exit out of the program");
			// used to store the character commands of the user
			StringBuilder userInput = new StringBuilder();
			userInput.Append(Console.ReadLine());

			// Runs until error or user quits out 
			while (!userInput.ToString().Equals("q"))
			{
				if (userInput.ToString().Equals("n"))
				{
					int newHouseSize = 0;
					Console.WriteLine("Please enter the house's size " +
									  "in square feet");
					
					try
					{
						newHouseSize = Convert.ToInt32(Console.ReadLine());
					}
					catch (FormatException)
					{
						Console.WriteLine("Invalid house size");
						return;
					}

					int newHouseRent = 0;
					Console.WriteLine("Please enter the house's rent " +
									  "in $/month");
					
					try
					{
						newHouseRent = Convert.ToInt32(Console.ReadLine());
					}
					catch (FormatException)
					{
						Console.WriteLine("Invalid house rent");
						return;
					}

					// Appends specified info to the end of the data file
					using (StreamWriter sw = File.AppendText(path)) 
					{
						string newData = newHouseSize.ToString() + '\t' + 
						                 newHouseRent.ToString();
						sw.WriteLine(newData);
					}
				}
				else if (userInput.ToString().Equals("e"))
				{
					string line;
					System.IO.StreamReader file = new System.IO.StreamReader(path);
					line = file.ReadLine();
					Estimator est = new Estimator();

					// Reads file until EOF
					while ((line = file.ReadLine()) != null)
					{
						// Splits each line by the tab character
						var dataSplit = line.Split('\t');
						if (dataSplit.Length != 2)
						{
							Console.WriteLine("Invalid data file");
							return;
						}

						int currHouseSize = 0;
						int currHouseRent = 0;

						try
						{
							currHouseSize = Convert.ToInt32(dataSplit[0]);
							currHouseRent = Convert.ToInt32(dataSplit[1]);
						}
						catch (FormatException)
						{
							Console.WriteLine("Invalid data types");
							return;
						}
						est.storeHouse(currHouseSize, currHouseRent);
					}

					file.Close();

					Console.WriteLine("Please enter the house size (sq. ft) " +
									  "that you wish to get a rent estimate for");
					int sizeEstimate = 0;
					try
					{
						sizeEstimate = Convert.ToInt32(Console.ReadLine());
					}
					catch (FormatException)
					{
						Console.WriteLine("Invalid house size");
						return;
					}

					double regSlope = 0;
					double minCost = 0;

					// Calculates which regression line is best for the data 
					for (double i = 0; i < 100; i += 0.001)
					{
						double currCost = est.averageCost(i);
						if (i == 0.0 || currCost < minCost)
						{
							regSlope = i;
							minCost = currCost;
						}
					}
					int estimate = (int)(regSlope * sizeEstimate);
					Console.WriteLine("The estimated cost of rent for a house " +
									  "with a size of " + sizeEstimate.ToString() +
									  "sq. ft is " + estimate.ToString());
				}

				Console.WriteLine();
				Console.WriteLine("Enter 'n' to add new housing data, " +
							  "'e' to get a rent estimate, " +
							  "and 'q' to exit out of the program");
				userInput.Clear();
				userInput.Append(Console.ReadLine());
			}
		}
	}

}