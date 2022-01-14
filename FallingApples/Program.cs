// FALLING APPLES by Dan Donchuk (thatnoobles)
// Feel free to use this however you want

#pragma warning disable CA1416	// SetWindowSize() warning about possibly breaking cross-platform, don't need to worry about it

using System;
using System.Collections.Generic;
using System.Threading;

namespace FallingApples
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.SetWindowSize(8, 34);
			Console.CursorVisible = false;
			Game.Start();
			Console.Read();
		}
	}

	public class Game
	{
		public static char[,] board = new char[32, 8];

		private static int frame = 0;
		private static int score = 0;
		private static Random random = new Random();
		private static bool gameOver = false;
		private static int spawnWait = 60;	// (In frames) the delay between apple spawns. Lower value = faster spawning

		public static void Start()
		{
			// Initialize the game board

			Console.Clear();
			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					board[i, j] = '.';
				}
			}

			board[31, 0] = '~';
			Update();
		}

		private static void Update()
		{
			while (true)
			{
				if (Console.KeyAvailable)	// Read input without blocking
				{
					ConsoleKeyInfo keyInfo = Console.ReadKey(true);

					if (keyInfo.Key == ConsoleKey.LeftArrow)
					{
						// Prevent player from going off the left side of the screen
						if (GetPlayerPosition() != 0)
						{
							int currentPlayerPos = GetPlayerPosition();

							board[31, currentPlayerPos] = '.';
							board[31, currentPlayerPos - 1] = '~';
						}
					}
					else if (keyInfo.Key == ConsoleKey.RightArrow)
					{
						// Prevent player from going off the right side of the screen
						if (GetPlayerPosition() != board.GetLength(1) - 1)
						{
							int currentPlayerPos = GetPlayerPosition();

							board[31, currentPlayerPos] = '.';
							board[31, currentPlayerPos + 1] = '~';
						}
					}
				}

				// Spawn an apple every so often. Interval decreases every 5 points
				if (frame % spawnWait == 0)
				{
					SpawnApple();
				}

				// Move apples every other frame
				if (frame % 2 == 0)
				{
					UpdateApples();
				}

				Display();
				Thread.Sleep(16);	// Relatively high framerate

				// Stop game if player loses
				if (gameOver)
				{
					Console.Clear();
					Console.SetWindowSize(80, 2);
					Console.Write($"Game over! Your score was {score}. Press enter to quit.");
					return;
				}
				
				frame++;
			}
		}

		private static void Display()
		{
			for (int i = 0; i < board.GetLength(0); i++)
			{
				// Set the cursor position to the start of each row instead of using Console.Clear to prevent screen flickering
				Console.SetCursorPosition(0, i);

				for (int j = 0; j < board.GetLength(1); j++)
				{
					Console.ForegroundColor = board[i, j] switch
					{
						'~' => ConsoleColor.Yellow,
						'O' => ConsoleColor.Red,
						_ => ConsoleColor.DarkGray
					};

					Console.Write(board[i, j]);
				}

				Console.Write("\n");
			}

			// Display the score at the bottom of the screen
			Console.ResetColor();
			Console.SetCursorPosition(0, 32);
			Console.WriteLine(score);
		}
	
		private static int GetPlayerPosition()
		{
			for (int i = 0; i < Game.board.GetLength(1); i++)
			{
				if (board[31, i] == '~')
				{
					return i;
				}
			}

			return -1;
		}
	
		private static void SpawnApple()
		{
			board[0, random.Next(0, board.GetLength(1))] = 'O';
		}
	
		private static void UpdateApples()
		{
			foreach (int i in FindRowsWithApples())
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					if (board[i, j] == 'O')
					{
						if (i + 1 < board.GetLength(0))
						{
							switch (board[i + 1, j])
							{
								// Move apple down if there is an empty space below it
								case '.':
									board[i, j] = '.';
									board[i + 1, j] = 'O';
									break;

								// Catch apple if player is below it
								case '~':
									board[i, j] = '.';
									score++;

									// Double speed every 5 points
									if (score % 5 == 0)
									{
										spawnWait /= 2;
									}
									
									break;

								default:
									break;
							}
						}
						else
						{
							// Mark the game as over if apple is about to go off-screen
							gameOver = true;
						}
					}
				}	
			}
		}

		private static List<int> FindRowsWithApples()
		{
			// Search through the board once to find all apples in one go--instead of constantly looping through the board
			// This prevents apple from being found, moving down and immediately being found again
			List<int> output = new List<int>();

			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board.GetLength(1); j++)
				{
					if (board[i, j] == 'O')
					{
						output.Add(i);
					}
				}
			}

			return output;
		}
	}
}
