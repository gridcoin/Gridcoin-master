﻿using System;
using System.Collections.Generic;

namespace OpenPop.Mime.Decode
{
	/// <summary>
	/// Contains common operations needed while decoding.
	/// </summary>
	internal static class Utility
	{
		/// <summary>
		/// Remove quotes, if found, around the string.
		/// </summary>
		/// <param name="text">Text with quotes or without quotes</param>
		/// <returns>Text without quotes</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/></exception>
		public static string RemoveQuotesIfAny(string text)
		{
			if(text == null)
				throw new ArgumentNullException("text");

			// Check if there are quotes at both ends and have at least two characters
			if(text.Length > 1 && text[0] == '"' && text[text.Length-1] == '"')
			{
				// Remove quotes at both ends
				return text.Substring(1, text.Length - 2);
			}

			// If no quotes were found, the text is just returned
			return text;
		}

		/// <summary>
		/// Split a string into a list of strings using a specified character.<br/>
		/// Everything inside quotes are ignored.
		/// </summary>
		/// <param name="input">A string to split</param>
		/// <param name="toSplitAt">The character to use to split with</param>
		/// <returns>A List of strings that was delimited by the <paramref name="toSplitAt"/> character</returns>
		public static List<string> SplitStringWithCharNotInsideQuotes(string input, char toSplitAt)
		{
			List<string> elements = new List<string>();

			int lastSplitLocation = 0;
			bool insideQuote = false;

			char[] characters = input.ToCharArray();

			for (int i = 0; i < characters.Length; i++)
			{
				char character = characters[i];
				if (character == '\"')
					insideQuote = !insideQuote;

				// Only split if we are not inside quotes
				if (character == toSplitAt && !insideQuote)
				{
					// We need to split
					int length = i - lastSplitLocation;
					elements.Add(input.Substring(lastSplitLocation, length));

					// Update last split location
					// + 1 so that we do not include the character used to split with next time
					lastSplitLocation = i + 1;
				}
			}

			// Add the last part
			elements.Add(input.Substring(lastSplitLocation, input.Length - lastSplitLocation));

			return elements;
		}
	}
}