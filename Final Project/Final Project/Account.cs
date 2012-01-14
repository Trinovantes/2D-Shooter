using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Final_Project
{
	class Account
	{
		static string file = Settings.Accounts + Settings.Account_Default;
		
		public static void CreateDefaultAccount(ref Account currentAccount)
		{
			File.Create(file);
			currentAccount = new Account();
		}

		public static bool DefaultAccountExist()
		{
			return File.Exists(Settings.Accounts + Settings.Account_Default);
		}
		
		public void SaveScore(int score)
		{
			// only save score if more than 0
			if (score > 0)
			{
				StreamWriter sw = new StreamWriter(file, true);
				sw.WriteLine(score);
				sw.Close();
			}
		}

		public int[] Scores()
		{
			StreamReader sr = new StreamReader(file);
			string contents = sr.ReadToEnd();
			string[] scores = contents.Split('\n');
			int[] output = new int[scores.Length];

			for (int i = 0; i < scores.Length; i++)
			{
				if (scores[i] != "")
				{
					output[i] = int.Parse(scores[i]);
				}
			}

			sr.Close();
			return output;
		}
	}
}
