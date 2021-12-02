#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	int horizontal = 0;
	int depth = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		char* direction;
		direction = strtok(buffer, " ");
		//printf("Direction is %s\n", direction);

		char* distance;
		distance = strtok(0, " ");
		int distanceValue = 0;
		sscanf(distance, "%d", &distanceValue);
		//printf("DistanceValue is %d\n", distanceValue);

		if(strcmp(direction, "forward") == 0)
		{
			horizontal += distanceValue;
		}else if(strcmp(direction, "up") == 0)
		{
			depth -= distanceValue;
		}else if(strcmp(direction, "down") == 0)
		{
			depth += distanceValue;
		}
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	printf("Ended at horizontal %d.\n", horizontal);
	printf("Ended at depth %d.\n", depth);
	printf("Puzzle answer %d.\n", horizontal * depth);
	fclose(file);

	return 0;
}
