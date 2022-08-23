namespace Stalky106.Extensions
{
	using System;
	using System.Text;

	public static class Extensions
    {
		public static readonly StringBuilder Builder = new StringBuilder();

		public static string ReplaceAfterToken(this string source, char token, Tuple<string, object>[] valuePairs)
		{
			if (valuePairs == null)
				throw new ArgumentNullException(nameof(valuePairs));

			Builder.Clear();

			int sourceLength = source.Length;

			for (int i = 0; i < sourceLength; i++)
			{
				char auxChar = token == char.MaxValue ? (char)(char.MaxValue - 1) : char.MaxValue;
				for (; i < sourceLength && (auxChar = source[i]) != token; i++)
					Builder.Append(auxChar);

				if (auxChar == token)
				{
					int movePos = 0;

					int length = valuePairs.Length;
					for (int ind = 0; ind < length; ind++)
					{
						Tuple<string, object> kvp = valuePairs[ind];
						int j, k;
						for (j = 0, k = i + 1; j < kvp.Item1.Length && k < source.Length && source[k] == kvp.Item1[j]; j++, k++) ;

						if (j == kvp.Item1.Length)
						{
							movePos = j;
							Builder.Append(kvp.Item2);
							break;
						}
					}

					if (movePos == 0)
						Builder.Append(token);
					else 
						i += movePos;
				}
			}

			return Builder.ToString();
		}
	}
}