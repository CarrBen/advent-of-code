#include <stdio.h>
#include <stdlib.h>

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);

	int lineNumber = 0;
	int *numbers = malloc(lineCount);

	fseek(file, 0, SEEK_SET);
	while(fgets(buffer, maxLineLength, file))
	{
		sscanf(buffer, "%d", &numbers[lineNumber]);
		lineNumber++;
	}
	fclose(file);

	int increaseCount = 0;
	for(int i=1; i<lineCount; i++)
	{
		if (numbers[i-1] < numbers[i])
		{
			increaseCount++;
		}
	}

	printf("Found %d increases.\n", increaseCount);

	return 0;
}
