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

	// Doesn't care about differing line lengths within the same file
	int *lines = malloc(lineCount * sizeof(long));
	int lineNumber = 0;

	fseek(file, 0, SEEK_SET);
	while(fgets(buffer, maxLineLength, file))
	{
		lines[lineNumber] = strtol(buffer, NULL, 2);
		lineNumber++;
	}

	for (int i=0; i<lineCount; i++)
	{
		printf("%d\n", lines[i]);
	}

	int *workingLines = malloc(lineCount * sizeof(int));
	int *keepLines = malloc(lineCount * sizeof(int));
	
	int remainingLines = lineCount;
	int column = 0;
	memcpy(keepLines, lines, lineCount * sizeof(int));
	while(remainingLines > 1)
	{
		memcpy(workingLines, keepLines, lineCount * sizeof(int));	
		int columnSum = 0;
		int keptLines = 0;
		for (int l=0; l<remainingLines; l++)
		{
			int offset = longestLine - column - 1;
			printf("%d, %d for %d\n", workingLines[l], ((workingLines[l] >> offset) & 1), offset);
			if (((workingLines[l] >> offset) & 1) == 1)
			{
				columnSum++;
			}
		}
		for (int l=0; l<remainingLines; l++)
		{
			long power = (long) pow(2.0, longestLine - column - 1);
			printf("Compare %ld to %d for sum of %d with %d lines\n", 
				power, workingLines[l], columnSum, remainingLines);
			if (2*columnSum >= remainingLines)
			{
				if ((workingLines[l] & power) == power)
				{
					keepLines[keptLines] = workingLines[l];
					keptLines++;
				}
			}else{
				if ((workingLines[l] & power) != power)
				{
					keepLines[keptLines] = workingLines[l];
					keptLines++;
				}
			}
		}
		printf("%d lines remaining after Oxygen column %d\n", keptLines, column);
		remainingLines = keptLines;
		column++;
	}
	// Ignore the fact that we may not end up keeping any lines
	long oxygen = keepLines[0];

	remainingLines = lineCount;
	column = 0;
	memcpy(keepLines, lines, lineCount * sizeof(int));
	while(remainingLines > 1)
	{
		memcpy(workingLines, keepLines, lineCount * sizeof(int));	
		int columnSum = 0;
		int keptLines = 0;
		for (int l=0; l<remainingLines; l++)
		{
			int offset = longestLine - column - 1;
			printf("%d, %d for %d\n", workingLines[l], ((workingLines[l] >> offset) & 1), offset);
			if (((workingLines[l] >> offset) & 1) == 1)
			{
				columnSum++;
			}
		}
		for (int l=0; l<remainingLines; l++)
		{
			long power = (long) pow(2.0, longestLine - column - 1);
			printf("Compare %ld to %d for sum of %d with %d lines\n", 
				power, workingLines[l], columnSum, remainingLines);
			if (2*columnSum < remainingLines)
			{
				if ((workingLines[l] & power) == power)
				{
					keepLines[keptLines] = workingLines[l];
					keptLines++;
				}
			}else{
				if ((workingLines[l] & power) != power)
				{
					keepLines[keptLines] = workingLines[l];
					keptLines++;
				}
			}
		}
		printf("%d lines remaining after CO2 column %d\n", keptLines, column);
		remainingLines = keptLines;
		column++;
	}
	// Ignore the fact that we may not end up keeping any lines
	long cotwo = keepLines[0];

	printf("Life Support: %ld\n", oxygen * cotwo);

	fclose(file);
	free(lines);
	free(workingLines);
	free(keepLines);

	return 0;
}
