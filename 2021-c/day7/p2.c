#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>

#define POSITION_COUNT (4096)

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 4096;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int crabCount = 0;
	int positionCounts[POSITION_COUNT];
	memset(&positionCounts, 0, POSITION_COUNT * sizeof(int));
	fgets(buffer, maxLineLength, file);
	
	char *token = strtok(buffer, ",");
	while(token) {
		int index = -1;
		sscanf(token, "%d", &index);
		positionCounts[index]++;
		crabCount++;
		token = strtok(NULL, ",");
	}
	
	for (int i=0; i<10; i++) {
		printf("%d,", positionCounts[i]);
	}
	printf("\n");

	printf("Found %d crabs in %s.\n", crabCount, filename);

	long int moveLeftCost[POSITION_COUNT];
	memset(&moveLeftCost, 0, POSITION_COUNT * sizeof(int));
	
	long int moveRightCost[POSITION_COUNT];
	memset(&moveRightCost, 0, POSITION_COUNT * sizeof(int));
	
	int movingCrabs = 0;
	long int fuelBurn = 0;
	for (int i=1; i<POSITION_COUNT; i++) {
		movingCrabs += positionCounts[i-1];
		fuelBurn += movingCrabs;
		moveRightCost[i] = fuelBurn + moveRightCost[i-1];
	}
	
	movingCrabs = 0;
	fuelBurn = 0;
	for (int i=POSITION_COUNT-2; i>=0; i--) {
		movingCrabs += positionCounts[i+1];
		fuelBurn += movingCrabs;
		moveLeftCost[i] = fuelBurn + moveLeftCost[i+1];
	}
	
	long int minFuel = INT_MAX;
	for (int i=0; i<POSITION_COUNT; i++) {
		long int sum = moveRightCost[i] + moveLeftCost[i];
		if (sum < minFuel) {
			printf("%d\t%16ld - %16ld\n", i, moveRightCost[i], moveLeftCost[i]);
			minFuel = sum;
		}
	}
	
	printf("Min fuel cost %ld\n", minFuel);

	return 0;
}
