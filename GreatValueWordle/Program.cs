using System;
using System.IO;

namespace GreatValuewordle
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.Clear();
			new Game();
		}
	}

	public class Game
	{
		private string word = "";
		private const int TOTAL_GUESSES = 6;
		private int remainingGuesses = TOTAL_GUESSES;
		private string[] guesses = new string[TOTAL_GUESSES]; 
		private string[] wordList = File.ReadAllLines("words.txt");

		public Game()
		{
			Random random = new Random();

			word = wordList[random.Next(0, 1572)];	// 1572 is a totally arbitrary number lmao
			Start();
		}

		private void Start()
		{
			Console.WriteLine("Welcome to discount Wordle. You know the rules.");
			Console.WriteLine("Press enter to continue...");
			Console.Read();
			Update();
		}

		private void Update()
		{
			while (remainingGuesses > 0)
			{
				Console.Clear();

				foreach (string word in guesses)
				{
					if (word != null)
					{
						for (int i = 0; i < word.Length; i++)
						{
							PrintLetterColor(word, word[i], i);
						}

						Console.Write("\n");
					}
				}

				Console.Write($"\nGuess a 5-letter word ({remainingGuesses} guesses left): ");
				string guess = Console.ReadLine().Trim().ToLower();

				if (guess.Length != 5 || !InWordList(guess))
				{
					continue;
				}

				if (guess == word)
				{
					Console.ForegroundColor = ConsoleColor.Green;    
					Console.WriteLine($"\nYou win! The word was {word}. Press enter to exit.");
					Console.ResetColor();
					Console.Read();
					Console.Clear();
					return;
				}

				guesses[remainingGuesses - 1] = guess;
				remainingGuesses--;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"\nYou're out of guesses! The word was {word}. Press enter to exit.");
			Console.ResetColor();
			Console.Read();
			Console.Clear();
		}

		private bool InWordList(string word)
		{
			foreach (string s in wordList)
			{
				if (s.Trim() == word)
				{
					return true;
				}
			}

			return false;
		}
	
		private void PrintLetterColor(string guess, char letter, int index)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;

			if (word.Contains(letter))
			{
				if (guess[index] == word[index])
				{
					Console.ForegroundColor = ConsoleColor.Green;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
				}
			}

			Console.Write(letter);
			Console.ResetColor();
		}
	}
}
