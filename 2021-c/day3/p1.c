#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int columnSums[maxLineLength];
	memset(columnSums, 0, sizeof(columnSums));
	int longestLine = 0;
	int lineCount = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;

		int lineLen = strlen(buffer) - 1;
		if (lineLen > longestLine)
		{
			longestLine = lineLen;
		}
		/*
		printf("LineLen %d %s", lineLen, buffer);
		for (int i=0; i<lineLen; i++)
		{
			columnSums[i] += buffer[i] - '0';
			printf("%d,", columnSums[i]);
		}
		printf("\n");
		*/
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	printf("Longest line %d. Got sums: ", longestLine);
	for (int i; i<longestLine; i++)
	{
		printf("%d,", columnSums[i]);
	}

	printf("\n");

	double gamma = 0;
	printf("Most common: ");
	for (int i = 0; i<longestLine; i++)
	{
		if (2 * columnSums[i] > lineCount)
		{
			double add = pow(2.0, (longestLine - i - 1));
			printf("%lf,", add);
			gamma += add;
		}
	}
	printf(" = %lf\n", gamma);

	double epsilon = 0;
	printf("Least common: ");
	for (int i = 0; i<longestLine; i++)
	{
		if (2 * columnSums[i] < lineCount)
		{
			double add = pow(2.0, (longestLine - i - 1));
			printf("%lf,", add);
			epsilon += add;
		}
	}
	printf(" = %lf\n", epsilon);
	
	printf("Power: %lf", epsilon * gamma);

	fclose(file);

	return 0;
}
