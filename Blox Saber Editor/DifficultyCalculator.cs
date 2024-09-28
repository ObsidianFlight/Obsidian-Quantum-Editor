using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound_Space_Editor
{
    class DifficultyCalculator
    {
		public static MapInfo ConvertMapNew(List<Note> mapdata, double speed)
		{
			double x1 = 0;
			double x2 = 0;
			double x3 = 0;
			double y1 = 0;
			double y2 = 0;
			double y3 = 0;
			int clockwise = 0;

			int maxDifficultyNotePosition = 0;
			

			//Splits mapdata into an array so I can use it's information

			string[] notesraw = new string[mapdata.Count];
			double[,] notearray = new double[notesraw.Length, 3];
			double[,] noteDifficultyArray = new double[notesraw.Length, 10];
			for (int i = 0; i < notesraw.Length; i++)
			{
				notearray[i, 0] = Convert.ToDouble(mapdata[i].X);
				notearray[i, 1] = Convert.ToDouble(mapdata[i].Y);
				notearray[i, 2] = Convert.ToDouble(mapdata[i].Ms) / speed;
			}
			int stack = 0;
			double[] maxDifficultyArray = new double[notesraw.Length];
			//Grabs all the information I need from the notes, Most likely done in an unefficient manner.
			for (int i = 0; i < notearray.GetLength(0); i++)
			{
				double timeSinceLastNote = 0;
				double distance = 0;
				double distanceFromCenter = 2;
				x1 = notearray[i, 0];
				y1 = notearray[i, 1];
				if (i != 0)
				{
					x2 = notearray[i - 1, 0];
					y2 = notearray[i - 1, 1];
					timeSinceLastNote = notearray[i, 2] - notearray[i - 1, 2];
				}
				double x = Math.Abs(x1 - x2);
				double y = Math.Abs(y1 - y2);
				double cx = Math.Abs(x1 - 1);
				double cy = Math.Abs(y1 - 1);
				distance = Math.Sqrt(x * x + y * y);
				distanceFromCenter = Math.Sqrt(cx * cx + cy * cy);
				double rotation = 0;
				double answer = 0;
				double direction = 12300;
				if (i > 1)
				{
					//"Just use the dot product of (blah blah)"
					//I KNOW!! I don't know how. I hope you like if statements.
					//Finds whether the last 3 notes are clockwise or not.
					x1 = notearray[i - 2 - stack, 0];
					x2 = notearray[i - 1, 0];
					x3 = notearray[i, 0];
					y1 = notearray[i - 2 - stack, 1];
					y2 = notearray[i - 1, 1];
					y3 = notearray[i, 1];
					double a1 = x1 - x2;
					double a2 = y1 - y2;
					double b1 = x3 - x2;
					double b2 = y3 - y2;
					double c1 = x2 - x3;
					double c2 = y3 - y2;
					if (c1 == 0 & c2 > 0) { direction = 90; }
					if (c1 == 0 & c2 < 0) { direction = 270; }
					if (c1 > 0 & c2 == 0) { direction = 0; }
					if (c1 < 0 & c2 == 0) { direction = 180; }
					if (c1 > 0 & c2 > 0)
					{
						//0-90 Quadrant
						direction = Math.Round(Math.Abs(Math.Atan((c2 / c1)) * (180 / Math.PI)), 4);
					}
					if (c1 < 0 & c2 > 0)
					{
						//90-180 Quadrant
						direction = Math.Round(Math.Abs(Math.Atan((c2 / c1)) * (180 / Math.PI)), 4) + 90;
					}
					if (c1 < 0 & c2 < 0)
					{
						//180-270 Quadrant
						direction = Math.Round(Math.Abs(Math.Atan((c2 / c1)) * (180 / Math.PI)), 4) + 180;
					}
					if (c1 > 0 & c2 < 0)
					{
						//270-360 Quadrant
						direction = Math.Round(Math.Abs(Math.Atan((c2 / c1)) * (180 / Math.PI)), 4) + 270;
					}

					//Checks if notes are a stack

					if (distance == 0)
					{
						rotation = 180 * (Math.PI / 180);
						stack += 1;
					}
					else
					{
						rotation = Math.Acos((((a1 * b1) + (a2 * b2)) / (Math.Sqrt((a1 * a1 + a2 * a2)) * Math.Sqrt((b1 * b1 + b2 * b2)))));
						stack = 0;
					}
					answer = Math.Round(rotation * (180 / Math.PI), 4);
				}
				if (i == 0 || i == 1) { answer = 180; }
				noteDifficultyArray[i, 0] = notearray[i, 2]; //Time in song in milliseconds
				noteDifficultyArray[i, 1] = timeSinceLastNote; //in milliseconds
				noteDifficultyArray[i, 2] = distance;
				noteDifficultyArray[i, 3] = stack; //the amount of notes stacked behind it
				noteDifficultyArray[i, 4] = direction; //only used for rotation/flow detection, not that I put much thought into it.
				noteDifficultyArray[i, 5] = answer; //angle between the current note and the last 2.
				noteDifficultyArray[i, 8] = distanceFromCenter;
				noteDifficultyArray[i, 9] = 0; //Changes further down in code to detect maps doing BS quantum offgrids.

				if (i > 0)
				{
					double n = noteDifficultyArray[i - 1, 4];
					if (n > 180) { n -= 360; }
					double m = noteDifficultyArray[i, 4];
					if (m > 180) { m -= 360; }
					double k = 0;
					if (n > 0) { k = m - n; }
					if (n < 0) { k = m - n; }
					if (n == 0) { k = m; }
					if (k < 0) { k += 360; }
					if (k > 0 & k < 180) { clockwise = -1; }
					if (k == 0 || k == 180) { clockwise = 0; }
					if (k > 180) { clockwise = 1; }
				}
				noteDifficultyArray[i, 6] = clockwise; // 0 = straight, 1 = clockwise, -1 = counterclockwise, 
			}

			int[] flowChecker = new int[noteDifficultyArray.GetLength(0)];
			for (int i = 0; i < noteDifficultyArray.GetLength(0); i++)
			{
				if (i == 0 || i == 1) { flowChecker[i] = 1; }
				else
				{
					int u = (int)noteDifficultyArray[i - 2, 6];
					int h = (int)noteDifficultyArray[i - 1, 6];
					int b = (int)noteDifficultyArray[i, 6];

					//Most likely when you are spinning
					if (u == 1 & h == 1 & b == 1) { flowChecker[i] = 0; }
					else if (u == -1 & h == -1 & b == -1) { flowChecker[i] = 0; }
					//Quantum Slider, or repetitive back and forth jumps/slides
					//If Angle is 0, Uses 1 instead.
					else if (u == 0 & h == 0 & b == 0) { if (noteDifficultyArray[i, 5] == 0) { flowChecker[i] = 1; } else { flowChecker[i] = 0; } }
					//Coming out of repetitive back and forth jumps, or a quantum slider
					else if (u == 0 & h == 0 & b == -1) { flowChecker[i] = 2; }
					else if (u == 0 & h == 0 & b == 1) { flowChecker[i] = 2; }
					//The Wiggle Patterns.
					else if (u == 1 & h == -1 & b == 1) { flowChecker[i] = 2; }
					else if (u == -1 & h == 1 & b == -1) { flowChecker[i] = 2; }
					//Most likely when flow is reversed, or the start of a slider, if flow is hard reversed the angle should be 0.
					else if (u == -1 & h == -1 & b == 0) { if (noteDifficultyArray[i, 5] == 0) { flowChecker[i] = 4; } else { flowChecker[i] = 1; } }
					else if (u == 1 & h == 1 & b == 0) { if (noteDifficultyArray[i, 5] == 0) { flowChecker[i] = 4; } else { flowChecker[i] = 1; } }
					else if (u == 1 & h == -1 & b == 0) { if (noteDifficultyArray[i, 5] == 0) { flowChecker[i] = 4; } else { flowChecker[i] = 2; } }
					else if (u == -1 & h == 1 & b == 0) { if (noteDifficultyArray[i, 5] == 0) { flowChecker[i] = 4; } else { flowChecker[i] = 2; } }
					//Also reverses flow, I'm not sure how much emphasis I should be putting on these.
					else if (u == 1 & h == 1 & b == -1) { flowChecker[i] = 4; }
					else if (u == -1 & h == -1 & b == 1) { flowChecker[i] = 4; }
					else if (u == 1 & h == -1 & b == -1) { flowChecker[i] = 3; }
					else if (u == -1 & h == 1 & b == 1) { flowChecker[i] = 3; }
					//Most likely found in patterns that include Slides
					else if (u == 1 & h == 0 & b == -1) { flowChecker[i] = 2; }
					else if (u == -1 & h == 0 & b == 1) { flowChecker[i] = 2; }
					//Most likely in Spin Patterns, if not detected in a spin, increase by 3
					else if (u == 1 & h == 0 & u == 1)
					{
						if (noteDifficultyArray[i, 5] < 60)
						{
							flowChecker[i] = 4;
						}
						else
						{
							flowChecker[i] = 1;
						}
					}
					else if (u == -1 & h == 0 & u == 1) { if (noteDifficultyArray[i, 5] < 60) { flowChecker[i] = 4; } else { flowChecker[i] = 1; } }
					else if (u == 0 & h == 1 & b == 0) { if (noteDifficultyArray[i, 5] < 60) { flowChecker[i] = 4; } else { flowChecker[i] = 1; } }
					else if (u == 0 & h == -1 & b == 0) { if (noteDifficultyArray[i, 5] < 60) { flowChecker[i] = 4; } else { flowChecker[i] = 1; } }
					else { flowChecker[i] = 1; }
				}
			}

			double prevFinal = 0;
			double consistencyTimer = 0;
			for (int i = 0; i < noteDifficultyArray.GetLength(0); i++)
			{
				if (i > 0)
				{
					double final = -1;
					if (noteDifficultyArray[i, 1] > consistencyTimer)
					{
						consistencyTimer += Clamp(noteDifficultyArray[i, 1] / 2, 1, 1000);
					}
					else
					{
						consistencyTimer -= noteDifficultyArray[i, 1];
						consistencyTimer = Clamp(consistencyTimer, 0, 5000);
						noteDifficultyArray[i, 1] += consistencyTimer;
					}
					double distanceoverN = 0;
					if (i > 4)
					{
						distanceoverN += noteDifficultyArray[i, 2] + noteDifficultyArray[i - 1, 2] + noteDifficultyArray[i - 2, 2] + noteDifficultyArray[i - 3, 2] + noteDifficultyArray[i - 4, 2];
					}
					if (i > 0)
					{
						if (noteDifficultyArray[i, 3] == 0 && noteDifficultyArray[i - 1, 3] > 0)
						{
							noteDifficultyArray[i, 1] += noteDifficultyArray[i - 1, 1] / 2;
						}
					}//Increases time since last note if the previous note was a stack
					final = Math.Pow(Math.Pow(Clamp(noteDifficultyArray[i, 1] / 1000, 0.02, 9999), -.5), 3.5) / 3.5; // Difficulty Based on Time
					if (noteDifficultyArray[i, 5] < 30 || 150 < noteDifficultyArray[i, 5])
					{
						if (noteDifficultyArray[i, 2] > 2.25)
						{
							noteDifficultyArray[i, 2] = ((noteDifficultyArray[i, 2] - 2) * .4) + 2;
						}
					}
					final *= Math.Pow(noteDifficultyArray[i, 2], 1.5) / 3.4; // Difficulty Based on Distance
					if (noteDifficultyArray[i, 2] < 1.1) { final *= 1.2; }
					else if (noteDifficultyArray[i, 2] < 1.8 && noteDifficultyArray[i, 5] > 70) { final *= 0.8; }
					else if (noteDifficultyArray[i, 2] > 1.9) { final *= 1.1; }
					if (noteDifficultyArray[i, 2] > 2.5) { final *= 0.8; }
					if (distanceoverN < 3 || noteDifficultyArray[i, 3] > 0) { final *= 0.85; }
					else
					{
						double flow = flowChecker[i] * 0.5;
						if (noteDifficultyArray[i, 2] > 1.2 && noteDifficultyArray[i, 2] < 2 && distanceoverN > 7) { flow *= 1.5; }
						final *= 0.5 + flow;
					} // Difficulty Based on Flow 

					if (noteDifficultyArray[i, 3] > 0) { final = prevFinal / (2.3 + (noteDifficultyArray[i, 3] * .7)); } //Difficulty Based on Stack
					if (noteDifficultyArray[i, 2] > 0.4 && noteDifficultyArray[i, 1] < 135)
					{
						if (noteDifficultyArray[i, 5] < 45)
						{
							noteDifficultyArray[i, 5] = 0;
						}
						final *= 2.5 - (noteDifficultyArray[i, 5] - 180) * 0.5 / 180 * (Math.Abs(Clamp(noteDifficultyArray[i, 2], 1, 2) - 0.7) * 3);
					} //Difficulty based on angle
					final *= Clamp(1 + (1 - noteDifficultyArray[i, 8]), 1, 1.5); // Difficulty based on distance from center
					final *= Clamp(2.4 - noteDifficultyArray[i, 9], 1, 2); // Difficulty change for offgrid shit (IDK if it works I made this a long while ago)
					final += 0.5;
					final /= 2.4;
					if (Double.IsNaN(final) == true) { final = 1; }
					noteDifficultyArray[i, 7] = final;
					prevFinal = final;
					//Console.WriteLine(Math.Round(final, 2));
				}
				//Console.WriteLine(noteDifficultyArray[i, 7]);
			}
			//Console.WriteLine("");
			//Console.WriteLine("Next up: Area Difficulty");
			//Console.WriteLine("");
			int notesBehind = 0;
			double maxDifficulty = 0;
			int timeOfMaxDifficulty = 0;
			int noteNumberOfMaxDifficulty = 0;
			double prevTempDiff = 0;
			double averageDifficulty = 0;
			double[] noteDifficulty = new double[noteDifficultyArray.GetLength(0)];
			for (int i = 0; i < noteDifficultyArray.GetLength(0); i++)
			{
				if (notesBehind < 32)
				{
					notesBehind++;
				}
				double[,] tempDiff = new double[notesBehind, 2];
				for (int j = 0; j < tempDiff.GetLength(0); j++)
				{
					tempDiff[j, 0] = noteDifficultyArray[i - j, 0]; //Gets Time in Song
					tempDiff[j, 1] = noteDifficultyArray[i - j, 7]; //Gets Difficulty
					if (tempDiff[j, 0] < noteDifficultyArray[i, 0] - 2500)
					{
						notesBehind--;
					}
				}
				double tempMaxDifficulty = 0;
				for (int j = 0; j < tempDiff.GetLength(0); j++)
				{
					tempMaxDifficulty += tempDiff[j, 1];
				}
				//Console.WriteLine("1. Addition of all notes: " + tempMaxDifficulty);
				tempMaxDifficulty /= 4.5;
				//Console.WriteLine("2. Division by 4.5: " + tempMaxDifficulty);
				//if (tempMaxDifficulty > prevTempDiff)
				//{
				//	tempMaxDifficulty -= 5 * Math.Log(tempMaxDifficulty - prevTempDiff);
				//Console.WriteLine("3. Not really sure what this does: " + tempMaxDifficulty);
				//}
				if (tempMaxDifficulty > maxDifficulty)
				{
					maxDifficulty = tempMaxDifficulty;
					maxDifficultyNotePosition = i;
				}
				//Console.WriteLine($"{i + 1}, {Math.Round(tempMaxDifficulty, 2)}");
				averageDifficulty += tempMaxDifficulty;
				noteDifficulty[i] = tempMaxDifficulty;
                maxDifficultyArray[i] = tempMaxDifficulty;
			}
			double overallDifficulty;
			double highDifficulty = 0;
			int highIndex = 0;
			double lowDifficulty = 0;
			int lowIndex = 0;
			Array.Sort(noteDifficulty);
			for (int i = 0; i < noteDifficulty.Length; i++)
			{
				if (i < Math.Ceiling(noteDifficulty.Length * .45))
				{
					lowDifficulty += noteDifficulty[i];
					lowIndex++;
				}
				else
				{
					highDifficulty += noteDifficulty[i];
					highIndex++;
				}
			}
			highDifficulty /= highIndex;
			highDifficulty /= 4;
			maxDifficulty /= 4;
			highDifficulty *= 0.8;
			maxDifficulty *= 0.8;
			overallDifficulty = highDifficulty * (1 - (highDifficulty / maxDifficulty) + 1);
			overallDifficulty += (maxDifficulty - overallDifficulty) / 1.5;
			overallDifficulty = Math.Round(overallDifficulty, 2);
			maxDifficulty = Math.Round(maxDifficulty, 2);

			MapInfo theData = new MapInfo();
			theData.NoteDifficulty = maxDifficultyArray;
			theData.mostDifficultNote = maxDifficultyNotePosition;
			theData.OverallDifficulty = overallDifficulty;
			theData.MaxDifficulty = maxDifficulty;
			return theData;
		}
		public static double Clamp(double value, double min, double max)
		{

			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}

			return value;
		}

		public class MapInfo
		{
			public double OverallDifficulty { get; set; }
			public double[] NoteDifficulty { get; set; }
			public int mostDifficultNote { get; set; }
			public double MaxDifficulty { get; set; }
		}
	}
}
